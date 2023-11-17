using System.Collections;
using System.Collections.Generic;
using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.ARFoundation;

namespace DAS.Urway
{
    /// <summary>
    /// Manages the welcome center prompt and anchor points for the AR experience.
    /// </summary>
    public class WelcomeCenterPromptController : MonoBehaviour
    {
        [FormerlySerializedAs("welcomeCenterPromtScreen")] [SerializeField]
        private WelcomeCenterPromtScreen m_welcomeCenterPromtScreen; // The welcome center prompt screen component.

        //First item should be start point sign 
        [FormerlySerializedAs("anchors")] [SerializeField] private List<GEOAnchor> m_anchors; // List of anchor points for the AR experience.
        [FormerlySerializedAs("geospatialController")] [SerializeField] private GeospatialController m_geospatialController; // The GeospatialController component.
        [FormerlySerializedAs("AnchorManager")] [SerializeField] private ARAnchorManager m_anchorManager; // The ARAnchorManager for managing AR anchors.
        [FormerlySerializedAs("heading")] [SerializeField] private double m_heading = 25; // The heading angle for the anchor rotation.
        [FormerlySerializedAs("activationDistance")] [SerializeField] private float m_activationDistance = 5; // The distance to activate the welcome center prompt.

        [Header("Debug")] private GeospatialPose m_debugPose; // The GeospatialPose used for debugging.
        [FormerlySerializedAs("debugLat")] [SerializeField] private double m_debugLat; // Debug latitude value.
        [FormerlySerializedAs("debugLong")] [SerializeField] private double m_debugLong; // Debug longitude value.
        [FormerlySerializedAs("debugAltitude")] [SerializeField] private double m_debugAltitude; // Debug altitude value.

        private bool m_anchorsEnabled; // Flag indicating if anchors are enabled.
        private int m_enabledAnchorsNumber; // Number of enabled anchors.
        private bool m_isVisible; // Flag indicating if the object is visible.
        private bool m_isTracking; // Flag indicating if tracking is active.
        private bool m_activateOnce = true; // Flag for activating only once.
        private bool m_doneSpawning = false; // Flag indicating if spawning is done.

