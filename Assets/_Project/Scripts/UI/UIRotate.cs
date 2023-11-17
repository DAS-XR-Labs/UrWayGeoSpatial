using UnityEngine;
using UnityEngine.Serialization;

namespace DAS.Urway
{
    /// <summary>
    /// Controls the UI rotation based on the device orientation.
    /// </summary>
    public class UIRotate : MonoBehaviour
    {
        private DeviceOrientation m_currentOrientation; // The current device orientation.

        [FormerlySerializedAs("distanceFilterPanel")] [SerializeField]
        private DistanceFilterPanel m_distanceFilterPanel; // The DistanceFilterPanel used to update rotation.

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        void Start()
        {
            m_currentOrientation = Input.deviceOrientation;
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        void Update()
        {
            DeviceOrientation _orientation = Input.deviceOrientation;

            if (m_currentOrientation == _orientation)
                return;

            switch (_orientation)
            {
                case DeviceOrientation.Portrait:
                    Screen.orientation = ScreenOrientation.Portrait;
                    break;
                case DeviceOrientation.PortraitUpsideDown:
                    Screen.orientation = ScreenOrientation.PortraitUpsideDown;
                    break;
                case DeviceOrientation.LandscapeLeft:
                    Screen.orientation = ScreenOrientation.LandscapeLeft;
                    break;
                case DeviceOrientation.LandscapeRight:
                    Screen.orientation = ScreenOrientation.LandscapeRight;
                    break;
                default:
                    Screen.orientation = ScreenOrientation.Portrait;
                    break;
            }

            m_currentOrientation = _orientation;

            m_distanceFilterPanel.RotateUpdate(Input.deviceOrientation);

            /* foreach( GPSCoordsOnScreenSpace poi in distanceFilterPanel.GEOSpacialController.AnchorObjects)
                poi.orientationResize.ScaleUpdate(Input.deviceOrientation); */
        }

    }
}
