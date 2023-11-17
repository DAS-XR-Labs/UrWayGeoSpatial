using System;
using System.Collections.Generic;
using DAS.Urway.ScavengerHuntMode;
using UnityEngine;
using UnityEngine.Serialization;

namespace DAS.Urway
{
    /// <summary>
    /// A container for received stickers data, marked as serializable for Unity.
    /// </summary>
    [Serializable]
    public class ReceivedStickersContainer
    {
        /// <summary>
        /// A list to store the received stickers.
        /// </summary>
        public List<string> ReceivedStickers;

        /// <summary>
        /// Default constructor initializes the list of received stickers.
        /// </summary>
        public ReceivedStickersContainer()
        {
            ReceivedStickers = new List<string>();
        }
    }

    /// <summary>
    /// Manages the scavenger hunt mechanics and interactions.
    /// </summary>
    public class ScavengerHunt : MonoBehaviour
    {
        // Field to store the player preferences key for stickers.
        private readonly string PLAYERPREFS_STICKERS_KEY = "stickers";

        // Events to handle initialization and updates.
        public Action OnInitialized;
        public Action<int, int> OnUpdate;

        // Serialized fields for various components and settings.
        [FormerlySerializedAs("scavengerHuntInfoPanel")] [SerializeField] private ScavengerHuntInfoPanel m_scavengerHuntInfoPanel;
        [FormerlySerializedAs("geospatialController")] [SerializeField] private GeospatialController m_geospatialController;
        [FormerlySerializedAs("locationsDataSet")] [SerializeField] private ScavengerHuntLocationDataSO m_locationsDataSet;
        [FormerlySerializedAs("prefab")] [SerializeField] private GameObject m_prefab;
        [FormerlySerializedAs("activateDistance")] [SerializeField] private float m_activateDistance;

        // Private fields for internal use.
        private bool m_isInitialized = false;
        private bool m_isCategoryEnabled = true;
        private Dictionary<string, ScavengerHuntARItem> m_tAnchors = new Dictionary<string, ScavengerHuntARItem>();
        private ReceivedStickersContainer m_receivedStickersContainer;
        private int m_anchorsNumber;

        // Properties to access received stickers and dataset.
        public List<string> ReceivedStickers => m_receivedStickersContainer.ReceivedStickers;
        public List<ScavengerHuntDataSet> DataSet => m_locationsDataSet.DataSet;

        /// <summary>
        /// Called when the object starts.
        /// </summary>
        private void Start()
        {
            LoadStickers();
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            m_geospatialController.OnLocalizationCompleted += OnLocalizationCompleted;
        }

        /// <summary>
        /// Event handler for geospatial localization completion.
        /// </summary>
        private void OnLocalizationCompleted()
        {
            foreach (var data in m_locationsDataSet.DataSet)
            {
                if (!m_receivedStickersContainer.ReceivedStickers.Contains(data.ID))
                {
                    m_geospatialController.GetAnchor(data.GeoData, (anchor) =>
                    {
                        ScavengerHuntARItem _anchorGO =
                            Instantiate(m_prefab, anchor.transform).GetComponent<ScavengerHuntARItem>();
                        _anchorGO.transform.localPosition = Vector3.zero;
                        _anchorGO.Init(data.ID, data.Info);
                        m_tAnchors.Add(data.ID, _anchorGO);

                        _anchorGO.OnClick += (id, item) =>
                        {
                            Debug.Log("On Item Clicked " + item.Name);
                            m_scavengerHuntInfoPanel.ShowInfo(id, item);
                        };
                        m_anchorsNumber++;
                        if (m_anchorsNumber == m_locationsDataSet.DataSet.Count)
                        {
                            m_isInitialized = true;
                            OnInitialized?.Invoke();
                        }
                    });
                }
            }
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        private void Update()
        {
            if (m_isInitialized)
            {
                CheckVisibility();
            }
        }

        /// <summary>
        /// Checks and updates the visibility of scavenger hunt items.
        /// </summary>
        private void CheckVisibility()
        {
            if (m_tAnchors != null)
            {
                Vector3 _cameraPos = Camera.main.transform.position;
                foreach (var item in m_tAnchors.Values)
                {
                    Vector3 _anchorPos = item.transform.position;
                    item.gameObject.SetActive(m_isCategoryEnabled &&
                                              Vector3.Distance(_cameraPos, _anchorPos) < m_activateDistance);
                }
            }
        }

        /// <summary>
        /// Called when the object becomes disabled and inactive.
        /// </summary>
        private void OnDisable()
        {
            m_geospatialController.OnLocalizationCompleted -= OnLocalizationCompleted;
        }

        /// <summary>
        /// Loads the received stickers from player preferences.
        /// </summary>
        private void LoadStickers()
        {
            if (PlayerPrefs.HasKey(PLAYERPREFS_STICKERS_KEY))
            {
                string _stickerString = PlayerPrefs.GetString(PLAYERPREFS_STICKERS_KEY);
                m_receivedStickersContainer = JsonUtility.FromJson<ReceivedStickersContainer>(_stickerString);
            }
            else
            {
                m_receivedStickersContainer = new ReceivedStickersContainer();
            }

            OnUpdate?.Invoke(m_receivedStickersContainer.ReceivedStickers.Count, DataSet.Count);
        }

        /// <summary>
        /// Saves the received stickers to player preferences.
        /// </summary>
        private void SaveReceivedStickers()
        {
            string _stickerString = JsonUtility.ToJson(m_receivedStickersContainer);
            PlayerPrefs.SetString(PLAYERPREFS_STICKERS_KEY, _stickerString);
        }

        /// <summary>
        /// Handles the reception of a sticker with the given ID.
        /// </summary>
        /// <param name="id">The ID of the received sticker.</param>
        public void StickerReceived(string id)
        {
            if (m_tAnchors.ContainsKey(id))
            {
                var _item = m_tAnchors[id];
                Destroy(_item.gameObject);
                m_tAnchors.Remove(id);
            }

            m_receivedStickersContainer.ReceivedStickers.Add(id);
            SaveReceivedStickers();
            OnUpdate?.Invoke(m_receivedStickersContainer.ReceivedStickers.Count, DataSet.Count);
        }

        /// <summary>
        /// Updates the visibility of the scavenger hunt category.
        /// </summary>
        /// <param name="isVisible">Whether the category should be visible or not.</param>
        public void UpdateCategoryVisibility(bool isVisible)
        {
            m_isCategoryEnabled = isVisible;
        }

        private int o;

        /// <summary>
        /// A test method used for debugging.
        /// </summary>
        [ContextMenu("Test")]
        public void Test()
        {
            m_scavengerHuntInfoPanel.ShowInfo(m_locationsDataSet.DataSet[o].ID, m_locationsDataSet.DataSet[o].Info);
            o++;
        }
    }
}
