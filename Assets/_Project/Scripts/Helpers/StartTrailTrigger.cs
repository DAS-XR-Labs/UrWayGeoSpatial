using UnityEngine;
using UnityEngine.Serialization;

namespace DAS.Urway
{
    /// <summary>
    /// Triggers the start of a trail route when a GameObject enters the trigger zone.
    /// </summary>
    public class StartTrailTrigger : MonoBehaviour
    {
        [FormerlySerializedAs("latitude")] [SerializeField] private double m_latitude; // The latitude coordinate for the trail route start point.
        [FormerlySerializedAs("longitude")] [SerializeField] private double m_longitude; // The longitude coordinate for the trail route start point.
        [FormerlySerializedAs("routeMe")] [SerializeField] private RouteMe m_routeMe; // The RouteMe component responsible for managing the route.

        private bool m_isTriggered; // Flag to keep track of whether the trigger has been activated.

        /// <summary>
        /// Called when a GameObject enters the trigger zone.
        /// Checks the trigger condition and initiates the trail route if applicable.
        /// </summary>
        /// <param name="other">The collider of the entering GameObject.</param>
        private void OnTriggerEnter(Collider other)
        {
            CheckTrigger(other.gameObject);
        }

        /// <summary>
        /// Called when a GameObject stays in the trigger zone.
        /// Checks the trigger condition and initiates the trail route if applicable.
        /// </summary>
        /// <param name="other">The collider of the staying GameObject.</param>
        private void OnTriggerStay(Collider other)
        {
            CheckTrigger(other.gameObject);
        }

        /// <summary>
        /// Called when a GameObject exits the trigger zone.
        /// Resets the trigger state.
        /// </summary>
        /// <param name="other">The collider of the exiting GameObject.</param>
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject != null && other.gameObject.tag == "StartTrailTrigger")
            {
                m_isTriggered = false;
            }
        }

        /// <summary>
        /// Checks if the trigger conditions are met and starts the trail route if not already triggered.
        /// </summary>
        /// <param name="target">The GameObject that entered the trigger zone.</param>
        private void CheckTrigger(GameObject target)
        {
            if (!m_isTriggered)
            {
                if (target != null && target.tag == "StartTrailTrigger")
                {
                    m_isTriggered = true;
                    m_routeMe.EndPoint.Location.Label = "Visitor center";
                    m_routeMe.EndPoint.Location.Altitude = 1f;
                    m_routeMe.EndPoint.Location.Latitude = m_latitude;
                    m_routeMe.EndPoint.Location.Longitude = m_longitude;
                    m_routeMe.InitRoute();
                }
            }
        }
    }
}