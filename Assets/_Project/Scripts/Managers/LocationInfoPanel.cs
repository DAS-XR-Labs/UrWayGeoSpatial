using System;
using DAS.Urway.Routes;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DAS.Urway
{
    /// <summary>
    /// Manages the location information panel and its interaction.
    /// </summary>
    public class LocationInfoPanel : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        public Action OnOpen; // Event for when the panel is opened
        public Action OnClose; // Event for when the panel is closed

        [FormerlySerializedAs("canvas")] [SerializeField] private RectTransform m_canvas; // Reference to the canvas
        [FormerlySerializedAs("scrollView")] [SerializeField] private ScrollRect m_scrollView; // Reference to the ScrollRect component
        [FormerlySerializedAs("scrollViewRect")] [SerializeField] private RectTransform m_scrollViewRect; // Reference to the ScrollRect's RectTransform
        [FormerlySerializedAs("rootRect")] [SerializeField] private RectTransform m_rootRect; // Reference to the root panel's RectTransform
        [FormerlySerializedAs("contentRec")] [SerializeField] private RectTransform m_contentRec; // Reference to the content's RectTransform
        [FormerlySerializedAs("alternateMenu")] [SerializeField] private RectTransform m_alternateMenu; // Reference to an alternate menu's RectTransform
        [FormerlySerializedAs("topPart")] [SerializeField] private RectTransform m_topPart; // Reference to the top part's RectTransform
        [FormerlySerializedAs("bottomPart")] [SerializeField] private RectTransform m_bottomPart; // Reference to the bottom part's RectTransform

        [FormerlySerializedAs("shortDescriptionRect")] [SerializeField]
        private RectTransform m_shortDescriptionRect; // Reference to the short description's RectTransform

        [FormerlySerializedAs("moreInfo")] [SerializeField] private RectTransform m_moreInfo; // Reference to more information's RectTransform

        [FormerlySerializedAs("rootHeight")] [SerializeField] private float m_rootHeight; // Height of the root panel
        [FormerlySerializedAs("shortDescriptionYpos")] [SerializeField] private float m_shortDescriptionYpos; // Y position for short description

        private float m_lastPosY; // Last Y position of the content
        private float m_tempPosY; // Temporary Y position during drag
        private float m_changeStateValue = 300; // Threshold for opening/closing panel
        private ScreenOrientation m_currentOrientation; // Current screen orientation
        private bool m_isOpened; // Flag for whether the panel is opened
        private float currentAspectRatio;

        /// <summary>
        /// Called when the object starts.
        /// Initializes the variables and components.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void Start()
        {
            m_scrollViewRect = m_scrollView.GetComponent<RectTransform>();
            Init();
        }

        /// <summary>
        /// Closes and initializes the location info panel.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        public void Init()
        {
            Close();
            m_isOpened = false;
            m_rootHeight = m_rootRect.rect.height;
            m_changeStateValue = m_rootHeight * 0.10f;
            m_contentRec.sizeDelta = new Vector2(m_canvas.rect.width, m_canvas.rect.height);

            // Vector2 sizeDelta = contentRec.sizeDelta;
            // sizeDelta.y = rootHeight;
            // topPart.sizeDelta = sizeDelta;
            // bottomPart.sizeDelta = sizeDelta;
            // sizeDelta.y = rootHeight;
            // //contentRec.sizeDelta = sizeDelta;
            m_shortDescriptionYpos = m_shortDescriptionRect.rect.height + m_contentRec.sizeDelta.y;

            m_alternateMenu.localPosition = new Vector3(m_alternateMenu.localPosition.x,
                -m_contentRec.sizeDelta.y - m_alternateMenu.rect.height, m_alternateMenu.localPosition.z);

            Vector2 _startPosition = m_contentRec.anchoredPosition;
            _startPosition.y = m_alternateMenu.rect.height;
            m_contentRec.anchoredPosition = _startPosition;
            m_lastPosY = 0;
        }


        /// <summary>
        /// Called every frame.
        /// Updates the panel's behavior based on screen orientation.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        void Update()
        {
            if (m_currentOrientation == Screen.orientation)
                return;

            float _aspectRatio = Screen.height / Screen.width;
            if (_aspectRatio != currentAspectRatio)
            {
                currentAspectRatio = _aspectRatio;
                Init();
                m_currentOrientation = Screen.orientation;
            }
        }

        /// <summary>
        /// Called when a drag operation starts.
        /// Stores the starting Y position of the content.
        /// </summary>
        /// <param name="eventData">Pointer event data for the drag operation.</param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            m_tempPosY = m_contentRec.anchoredPosition.y;
        }

        /// <summary>
        /// Called when a drag operation ends.
        /// Determines whether to open or close the panel based on the drag difference.
        /// </summary>
        /// <param name="eventData">Pointer event data for the drag operation.</param>
        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.LogWarning("Location Info Drag End");
            float _diff = m_tempPosY - m_contentRec.anchoredPosition.y;
            if (Mathf.Abs(_diff) > m_changeStateValue)
            {
                if (_diff > 0)
                {
                    OnClose?.Invoke();
                    Close();
                }
                else
                {
                    OnOpen?.Invoke();
                    Open();
                }
            }
            else
            {
                m_scrollView.enabled = false;
                m_contentRec.DOAnchorPosY(m_tempPosY, 0.5f).OnComplete(() => { m_scrollView.enabled = true; });
            }
        }

        /// <summary>
        /// Shows the short information box.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        public void ShowShortInfoBox()
        {
            if (!m_isOpened)
            {
                Debug.LogWarning("Show Info Box: " + m_shortDescriptionYpos);
                m_scrollView.enabled = false;
                m_contentRec.DOAnchorPosY(m_shortDescriptionYpos, .5f).OnComplete(() =>
                {
                    m_isOpened = true;
                    Debug.Log("Show Info Box Complete");
                    m_scrollViewRect.offsetMin = new Vector2(m_scrollViewRect.offsetMin.x, m_alternateMenu.rect.height);
                    m_scrollView.enabled = true;
                });
            }
        }

        /// <summary>
        /// Opens the location info panel.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        public void Open()
        {
            //stickersButton.SetActive(false);
            m_scrollView.enabled = false;
            m_contentRec.DOAnchorPosY(m_rootHeight, 0.5f).OnComplete(() =>
            {
                m_scrollView.enabled = true;
            });
        }

        /// <summary>
        /// Closes the location info panel.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        public void Close()
        {
            if (m_isOpened)
            {
                m_shortDescriptionRect.gameObject.SetActive(true);
                m_scrollView.enabled = false;
                m_contentRec.DOAnchorPosY(0, 0.5f).OnComplete(() =>
                {
                    m_isOpened = false;
                    m_scrollViewRect.offsetMin = new Vector2(m_scrollViewRect.offsetMin.x, 0);
                    m_scrollView.enabled = true;
                });
            }
        }
    }
}