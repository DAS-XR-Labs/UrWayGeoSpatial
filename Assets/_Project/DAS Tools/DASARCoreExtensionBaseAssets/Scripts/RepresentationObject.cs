using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DAS.Urway
{
    public class RepresentationObject : MonoBehaviour
    {
        public static Action<LocationInfo, MarkerInfo> OnPOIButtonClick;

        [SerializeField] private Image icon;
        [SerializeField] private Image outline;
        [SerializeField] private UIScale uiScale;
        [SerializeField] private Rotate rotate;
        [SerializeField] private Canvas canvas;
        [SerializeField] private Transform scaleGroup;

        private Button btn;

        private bool isActive;
        private LocationInfo locationInfo;
        private MarkerInfo markerInfo;

        public Image Icon => icon;
        public Image Outline => outline;
        public Transform ScaleGroup => scaleGroup;

        public bool IsActive
        {
            get => isActive;
            set => isActive = value;
        }

        public void Init(Camera camera, LocationInfo lInfo, MarkerInfo mInfo, Button btn)
        {
            uiScale.cam = camera;
            rotate.cameraTransform = camera.transform;
            canvas.worldCamera = camera;
            locationInfo = lInfo;
            markerInfo = mInfo;
            this.btn = btn;
            this.btn.onClick.AddListener(OnButtonClick);
        }

        [ContextMenu("Button Press")]
        private void OnButtonClick()
        {
            if (isActive)
            {
                OnPOIButtonClick?.Invoke(locationInfo, markerInfo);
            }
        }

        public void SetIcon(Sprite sprite)
        {
            icon.sprite = sprite;
            if (sprite != null)
            {
                Debug.Log("SetIcon: " + sprite.name);
            }
        }

        private void OnDestroy()
        {
            btn.onClick.RemoveAllListeners();
        }
    }
}