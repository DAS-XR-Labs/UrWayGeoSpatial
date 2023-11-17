using DAS.Urway;
using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;
using LocationInfo = DAS.Urway.LocationInfo;

namespace DAS.Urway
{
    public class GPSCoordsOnScreenSpace : MonoBehaviour
    {
        [SerializeField] private bool onScreen = false;
        public string locationName;
        [SerializeField] private Camera camera;
        [SerializeField] private GameObject coordRepresentationImagePrefab;

        [SerializeField] Transform farawayMarkerSpawnPoint;
        //[SerializeField] BoxCollider boxCollider;

        [SerializeField] private RepresentationObject representationObject;
        private MarkerInfo markerInfo;
        private LocationInfo locationData;
        private GeospatialController geospatialController;

        private GameObject currentImageRepresentation;
        private float farDistanceScaleFactor = 0.1f;
        private float closeDistanceScaleFactor = 1.2f;
        private float currentDistanceFromCamera;
        private bool enabledByCategoryFilter;
        private float visibleDistance;
        private Sprite categoryIcon;
        private Vector3 representationObjectInitLocalScale;
        private LocationInfoPanel locationInfoPanel;
        private LocationInfoController locationInfoController;
        private float fillingTime = 2;
        private Tween loadingTween;
        private bool isTracking;

        public Category GetCategory => locationData.Category;
        public Category[] GetExtraCategoryFilter => locationData.ExtraCategoryFilter;
        public float CurrentDistance => currentDistanceFromCamera;
        public POIOrientationResize orientationResize;

        public Camera CurrentCamera => camera;

        private void Awake()
        {
            if (camera == null)
            {
                camera = Camera.main;
            }

            enabledByCategoryFilter = true;
            isTracking = true;
        }

        public void ApplyCategoryFilter(bool isVisible)
        {
            representationObject.gameObject.SetActive(isVisible);
            //boxCollider.gameObject.SetActive(isVisible);
            enabledByCategoryFilter = isVisible;
        }

        public void SetGeospatialController(GeospatialController controller)
        {
            geospatialController = controller;
            geospatialController.OnTrackingStateUpdate += OnTrackingUpdate;
        }



        public void SetVisible(bool isVisible)
        {
            if (GetCategory == Category.ScavengerHunt)
            {
                gameObject.SetActive(isVisible && enabledByCategoryFilter);
            }
            else
                representationObject?.gameObject.SetActive(isVisible && enabledByCategoryFilter);
            //target.IsActive = isVisible;
            //representationObject.IsActive = isVisible;
        }

        public void SetActiveRange(float drawingDistance)
        {
            visibleDistance = drawingDistance;
        }

        public void SetMarkerInfo(MarkerInfo mInfo, LocationInfo locData, Sprite icon,
            LocationInfoPanel locationInfoPanel, LocationInfoController locationInfoController)
        {
            this.locationInfoPanel = locationInfoPanel;
            markerInfo = mInfo;
            locationData = locData;
            //placedMarkerInfo.SetData(mInfo, locationData.category, locationData.description);
            categoryIcon = icon;
            locationName = mInfo.LocationName;
            this.locationInfoController = locationInfoController;
            SetImageRepresentation();
        }

        private void OnTrackingUpdate(bool value)
        {
            isTracking = value;
        }


        private void Update()
        {
            Vector3 screenPos = camera.WorldToScreenPoint(transform.position);
            onScreen = screenPos.x > 0f && screenPos.x < Screen.width && screenPos.y > 0f &&
                       screenPos.y < Screen.height;

            currentDistanceFromCamera = Vector3.Distance(camera.transform.position, transform.position);

            float HALF_MILE_METERS = 800;
            //bool isActive =  placedMarkerInfo.category == Category.ScavengerHunt ? 
            //                    currentDistanceFromCamera < HALF_MILE_METERS : 
            //                    currentDistanceFromCamera < visibleDistance;

            bool isActive = (currentDistanceFromCamera < visibleDistance) && isTracking;
            SetVisible(isActive);

            if (representationObject != null)
            {
                //farawayMarkerSpawnPoint.transform.localScale = representationObjectInitLocalScale * CalculateScale(currentDistanceFromCamera);
                //representationObject.Icon.transform.localScale = Vector3.one * CalculateScale(currentDistanceFromCamera);
                float scale = CalculateScale(currentDistanceFromCamera);

                //Vector2 iconSize = representationObject.Icon.rectTransform.sizeDelta;
                //Vector3 coliderSize  = (currentDistanceFromCamera/ 11)  * Vector3.one;
                // Vector3 coliderSize = iconSize * scale/100;
                //coliderSize.z = 0.01f;
                //boxCollider.size = coliderSize;

                representationObject.ScaleGroup.localScale = Vector3.one * scale;
                representationObject.IsActive = isActive;
            }

            if (currentImageRepresentation != null)
            {
                currentImageRepresentation.transform.position = farawayMarkerSpawnPoint.position;
            }
        }

        public void SetImageRepresentation()
        {
            GameObject newGPSCoordObject = Instantiate(coordRepresentationImagePrefab, farawayMarkerSpawnPoint);
            representationObject = newGPSCoordObject.GetComponent<RepresentationObject>();
            currentImageRepresentation = newGPSCoordObject;
            //target.Init(representationObject, placedMarkerInfo); Never Set
            representationObjectInitLocalScale = representationObject.transform.localScale;
            representationObject.Init(camera, locationData, markerInfo,  gameObject.GetComponent<Button>());
            representationObject.SetIcon(categoryIcon);
        }

        private float CalculateScale(float dist)
        {
            var minActiveDistance = 0;
            var maxActiveDistance = visibleDistance;
            float normalizedDist = (dist - minActiveDistance) / (maxActiveDistance - minActiveDistance);
            float inversion = 1 - normalizedDist;

            float outputRange = closeDistanceScaleFactor - farDistanceScaleFactor;
            float convertedValue = (inversion * outputRange) + farDistanceScaleFactor;
            if (visibleDistance == 0)
                return 0;

            return convertedValue;
        }

        public void SelectPOI()
        {
            Debug.Log("Begin POI Selection");
            loadingTween = representationObject.Outline
                .DOFillAmount(1, fillingTime)
                .OnUpdate(() =>
                {
                    if (representationObject.Outline.fillAmount == 1)
                    {
                        Debug.Log("Tween Completed");
                        OnTweenComplete();
                    }
                })
                .SetEase(Ease.InQuad);
        }



        private void OnTweenUpdate()
        {

        }

        private void OnTweenComplete()
        {
            Debug.Log("Finish Selecting POI: " + locationName + " | " + locationInfoController.ToString() + " | " +
                      locationInfoPanel.ToString());
            locationInfoController.SetData(locationData, markerInfo);
            locationInfoPanel.ShowShortInfoBox();
            representationObject.Outline.fillAmount = 0;

        }

        private void OnDisable()
        {
            if (geospatialController != null)
            {
                geospatialController.OnTrackingStateUpdate -= OnTrackingUpdate;
            }
        }

        public void UnselectPOI()
        {
            Debug.Log("Cancel POI Selection");
            if (loadingTween != null)
            {
                loadingTween.Kill();
            }

            representationObject.Outline.fillAmount = 0;
        }
    }
}