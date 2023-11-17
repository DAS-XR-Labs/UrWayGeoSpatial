using System;
using UnityEngine;
using ARLocation.MapboxRoutes;
using UnityEngine.Serialization;

namespace DAS.Urway
{
    public class RouteMe : MonoBehaviour
    {
        public Action OnRouteStart; // Event invoked when a route starts.
        public Action OnRouteEnd; // Event invoked when a route ends.

        [FormerlySerializedAs("loadRoute")] public bool LoadRoute; // Flag indicating if a route should be loaded.
        [FormerlySerializedAs("endRoute")] public bool EndRoute; // Flag indicating if the route should be ended.

        [FormerlySerializedAs("route")] [SerializeField] private MapboxRoute Route; // Reference to the MapboxRoute component used for routing.

        public delegate void NavHandler(); // Delegate for handling navigation events.

        public static event NavHandler EndNavigation; // Event invoked when navigation ends.

        //[SerializeField] private PlacedMarkerInfo placedMarker;

        [FormerlySerializedAs("startPoint")] public RouteWaypoint StartPoint; // Starting waypoint for the route.
        [FormerlySerializedAs("endPoint")] public RouteWaypoint EndPoint; // Ending waypoint for the route

        //public List<GameObject> signs = new List<GameObject>();
        //public GameObject arrowCanvas;

        private bool m_isActive; // Flag indicating if navigation is currently active.
        public bool IsTracking => m_isActive; // Gets whether navigation is currently active.

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void Update()
        {
            if (LoadRoute)
            {
                LoadRoute = false;
                InitRoute();
            }

            if (EndRoute)
            {
                EndRoute = false;
                StopRoute();
            }

        }

        /// <summary>
        /// Initiates the routing process.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        public void InitRoute()
        {
            m_isActive = true;
            OnRouteStart?.Invoke();
            Route.gameObject.SetActive(true);
            Debug.Log("Loading Route to " + EndPoint.Location.Label);
            Debug.Log("Lat " + EndPoint.Location.Latitude);
            Debug.Log("Lon " + EndPoint.Location.Longitude);

            if (Route != null) StartCoroutine(Route.LoadRoute(StartPoint, EndPoint));
            //{

            //}
            //else
            //{
            //    //route = FindObjectOfType<MapboxRoute>();
            //    StartCoroutine(route.LoadRoute(startPoint, endPoint));
            //}

            //if (arrowCanvas != null) arrowCanvas.SetActive(true);

        }

        /// <summary>
        /// Ends the ongoing route navigation.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        public void StopRoute()
        {
            m_isActive = false;
            OnRouteEnd?.Invoke();
            if (EndNavigation != null) EndNavigation();

            //arrowCanvas = GameObject.Find("[OnScreenTargetIndicatorCanvas]");
            //arrowCanvas.SetActive(false);
            //foreach (GameObject go in signs) go.SetActive(false);
            Route.gameObject.SetActive(false);
        }
    }
}