using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using Google.XR.ARCoreExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace DAS.Urway
{
    /// <summary>
    /// Controller for Geospatial
    /// </summary>
    public class GeospatialController : MonoBehaviour
    {
        #region Parameters

        public Action<bool> OnTrackingStateUpdate;
        public Action OnLocalizationCompleted;

        [Header("AR Components")]
        /// <summary>
        /// The ARSessionOrigin used in the.
        /// </summary>
        public ARSessionOrigin SessionOrigin;

        /// <summary>
        /// The ARSession used in the .
        /// </summary>
        public ARSession Session;

        /// <summary>
        /// The ARAnchorManager used in the sample.
        /// </summary>
        public ARAnchorManager AnchorManager;

        /// <summary>
        /// The AREarthManager used in the sample.
        /// </summary>
        public AREarthManager EarthManager;

        /// <summary>
        /// The ARStreetscapeGeometryManager used in the sample.
        /// </summary>
        public ARStreetscapeGeometryManager StreetscapeGeometryManager;

        /// <summary>
        /// The ARCoreExtensions used in the sample.
        /// </summary>
        public ARCoreExtensions ARCoreExtensions;

        [Header("UI Elements")]
        /// <summary>
        /// A 3D object that presents an Geospatial Anchor.
        /// </summary>
        public GameObject GeospatialPrefab;

        /// <summary>
        /// UI element containing all AR view contents.
        /// </summary>
        public GameObject ARViewCanvas;

        /// <summary>
        /// UI element for clearing all anchors, including history.
        /// </summary>
        public Button ClearAllButton;

        /// <summary>
        /// UI element for adding a new anchor at current location.
        /// </summary>
        public Button SetAnchorButton;

        /// <summary>
        /// UI element to display information at runtime.
        /// </summary>
        public GameObject InfoPanel;

        /// <summary>
        /// Text displaying <see cref="GeospatialPose"/> information at runtime.
        /// </summary>
        public Text InfoText;

        /// <summary>
        /// Text displaying in a snack bar at the bottom of the screen.
        /// </summary>
        public Text SnackBarText;

        /// <summary>
        /// Text displaying debug information, only activated in debug build.
        /// </summary>
        public Text DebugText;

        /// <summary>
        /// Help message shows while localizing.
        /// </summary>
        private const string m_localizingMessage = "Localizing your device to set anchor.";

        /// <summary>
        /// Help message shows when <see cref="AREarthManager.EarthTrackingState"/> is not tracking
        /// or the pose accuracies are beyond thresholds.
        /// </summary>
        private const string m_localizationInstructionMessage =
            "Point your camera at buildings, stores, and signs near you.";

        /// <summary>
        /// Help message shows when location fails or hits timeout.
        /// </summary>
        private const string m_localizationFailureMessage =
            "Localization not possible.\n" +
            "Close and open the app to restart the session.";

        /// <summary>
        /// Help message shows when location success.
        /// </summary>
        private const string m_localizationSuccessMessage = "Localization completed.";

        /// <summary>
        /// The timeout period waiting for localization to be completed.
        /// </summary>
        private const float m_timeoutSeconds = 180;

        /// <summary>
        /// Indicates how long a information text will display on the screen before terminating.
        /// </summary>
        private const float m_errorDisplaySeconds = 3;

        /// <summary>
        /// The key name used in PlayerPrefs which indicates whether the privacy prompt has
        /// displayed at least one time.
        /// </summary>
        private const string m_hasDisplayedPrivacyPromptKey = "HasDisplayedGeospatialPrivacyPrompt";

        /// <summary>
        /// The key name used in PlayerPrefs which stores geospatial anchor history data.
        /// The earliest one will be deleted once it hits storage limit.
        /// </summary>
        private const string m_persistentGeospatialAnchorsStorageKey = "PersistentGeospatialAnchors";

        /// <summary>
        /// Help message shown while initializing Geospatial functionalities.
        /// </summary>
        private const string m_localizationInitializingMessage =
            "Initializing Geospatial functionalities.";

        /// <summary>
        /// The limitation of how many Geospatial Anchors can be stored in local storage.
        /// </summary>
        private const int m_storageLimit = 100;

        /// <summary>
        /// Accuracy threshold for orientation yaw accuracy in degrees that can be treated as
        /// localization completed.
        /// </summary>
        private const double m_orientationYawAccuracyThreshold = 25;

        /// <summary>
        /// Accuracy threshold for heading degree that can be treated as localization completed.
        /// </summary>
        private const double m_headingAccuracyThreshold = 25;

        /// <summary>
        /// Accuracy threshold for altitude and longitude that can be treated as localization
        /// completed.
        /// </summary>
        private const double m_horizontalAccuracyThreshold = 20;

        [FormerlySerializedAs("categoryDataSO")] [Header("Settings")] [SerializeField]
        /// <summary>
        /// The CategoryDataSO used in the project.
        /// </summary>
        private CategoryDataSO m_categoryDataSO;

        [FormerlySerializedAs("locationDataSO")] [SerializeField]
        /// <summary>
        /// The LocationDataSO used in the project.
        /// </summary>
        private LocationDataSO m_locationDataSO;

        [FormerlySerializedAs("heading")] [SerializeField]
        /// <summary>
        /// The heading value used in the project.
        /// </summary>
        private double m_heading = 25;

        [FormerlySerializedAs("spawnRadius")] [SerializeField]
        /// <summary>
        /// The spawn radius used in the project (in kilometers).
        /// </summary>
        private double m_spawnRadius = 60;

        [FormerlySerializedAs("locationDataScript")] [SerializeField]
        /// <summary>
        /// The LocationData used in the project.
        /// </summary>
        private LocationData m_locationDataScript;

        [FormerlySerializedAs("localizingIcon")] [SerializeField]
        /// <summary>
        /// The GameObject representing the localizing icon in the project.
        /// </summary>
        private GameObject m_localizingIcon;

        [FormerlySerializedAs("locationInfoPanel")] [SerializeField]
        /// <summary>
        /// The LocationInfoPanel used in the project.
        /// </summary>
        private LocationInfoPanel m_locationInfoPanel;

        [FormerlySerializedAs("locationInfoController")] [SerializeField]
        /// <summary>
        /// The LocationInfoController used in the project.
        /// </summary>
        private LocationInfoController m_locationInfoController;

        [FormerlySerializedAs("popupPanel")] [SerializeField]
        /// <summary>
        /// The PopupPanel used in the project.
        /// </summary>
        private PopupPanel m_popupPanel;

        [FormerlySerializedAs("uiRotate")] [SerializeField]
        /// <summary>
        /// The UIRotate used in the project.
        /// </summary>
        private UIRotate m_uiRotate;

        [Header("Debug")]
        /// <summary>
        /// The GeospatialPose used for debugging in the project.
        /// </summary>
        private GeospatialPose m_debugPose;

        [FormerlySerializedAs("debugLat")] [SerializeField]
        /// <summary>
        /// The debug latitude value in the project.
        /// </summary>
        private double m_debugLat;

        [FormerlySerializedAs("debugLong")] [SerializeField]
        /// <summary>
        /// The debug longitude value in the project.
        /// </summary>
        private double m_debugLong;

        [FormerlySerializedAs("debugAltitude")] [SerializeField]
        /// <summary>
        /// The debug altitude value in the project.
        /// </summary>
        private double m_debugAltitude;

        /// <summary>
        /// Indicates whether markers have been created in the project.
        /// </summary>
        private bool m_createdMarkers = false;

        /// <summary>
        /// Indicates whether a stop delay is active in the project.
        /// </summary>
        private bool m_stopDelay = false;

        /// <summary>
        /// The drawing distance value in the project.
        /// </summary>
        private float m_drawingDistance;

        /// <summary>
        /// Indicates whether the application is waiting for location service in the project.
        /// </summary>
        private bool m_waitingForLocationService = false;

        /// <summary>
        /// Indicates whether the application is in AR view in the project.
        /// </summary>
        private bool m_isInARView = false;

        /// <summary>
        /// Indicates whether the application is returning in the project.
        /// </summary>
        private bool m_isReturning = false;

        /// <summary>
        /// Indicates whether the application is localizing in the project.
        /// </summary>
        private bool m_isLocalizing = false;

        /// <summary>
        /// Indicates whether geospatial functionality is being enabled in the project.
        /// </summary>
        private bool m_enablingGeospatial = false;

        /// <summary>
        /// Indicates whether history should be resolved in the project.
        /// </summary>
        private bool m_shouldResolvingHistory = false;

        /// <summary>
        /// The time passed during localization in the project.
        /// </summary>
        private float m_localizationPassedTime = 0f;

        /// <summary>
        /// The time for configuration preparation in the project.
        /// </summary>
        private float m_configurePrepareTime = 3f;

        /// <summary>
        /// The collection of geospatial anchor history in the project.
        /// </summary>
        private GeospatialAnchorHistoryCollection m_historyCollection = null;

        /// <summary>
        /// List of GPS coordinates on screen space in the project.
        /// </summary>
        private List<GPSCoordsOnScreenSpace> m_anchorObjects = new List<GPSCoordsOnScreenSpace>();

        private IEnumerator m_startLocationService = null;
        private IEnumerator m_asyncCheck = null;

        private GeospatialPose m_currentPose;

        public GeospatialPose CurrentPose => m_currentPose;

        public List<GPSCoordsOnScreenSpace> AnchorObjects
        {
            get { return m_anchorObjects; }
        }


        public float DrawingDistance
        {
            get => m_drawingDistance;
            set => m_drawingDistance = value;
        }

        public List<LocationDataSet> LocationDataSet =>
            m_locationDataSO.DataSet; //.Concat(scavengerLocationDataSO.DataSet).ToList<LocationDataSet>();

        #endregion

        #region UnityEvents

        /// <summary>
        /// Unity's Awake() method.
        /// </summary>
        public void Awake()
        {
            OnGetStartedClicked();

            // Lock screen to portrait.
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.orientation = ScreenOrientation.Portrait;

            // Enable geospatial sample to target 60fps camera capture frame rate
            // on supported devices.
            // Note, Application.targetFrameRate is ignored when QualitySettings.vSyncCount != 0.
            Application.targetFrameRate = 60;

            if (SessionOrigin == null)
            {
                Debug.LogError("Cannot find ARSessionOrigin.");
            }

            if (Session == null)
            {
                Debug.LogError("Cannot find ARSession.");
            }

            if (ARCoreExtensions == null)
            {
                Debug.LogError("Cannot find ARCoreExtensions.");
            }
        }

        /// <summary>
        /// Unity's OnEnable() method.
        /// </summary>
        public void OnEnable()
        {
            SwitchToARView(PlayerPrefs.HasKey(m_hasDisplayedPrivacyPromptKey));
            m_startLocationService = StartLocationService();
            StartCoroutine(m_startLocationService);

            m_isReturning = false;
            m_enablingGeospatial = false;
            InfoPanel.SetActive(false);
            SetAnchorButton.gameObject.SetActive(false);
            ClearAllButton.gameObject.SetActive(false);
            DebugText.gameObject.SetActive(Debug.isDebugBuild && EarthManager != null);

            m_localizationPassedTime = 0f;
            m_isLocalizing = true;
            SnackBarText.text = m_localizingMessage;
            Debug.Log("Set Localizing image on");
            m_localizingIcon.SetActive(true);

#if UNITY_IOS
            Debug.Log("Start location services.");
            Input.location.Start();
#endif

            if (StreetscapeGeometryManager == null)
            {
                Debug.LogWarning("StreetscapeGeometryManager must be set in the " +
                                 "GeospatialController Inspector to render StreetscapeGeometry.");
            }


            StartCoroutine(CheckInternetConnection((isConnected) =>
            {
                if (!isConnected)
                {
                    m_popupPanel.Show(PopupMessageType.NoInternet, () => { SceneManager.LoadScene(0); });
                }
            }));
        }

        /// <summary>
        /// Unity's OnDisable() method.
        /// </summary>
        public void OnDisable()
        {
            StopCoroutine(m_asyncCheck);
            m_asyncCheck = null;
            StopCoroutine(m_startLocationService);
            m_startLocationService = null;
            Debug.Log("Stop location services.");
            Input.location.Stop();
            m_anchorObjects.Clear();
        }

        /// <summary>
        /// Unity's Update() method.
        /// </summary>
        public void Update()
        {
            if (!PlatformsHelper.IsEditor)
            {
                GeoUpdate();
                return;
            }

            if (m_createdMarkers)
            {
                return;
            }
            
            m_createdMarkers = true;
            foreach (LocationDataSet data in LocationDataSet)
            {
                Debug.Log("Test Location: " + data.Info.Name);
                Vector3 _vInfo =
                    new Vector3((float)data.GeoData.Latitude, (float)data.GeoData.Altitude,
                        (float)data.GeoData.Longitude);

                SpawnDebugMarker(GeospatialPrefab, data,
                    ConvertGPStoUCS(new Vector3((float)m_debugLat, (float)m_debugAltitude, (float)m_debugLong),
                        _vInfo)); // Only spawn markers withing the spawn radius
            }

            OnLocalizationCompleted?.Invoke();

            m_createdMarkers = true;
            //StartCoroutine(hunt.LoadSavedStickers());
            m_isLocalizing = false;
            Debug.Log("Set Localizing image off");
            m_localizingIcon.SetActive(false);
            OnTrackingStateUpdate?.Invoke(true);

            Debug.Log("Finish Spawning Markers");
            ShowMarker(0);

        }

        #endregion

        #region PublicMethods

        /// <summary>
        /// Callback handling "Get Started" button click event in Privacy Prompt.
        /// </summary>
        public void OnGetStartedClicked()
        {
            PlayerPrefs.SetInt(m_hasDisplayedPrivacyPromptKey, 1);
            PlayerPrefs.Save();
            SwitchToARView(true);
        }

        /// <summary>
        /// Sets the LocationDataSO (Location Data Scriptable Object) used for marker placement.
        /// </summary>
        /// <param name="locationDataSO">The LocationDataSO to be set.</param>
        public void SetLocationDataSO(LocationDataSO locationDataSO)
        {
            this.m_locationDataSO = locationDataSO;
            Clear();
            m_createdMarkers = false;
        }

        /// <summary>
        /// Callback handling "Learn More" Button click event in Privacy Prompt.
        /// </summary>
        public void OnLearnMoreClicked()
        {
            Application.OpenURL(
                "https://developers.google.com/ar/data-privacy");
        }

        /// <summary>
        /// Callback handling "Clear All" button click event in AR View.
        /// </summary>
        public void OnClearAllClicked()
        {
            foreach (var anchor in m_anchorObjects)
            {
                Destroy(anchor.gameObject);
            }

            m_anchorObjects.Clear();
            m_historyCollection.Collection.Clear();
            SnackBarText.text = "Anchor(s) cleared!";
            ClearAllButton.gameObject.SetActive(false);
            //SaveGeospatialAnchorHistory();
        }

        /// <summary>
        /// Updates the geospatial information and handles Geospatial mode in the project.
        /// </summary>
        public void GeoUpdate()
        {
            if (!m_isInARView)
                return;

            UpdateDebugInfo();

            // Check session error status.
            LifecycleUpdate();
            if (m_isReturning)
                return;

            if (ARSession.state != ARSessionState.SessionInitializing &&
                ARSession.state != ARSessionState.SessionTracking)
            {
                return;
            }

            // Check feature support and enable Geospatial API when it's supported.
            var _featureSupport = EarthManager.IsGeospatialModeSupported(GeospatialMode.Enabled);

            //Debug.Log(featureSupport);
            switch (_featureSupport)
            {
                case FeatureSupported.Unknown:
                    return;
                case FeatureSupported.Unsupported:
                    ReturnWithReason("Geospatial API is not supported by this devices.");
                    m_popupPanel.Show(PopupMessageType.GeospatialAPINotSupported);
                    return;
                case FeatureSupported.Supported:
                    if (ARCoreExtensions.ARCoreExtensionsConfig.GeospatialMode ==
                        GeospatialMode.Disabled)
                    {
                        Debug.Log("Geospatial sample switched to GeospatialMode.Enabled.");
                        ARCoreExtensions.ARCoreExtensionsConfig.GeospatialMode =
                            GeospatialMode.Enabled;
                        ARCoreExtensions.ARCoreExtensionsConfig.StreetscapeGeometryMode =
                            StreetscapeGeometryMode.Enabled;
                        m_configurePrepareTime = 3.0f;
                        m_enablingGeospatial = true;
                        return;
                    }

                    break;
            }

            // Waiting for new configuration taking effect.
            if (m_enablingGeospatial)
            {
                m_configurePrepareTime -= Time.deltaTime;
                if (m_configurePrepareTime < 0)
                {
                    m_enablingGeospatial = false;
                }
                else
                {
                    return;
                }
            }

            // Check earth state.
            var _earthState = EarthManager.EarthState;
            if (_earthState == EarthState.ErrorEarthNotReady)
            {
                SnackBarText.text = m_localizationInitializingMessage;
                return;
            }
            else if (_earthState != EarthState.Enabled)
            {
                string _errorMessage = "Geospatial sample encountered an EarthState error: " + _earthState;
                Debug.LogWarning(_errorMessage);
                SnackBarText.text = _errorMessage;
                return;
            }

            // Check earth localization.
            bool _isSessionReady = ARSession.state == ARSessionState.SessionTracking &&
                                   Input.location.status == LocationServiceStatus.Running;

            var _earthTrackingState = EarthManager.EarthTrackingState;
            m_currentPose = _earthTrackingState == TrackingState.Tracking
                ? EarthManager.CameraGeospatialPose
                : new GeospatialPose();

            if (!_isSessionReady || _earthTrackingState != TrackingState.Tracking ||
                m_currentPose.OrientationYawAccuracy > m_orientationYawAccuracyThreshold ||
                m_currentPose.HorizontalAccuracy > m_horizontalAccuracyThreshold)
            {
                // Lost localization during the session.
                if (!m_isLocalizing)
                {
                    m_uiRotate.enabled = false;
                    OnTrackingStateUpdate?.Invoke(false);
                    m_isLocalizing = true;
                    m_localizingIcon.SetActive(true);
                    m_localizationPassedTime = 0f;
                    SetAnchorButton.gameObject.SetActive(false);
                    ClearAllButton.gameObject.SetActive(false);
                }

                if (m_localizationPassedTime > m_timeoutSeconds)
                {
                    Debug.LogError("Geospatial sample localization passed timeout.");
                    ReturnWithReason(m_localizationFailureMessage);
                    m_popupPanel.Show(PopupMessageType.GeospatialLocalizationTimeout,
                        () => { SceneManager.LoadScene(0); });
                }
                else
                {
                    m_localizationPassedTime += Time.deltaTime;
                    SnackBarText.text = m_localizationInstructionMessage;
                }
            }
            else if (m_isLocalizing)
            {
                OnTrackingStateUpdate?.Invoke(true);
                m_isLocalizing = false;
                m_localizingIcon.SetActive(false);
                m_localizationPassedTime = 0f;

                if (m_createdMarkers == false)
                {
                    PlaceAnchors();
                    OnLocalizationCompleted?.Invoke();
                    m_createdMarkers = true;
                    //StartCoroutine(hunt.LoadSavedStickers());
                }

                SnackBarText.text = m_localizationSuccessMessage;
                m_uiRotate.enabled = true;
                UpdateAnchorsState(true);
            }

            InfoPanel.SetActive(true);
            if (_earthTrackingState == TrackingState.Tracking)
            {
                InfoText.text = string.Format(
                    "Latitude/Longitude: {1}°, {2}°{0}" +
                    "Horizontal Accuracy: {3}m{0}" +
                    "Altitude: {4}m{0}" +
                    "Vertical Accuracy: {5}m{0}" +
                    "Heading: {6}°{0}" +
                    "Heading Accuracy: {7}°",
                    Environment.NewLine,
                    m_currentPose.Latitude.ToString("F6"),
                    m_currentPose.Longitude.ToString("F6"),
                    m_currentPose.HorizontalAccuracy.ToString("F6"),
                    m_currentPose.Altitude.ToString("F2"),
                    m_currentPose.VerticalAccuracy.ToString("F2"),
                    m_currentPose.Heading.ToString("F1"),
                    m_currentPose.HeadingAccuracy.ToString("F1"));
            }
            else
            {
                InfoText.text = "GEOSPATIAL POSE: not tracking";
            }
        }

        /// <summary>
        /// Checks if a location is far from a given radius.
        /// </summary>
        /// <param name="radius">The radius for comparison.</param>
        /// <param name="pose">The GeospatialPose to compare with.</param>
        /// <param name="info">The MarkerInfo for comparison.</param>
        /// <returns>True if the location is far from the radius, false otherwise.</returns>
        public bool isFar(double radius, GeospatialPose pose, MarkerInfo info)
        {
            double _distance = HaversineFormula(pose.Latitude, pose.Longitude, info.Latitude, info.Longitude);
            if (_distance <= radius) return true;
            else return false;
        }

        /// <summary>
        /// Applies a distance filter to anchors based on the maximum distance.
        /// </summary>
        /// <param name="maxDist">The maximum distance for filtering.</param>
        public void ApplyDistanceFilter(float maxDist)
        {
            foreach (var anchor in m_anchorObjects)
            {
                anchor.SetActiveRange(maxDist);
            }
        }

        /// <summary>
        /// Returns the current position based on geospatial data.
        /// </summary>
        /// <returns>The current geospatial position as a Vector3.</returns>
        public Vector3 MyPOS()
        {
            Vector3 _pos = new Vector3();
            GeospatialPose _pose =
                EarthManager.EarthState == EarthState.Enabled &&
                EarthManager.EarthTrackingState == TrackingState.Tracking
                    ? EarthManager.CameraGeospatialPose
                    : new GeospatialPose();

            _pos.x = (float)_pose.Latitude;
            _pos.y = (float)_pose.Longitude;
            _pos.z = (float)_pose.Altitude;

            return _pos;
        }

        /// <summary>
        /// Retrieves an anchor based on geospatial data and invokes a callback when the anchor is created.
        /// </summary>
        /// <param name="GeoData">The geospatial data for the anchor.</param>
        /// <param name="onAnchorCreated">Callback function to execute when the anchor is created.</param>
        public void GetAnchor(MarkerInfo GeoData, Action<GameObject> onAnchorCreated)
        {
            if (!PlatformsHelper.IsEditor)
            {
                var _pose = EarthManager.CameraGeospatialPose;
                if (isFar(m_spawnRadius, _pose, GeoData))
                {
                    StartCoroutine(AvailabilityCheck(GeoData.Latitude, GeoData.Longitude, (isAvailable) =>
                    {
                        if (isAvailable)
                        {
                            switch (GeoData.Type)
                            {
                                case AnchorType.Rooftop:
                                    PlaceARAnchor(GeoData, onAnchorCreated);
                                    break;
                                case AnchorType.Terrain:
                                    PlaceARAnchor(GeoData, onAnchorCreated);
                                    break;
                                case AnchorType.Geospatial:
                                    PlaceGeospatialAnchor(GeoData, onAnchorCreated);
                                    break;
                            }
                        }
                        else
                        {
                            GeoData.Type = AnchorType.Geospatial;
                            PlaceGeospatialAnchor(GeoData, onAnchorCreated);
                        }
                    }));
                }
                return;
            }
            
            //Debug anchor creation

            Quaternion _quaternion = Quaternion.AngleAxis(180f - (float)m_heading, Vector3.up);
            GameObject _anchorGO = new GameObject("Anchor");
            Vector3 _vInfo =
                new Vector3((float)GeoData.Latitude, (float)GeoData.Altitude,
                    (float)GeoData.Longitude);
            Vector3 _pos =
                ConvertGPStoUCS(new Vector3((float)m_debugLat, (float)m_debugAltitude, (float)m_debugLong), _vInfo);
            _anchorGO.transform.position = _pos;
            _anchorGO.transform.rotation = _quaternion;
            onAnchorCreated?.Invoke(_anchorGO);

        }

        /// <summary>
        /// Applies a category filter to the list of anchor objects.
        /// </summary>
        /// <param name="filter">The list of categories to filter by.</param>
        public void ApplyCategoryFilter(List<Category> filter)
        {
            if (filter != null && filter.Count > 0)
            {
                foreach (var anchor in m_anchorObjects)
                {
                    //Check primary categories 
                    bool _isActive = filter.Contains(anchor.GetCategory) || filter.Contains(Category.All);
                    //Check extra options
                    if (!_isActive)
                    {
                        if (anchor.GetExtraCategoryFilter != null && anchor.GetExtraCategoryFilter.Length > 0)
                        {
                            foreach (var category in anchor.GetExtraCategoryFilter)
                            {
                                _isActive = filter.Contains(category);
                                if (_isActive)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    anchor.ApplyCategoryFilter(filter.Contains(anchor.GetCategory) || filter.Contains(Category.All));
                }
            }
            else
            {
                foreach (var anchor in m_anchorObjects)
                {
                    anchor.ApplyCategoryFilter(false);
                }
            }
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="angle">The angle in degrees.</param>
        /// <returns>The angle in radians.</returns>
        public static double ToRad(double angle)
        {
            return Mathf.PI * angle / 180.0;
        }

        /// <summary>
        /// Shows markers of a specified type.
        /// </summary>
        /// <param name="markerType">The type of markers to show.</param>
        public void ShowMarker(int markerType)
        {
            foreach (GPSCoordsOnScreenSpace temp in m_anchorObjects)
            {
                temp.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Converts feet to meters.
        /// </summary>
        /// <param name="feet">The length in feet to convert.</param>
        /// <returns>The length in meters.</returns>
        public static double ConvertFeetToMeters(double feet)
        {
            double _conversionFactor = 0.3048f;
            double _meters = feet * _conversionFactor;
            return _meters;
        }

        #endregion

        #region PrivateMethods

        /// <summary>
        /// Clears all geospatial markers from the scene.
        /// </summary>
        private void Clear()
        {
            foreach (var anchorObj in m_anchorObjects)
            {
                Destroy(anchorObj.gameObject);
            }

            m_anchorObjects.Clear();
        }

        private IEnumerator StartLocationService()
        {
            m_waitingForLocationService = true;
#if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Debug.Log("Requesting the fine location permission.");
                Permission.RequestUserPermission(Permission.FineLocation);
                yield return new WaitForSeconds(3.0f);
            }
#endif

            if (!Input.location.isEnabledByUser)
            {
                Debug.Log("Location service is disabled by the user.");
                m_waitingForLocationService = false;
                yield break;
            }

            Debug.Log("Starting location service.");
            Input.location.Start();

            while (Input.location.status == LocationServiceStatus.Initializing)
            {
                yield return null;
            }

            m_waitingForLocationService = false;
            if (Input.location.status != LocationServiceStatus.Running)
            {
                Debug.LogWarningFormat(
                    "Location service ended with {0} status.", Input.location.status);
                Input.location.Stop();
            }
        }

        /// <summary>
        /// Updates the state of anchors based on tracking state.
        /// </summary>
        /// <param name="isTracking">True if tracking is active, false otherwise.</param>
        private void UpdateAnchorsState(bool isTracking)
        {
            foreach (var go in m_anchorObjects)
            {
                go.gameObject.SetActive(isTracking);
            }
        }

        /// <summary>
        /// Switches to or from AR view.
        /// </summary>
        /// <param name="enable">True to enable AR view, false to disable.</param>
        private void SwitchToARView(bool enable)
        {
            m_isInARView = enable;
            SessionOrigin.gameObject.SetActive(enable);
            Session.gameObject.SetActive(enable);
            ARCoreExtensions.gameObject.SetActive(enable);
            ARViewCanvas.SetActive(enable);
            if (enable && m_asyncCheck == null)
            {
                m_asyncCheck = AvailabilityCheck();
                StartCoroutine(m_asyncCheck);
            }
        }

        /// <summary>
        /// Coroutine to check AR availability and VPS (Visual Positioning Service) availability.
        /// </summary>
        private IEnumerator AvailabilityCheck()
        {
            if (ARSession.state == ARSessionState.None)
            {
                yield return ARSession.CheckAvailability();
            }

            // Waiting for ARSessionState.CheckingAvailability.
            yield return null;

            if (ARSession.state == ARSessionState.NeedsInstall)
            {
                yield return ARSession.Install();
            }

            // Waiting for ARSessionState.Installing.
            yield return null;
#if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Debug.Log("Requesting camera permission.");
                Permission.RequestUserPermission(Permission.Camera);
                yield return new WaitForSeconds(3.0f);
            }

            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                // User has denied the request.
                Debug.LogWarning(
                    "Failed to get the camera permission. VPS availability check isn't available.");
                yield break;
            }
#endif

            while (m_waitingForLocationService)
            {
                yield return null;
            }

            if (Input.location.status != LocationServiceStatus.Running)
            {
                Debug.LogWarning(
                    "Location services aren't running. VPS availability check is not available.");
                yield break;
            }

            // Update event is executed before coroutines so it checks the latest error states.
            if (m_isReturning)
            {
                yield break;
            }

            var _location = Input.location.lastData;
            var _vpsAvailabilityPromise =
                AREarthManager.CheckVpsAvailabilityAsync(_location.latitude, _location.longitude);
            yield return _vpsAvailabilityPromise;

            Debug.LogFormat("VPS Availability at ({0}, {1}): {2}",
                _location.latitude, _location.longitude, _vpsAvailabilityPromise.Result);
        }

        /// <summary>
        /// Updates the various aspects of the app's lifecycle.
        /// </summary>
        private void LifecycleUpdate()
        {
            // Pressing 'back' button quits the app.
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Application.Quit();
            }

            if (m_isReturning)
            {
                return;
            }

            // Only allow the screen to sleep when not tracking.
            var _sleepTimeout = SleepTimeout.NeverSleep;
            if (ARSession.state != ARSessionState.SessionTracking)
            {
                _sleepTimeout = SleepTimeout.SystemSetting;
            }

            Screen.sleepTimeout = _sleepTimeout;

            // Quit the app if ARSession is in an error status.
            string returningReason = string.Empty;
            if (ARSession.state != ARSessionState.CheckingAvailability &&
                ARSession.state != ARSessionState.Ready &&
                ARSession.state != ARSessionState.SessionInitializing &&
                ARSession.state != ARSessionState.SessionTracking)
            {
                returningReason = string.Format(
                    "Geospatial sample encountered an ARSession error state {0}.\n" +
                    "Please start the app again.",
                    ARSession.state);
            }
#if UNITY_IOS
            else if (Input.location.status == LocationServiceStatus.Failed)
            {
                returningReason =
                    "Geospatial sample failed to start location service.\n" +
                    "Please start the app again and grant precise location permission.";
            }
#endif
            else if (SessionOrigin == null || Session == null || ARCoreExtensions == null)
            {
                returningReason = string.Format(
                    "Geospatial sample failed with missing AR Components.");
            }

            ReturnWithReason(returningReason);
        }

        /// <summary>
        /// Handles returning from the app and displays a reason if applicable.
        /// </summary>
        /// <param name="reason">The reason for returning or null if not applicable.</param>
        private void ReturnWithReason(string reason)
        {
            if (string.IsNullOrEmpty(reason))
            {
                return;
            }

            SetAnchorButton.gameObject.SetActive(false);
            ClearAllButton.gameObject.SetActive(false);
            InfoPanel.SetActive(false);

            Debug.LogError(reason);
            SnackBarText.text = reason;
            m_isReturning = true;
        }

        /// <summary>
        /// Quits the application.
        /// </summary>
        private void QuitApplication()
        {
            Application.Quit();
        }

        /// <summary>
        /// Updates debug information displayed in the app.
        /// </summary>
        private void UpdateDebugInfo()
        {
            if (!Debug.isDebugBuild || EarthManager == null)
            {
                return;
            }

            GeospatialPose _pose =
                EarthManager.EarthState == EarthState.Enabled &&
                EarthManager.EarthTrackingState == TrackingState.Tracking
                    ? EarthManager.CameraGeospatialPose
                    : new GeospatialPose();
            FeatureSupported _supported = EarthManager.IsGeospatialModeSupported(GeospatialMode.Enabled);
            DebugText.text =
                $"IsReturning: {m_isReturning}\n" +
                $"IsLocalizing: {m_isLocalizing}\n" +
                $"SessionState: {ARSession.state}\n" +
                $"LocationServiceStatus: {Input.location.status}\n" +
                $"FeatureSupported: {_supported}\n" +
                $"EarthState: {EarthManager.EarthState}\n" +
                $"EarthTrackingState: {EarthManager.EarthTrackingState}\n" +
                $"  LAT/LNG: {_pose.Latitude:F6}, {_pose.Longitude:F6}\n" +
                $"  HorizontalAcc: {_pose.HorizontalAccuracy:F6}\n" +
                $"  ALT: {_pose.Altitude:F2}\n" +
                $"  VerticalAcc: {_pose.VerticalAccuracy:F2}\n" +
                $"  Heading: {_pose.Heading:F2}\n" +
                $"  HeadingAcc: {_pose.HeadingAccuracy:F2}";
        }

        /// <summary>
        /// Places anchors in the scene based on location data.
        /// </summary>
        private void PlaceAnchors()
        {
            foreach (LocationDataSet data in LocationDataSet)
            {
                GetAnchor(data.GeoData, (anchor) => { SetContentToAnchor(data, anchor.gameObject); });
            }
        }

        // <summary>
        /// Places a geospatial anchor based on marker information and invokes a callback when the anchor is created.
        /// </summary>
        /// <param name="data">The marker information containing geospatial data.</param>
        /// <param name="onAnchorCreated">Callback function to execute when the anchor is created.</param>
        private void PlaceGeospatialAnchor(MarkerInfo data, Action<GameObject> onAnchorCreated)
        {
            StartCoroutine(PlaceGeospatial(data, onAnchorCreated));
        }

        /// <summary>
        /// Coroutine to place a geospatial anchor based on marker information and invoke a callback when the anchor is created.
        /// </summary>
        /// <param name="markerInfo">The marker information containing geospatial data.</param>
        /// <param name="onAnchorCreated">Callback function to execute when the anchor is created.</param>
        private IEnumerator PlaceGeospatial(MarkerInfo markerInfo, Action<GameObject> onAnchorCreated)
        {
            Quaternion _quaternion = Quaternion.AngleAxis(180f - (float)m_heading, Vector3.up);
            double _altitudeInM = ConvertFeetToMeters(markerInfo.Altitude);
            ARGeospatialAnchor _anchor =
                AnchorManager.AddAnchor(markerInfo.Latitude, markerInfo.Longitude, _altitudeInM + markerInfo.Offset,
                    _quaternion);

            yield return new WaitForSeconds(2);
            onAnchorCreated?.Invoke(_anchor.gameObject);
        }

        /// <summary>
        /// Places an AR anchor based on marker information and invokes a callback when the anchor is created.
        /// </summary>
        /// <param name="GeoData">The marker information containing geospatial data.</param>
        /// <param name="onAnchorCreated">Callback function to execute when the anchor is created.</param>
        /// <param name="pose">The pose of the anchor (optional).</param>
        /// <param name="trackableId">The trackable ID of the anchor (optional).</param>
        /// <returns>The AR anchor that was created.</returns>
        private ARAnchor PlaceARAnchor(MarkerInfo GeoData, Action<GameObject> onAnchorCreated = null,
            Pose pose = new Pose(),
            TrackableId trackableId = new TrackableId())
        {
            ARAnchor _anchor = null;
            Quaternion _eunRotation = Quaternion.AngleAxis(180f - (float)m_heading, Vector3.up);

            GeospatialAnchorHistory _history = new GeospatialAnchorHistory(
                GeoData.Latitude, GeoData.Longitude, GeoData.Altitude, GeoData.Type,
                EarthManager.CameraGeospatialPose.EunRotation);

            switch (GeoData.Type)
            {
                case AnchorType.Rooftop:
                    ResolveAnchorOnRooftopPromise _rooftopPromise =
                        AnchorManager.ResolveAnchorOnRooftopAsync(
                            _history.Latitude, _history.Longitude,
                            GeoData.Offset, _eunRotation);
                    StartCoroutine(CheckRooftopPromise(GeoData, _rooftopPromise, _history, onAnchorCreated));
                    return null;

                case AnchorType.Terrain:
                    ResolveAnchorOnTerrainPromise _terrainPromise =
                        AnchorManager.ResolveAnchorOnTerrainAsync(
                            _history.Latitude, _history.Longitude,
                            GeoData.Offset, _eunRotation);

                    StartCoroutine(CheckTerrainPromise(GeoData, _terrainPromise, _history, onAnchorCreated));
                    return null;
            }

            return _anchor;
        }

        /// <summary>
        /// Coroutine to check and handle the promise for resolving a rooftop anchor.
        /// </summary>
        /// <param name="data">The marker information containing geospatial data.</param>
        /// <param name="promise">The promise for resolving the anchor on a rooftop.</param>
        /// <param name="history">The history of the geospatial anchor.</param>
        /// <param name="onAnchorCreated">Callback function to execute when the anchor is created.</param>
        private IEnumerator CheckRooftopPromise(MarkerInfo data, ResolveAnchorOnRooftopPromise promise,
            GeospatialAnchorHistory history, Action<GameObject> onAnchorCreated = null)
        {
            yield return promise;
            var _result = promise.Result;
            if (_result.RooftopAnchorState == RooftopAnchorState.Success &&
                _result.Anchor != null)
            {
                onAnchorCreated?.Invoke(_result.Anchor.gameObject);
            }

            yield break;
        }

        /// <summary>
        /// Coroutine to check and handle the promise for resolving a terrain anchor.
        /// </summary>
        /// <param name="data">The marker information containing geospatial data.</param>
        /// <param name="promise">The promise for resolving the anchor on terrain.</param>
        /// <param name="history">The history of the geospatial anchor.</param>
        /// <param name="onAnchorCreated">Callback function to execute when the anchor is created.</param>
        private IEnumerator CheckTerrainPromise(MarkerInfo data, ResolveAnchorOnTerrainPromise promise,
            GeospatialAnchorHistory history, Action<GameObject> onAnchorCreated = null)
        {
            yield return promise;

            var _result = promise.Result;
            if (_result.TerrainAnchorState == TerrainAnchorState.Success &&
                _result.Anchor != null)
            {
                onAnchorCreated?.Invoke(_result.Anchor.gameObject);
            }

            yield break;
        }

        /// <summary>
        /// Spawns a debug marker at a specified location with offset.
        /// </summary>
        /// <param name="prefab">The prefab for the debug marker.</param>
        /// <param name="data">The location data for the marker.</param>
        /// <param name="offset">The offset to apply to the marker's position.</param>
        private void SpawnDebugMarker(GameObject prefab, LocationDataSet data, Vector3 offset)
        {
            MarkerInfo _markerInfo = data.GeoData;
            Quaternion _quaternion = Quaternion.AngleAxis(180f - (float)m_heading, Vector3.up);
            GPSCoordsOnScreenSpace _anchorGO =
                Instantiate(prefab, offset, _quaternion).GetComponent<GPSCoordsOnScreenSpace>();

            _anchorGO.SetActiveRange(m_drawingDistance);

            m_locationDataScript.LocationDictionary.TryGetValue(data.GeoData.LocationID,
                out LocationDataSet locDataSet);

            LocationInfo _locData;
            if (locDataSet == null)
            {
                _locData = data.Info;
            }
            else
            {
                _locData = locDataSet.Info;
            }

            CategoryInfo _categoryInfo =
                m_categoryDataSO.Categories.Find(x => x.CategoryType.Equals(_locData.Category));
            Sprite _categoryIcon = null;

            if (_categoryInfo != null)
            {
                _categoryIcon = _categoryInfo.Icon;
            }

            if (_locData.ID != null && data.GeoData != null && _categoryIcon != null)
            {
                _anchorGO.SetMarkerInfo(data.GeoData, _locData, _categoryIcon, m_locationInfoPanel,
                    m_locationInfoController);
                m_anchorObjects.Add(_anchorGO);
            }
            else
            {
                Debug.LogError("Location Not Found: " + _locData.ID);
            }
        }

        /// <summary>
        /// Sets content to an anchor object based on location data and a GameObject anchor.
        /// </summary>
        /// <param name="data">The location data for setting content to the anchor.</param>
        /// <param name="anchor">The GameObject representing the anchor.</param>
        private void SetContentToAnchor(LocationDataSet data, GameObject anchor)
        {
            if (anchor != null)
            {
                GPSCoordsOnScreenSpace _anchorGO = Instantiate(GeospatialPrefab, anchor.transform)
                    .GetComponent<GPSCoordsOnScreenSpace>();

                _anchorGO.SetGeospatialController(this);
                _anchorGO.transform.position = anchor.transform.position;
                _anchorGO.SetActiveRange(m_drawingDistance);
                m_anchorObjects.Add(_anchorGO);
                Debug.Log(m_anchorObjects.Count);

                m_locationDataScript.LocationDictionary.TryGetValue(data.GeoData.LocationID,
                    out LocationDataSet locDataSet);

                LocationInfo _locData;
                if (locDataSet == null)
                {
                    _locData = data.Info;
                }
                else
                {
                    _locData = locDataSet.Info;
                }

                CategoryInfo _categoryInfo =
                    m_categoryDataSO.Categories.Find(x => x.CategoryType.Equals(_locData.Category));

                Sprite _categoryIcon = null;
                if (_categoryInfo != null)
                {
                    _categoryIcon = _categoryInfo.Icon;
                }

                _anchorGO.SetMarkerInfo(data.GeoData, _locData, _categoryIcon, m_locationInfoPanel,
                    m_locationInfoController);
            }

            ShowMarker(0);
        }


        /// <summary>
        /// Calculates the distance between two GPS coordinates using the Haversine formula.
        /// </summary>
        /// <param name="pos1">The first GPS coordinates Vector3 type</param>
        /// <param name="pos2">The second GPS coordinates Vector3 type</param>
        /// <returns>The calculated distance in kilometers.</returns>
        private double HaversineFormula(Vector3 pos1, Vector3 pos2)
        {
            return (HaversineFormula(pos1.x, pos1.y, pos2.x, pos2.y));
        }

        /// <summary>
        /// Calculates the distance between two GPS coordinates using the Haversine formula.
        /// </summary>
        /// <param name="pos1">The first GPS coordinates Vector2 type</param>
        /// <param name="pos2">The second GPS coordinates Vector2 type</param>
        /// <returns>The calculated distance in kilometers.</returns>
        private double HaversineFormula(Vector2 pos1, Vector2 pos2)
        {
            return (HaversineFormula(pos1.x, pos1.y, pos2.x, pos2.y));
        }

        /// <summary>
        /// Calculates the distance between two GPS coordinates using the Haversine formula.
        /// </summary>
        /// <param name="lat1">The latitude of the first GPS coordinate.</param>
        /// <param name="lon1">The longitude of the first GPS coordinate.</param>
        /// <param name="lat2">The latitude of the second GPS coordinate.</param>
        /// <param name="lon2">The longitude of the second GPS coordinate.</param>
        /// <returns>The calculated distance in kilometers.</returns>
        private double HaversineFormula(double lat1, double lon1, double lat2, double lon2)
        {
            double _distance = 0;
            double _earthRadius = 6372.8; // radius of the earth in km
            double _dLat = ToRad(lat2 - lat1);
            double _dLon = ToRad(lon2 - lon1);
            lat1 = ToRad(lat1);
            lat2 = ToRad(lat2);

            var _a = Mathf.Sin((float)_dLat / 2) * Mathf.Sin((float)_dLat / 2) + Mathf.Sin((float)_dLon / 2) *
                Mathf.Sin((float)_dLon / 2) * Mathf.Cos((float)lat1) * Mathf.Cos((float)lat2);
            var _c = 2 * Mathf.Asin(Mathf.Sqrt(_a));

            _distance = _earthRadius * _c;
            return _distance;
        }

        /// <summary>
        /// Converts GPS coordinates to Unity coordinate system (UCS) with respect to a specified origin.
        /// </summary>
        /// <param name="origin">The origin GPS coordinates.</param>
        /// <param name="loc">The GPS coordinates to convert.</param>
        /// <returns>The converted Unity coordinates.</returns>
        private Vector3 ConvertGPStoUCS(Vector3 origin, Vector3 loc)
        {
            //var metersLatLong = FindMetersPerLat(origin.x);
            //float zPosition = metersLatLong.x * (loc.x - origin.x); //Calc current lat
            //float xPosition = metersLatLong.y * (loc.y - origin.y); //Calc current lat
            return new Vector3((float)(loc.x - origin.x) * 10000, (float)(loc.y - origin.y) / 10f,
                (float)(loc.z - origin.z) * 10000);
        }

        /// <summary>
        /// Performs an availability check for VPS (Visual Positioning Service) at a specific GPS location.
        /// </summary>
        /// <param name="latitude">The latitude of the GPS location.</param>
        /// <param name="longitude">The longitude of the GPS location.</param>
        /// <param name="onChecked">Callback function to execute when the check is complete.</param>
        /// <returns>An IEnumerator for the coroutine.</returns>
        private IEnumerator AvailabilityCheck(double latitude, double longitude, Action<bool> onChecked)
        {
            var _vpsAvailabilityPromise =
                AREarthManager.CheckVpsAvailabilityAsync(latitude, longitude);
            yield return _vpsAvailabilityPromise;
            onChecked?.Invoke(_vpsAvailabilityPromise.Result == VpsAvailability.Available);
        }

        /// <summary>
        /// Checks the internet connection and invokes a callback with the result.
        /// </summary>
        /// <param name="action">Callback function to execute with the internet connection status.</param>
        /// <returns>An IEnumerator for the coroutine.</returns>
        private IEnumerator CheckInternetConnection(Action<bool> action)
        {
            UnityWebRequest _request = new UnityWebRequest("https://google.com");
            yield return _request.SendWebRequest();
            action(_request.error == null);
        }

        #endregion
    }
}