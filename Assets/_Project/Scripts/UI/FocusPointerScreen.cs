using UnityEngine;
using UnityEngine.Serialization;

namespace DAS.Urway
{
    public class FocusPointerScreen : MonoBehaviour
    {
        // Object that holds yMin/yMax
        [FormerlySerializedAs("screenArrow")] [SerializeField]
        private Transform
            m_screenArrow; // The Transform representing the object that holds yMin/yMax values for the pointer.

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void Awake()
        {
            m_screenArrow.gameObject.SetActive(false);
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnEnable()
        {

        }

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void Start()
        {

        }

        /// <summary>
        /// Initializes the focus pointer screen with an AR camera.
        /// </summary>
        /// <param name="arCamera">The AR camera to use.</param>
        /// <returns>No expected outputs.</returns>
        private void Init(Camera arCamera)
        {
        }

        /// <summary>
        /// Sets the focus target and enables tracking.
        /// </summary>
        /// <param name="target">The transform of the focus target.</param>
        /// <returns>No expected outputs.</returns>
        public void SetTargetSet(Transform target)
        {
            EnableTracking(target);
        }

        /// <summary>
        /// Disables tracking and removes the focus target.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        public void RemoveTarget()
        {
            DisableTracking();
        }

        /// <summary>
        /// Enables tracking of the focus target.
        /// </summary>
        /// <param name="target">The transform of the focus target.</param>
        /// <returns>No expected outputs.</returns>
        public void EnableTracking(Transform target)
        {
            m_screenArrow.gameObject.SetActive(true);
        }

        /// <summary>
        /// Disables tracking of the focus target.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        public void DisableTracking()
        {
            m_screenArrow.gameObject.SetActive(false);
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void Update()
        {

        }

        /// <summary>
        /// Called when the object becomes disabled and inactive.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnDisable()
        {

        }
    }
}