using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DAS.Urway
{
    /// <summary>
    /// Controls the behavior of the navigation panel in the UI.
    /// </summary>
    public class NavigationPanel : MonoBehaviour
    {
        [FormerlySerializedAs("_routeMe")] [SerializeField] private RouteMe m_routeMe; // The RouteMe component for route management.
        [FormerlySerializedAs("endRouteButton")] [SerializeField] private Button m_endRouteButton; // The Button component for the end route button.


        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void Start()
        {
            m_endRouteButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnEnable()
        {
            m_endRouteButton.onClick.AddListener(EndRouteButtonOnClick);
            m_routeMe.OnRouteStart += OnStartRoute;
        }

        /// <summary>
        /// Called when the object becomes disabled and inactive.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnDisable()
        {
            m_endRouteButton.onClick.RemoveListener(EndRouteButtonOnClick);
            m_routeMe.OnRouteStart -= OnStartRoute;
        }

        /// <summary>
        /// Handles the click event of the end route button.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        public void EndRouteButtonOnClick()
        {
            Debug.Log("Ending Route");
            m_endRouteButton.gameObject.SetActive(false);
            m_routeMe.StopRoute();
        }

        /// <summary>
        /// Called when a route is started.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnStartRoute()
        {
            m_endRouteButton.gameObject.SetActive(true);
        }
    }
}