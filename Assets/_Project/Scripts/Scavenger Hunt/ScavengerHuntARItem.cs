using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace DAS.Urway.ScavengerHuntMode
{
    /// <summary>
    /// Represents an AR item in a scavenger hunt.
    /// </summary>
    public class ScavengerHuntARItem : MonoBehaviour
    {
        /// <summary>
        /// Event that is triggered when the AR item is clicked.
        /// </summary>
        /// <param name="id">The ID of the scavenger hunt item.</param>
        /// <param name="scavengerHuntInfo">Information about the scavenger hunt item.</param>
        public Action<string, ScavengerHuntInfo> OnClick;

        /// <summary>
        /// The duration of the spawn animation.
        /// </summary>
        [FormerlySerializedAs("spawnAnimationDuration")]
        public float SpawnAnimationDuration;

        /// <summary>
        /// The easing function used for the spawn animation.
        /// </summary>
        [FormerlySerializedAs("easing")] public Ease Easing;

        private ScavengerHuntInfo m_scavengerHuntInfo;
        private string m_id;
        private Vector3 m_initScale;

        private void Awake()
        {
            m_initScale = transform.localScale;
        }

        private void OnEnable()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(m_initScale, SpawnAnimationDuration).SetEase(Easing);
        }

        /// <summary>
        /// Initializes the AR item with the given ID and scavenger hunt information.
        /// </summary>
        /// <param name="id">The ID of the scavenger hunt item.</param>
        /// <param name="scavengerHuntInfo">Information about the scavenger hunt item.</param>
        public void Init(string id, ScavengerHuntInfo scavengerHuntInfo)
        {
            m_id = id;
            m_scavengerHuntInfo = scavengerHuntInfo;
        }

        private void Update()
        {
            if (PlatformsHelper.IsEditor)
            {
                // Check for mouse input in the editor
                if (Input.GetMouseButtonDown(0))
                {
                    Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit _hit;
                    if (Physics.Raycast(_ray, out _hit) && _hit.collider.gameObject == gameObject)
                    {
                        OnClick?.Invoke(m_id, m_scavengerHuntInfo);
                    }
                }

                return;
            }

            if (Input.touchCount > 0)
            {
                Touch _touch = Input.GetTouch(0);
                if (_touch.phase == TouchPhase.Began)
                {
                    Ray _ray = Camera.main.ScreenPointToRay(_touch.position);
                    RaycastHit _hit;
                    if (Physics.Raycast(_ray, out _hit) && _hit.collider.gameObject == gameObject)
                    {
                        OnClick?.Invoke(m_id, m_scavengerHuntInfo);
                    }
                }
            }
        }
    }
}