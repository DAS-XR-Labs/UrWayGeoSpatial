using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DAS.Urway
{
    /// <summary>
    /// Controls the behavior of a slider with step increments in the UI.
    /// Implements interfaces for handling drag and pointer events.
    /// </summary>
    public class SliderStep : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [FormerlySerializedAs("slider")] [SerializeField] private Slider m_slider; // The Slider component for controlling the slider.
        [FormerlySerializedAs("stepSize")] [SerializeField] private float m_stepSize = .25f; // The step size by which the slider value is incremented.
        [FormerlySerializedAs("sliderLabel")] [SerializeField] private GameObject m_sliderLabel; // The GameObject that displays the slider label.

        private bool m_isDragging = false; // Flag indicating whether the slider is being dragged.
        [FormerlySerializedAs("roundedValue")] [SerializeField] private float m_roundedValue; // The rounded value of the slider after applying step increments.


        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void Start()
        {
            m_slider.onValueChanged.AddListener(OnSliderValueChanged);
            m_sliderLabel.SetActive(false);
        }

        /// <summary>
        /// Called when the slider value changes.
        /// Rounds the value to the nearest step increment.
        /// </summary>
        /// <param name="value">The new slider value.</param>
        /// <returns>No expected outputs.</returns>
        private void OnSliderValueChanged(float value)
        {
            m_roundedValue = Mathf.Round(value / m_stepSize) * m_stepSize;
        }

        /// <summary>
        /// Called when the user begins dragging the slider.
        /// </summary>
        /// <param name="eventData">The pointer event data.</param>
        /// <returns>No expected outputs.</returns>
        public void OnBeginDrag(PointerEventData eventData)
        {
            // sliderLabel.SetActive(true);
        }

        /// <summary>
        /// Called when the user stops dragging the slider.
        /// </summary>
        /// <param name="eventData">The pointer event data.</param>
        /// <returns>No expected outputs.</returns>
        public void OnEndDrag(PointerEventData eventData)
        {
            // slider.SetValueWithoutNotify(roundedValue);
            m_sliderLabel.SetActive(false);
        }

        /// <summary>
        /// Called when the user presses the pointer down on the slider.
        /// </summary>
        /// <param name="eventData">The pointer event data.</param>
        /// <returns>No expected outputs.</returns>
        public void OnPointerDown(PointerEventData eventData)
        {
            m_sliderLabel.SetActive(true);
        }

        /// <summary>
        /// Called when the user releases the pointer on the slider.
        /// </summary>
        /// <param name="eventData">The pointer event data.</param>
        /// <returns>No expected outputs.</returns>
        public void OnPointerUp(PointerEventData eventData)
        {
            // slider.SetValueWithoutNotify(roundedValue);
            m_sliderLabel.SetActive(false);
        }
    }
}