        /// <summary>
        /// Called when the object is enabled.
        /// Initializes variables and subscribes to tracking state update events.
        /// </summary>
        private void OnEnable()
        {
            m_isVisible = false;
            m_enabledAnchorsNumber = 0;
            m_geospatialController.OnTrackingStateUpdate += OnTrackingStateUpdate;
            foreach (var geoAnchor in m_anchors)
            {
                geoAnchor.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Handles tracking state updates.
        /// Manages anchor visibility based on tracking state.
        /// </summary>
        /// <param name="isTracking">Indicates if tracking is currently active.</param>
        private void OnTrackingStateUpdate(bool isTracking)
        {
            m_isTracking = isTracking;
            foreach (var geoAnchor in m_anchors)
            {
                if (isTracking)
                {
                    if (m_anchorsEnabled)
                    {
                        geoAnchor.gameObject.SetActive(isTracking && m_isVisible);
                    }
                    else
                    {
                        SetAnchor(geoAnchor);
                    }
                }
                else
                {
                    geoAnchor.gameObject.SetActive(isTracking && m_isVisible);
                }
            }
        }

        /// <summary>
        /// Updates the state of the welcome center prompt and anchor points.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void Update()
        {
            if (m_isTracking)
            {
                TrackCameraToStartPointDistance();
            }

            // foreach (var geoAnchor in anchors)
            // {
            //     geoAnchor.gameObject.SetActive(_isTracking && _isVisible);
            // }
        }

        /// <summary>
        /// Sets an anchor point and initializes ARGeospatialAnchor.
        /// </summary>
        /// <param name="geoAnchor">The GEOAnchor component to set as an anchor.</param>
        private void SetAnchor(GEOAnchor geoAnchor)
        {
            m_enabledAnchorsNumber++;
            if (m_enabledAnchorsNumber == m_anchors.Count)
            {
                m_anchorsEnabled = true;
            }

            StartCoroutine(SetAnchorWithDelay(geoAnchor));
        }

        /// <summary>
        /// Sets an anchor point with a delay and initializes ARGeospatialAnchor.
        /// </summary>
        /// <param name="geoAnchor">The GEOAnchor component to set as an anchor.</param>
        private IEnumerator SetAnchorWithDelay(GEOAnchor geoAnchor)
        {
            geoAnchor.gameObject.SetActive(true);
            Quaternion _quaternion = Quaternion.AngleAxis(180f - (float) m_heading, Vector3.up);
            double _altitudeInM = ConvertFeetToMeters(geoAnchor.Altitude);
            double _latitude = geoAnchor.Latitude;
            double _longitude = geoAnchor.Longitude;
            ARGeospatialAnchor _anchor = m_anchorManager.AddAnchor(_latitude, _longitude, _altitudeInM, _quaternion);
            geoAnchor.gameObject.SetActive(false);

            yield return new WaitForSeconds(2);

            if (_anchor == null && PlatformsHelper.IsEditor)
            {
                _anchor = new GameObject("Anchor").AddComponent<ARGeospatialAnchor>();
                _anchor.transform.rotation = _quaternion;
                Vector3 _vInfo = new Vector3((float)geoAnchor.Latitude, (float)geoAnchor.Altitude,
                    (float)geoAnchor.Longitude);
                Vector3 _pos = ConvertGPStoUCS(
                    new Vector3((float)m_debugLat, (float)m_debugAltitude, (float)m_debugLong),
                    _vInfo);
                _anchor.transform.position = _pos;
            }

            geoAnchor.gameObject.SetActive(m_isVisible);
            geoAnchor.transform.SetParent(_anchor.transform);
            geoAnchor.transform.localPosition = Vector3.zero;
            //geoAnchor.transform.position = anchor.transform.position;
            m_doneSpawning = true;
        }


        /// <summary>
        /// Tracks the camera distance to the start point and triggers prompt activation.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void TrackCameraToStartPointDistance()
        {
            if (m_anchors.Count > 0 && m_anchorsEnabled && !m_isVisible && m_activateOnce && m_doneSpawning)
            {
                Vector3 _cameraPos = Camera.main.gameObject.transform.position;
                Vector3 _targetPos = m_anchors[0].transform.position;
                _cameraPos.y = 0;
                _targetPos.y = 0;
                float _distance = Vector3.Distance(_cameraPos, _targetPos);

                if (_distance < m_activationDistance)
                {
                    m_activateOnce = false;
                    m_isVisible = true;
                    UpdateAnchorsVisibility();
                    m_welcomeCenterPromtScreen.ActivateWelcomeCenterPromt(m_anchors[1].transform, () =>
                    {
                        m_isVisible = false;
                        UpdateAnchorsVisibility();
                    });
                }
            }
        }

        /// <summary>
        /// Called when the object is disabled.
        /// Unsubscribes from tracking state update events.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void UpdateAnchorsVisibility()
        {
            foreach (var geoAnchor in m_anchors)
            {
                geoAnchor.gameObject.SetActive(m_isTracking && m_isVisible);
            }
        }

        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnDisable()
        {
            m_geospatialController.OnTrackingStateUpdate -= OnTrackingStateUpdate;
        }

        /// <summary>
        /// Converts feet to meters.
        /// </summary>
        /// <param name="feet">The value in feet to convert.</param>
        /// <returns>The converted value in meters.</returns>
        public static double ConvertFeetToMeters(double feet)
        {
            double _conversionFactor = 0.3048;
            double _meters = feet * _conversionFactor;
            return _meters;
        }

        /// <summary>
        /// Converts GPS coordinates to Unity coordinate system.
        /// </summary>
        /// <param name="origin">The origin point for conversion.</param>
        /// <param name="loc">The GPS coordinates to convert.</param>
        /// <returns>The converted coordinates in Unity coordinate system.</returns>
        private Vector3 ConvertGPStoUCS(Vector3 origin, Vector3 loc)
        {
            return new Vector3((float) (loc.x - origin.x) * 10000, (float) (loc.y - origin.y) / 10f,
                (float) (loc.z - origin.z) * 10000);
        }

        [ContextMenu("Test")]
        /// <summary>
        /// Initiates a test action.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        public void Test()
        {
            m_welcomeCenterPromtScreen.ActivateWelcomeCenterPromt(m_anchors[1].transform, () => { m_isVisible = false; });
        }
    }
}