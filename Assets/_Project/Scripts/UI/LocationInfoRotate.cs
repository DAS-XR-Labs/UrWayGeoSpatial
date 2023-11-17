using UnityEngine;
using UnityEngine.Serialization;

namespace DAS.Urway
{
    /// <summary>
    /// Controls the rotation of the location information panel based on device orientation.
    /// </summary>
    public class LocationInfoRotate : MonoBehaviour
    {
        [FormerlySerializedAs("content")] public RectTransform Content; // The RectTransform component of the content within the panel.
        [FormerlySerializedAs("alternateMenu")] public RectTransform AlternateMenu; // The RectTransform component of an alternate menu.
        private DeviceOrientation CurrentOrientation; // The current device orientation.

        [FormerlySerializedAs("screenSize")] public Vector2 ScreenSize; // The size of the screen.

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        void Start()
        {
            ScreenSizeUpdate();
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void Update()
        {
            if (!PlatformsHelper.IsEditor)
            {
                if (CurrentOrientation == Input.deviceOrientation)
                {
                    return;
                }

                ScreenSizeUpdate();
                CurrentOrientation = Input.deviceOrientation;
            }
            else
            {
                ScreenSizeUpdate(); // For Editor Testing 
            }
        }

        /// <summary>
        /// Updates the screen size based on device orientation changes.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void ScreenSizeUpdate()
        {
            ScreenSize = new Vector2(Screen.width, Screen.height);
        }
    }
}
