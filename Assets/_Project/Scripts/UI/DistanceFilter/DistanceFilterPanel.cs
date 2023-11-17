using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DAS.Urway
{
    public class DistanceFilterPanel : MonoBehaviour
    {
        private string m_lableTextFormat = "{0:#.##} mi"; // The format for the label text displaying distance values.
        RectTransform m_rect; // The RectTransform component attached to this Distance Filter panel.

        [FormerlySerializedAs("geospatialController")] [SerializeField]
        private GeospatialController
            m_geospatialController; // The GeospatialController instance associated with this Distance Filter panel.

        public GeospatialController
            m_geoSpacialController // A public property to access the GeospatialController instance.
        {
            get { return m_geospatialController; }
        }

        [FormerlySerializedAs("rootObject")] [SerializeField] private GameObject m_rootObject; // The root object of the Distance Filter panel.
        [FormerlySerializedAs("distanceSlider")] [SerializeField] private Slider m_distanceSlider; // The slider used to adjust the distance filter value.
        [FormerlySerializedAs("defaultValue")] [SerializeField] private float m_defaultValue; // The default value for the distance slider.

        [FormerlySerializedAs("sliderValueText")] [SerializeField]
        private TMP_Text m_sliderValueText; // The text displaying the current value of the distance slider.

        [FormerlySerializedAs("hideOnStart")] [SerializeField] private bool m_hideOnStart; // Whether to hide the Distance Filter panel on start.

        [FormerlySerializedAs("portraitOffset")] [Header("UI Rotate")] [SerializeField]
        private float m_portraitOffset = -550f; // The offset for the panel's position in portrait orientation.

        [FormerlySerializedAs("landscapeOffset")] [SerializeField]
        private float m_landscapeOffset = -600f; // The offset for the panel's position in landscape orientation.

        private float m_oneMile = 1609.344f; // The conversion factor to convert miles to meters.

        /// <summary>
        /// Initializes the DistanceFilterPanel. Sets up initial values for UI elements, subscribes to events.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void Start()
        {
            m_rect = GetComponent<RectTransform>();
            m_distanceSlider.value = m_defaultValue;
            m_geospatialController.DrawingDistance = ConvertMilesToMeters(m_distanceSlider.value);
            UpdateSliderLabel(m_distanceSlider.value);
            m_rootObject.SetActive(!m_hideOnStart);
            OnSliderValuesChanged(m_distanceSlider.value);
        }

        /// <summary>
        /// Handles updates to the visibility of the panel based on AR tracking state.
        /// </summary>
        /// <param name="isTracking">Flag indicating whether AR tracking is active.</param>
        /// <returns>No expected outputs.</returns>
        private void OnTrackingStateUpdate(bool isTracking)
        {
            m_rootObject.SetActive(isTracking);
        }

        /// <summary>
        /// Subscribes to events when the script is enabled.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnEnable()
        {
            m_distanceSlider.onValueChanged.AddListener(OnSliderValuesChanged);
            m_geospatialController.OnTrackingStateUpdate += OnTrackingStateUpdate;
        }

        /// <summary>
        /// Updates the distance filter value and UI label when the slider value changes.
        /// </summary>
        /// <param name="value">New value of the slider.</param>
        /// <returns>No expected outputs.</returns>
        private void OnSliderValuesChanged(float value)
        {
            m_geospatialController.ApplyDistanceFilter(ConvertMilesToMeters(value));
            UpdateSliderLabel(value);
        }

        /// <summary>
        /// Unsubscribes from events when the script is disabled.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnDisable()
        {
            m_distanceSlider.onValueChanged.RemoveListener(OnSliderValuesChanged);
            m_geospatialController.OnTrackingStateUpdate -= OnTrackingStateUpdate;
        }

        /// <summary>
        /// Converts a distance value from miles to meters.
        /// </summary>
        /// <param name="miles">Distance value in miles.</param>
        /// <returns>Converted distance value in meters.</returns>
        private float ConvertMilesToMeters(float miles)
        {
            if (miles == 0f)
                return 0f;

            return miles * m_oneMile;
        }

        /// <summary>
        /// Updates the UI slider label with the provided value.
        /// </summary>
        /// <param name="value">New value to be displayed on the slider label.</param>
        /// <returns>No expected outputs.</returns>
        private void UpdateSliderLabel(float value)
        {
            m_sliderValueText.text = string.Format(m_lableTextFormat, value);
        }

        /// <summary>
        /// Adjusts the position and offset of the panel for a specific orientation.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        public void RectCheck()
        {
            m_rect.anchoredPosition = new Vector3(m_landscapeOffset, m_rect.localPosition.y, m_rect.localPosition.z);
            m_rect.offsetMin = new Vector2(m_rect.offsetMin.x, 500f);
        }

        /// <summary>
        /// Updates the position and offset of the panel based on the device's orientation.
        /// </summary>
        /// <param name="currentOritentation">Current orientation of the device.</param>
        /// <returns>No expected outputs.</returns>
        public void RotateUpdate(DeviceOrientation currentOritentation)
        {
            switch (currentOritentation)
            {
                case DeviceOrientation.Portrait:
                    m_rect.anchoredPosition = new Vector3(m_portraitOffset, m_rect.localPosition.y, m_rect.localPosition.z);
                    m_rect.offsetMin = new Vector2(m_rect.offsetMin.x, 50f);
                    break;
                case DeviceOrientation.PortraitUpsideDown:
                    m_rect.anchoredPosition = new Vector3(m_portraitOffset, m_rect.localPosition.y, m_rect.localPosition.z);
                    m_rect.offsetMin = new Vector2(m_rect.offsetMin.x, 50f);
                    break;
                case DeviceOrientation.LandscapeLeft:
                    m_rect.anchoredPosition = new Vector3(m_landscapeOffset, m_rect.localPosition.y, m_rect.localPosition.z);
                    m_rect.offsetMin = new Vector2(m_rect.offsetMin.x, 500f);
                    break;
                case DeviceOrientation.LandscapeRight:
                    m_rect.anchoredPosition = new Vector3(m_landscapeOffset, m_rect.localPosition.y, m_rect.localPosition.z);
                    m_rect.offsetMin = new Vector2(m_rect.offsetMin.x, 500f);
                    break;
                default:
                    m_rect.anchoredPosition = new Vector3(m_portraitOffset, m_rect.localPosition.y, m_rect.localPosition.z);
                    m_rect.offsetMin = new Vector2(m_rect.offsetMin.x, 50f);
                    break;
            }
        }
    }
}