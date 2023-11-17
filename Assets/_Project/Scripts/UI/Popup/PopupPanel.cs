using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DAS.Urway
{
    /// <summary>
    /// Controls the behavior of a popup panel in the UI.
    /// </summary>
    public class PopupPanel : MonoBehaviour
    {
        [FormerlySerializedAs("panelRootObject")] [SerializeField] private GameObject m_panelRootObject; // The root GameObject of the panel.

        [FormerlySerializedAs("titleText")] [Header("UI Elements")] [SerializeField]
        private TMP_Text m_titleText; // The TMP_Text component for the title text in the panel.

        [FormerlySerializedAs("messageText")] [SerializeField] private TMP_Text m_messageText; // The TMP_Text component for the message text in the panel.
        [FormerlySerializedAs("okButton")] [SerializeField] private Button m_okButton; // The Button component for the OK button in the panel.

        [FormerlySerializedAs("popupMessages")] [Header("Data")] [SerializeField]
        private List<PopupMessage> m_popupMessages; // The list of predefined popup messages

        private Dictionary<PopupMessageType, PopupMessage>
            m_tMessages; // A dictionary that maps PopupMessageType to PopupMessage data.

        private Action m_okButtonCallback; // The callback action for the OK button.

        [FormerlySerializedAs("testType")] [SerializeField] private PopupMessageType m_testType; // The predefined PopupMessageType for testing.

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void Awake()
        {
            m_panelRootObject.SetActive(false);
            Init();
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnEnable()
        {
            m_okButtonCallback = null;
            m_okButton.onClick.AddListener(OnOkButtonClick);
            //panelRootObject.SetActive(false);
        }

        /// <summary>
        /// Initializes the dictionary of PopupMessage data.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void Init()
        {
            m_tMessages = new Dictionary<PopupMessageType, PopupMessage>();
            foreach (var item in m_popupMessages)
            {
                m_tMessages.Add(item.PopupMessageType, item);
            }
        }

        /// <summary>
        /// Called when the object becomes disabled and inactive.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnDisable()
        {
            m_okButton.onClick.RemoveListener(OnOkButtonClick);
        }

        /// <summary>
        /// Handles the click event of the OK button.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnOkButtonClick()
        {
            m_okButtonCallback?.Invoke();
            Hide();
        }

        /// <summary>
        /// Shows the popup panel with the specified message type.
        /// </summary>
        /// <param name="messageType">The PopupMessageType to show.</param>
        /// <param name="onCloseCallback">The callback action for when the panel is closed.</param>
        /// <param name="overrideCallback">Flag to override the callback with the provided callback.</param>
        /// <returns>No expected outputs.</returns>
        public void Show(PopupMessageType messageType, Action onCloseCallback = null, bool overrideCallback = true)
        {
            if (m_tMessages.ContainsKey(messageType))
            {
                PopupMessage _messageData = m_tMessages[messageType];
                m_titleText.text = _messageData.Title;
                m_messageText.text = _messageData.Text;
                if (overrideCallback)
                {
                    m_okButtonCallback = onCloseCallback;
                }

                m_panelRootObject.SetActive(true);
            }
        }

        /// <summary>
        /// Hides the popup panel.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        public void Hide()
        {
            m_panelRootObject.SetActive(false);
            m_okButtonCallback = null;
        }

        /// <summary>
        /// Test method to show the popup panel with a predefined message type.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        [ContextMenu("Test")]
        public void Test()
        {
            Show(m_testType);
        }
    }
}