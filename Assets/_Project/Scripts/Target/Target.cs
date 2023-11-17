using System;
using DG.Tweening;
using UnityEngine;

namespace DAS.Urway
{
    /// <summary>
    /// Controls the behavior and interactions of a target object.
    /// </summary>
    public class Target : MonoBehaviour
    {
        public Action OnLoaded;

        private RepresentationObject m_representationObject; // The representation object associated with the target.
        private PlacedMarkerInfo m_markerInfo; // Information about the placed marker.
        private float m_fillingTime = 2; // Time taken for filling the loading outline.
        private Tween m_loadingTween; // Tween for loading animation.
        private bool m_isActive; // Indicates whether the target is active.

        public bool IsActive // Gets or sets the active state of the target.
        {
            get => m_isActive;
            set => m_isActive = value;
        }

        /// <summary>
        /// Initializes the target with representation object and marker info.
        /// </summary>
        /// <param name="ro">Representation object associated with the target.</param>
        /// <param name="info">Placed marker information.</param>
        /// <returns>No expected outputs.</returns>
        public void Init(RepresentationObject ro, PlacedMarkerInfo info)
        {
            m_representationObject = ro;
            m_markerInfo = info;
        }


        /// <summary>
        /// Selects the target and invokes relevant callbacks.
        /// </summary>
        /// <param name="OnSelect">Callback on selection.</param>
        /// <param name="onLoadedCallback">Callback when loaded.</param>
        /// <returns>No expected outputs.</returns>
        public void Select(Action<PlacedMarkerInfo> OnSelect, Action<PlacedMarkerInfo> onLoadedCallback)
        {
            Debug.Log("Select POI");
            if (IsActive)
            {
                if (m_representationObject != null)
                {
                    OnSelect?.Invoke(m_markerInfo);
                    //representationObject.ShowTitle(true);
                    m_loadingTween = m_representationObject.Outline.DOFillAmount(1, m_fillingTime).OnComplete(() =>
                    {
                        onLoadedCallback?.Invoke(m_markerInfo);
                        m_representationObject.Outline.fillAmount = 0;
                    }).SetEase(Ease.InQuad);
                }
                else
                {
                    Debug.Log("RepresentationObject is null");
                }
            }
        }

        /// <summary>
        /// Unselects the target and resets loading animation.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        public void Unselect()
        {
            Debug.Log("Unselect POI");
            if (m_loadingTween != null)
            {
                m_loadingTween.Kill();
            }

            m_representationObject.Outline.fillAmount = 0;
            //representationObject.ShowTitle(false);
        }
    }
}
