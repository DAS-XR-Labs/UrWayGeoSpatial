using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace DAS.Urway
{
    public class ArrowMovement : MonoBehaviour
    {
        [FormerlySerializedAs("amplitude")] [SerializeField] private float m_amplitude; // Amplitude of the arrow's vertical movement.
        [FormerlySerializedAs("rotation")] [SerializeField] private Vector3 m_rotation; // Euler rotation angles for the arrow's rotation.
        [FormerlySerializedAs("rotationSpeed")] [SerializeField] private float m_rotationSpeed; // Rotation speed of the arrow.
        private Vector3 m_arrowPos; // Initial position of the arrow.

        /// <summary>
        /// Handles the movement and rotation of an arrow object.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void Start()
        {
            m_arrowPos = transform.localPosition;
        }

        /// <summary>
        /// Updates the arrow's position and rotation in the game loop.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        void Update()
        {
            transform.localPosition = new Vector3(m_arrowPos.x, Mathf.Sin(Time.time) * m_amplitude, m_arrowPos.z);
            transform.Rotate(m_rotation * m_rotationSpeed * Time.deltaTime);
        }
    }
}