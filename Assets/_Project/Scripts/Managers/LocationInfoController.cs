using System;
using System.Collections;
using DAS.Urway.Deals;
using DAS.Urway.Routes;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DAS.Urway
{
    public class LocationInfoController : MonoBehaviour
    {
        [SerializeField] private string m_apiKey = "";
        private string m_textFormat = "{0:0.00} mi";

        [FormerlySerializedAs("geospatialController")] [SerializeField] private GeospatialController m_geospatialController; // GeospatialController reference
        [FormerlySerializedAs("locationDatacontroller")] [SerializeField] private LocationData m_locationDatacontroller; // LocationData reference
       
        //[FormerlySerializedAs("routeMe")] [SerializeField] private RouteMe m_routeMe; // RouteMe reference
        [SerializeField] private RouteController m_routeController;
        [FormerlySerializedAs("dealsManager")] [SerializeField] private DealsManager m_dealsManager; // DealsManager reference
        [FormerlySerializedAs("pointer")] [SerializeField] private GameObject m_pointer; // Reference to a Pointer GameObject
        [FormerlySerializedAs("popupPanel")] [SerializeField] private PopupPanel m_popupPanel; // PopupPanel reference

        // UI elements
        [FormerlySerializedAs("image")] [SerializeField] private Image m_image;

        //[SerializeField] private Image image2;
        [FormerlySerializedAs("nameText")] [SerializeField] private TextMeshProUGUI m_nameText;

        //[SerializeField] private TextMeshProUGUI nameText2;
        [FormerlySerializedAs("websiteText")] [SerializeField] private TextMeshProUGUI m_websiteText;
        [FormerlySerializedAs("addressText")] [SerializeField] private TextMeshProUGUI m_addressText;
        [FormerlySerializedAs("phoneText")] [SerializeField] private TextMeshProUGUI m_phoneText;
        [FormerlySerializedAs("hoursText")] [SerializeField] private TextMeshProUGUI m_hoursText;
        [FormerlySerializedAs("starsText")] [SerializeField] private TextMeshProUGUI m_starsText;
        [FormerlySerializedAs("distanceText1")] [SerializeField] private TextMeshProUGUI m_distanceText1;
        [FormerlySerializedAs("timeText")] [SerializeField] private TextMeshProUGUI m_timeText;
        [FormerlySerializedAs("typeText")] [SerializeField] private TextMeshProUGUI m_typeText;
        [FormerlySerializedAs("descriptionText")] [SerializeField] private TextMeshProUGUI m_descriptionText;

        [FormerlySerializedAs("loadingTexture")] [SerializeField] private Texture2D m_loadingTexture; // Loading texture for image placeholders
        [FormerlySerializedAs("errorTexture")] [SerializeField] private Texture2D m_errorTexture; // Error texture for image placeholders
        
        [FormerlySerializedAs("starControllerScript")] [SerializeField] private StarController m_starControllerScript; // StarController reference
        [FormerlySerializedAs("routeButton")] [SerializeField] private Button m_routeButton; // Button for route action
        [FormerlySerializedAs("routeButton")] [SerializeField] private Button m_stopRouteButton; // Button for route action
        [FormerlySerializedAs("dealsButton")] [SerializeField] private Button m_dealsButton; // Button for deals action
        [FormerlySerializedAs("locationInfoPanel")] [SerializeField] private LocationInfoPanel m_locationInfoPanel; // LocationInfoPanel reference
        [FormerlySerializedAs("dealsPanel")] [SerializeField] private ActiveDealsPanel m_dealsPanel; // ActiveDealsPanel reference

        private DealData m_connectedDeal; // Reference to DealData

        [FormerlySerializedAs("testName")] public string TestName; // A test name variable
        [FormerlySerializedAs("testId")] public string TestId; // A test ID variable

        [FormerlySerializedAs("leftSection")] [SerializeField] private GameObject m_leftSection;
        [FormerlySerializedAs("navigationBar")] [SerializeField] private GameObject m_navigationBar;
        [FormerlySerializedAs("ratingsSection")] [SerializeField] private GameObject m_ratingsSection;
        [FormerlySerializedAs("bookingArea")] [SerializeField] private GameObject m_bookingArea;

        private LocationInfo m_locationInfo; // Reference to a LocationInfo object
        private MarkerInfo m_geoData; // Reference to a LocationInfo object
        private Category m_currentPOICategory;
        private int m_nextUpdate = 2;
        private int m_updateSeconds = 2;
        private bool m_wasInternetAvailable = false;
        
        public delegate void DataLoadedCallback(LocationInfo info);

        private void OnEnable()
        {
            m_stopRouteButton.gameObject.SetActive(false);
            m_routeButton.onClick.AddListener(OnRouteButtonClicked);
            m_stopRouteButton.onClick.AddListener(OnStopRouteButtonClicked);
            m_routeController.OnStartRoute += OnStartRoute;
            m_dealsButton.onClick.AddListener(OnShowDealsButtonClicked);
            m_geospatialController.OnTrackingStateUpdate += OnTrackingStateUpdate;
        }

        private void OnDisable()
        {
            m_routeButton.onClick.RemoveListener(OnRouteButtonClicked);
            m_stopRouteButton.onClick.RemoveListener(OnStopRouteButtonClicked);

            m_routeController.OnStartRoute -= OnStartRoute;
            m_dealsButton.onClick.RemoveListener(OnShowDealsButtonClicked);
            m_geospatialController.OnTrackingStateUpdate -= OnTrackingStateUpdate;
        }

        private void OnRouteButtonClicked()
        {
            Route();
        }

        private void OnStopRouteButtonClicked()
        {
            m_routeController.Stop();
            m_stopRouteButton.gameObject.SetActive(false);
        }

        private void OnTrackingStateUpdate(bool isTracking)
        {
            m_pointer.SetActive(isTracking);
            if (!isTracking)
            {
                m_locationInfoPanel.Close();
            }
        }

        public void UpdateStars(float stars)
        {
            m_starControllerScript.UpdateStars(stars);
        }

        private void OnStartRoute()
        {
            m_locationInfoPanel.Close();
            m_stopRouteButton.gameObject.SetActive(true);
        }


        private void OnShowDealsButtonClicked()
        {
            if (m_currentPOICategory == Category.ScavengerHunt)
            {
                m_dealsPanel.Hide();
                m_dealsButton.interactable = false;
                m_locationInfoPanel.Close();
            }
            else
            {
                m_dealsPanel.Show(m_connectedDeal);
            }
        }

        public void Route()
        {
            Routes.Location destination = new Routes.Location(m_geoData.Latitude, m_geoData.Longitude);
            m_routeController.StartRoute(destination);
        }
        
        public void SetData(LocationInfo data, MarkerInfo geoData) //PlacedMarkerInfo data
        {
            Debug.Log("Setting Location Data");
            m_locationInfo = data;
            ApplyData(m_locationInfo);
        }

        
        /// <summary>
        /// Applies the location data to UI elements.
        /// </summary>
        /// <param name="info">The location data to apply.</param>
        /// <returns>No expected outputs.</returns>
        private void ApplyData(LocationInfo info)
        {
            //Debug.Log("Check String Value: " + info.placeID.ToString() + " | " + String.IsNullOrEmpty(info.placeID) + " | " + info.name.ToString());

            if (String.IsNullOrEmpty(info.PlaceID))
            {
                m_nameText.text = !String.IsNullOrEmpty(info.Name) ? info.Name : "N/A";
                m_descriptionText.text = !String.IsNullOrEmpty(info.Description) ? info.Description : "N/A";
                m_websiteText.text = !String.IsNullOrEmpty(info.Website) ? info.Website : "N/A";
                m_addressText.text = !String.IsNullOrEmpty(info.Address) ? info.Address : "N/A";
                m_phoneText.text = !String.IsNullOrEmpty(info.Phone) ? info.Phone : "N/A";
                m_hoursText.text = !String.IsNullOrEmpty(info.Hours) ? info.Hours : "N/A";
                m_starsText.text = !String.IsNullOrEmpty(info.Stars.ToString()) ? info.Stars.ToString() : "N/A";
                UpdateStars(info.Stars);

                if (info.DispImage != null)
                {
                    m_image.sprite = info.DispImage;
                    //image2.sprite = info.dispImage;
                }

                //Re-implement distance?
                Debug.Log("Local Data applied");
            }
            else
            {
                m_locationDatacontroller.GetLocationInfo(info.Name, info.ID, OnDataLoaded);
                Debug.Log("Load cache data");
            }
        }

      

        /// <summary>
        /// This is the Update function. It is called once per frame. It does not have any specific purpose in this code.
        /// Checks for internet connection status periodically.
        /// Displays a popup if the connection status changes.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        void Update()
        {
            if (Time.time >= m_nextUpdate)
            {
                m_nextUpdate = Mathf.FloorToInt(Time.time) + m_updateSeconds;

                StartCoroutine(CheckInternetConnection(isConnected =>
                {
                    if (isConnected != m_wasInternetAvailable)
                    {
                        m_wasInternetAvailable = isConnected;

                        if (!isConnected)
                        {
                            Debug.Log("Internet Not Available");
                            m_popupPanel.Show(PopupMessageType.NoInternet, null, false);
                        }
                    }
                }));
            }
        }

        /// <summary>
        /// Coroutine to check internet connection.
        /// </summary>
        /// <param name="action">The action to perform based on the connection status.</param>
        /// <returns>Coroutine IEnumerator.</returns>
        IEnumerator CheckInternetConnection(Action<bool> action)
        {
            UnityWebRequest request = new UnityWebRequest("https://google.com");
            yield return request.SendWebRequest();
            if (request.error != null)
            {
                //Debug.Log("Error");
                action(false);
            }
            else
            {
                //Debug.Log("Success");
                action(true);
            }
        }


        /// <summary>
        /// Callback function when location data is loaded.
        /// Applies loaded data to UI elements.
        /// </summary>
        /// <param name="info">The loaded location data.</param>
        /// <returns>No expected outputs.</returns>
        private void OnDataLoaded(LocationInfo info)
        {
            //This is a commercial place pull data for it
            m_nameText.text = !String.IsNullOrEmpty(info.Name) ? info.Name : "N/A";
            m_descriptionText.text = !String.IsNullOrEmpty(info.Description) ? info.Description : "N/A";
            m_websiteText.text = !String.IsNullOrEmpty(info.Website) ? info.Website : "N/A";
            m_addressText.text = !String.IsNullOrEmpty(info.Address) ? info.Address : "N/A";
            m_phoneText.text = !String.IsNullOrEmpty(info.Phone) ? info.Phone : "N/A";
            m_hoursText.text = !String.IsNullOrEmpty(info.Hours) ? info.Hours : "N/A";
            m_starsText.text = info.Stars.ToString();
            UpdateStars(info.Stars);

            m_nameText.text = info.Name;
            //nameText2.text = info.name;

            Debug.Log("Cache data loaded");

            // Disabled for ouray launch
            // if (dealsManager.isEnabled)
            // {
            //     connectedDeal = dealsManager.GetDealByLocation(obj.markerInfo.latitude, obj.markerInfo.longitude);
            //     dealsButton.interactable = connectedDeal != null;
            // }

            // Disabled for ouray launch. And never re-implement. It was not good.
            //if (obj.markerInfo != null)
            //{
            //    int dist = Mathf.RoundToInt((float)obj.distance * 1000);
            //    distanceText1.text = string.Format(textFormat, ConvertMetersToMiles(dist));
            //    distanceText2.text = string.Format(textFormat, ConvertMetersToMiles(dist));
            //    timeText.text = (dist / 75).ToString() + " min"; //75 m/s walking speed
            //    descriptionText.text = info.description;
            //    typeText.text = obj.type;


            //    // Disabled for ouray launch
            //    //routeMe.endPoint.Location.Label = obj.markerInfo.locationName;
            //    //routeMe.endPoint.Location.Altitude = 1f;
            //    //routeMe.endPoint.Location.Latitude = obj.markerInfo.latitude;
            //    //routeMe.endPoint.Location.Longitude = obj.markerInfo.longitude;
            //}


            if (info.DispImage == null)
            {
                DownloadImage(info.PhotoRef, 400);
            }
            else
            {
                m_image.sprite = info.DispImage;
            }
        }


        /// <summary>
        /// Downloads an image using the given photo reference and max width.
        /// </summary>
        /// <param name="photoReference">The photo reference for the image.</param>
        /// <param name="maxWidth">The maximum width of the image.</param>
        public void DownloadImage(string photoReference, float maxWidth)
        {
            string s = ImgURLBuilder(photoReference, maxWidth);

            Davinci.get()
                .load(s)
                .setCached(true)
                .setFadeTime(0)
                .setLoadingPlaceholder(m_loadingTexture)
                .setErrorPlaceholder(m_errorTexture)
                .into(m_image)
                .start();

            Davinci.get().load(s).into(m_image).start();
        }

        /// <summary>
        /// Builds an image URL using the provided photo reference and max width.
        /// </summary>
        /// <param name="photoReference">The photo reference for the image.</param>
        /// <param name="maxWidth">The maximum width of the image.</param>
        /// <returns>The constructed image URL.</returns>
        public string ImgURLBuilder(string photoReference, float maxWidth)
        {
            string photoURL = "https://maps.googleapis.com/maps/api/place/photo?maxwidth=" + maxWidth.ToString() +
                              "&photo_reference=" + photoReference + "&key=" + m_apiKey;
            Debug.Log(photoURL);
            return photoURL;
        }

        /// <summary>
        /// Converts meters to miles.
        /// </summary>
        /// <param name="meters">The distance in meters to convert.</param>
        /// <returns>The equivalent distance in miles.</returns>
        private float ConvertMetersToMiles(float meters)
        {
            float miles = meters * 0.000621371f; // 1 meter = 0.000621371 miles
            return miles;
        }

        /// <summary>
        /// Releases any connected deal data.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        public void Release()
        {
            m_connectedDeal = null;
        }
    }
}