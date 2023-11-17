using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace DAS.Urway
{
    /// <summary>
    /// Controls the behavior of a target pointer that interacts with location info and objects.
    /// </summary>
    public class TargetPointer : MonoBehaviour
    {
        [FormerlySerializedAs("locationInfoController")] [SerializeField]
        private LocationInfoController
            m_locationInfoController; // The LocationInfoController responsible for managing location information.

        [FormerlySerializedAs("locationInfoPanel")] [SerializeField]
        private LocationInfoPanel
            m_locationInfoPanel; // The LocationInfoPanel responsible for displaying location information panel.

        [FormerlySerializedAs("routeMe")] [SerializeField] private RouteMe m_routeMe; // The RouteMe component for handling route tracking.
        [FormerlySerializedAs("pointer")] [SerializeField] private GameObject m_pointer; // The GameObject representing the pointer.

        private float m_detectionDistance = Mathf.Infinity; // Detection distance for the pointer.
        private GameObject m_lastTarget; // The last target GameObject interacted with by the pointer.
        private bool m_isTracking; // Indicates if the pointer is actively tracking


        [FormerlySerializedAs("pointerTextParentObject")] public GameObject PointerTextParentObject; // Parent GameObject of the pointer's text display.
        [FormerlySerializedAs("pointerText")] public TextMeshProUGUI PointerText; // TextMeshProUGUI component displaying the pointer's text.


        /// <summary>
        /// Initializes tracking state and text display for the pointer.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void Start()
        {
            m_isTracking = true;
            if (PointerTextParentObject != null && PointerText == null)
                PointerText = PointerTextParentObject.GetComponentInChildren<TextMeshProUGUI>();
            PointerText.text = "";
            PointerTextParentObject.SetActive(false);
        }

        /// <summary>
        /// Subscribes to necessary events when the object becomes enabled and active.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnEnable()
        {
            m_locationInfoPanel.OnOpen += OnLocationInfoPanelOpen;
            m_locationInfoPanel.OnClose += OnLocationInfoPanelClose;
            m_locationInfoPanel.OnClose += OnLocationInfoPanelClose;
            RepresentationObject.OnPOIButtonClick += OnRepresentationObjectButtonClose;
        }

        /// <summary>
        /// Event handler when the location info panel opens.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnLocationInfoPanelOpen()
        {
            m_isTracking = false;
            m_pointer.SetActive(m_isTracking);
        }


        /// <summary>
        /// Event handler when the location info panel closes.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnLocationInfoPanelClose()
        {
            m_isTracking = true;
            m_pointer.SetActive(m_isTracking);
        }


        /// <summary>
        /// Event handler when a representation object button is closed.
        /// </summary>
        /// <param name="info">Location information associated with the button.</param>
        /// <param name="markerInfo">Location geo information associated with the button.</param>
        /// <returns>No expected outputs.</returns>
        private void OnRepresentationObjectButtonClose(LocationInfo info, MarkerInfo markerInfo) //PlacedMarkerInfo placedMarkerInfo
        {
            m_locationInfoController.SetData(info, markerInfo);
            //if (lastTarget) lastTarget.Unselect();
            //locationInfoController.SetData(placedMarkerInfo);
            m_locationInfoPanel.ShowShortInfoBox();
        }

        /// <summary>
        /// Unsubscribes from events when the object becomes disabled and inactive.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnDisable()
        {
            m_locationInfoPanel.OnOpen -= OnLocationInfoPanelOpen;
            m_locationInfoPanel.OnClose -= OnLocationInfoPanelClose;
            RepresentationObject.OnPOIButtonClick -= OnRepresentationObjectButtonClose;
        }

        /// <summary>
        /// Updates the behavior of the target pointer based on tracking and interactions.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void Update()
        {
            if (m_isTracking && !m_routeMe.IsTracking)
            {
                RaycastHit _hit;
                Transform _targetPointScreenPosition = Camera.main.transform;

                //FIER: Throw a filter on this so it only hit's labels / Miners
                if (Physics.Raycast(_targetPointScreenPosition.position,
                    _targetPointScreenPosition.TransformDirection(Vector3.forward), out _hit, m_detectionDistance))
                {
                    if (_hit.transform.gameObject.layer == 6)
                    {
                        if (m_lastTarget == null)
                        {
                            m_lastTarget = _hit.transform.gameObject;
                            var _POI = m_lastTarget.GetComponentInParent<GPSCoordsOnScreenSpace>();

                            PointerText.text = _POI.locationName;
                            PointerTextParentObject.SetActive(true);
                            _POI.SelectPOI();
                        }
                        else
                        {
                            if (m_lastTarget != _hit.transform.gameObject)
                            {
                                m_lastTarget.GetComponentInParent<GPSCoordsOnScreenSpace>().UnselectPOI();
                                m_lastTarget = null;
                                m_lastTarget = _hit.transform.gameObject;
                                var _POI = m_lastTarget.GetComponentInParent<GPSCoordsOnScreenSpace>();

                                PointerText.text = _POI.locationName;
                                PointerTextParentObject.SetActive(true);
                                _POI.SelectPOI();
                            }
                        }
                    }
                }
                else
                {
                    if (m_lastTarget != null)
                    {
                        m_lastTarget.GetComponentInParent<GPSCoordsOnScreenSpace>().UnselectPOI();
                        m_lastTarget = null;

                        PointerTextParentObject.SetActive(false);
                        PointerText.text = "";
                    }
                }
            }
        }
    }
}