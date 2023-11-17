using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DAS.Urway.Routes
{
    public enum PlacementMode
    {
        UserLevel = 0,
        GroundLevel = 1
    }

    public class RouteController : MonoBehaviour
    {
        public Action OnStartRoute;
        
        [SerializeField] private RoutesAPI m_api;
        [SerializeField] private GeospatialController m_geoController;
        [SerializeField] private PlacementMode m_lacementMode;

        [Header("Visual")] [SerializeField] private LineRenderer m_lineRenderer;

        private Route m_route;
        private List<MarkerInfo> m_waypoints = new List<MarkerInfo>();
        private List<GameObject> m_routeAnchors = new List<GameObject>();
        private bool m_isInitialized;
        private bool m_isTracking;

        private void Awake()
        {
            m_isInitialized = false;
        }

        public void StartRoute(Location destination)
        {
            Location _start = new Location(m_geoController.CurrentPose.Latitude, m_geoController.CurrentPose.Longitude);
            StartRoute(_start, destination);
        }

        public void StartRoute(Location start, Location destination)
        {
            m_api.GetRoute(start, destination, route =>
            {
                m_route = route;
                CreateWaypoints();
                SetAnchors();
                m_isInitialized = true;
            });
        }


        public void Stop()
        {
            m_isInitialized = false;
            m_isTracking = false;
            Release();
            m_lineRenderer.positionCount = 0;
        }

        private void CreateWaypoints()
        {
            if (m_route.legs.Count > 0)
            {
                var _steps = m_route.legs[0].steps;
                for (int i = 0; i < _steps.Count; i++)
                {
                    MarkerInfo _marker = new MarkerInfo();
                    _marker.Type = m_lacementMode == PlacementMode.GroundLevel
                        ? AnchorType.Terrain
                        : AnchorType.Geospatial;
                    _marker.LocationID = "waypoint " + i;

                    //TODO add debug value
                    _marker.Altitude = m_geoController.CurrentPose.Altitude;
                    _marker.Latitude = _steps[i].startLocation.latLng.latitude;
                    _marker.Longitude = _steps[i].startLocation.latLng.longitude;

                    if (i == _steps.Count - 1)
                    {
                        _marker.Latitude = _steps[i].endLocation.latLng.latitude;
                        _marker.Longitude = _steps[i].endLocation.latLng.longitude;
                    }

                    m_waypoints.Add(_marker);
                }
            }
        }

        private void SetAnchors()
        {
            int _k = 0;
            for (int i = 0; i < m_waypoints.Count; i++)
            {
                m_geoController.GetAnchor(m_waypoints[i], anchor =>
                {
                    m_routeAnchors.Add(anchor);
                    _k++;
                    if (_k == m_waypoints.Count)
                    {
                        SetVisual();
                    }
                });
            }
        }

        private void SetVisual()
        {
            m_lineRenderer.positionCount = m_routeAnchors.Count;
            for (int i = 0; i < m_routeAnchors.Count; i++)
            {
                m_lineRenderer.SetPosition(i, m_routeAnchors[i].transform.position);
            }
            OnStartRoute?.Invoke();
        }

        private void Release()
        {
            m_route = null;
            m_waypoints.Clear();
            for (int i = 0; i < m_routeAnchors.Count; i++)
            {
                Destroy(m_routeAnchors[i]);
            }

            m_routeAnchors.Clear();
        }

        [Button]
        public void TestStart()
        {
            Location _startPoint = new Location(52.23227790253855f, 20.987447291784342f);
            Location _endPoint = new Location(52.23462360829125f, 20.990806379747738f);

            StartRoute(_endPoint, _startPoint);
        }

        [Button]
        public void TestStop()
        {
            Stop();
        }
    }
}