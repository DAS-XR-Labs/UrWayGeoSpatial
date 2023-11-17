using ARLocation.MapboxRoutes;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace DAS.Urway
{
    public class GeoPlaces : MonoBehaviour
    {
        [FormerlySerializedAs("infoArrayIndex")] public int InfoArrayIndex; // Index of the location data in the infoArray.
        [FormerlySerializedAs("searchInfoArray")] public bool SearchInfoArray; // Flag to search and display information from the infoArray.

        [FormerlySerializedAs("directionsToLocation")] public bool DirectionsToLocation; // Flag to guide the user to a specific location.

        //public TextMeshProUGUI inputField;
        [FormerlySerializedAs("inputField")] public TMP_InputField InputField; // The input field for searching location information.
        [FormerlySerializedAs("geospatialControllerScript")] [SerializeField] private GeospatialController m_geospatialControllerScript; // The GeospatialController component.
        [FormerlySerializedAs("placeInfoScript")] [SerializeField] private PlaceInfo m_placeInfoScript; // The PlaceInfo component.
        [FormerlySerializedAs("route")] [SerializeField] private MapboxRoute m_route; // The MapboxRoute component.

        //public TMPro outputText;

        [FormerlySerializedAs("coordWindow")] public Vector3 CoordWindow; //just for ez viewing

        /// <summary>
        /// Initializes the GeoPlaces component by assigning references to dependent scripts.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void Start()
        {
            if (m_geospatialControllerScript == null)
                m_geospatialControllerScript = FindObjectOfType<GeospatialController>();
            if (m_placeInfoScript == null) m_placeInfoScript = GetComponent<PlaceInfo>();
            if (m_route == null) m_route = GetComponent<MapboxRoute>();
        }

        /// <summary>
        /// Monitors for the trigger to search and display information from the infoArray.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        public void Update()
        {
            if (SearchInfoArray)
            {
                SearchInfoArray = false;
                SeachInfoArray(InfoArrayIndex);
            }
        }

        /// <summary>
        /// Searches for and displays information from the infoArray based on the provided index.
        /// </summary>
        /// <param name="index">The index of the location data in the infoArray to search for.</param>
        /// <returns>No expected outputs.</returns>
        public void SeachInfoArray(int index)
        {
            List<LocationDataSet> _infoArray = new List<LocationDataSet>();
            _infoArray = m_geospatialControllerScript.LocationDataSet;
            Debug.Log("Searching info array...");
            if (_infoArray.Count > index)
            {
                string _name = _infoArray[index].GeoData.LocationName;
                //string rating = infoArray[index].stars;
                //string y = infoArray[index].
                Debug.Log("name is " + _name);

                //outputText.text = "name : " + name + " rating " + rating + " " + ;
                m_placeInfoScript.GpsCoordinates.x = (float) _infoArray[index].GeoData.Latitude;
                m_placeInfoScript.GpsCoordinates.y = (float) _infoArray[index].GeoData.Longitude;
                m_placeInfoScript.Range = 2000f;
                m_placeInfoScript.LocationBias = true;
                m_placeInfoScript.SearchQuery = _name;
                //placeInfoScript.Search(name);

                if (DirectionsToLocation)
                {

                    ARLocation.Location _poi = new ARLocation.Location(
                        (float) _infoArray[index].GeoData.Latitude,
                        (float) _infoArray[index].GeoData.Longitude,
                        (float) _infoArray[index].GeoData.Altitude);

                    //poi.Latitude = (float)infoArray[index].latitude;
                    //poi.Longitude = (float)infoArray[index].longitude;
                    //poi.Altitude = (float)infoArray[index].altitude;
                    directUserToLocation(_poi);
                }

            }
        }

        /// <summary>
        /// Searches for and displays information from the infoArray based on the index obtained from the inputField.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        public void SeachInfoArray()
        {
            try
            {
                Debug.Log(InputField.text.ToString());
                //int value = 0;
                InfoArrayIndex = Convert.ToInt32(InputField.text.ToString());
                //infoArrayIndex = int.Parse(inputField.text.ToString(), value);
                SeachInfoArray(InfoArrayIndex);
            }
            catch (InvalidCastException e)
            {
                Debug.LogException(e);
            }

        }

        /// <summary>
        /// Guides the user to a specific location using ARLocation and MapboxRoute.
        /// </summary>
        /// <param name="poi">The target location to guide the user towards.</param>
        /// <returns>No expected outputs.</returns>
        public void directUserToLocation(ARLocation.Location poi)
        {
            double _lat = poi.Latitude;
            double _lng = poi.Longitude;
            double _alt = poi.Altitude;
            Vector3 _verify = new Vector3((float) _lat, (float) _lng, (float) _alt);
            CoordWindow = _verify;

            string _mapboxToken = m_route.Settings.MapboxToken;
            MapboxApi _api = new MapboxApi(_mapboxToken);
            RouteLoader _loader = new RouteLoader(_api);
            RouteWaypoint _start = new RouteWaypoint {Type = RouteWaypointType.UserLocation};
            RouteWaypoint _dest = new RouteWaypoint {Type = RouteWaypointType.Location, Location = poi};
            StartCoroutine(_loader.LoadRoute(_start, _dest));

        }
    }
}
