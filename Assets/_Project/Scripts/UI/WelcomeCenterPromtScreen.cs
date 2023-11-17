using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DAS.Urway
{
    /// <summary>
    /// Manages the behavior of the welcome center prompt screen.
    /// </summary>
    public class WelcomeCenterPromtScreen : MonoBehaviour
    {
        [FormerlySerializedAs("okButton")] [SerializeField] private Button m_okButton; // The Button component for the "OK" button.
        [FormerlySerializedAs("promtTextRootObject")] [SerializeField] private GameObject m_promtTextRootObject; // The root GameObject for the prompt text.

        [FormerlySerializedAs("focusPointerScreen")] [SerializeField]
        private FocusPointerScreen m_focusPointerScreen; // The FocusPointerScreen component for handling focus pointers.

        [FormerlySerializedAs("welcomeCenterArrow")] [SerializeField] private GameObject m_welcomeCenterArrow; // The GameObject representing the welcome center arrow.

        private Action m_onOkButtonClicked; // Action to execute when the "OK" button is clicked.

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnEnable()
        {
            m_promtTextRootObject.SetActive(false);
        }

        /// <summary>
        /// Called when the object becomes disabled and inactive.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnDisable()
        {
        }

        /// <summary>
        /// Event handler for the "OK" button click.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        public void OnOkButtonClicked()
        {
            m_promtTextRootObject.SetActive(false);
            m_focusPointerScreen.RemoveTarget();
            m_welcomeCenterArrow.SetActive(false);
        }

        /// <summary>
        /// Activates the welcome center prompt with the specified focus target.
        /// </summary>
        /// <param name="focusTarget">Transform of the focus target.</param>
        /// <param name="onDismissButtonClicked">Action to execute when the dismiss button is clicked.</param>
        /// <returns>No expected outputs.</returns>
        public void ActivateWelcomeCenterPromt(Transform focusTarget, Action onDismissButtonClicked)
        {
            m_promtTextRootObject.SetActive(true);
            m_focusPointerScreen.SetTargetSet(focusTarget);
        }
    }
}