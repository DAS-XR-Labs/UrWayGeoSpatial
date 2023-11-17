using DAS.Urway.Deals;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DAS.Urway
{
    public class ActiveDealsPanel : MonoBehaviour
    {
        [FormerlySerializedAs("routeMe")] [SerializeField] private RouteMe m_routeMe; // Reference to the RouteMe component.
        [FormerlySerializedAs("LocationInfoPanel")] [SerializeField] private LocationInfoPanel m_locationInfoPanel; // Reference to the LocationInfoPanel.

        [FormerlySerializedAs("rootObject")] [SerializeField] private GameObject m_rootObject; // Root object of the active deals panel.
        [FormerlySerializedAs("hideOnStart")] [SerializeField] private bool m_hideOnStart; // Flag indicating whether to hide the panel on start.

        [FormerlySerializedAs("dealCompany")] [SerializeField] private TMP_Text m_dealCompany; // Text displaying the deal company's name.
        [FormerlySerializedAs("dealText")] [SerializeField] private TMP_Text m_dealText; // Text displaying the deal description.
        [FormerlySerializedAs("dealCard")] [SerializeField] private GameObject m_dealCard; // GameObject representing the deal card.
        [FormerlySerializedAs("navigateButton")] [SerializeField] private Button m_navigateButton; // Button for navigating to the deal's location.

        [FormerlySerializedAs("descriptionPanel")] [SerializeField] private GameObject m_descriptionPanel; // Panel displaying the description of the deal.
        [FormerlySerializedAs("descriptionPanelGo")] [SerializeField] private Button m_descriptionPanelGo; // Button to close the description panel.

        [FormerlySerializedAs("descriptionPanelText")] [SerializeField]
        private TMP_Text m_descriptionPanelText; // Text displaying the description in the description panel.

        [FormerlySerializedAs("descriptionPanelTitle")] [SerializeField] private TMP_Text m_descriptionPanelTitle; // Text displaying the title in the description panel.

        private DealData m_currentDeal; // Represents the currently displayed deal.

        /// <summary>
        /// Awake method to initialize the initial state of the panel.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void Awake()
        {
            m_rootObject.SetActive(!m_hideOnStart);
            m_descriptionPanel.SetActive(false);
        }

        /// <summary>
        /// Displays the active deals panel.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        public void Show()
        {
            Clear();
            m_rootObject.SetActive(true);
        }

        /// <summary>
        /// Displays the active deals panel with specific deal data.
        /// </summary>
        /// <param name="dealData">The deal data to display.</param>
        /// <returns>No expected outputs.</returns>
        public void Show(DealData dealData)
        {
            m_currentDeal = dealData;
            InitRoteMe(m_currentDeal);
            m_dealCompany.text = dealData.Company_name;
            m_dealText.text = dealData.Deal_description;
            m_rootObject.SetActive(true);
            m_dealCard.SetActive(true);
        }

        /// <summary>
        /// Initializes the RouteMe component with deal data.
        /// </summary>
        /// <param name="dealData">The deal data to initialize with.</param>
        /// <returns>No expected outputs.</returns>
        private void InitRoteMe(DealData dealData)
        {
            if (m_currentDeal != null)
            {
                m_routeMe.EndPoint.Location.Label = dealData.Company_name;
                m_routeMe.EndPoint.Location.Altitude = 1f;
                m_routeMe.EndPoint.Location.Latitude = dealData.Location.Lat;
                m_routeMe.EndPoint.Location.Longitude = dealData.Location.Lng;
            }
        }

        /// <summary>
        /// Displays the description panel with deal data and location.
        /// </summary>
        /// <param name="dealData">The deal data for which to display the description panel.</param>
        /// <param name="location">The location information.</param>
        /// <returns>No expected outputs.</returns>
        public void ShowDescriptionPanel(DealData dealData, Vector2 location)
        {
            m_currentDeal = dealData;
            InitRoteMe(m_currentDeal);
            m_descriptionPanelText.text = dealData.Deal_description;
            m_descriptionPanelTitle.text = dealData.Company_name;
            m_descriptionPanel.SetActive(true);
        }

        /// <summary>
        /// Hides the active deals panel.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        public void Hide()
        {
            Clear();
            m_rootObject.SetActive(false);

        }

        /// <summary>
        /// Unity OnEnable method. Adds event listeners.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnEnable()
        {

            m_descriptionPanelGo.onClick.AddListener(OnDescriptionPanelGoClicked);
            m_navigateButton.onClick.AddListener(OnNavigateButtonClicked);
        }

        /// <summary>
        /// Unity OnDisable method. Removes event listeners and clears state.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnDisable()
        {
            Clear();

            m_descriptionPanelGo.onClick.RemoveListener(OnDescriptionPanelGoClicked);
            m_navigateButton.onClick.RemoveListener(OnNavigateButtonClicked);

        }

        /// <summary>
        /// Handles the click event of the description panel's "Go" button.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnDescriptionPanelGoClicked()
        {
            m_descriptionPanel.SetActive(false);
            m_routeMe.InitRoute();
        }

        /// <summary>
        /// Handles the click event of the navigate button.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnNavigateButtonClicked()
        {
            m_locationInfoPanel.Close();
            m_routeMe.InitRoute();
            Hide();
        }

        /// <summary>
        /// Clears the displayed deal information.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        public void Clear()
        {
            m_dealCard.SetActive(false);
            m_dealCompany.text = string.Empty;
            m_dealText.text = string.Empty;
            m_currentDeal = null;
        }
    }
}
