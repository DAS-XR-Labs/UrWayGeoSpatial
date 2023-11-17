using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DAS.Urway
{
    /// <summary>
    /// Represents a selectable category item. Handles interactions and UI updates for category items.
    /// </summary>
    public class CategoryItem : MonoBehaviour
    {
        #region Parameters
        public Action<Category, bool> OnClicked; // Event triggered when the category item is clicked.

        [FormerlySerializedAs("rectTransform")] [SerializeField] private RectTransform m_rectTransform; // The rect transform of the category item.
        [FormerlySerializedAs("icon")] [SerializeField] private Image m_icon; // The icon representing the category.
        [FormerlySerializedAs("name")] [SerializeField] private TMP_Text m_name; // The text displaying the category name.
        [FormerlySerializedAs("button")] [SerializeField] private Button m_button; // The button component for interactions.
        [FormerlySerializedAs("closeIndicator")] [SerializeField] private GameObject m_closeIndicator; // The indicator for showing selection.

        [FormerlySerializedAs("defaultColor")] [SerializeField] private Color m_defaultColor; // The default color for the category item.
        [FormerlySerializedAs("selectColor")] [SerializeField] private Color m_selectColor; // The color when the category is selected.
        [FormerlySerializedAs("isSelectedOnStart")] [SerializeField] private bool m_isSelectedOnStart; // Flag indicating if the category is selected on start.

        [FormerlySerializedAs("textDefaultColor")] [SerializeField] private Color m_textDefaultColor; // The default color for text.
        [FormerlySerializedAs("textSelectColor")] [SerializeField] private Color m_textSelectColor; // The color for text when the category is selected.


        private Category m_category; // The type of category represented by this item.
        private float m_minSize = 200; // The minimum size of the category item.
        private float m_widthCorrection = 150; // Correction factor for width calculation.

        private bool m_isSelected; // Flag indicating if the category is currently selected.

        public Category CategoryType => m_category; // The type of category represented by this item.
        #endregion

        /// <summary>
        /// Start method initializes the default state of the category item.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void Start()
        {
            m_isSelected = false;
            m_button.targetGraphic.color = m_defaultColor;
            m_closeIndicator.SetActive(false);
            if (m_isSelectedOnStart)
            {
                Click();
            }

            m_name.color = m_textDefaultColor;
        }

        /// <summary>
        /// OnEnable method. Adds a listener for the button click event.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnEnable()
        {
            m_button.onClick.AddListener(OnButtonClick);
            StartCoroutine(FitSize());
        }

        /// <summary>
        /// Handles the button click event of the category item.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnButtonClick()
        {
            Click();
            OnClicked?.Invoke(m_category, m_isSelected);
        }

        /// <summary>
        /// Handles the click event of the category item, changing its selection state.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        public void Click()
        {
            m_isSelected = !m_isSelected;
            m_button.targetGraphic.color = m_isSelected ? m_selectColor : m_defaultColor;
            m_closeIndicator.SetActive(m_isSelected);

            m_name.color = m_isSelected ? m_textSelectColor : m_textDefaultColor;


        }

        /// <summary>
        /// Initializes the CategoryItem.
        /// </summary>
        /// <param name="info">Information about the category.</param>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        public void Init(CategoryInfo info)
        {
            m_category = info.CategoryType;
            m_name.text = info.Name;
            if (info.Icon != null)
            {
                m_icon.sprite = info.Icon;
            }

            StartCoroutine(FitSize());
        }

        /// <summary>
        /// Coroutine that adjusts the size of the category item to fit its content.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private IEnumerator FitSize()
        {
            yield return new WaitForEndOfFrame();
            float _newSize = m_widthCorrection + m_name.bounds.size.x;
            float _preferredWidth = _newSize > m_minSize ? _newSize : m_minSize;
            m_rectTransform.sizeDelta = new Vector2(_preferredWidth, m_rectTransform.sizeDelta.y);
        }

        [ContextMenu("UpdateUI")]
        public void UpdateUI()
        {
            StartCoroutine(FitSize());
        }

        /// <summary>
        /// OnDisable method. Removes the listener for the button click event.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnDisable()
        {
            m_button.onClick.RemoveListener(OnButtonClick);
        }
    }
}