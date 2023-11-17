using UnityEngine;
using UnityEngine.Serialization;

namespace DAS.Urway
{
    /// <summary>
    /// This class animates the phone image back and forth on the screen.
    ///
    /// Replace with an animator asap.
    /// </summary>
    public class BackForth : MonoBehaviour
    {
        public float HorizontalSpeed = 1; // The horizontal speed of the animation.
        [FormerlySerializedAs("target1")] public RectTransform Target1; // The first target position for animation.
        [FormerlySerializedAs("target2")] public RectTransform Target2; // The second target position for animation.
        public float MaxHorizontalPosition = 200; // The maximum horizontal position for animation
        public float MinHorizontalPosition = -200; // The minimum horizontal position for animation.
        [FormerlySerializedAs("moveToTarget1")] public bool MoveToTarget1; // Indicates whether the animation should move to target1.
        private RectTransform m_rectTransform; // The RectTransform component of the object.


        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        void Start()
        {
            MoveToTarget1 = true;
            m_rectTransform = GetComponent<RectTransform>();
        }

        /// <summary>
        /// Update is called once per frame. Animates the RectTransform to move between target positions.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        void Update()
        {
            var _step = HorizontalSpeed * Time.deltaTime;

            if (m_rectTransform.localPosition.x > MaxHorizontalPosition && MoveToTarget1)
            {
                MoveToTarget1 = false;
            }
            else if (m_rectTransform.localPosition.x < MinHorizontalPosition && !MoveToTarget1)
            {
                MoveToTarget1 = true;
            }
            else if (MoveToTarget1)
            {
                transform.position = Vector3.MoveTowards(transform.position, Target1.position, _step);
            }

            else if (!MoveToTarget1)
            {
                transform.position = Vector3.MoveTowards(transform.position, Target2.position, _step);
            }
        }
    }
}