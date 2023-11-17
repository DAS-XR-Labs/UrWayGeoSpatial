using UnityEngine;
using UnityEngine.Serialization;

namespace DAS.Urway
{
    /// <summary>
    /// Controls the resizing of a UI element based on the device orientation.
    /// </summary>
    public class POIOrientationResize : MonoBehaviour
    {
        [FormerlySerializedAs("portraitScale")] [SerializeField] private float m_portraitScale = 1f; // The scale factor to apply in portrait orientation.
        [FormerlySerializedAs("landscapeOffset")] [SerializeField] private float m_landscapeOffset = 2f; // The scale factor to apply in landscape orientation.
        private RectTransform m_rect; // The RectTransform component of the UI element.

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        void Start()
        {
            m_rect = GetComponent<RectTransform>();
        }

        /// <summary>
        /// Updates the scale of the UI element based on the current device orientation.
        /// </summary>
        /// <param name="currentOrientation">The current device orientation.</param>
        /// <returns>No expected outputs.</returns>
        public void ScaleUpdate(DeviceOrientation currentOritentation)
        {
            switch (currentOritentation)
            {
                case DeviceOrientation.Portrait:
                    transform.localScale = new Vector3(m_portraitScale, m_portraitScale, m_portraitScale);
                    break;
                case DeviceOrientation.PortraitUpsideDown:
                    transform.localScale = new Vector3(m_portraitScale, m_portraitScale, m_portraitScale);
                    break;
                case DeviceOrientation.LandscapeLeft:
                    transform.localScale = new Vector3(m_landscapeOffset, m_landscapeOffset, m_landscapeOffset);
                    break;
                case DeviceOrientation.LandscapeRight:
                    transform.localScale = new Vector3(m_landscapeOffset, m_landscapeOffset, m_landscapeOffset);
                    break;
                default:
                    transform.localScale = new Vector3(m_portraitScale, m_portraitScale, m_portraitScale);
                    break;
            }
        }
    }
}