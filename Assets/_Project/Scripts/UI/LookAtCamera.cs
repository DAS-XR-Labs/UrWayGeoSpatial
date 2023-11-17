using UnityEngine;

namespace DAS.Urway
{
    public class LookAtCamera : MonoBehaviour
    {
        private Camera mainCamera; // Reference to the main camera

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        void Start()
        {
            mainCamera = Camera.main;
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        void Update()
        {
            if (mainCamera != null)
            {
                transform.LookAt(mainCamera.transform.position);
            }
        }
    }
}
