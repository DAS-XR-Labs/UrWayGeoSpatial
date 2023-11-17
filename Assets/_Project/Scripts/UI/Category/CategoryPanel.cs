using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace DAS.Urway
{
    public class CategoryPanel : MonoBehaviour
    {
        // The CategoryDataSO instance associated with this CategoryPanel
        [FormerlySerializedAs("categoryDataSO")] [SerializeField] private CategoryDataSO m_categoryDataSO;
        // The prefab that will be used for each category item's content.
        [FormerlySerializedAs("contentPrefab")] [SerializeField] private Transform m_contentPrefab;
        // The prefab for individual category items.
        [FormerlySerializedAs("itemPrefab")] [SerializeField] private GameObject m_itemPrefab;
        // The GeospatialController instance associated with this CategoryPanel.
        [FormerlySerializedAs("geospatialController")] [SerializeField] private GeospatialController m_geospatialController;

        [FormerlySerializedAs("scavengerHunt")] [SerializeField] private ScavengerHunt m_scavengerHunt;

        [FormerlySerializedAs("rootObject")] [SerializeField] private GameObject m_rootObject; // The root object of the panel.
        [FormerlySerializedAs("hideOnStart")] [SerializeField] private bool m_hideOnStart; // Flag to control whether the panel is hidden on start.
        [FormerlySerializedAs("allActiveButton")] [SerializeField] private CategoryItem m_allActiveButton; // The button representing "All Active" categories.

        private List<CategoryItem> m_panelItems = new List<CategoryItem>(); // List of category items in the panel.
        private List<Category> m_filters = new List<Category>(); // List of filters for category selection.

        /// <summary>
        /// Initializes the CategoryPanel by populating the list, setting up filters, and applying initial settings.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void Start()
        {
            PopulateList();
            m_rootObject.SetActive(!m_hideOnStart);
            InitFilter();
            m_geospatialController.ApplyCategoryFilter(m_filters);
        }

        /// <summary>
        /// Subscribes to necessary events when the script is enabled.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnEnable()
        {
            m_geospatialController.OnTrackingStateUpdate += OnTrackingStateUpdate;
            m_allActiveButton.OnClicked += AllActiveButtonClick;
        }

        /// <summary>
        /// Unsubscribes from events when the script is disabled.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnDisable()
        {
            m_geospatialController.OnTrackingStateUpdate -= OnTrackingStateUpdate;
            m_allActiveButton.OnClicked -= AllActiveButtonClick;
        }


        /// <summary>
        /// Initializes the filter by making allActiveButton active and adding Category.All to filters.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        public void InitFilter()
        {
            m_allActiveButton.Click();
            m_filters.Add(Category.All);
        }

        /// <summary>
        /// Handles updates to the visibility of the panel based on AR tracking state.
        /// </summary>
        /// <param name="isTracking">Flag indicating whether AR tracking is active.</param>
        /// <returns>No expected outputs.</returns>
        private void OnTrackingStateUpdate(bool isTracking)
        {
            m_rootObject.SetActive(isTracking);
        }

      

        /// <summary>
        /// Populates the list of category items by instantiating and initializing them.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void PopulateList()
        {
            for (int i = 0; i < m_categoryDataSO.Categories.Count; i++)
            {
                CategoryItem _categoryItem = Instantiate(m_itemPrefab, m_contentPrefab).GetComponent<CategoryItem>();
                _categoryItem.Init(m_categoryDataSO.Categories[i]);
                _categoryItem.OnClicked += OnCategoryClicked;
                m_panelItems.Add(_categoryItem);
            }
        }

        /// <summary>
        /// Handles the event when a category is clicked, updating the filter settings.
        /// </summary>
        /// <param name="category">The clicked category.</param>
        /// <param name="isSelected">Flag indicating whether the category is selected.</param>
        /// <returns>No expected outputs.</returns>
        private void OnCategoryClicked(Category category, bool isSelected)
        {
            if (isSelected)
            {
                m_filters.Add(category);
                if (m_filters.Contains(Category.All))
                {
                    m_filters.Remove(Category.All);
                    m_allActiveButton.Click();
                }
            }
            else
            {
                m_filters.Remove(category);
            }


            m_geospatialController.ApplyCategoryFilter(m_filters);

            if (m_scavengerHunt.enabled)
            {
                m_scavengerHunt.UpdateCategoryVisibility(m_filters.Contains(Category.All) || m_filters.Contains(Category.ScavengerHunt));
            }
        }

        /// <summary>
        /// Handles the event when the "All Active" button is clicked, updating the filter settings.
        /// </summary>
        /// <param name="category">The "All Active" category.</param>
        /// <param name="enable">Flag indicating whether to enable "All Active".</param>
        /// <returns>No expected outputs.</returns>
        private void AllActiveButtonClick(Category category, bool enable)
        {
            if (enable)
            {
                for (int i = 0; i < m_panelItems.Count; i++)
                {
                    if (m_filters.Contains(m_panelItems[i].CategoryType))
                    {
                        m_panelItems[i].Click();
                    }
                }

                m_filters.Clear();
                if (!m_filters.Contains(Category.All))
                {
                    m_filters.Add(Category.All);
                }
            }
            else
            {
                if (m_filters.Contains(Category.All))
                {
                    m_filters.Remove(Category.All);
                }
            }


            m_geospatialController.ApplyCategoryFilter(m_filters);
        }

        /// <summary>
        /// Releases resources and clears event listeners.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void Release()
        {
            foreach (var item in m_panelItems)
            {
                item.OnClicked -= OnCategoryClicked;
                Destroy(item.gameObject);
            }

            m_panelItems.Clear();
        }
    }
}