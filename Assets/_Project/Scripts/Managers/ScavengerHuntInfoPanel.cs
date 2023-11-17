using System;
using System.Collections;
using DAS.Urway.ScavengerHuntMode;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;


namespace DAS.Urway
{
    public class ScavengerHuntInfoPanel : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        public Action OnOpen; // Event for when the panel is opened
        public Action OnClose; // Event for when the panel is closed


        [FormerlySerializedAs("locationInfoPanel")] [SerializeField] private LocationInfoPanel m_locationInfoPanel;
       
        [FormerlySerializedAs("canvas")]
        [Header("Panel elements")]
        [SerializeField] private RectTransform m_canvas; // Reference to the canvas
        [FormerlySerializedAs("scrollView")] [SerializeField] private ScrollRect m_scrollView; // Reference to the ScrollRect component
        [FormerlySerializedAs("scrollViewRect")] [SerializeField] private RectTransform m_scrollViewRect; // Reference to the ScrollRect's RectTransform
        [FormerlySerializedAs("rootRect")] [SerializeField] private RectTransform m_rootRect; // Reference to the root panel's RectTransform
        [FormerlySerializedAs("contentRec")] [SerializeField] private RectTransform m_contentRec; // Reference to the content's RectTransform
        [FormerlySerializedAs("alternateMenu")] [SerializeField] private RectTransform m_alternateMenu; // Reference to an alternate menu's RectTransform
        [FormerlySerializedAs("shortDescriptionRect")] [SerializeField] private RectTransform m_shortDescriptionRect; // Reference to the short description's RectTransform
        [FormerlySerializedAs("enableScrollView")] [SerializeField] private bool m_enableScrollView; // Reference to the short description's RectTransform

        [FormerlySerializedAs("scavengerHunt")]
        [Space(5)]
        [SerializeField] private ScavengerHunt m_scavengerHunt;
        [FormerlySerializedAs("title")] [SerializeField] private TMP_Text m_title;
        [FormerlySerializedAs("storyDescription")] [SerializeField] private TMP_Text m_storyDescription;
        [FormerlySerializedAs("stickerPlace")] [SerializeField] private Image m_stickerPlace;
        [FormerlySerializedAs("sticker")] [SerializeField] private Image m_sticker;
        [FormerlySerializedAs("closeBtn")] [SerializeField] private Button m_closeBtn;
        
        
        private float m_rootHeight; // Height of the root panel
        private float m_descriptionYpos; // Y position for short description
        private float m_lastPosY; // Last Y position of the content
        private float m_tempPosY; // Temporary Y position during drag
        private float m_changeStateValue = 300; // Threshold for opening/closing panel
        private ScreenOrientation m_currentOrientation; // Current screen orientation
        private bool m_isOpened; // Flag for whether the panel is opened
        private float m_currentAspectRatio;
        private bool m_isClosing;
        private string m_currentStoryId;


        private void OnEnable()
        {
            m_closeBtn.onClick.AddListener(OnCloseButtonClicked);
        }

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
            m_descriptionYpos = m_shortDescriptionRect.rect.height + m_contentRec.sizeDelta.y;
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
        private void Update()
        {
            if (m_currentOrientation == Screen.orientation)
                return;

            float aspectRatio = Screen.height / Screen.width;
            if (aspectRatio != m_currentAspectRatio)
            {
                m_currentAspectRatio = aspectRatio;
                Init();
                m_currentOrientation = Screen.orientation;
            }
        }

        private void OnCloseButtonClicked()
        {
            ClaimStickerAndClose();
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
        /// Shows the information box.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        public void ShowInfo(string id, ScavengerHuntInfo data)
        {
            m_currentStoryId = id;
            m_title.text = data.Name;
            m_storyDescription.text = data.Description;

            m_sticker.sprite = data.Sticker;
            m_stickerPlace.sprite = data.Sticker;
            m_sticker.gameObject.SetActive(false);
            
            if (!m_isOpened)
            {
                m_scrollView.enabled = false;
                m_contentRec.DOAnchorPosY(m_descriptionYpos, .5f).OnComplete(() =>
                {
                    m_isOpened = true;
                    m_scrollViewRect.offsetMin = new Vector2(m_scrollViewRect.offsetMin.x, m_alternateMenu.rect.height);
                    m_scrollView.enabled = m_enableScrollView;
                });
            }
        }

        /// <summary>
        /// Opens the info panel.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        public void Open()
        {
            //stickersButton.SetActive(false);
            m_scrollView.enabled = false;
            if (m_locationInfoPanel != null)
            {
                m_locationInfoPanel.Close();
            }
            m_contentRec.DOAnchorPosY(m_rootHeight, 0.5f).OnComplete(() => { m_scrollView.enabled = true; });
        }

        /// <summary>
        /// Closes the info panel.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        public void Close()
        {
            m_shortDescriptionRect.gameObject.SetActive(true);
            m_scrollView.enabled = false;
            m_contentRec.DOAnchorPosY(0, 0.5f).OnComplete(() =>
            {
                m_isOpened = false;
                m_isClosing = false;
                m_scrollViewRect.offsetMin = new Vector2(m_scrollViewRect.offsetMin.x, 0);
                m_scrollView.enabled = m_enableScrollView;
            });
        }

        private void ClaimStickerAndClose()
        {
            if (!m_isClosing)
            {
                m_scavengerHunt.StickerReceived(m_currentStoryId);
                m_isClosing = true;
                m_sticker.gameObject.SetActive(true);
                m_sticker.transform.localScale = Vector3.zero;
                m_sticker.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutElastic).OnComplete(()=>
                {
                  StartCoroutine(CloseWithDelay());
                });
            }
        }

        private IEnumerator CloseWithDelay()
        {
            yield return new WaitForSeconds(1);
            Close();
        }

        private void OnDisable()
        {
            m_closeBtn.onClick.RemoveListener(OnCloseButtonClicked);
        }
        
    }
}