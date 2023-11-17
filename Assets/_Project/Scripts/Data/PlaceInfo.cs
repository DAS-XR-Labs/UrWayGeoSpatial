using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System;
using UnityEngine.Serialization;

namespace DAS.Urway
{
    [System.Serializable]
    public class ResultContainer
    {
        [FormerlySerializedAs("result")] public Result Result;
    }

    [System.Serializable]
    public class Result
    {
        [FormerlySerializedAs("current_opening_hours")] public CurrentOpeningHours Current_opening_hours;
        [FormerlySerializedAs("editorial_summary")] public EditorialSummary Editorial_summary;
        [FormerlySerializedAs("formatted_address")] public string Formatted_address;
        [FormerlySerializedAs("formatted_phone_number")] public string Formatted_phone_number;
        [FormerlySerializedAs("geometry")] public Geometry Geometry;
        [FormerlySerializedAs("website")] public string Website;
    }

    [System.Serializable]
    public class EditorialSummary
    {
        [FormerlySerializedAs("overview")] public string Overview;
    }


    [System.Serializable]
    public class Container
    {
        [FormerlySerializedAs("candidates")] public List<Candidate> Candidates;
        [FormerlySerializedAs("status")] public string Status;
    }

    [System.Serializable]
    public class CurrentOpeningHours
    {
        [FormerlySerializedAs("open_now")] public bool Open_now;
        [FormerlySerializedAs("weekday_text")] public List<string> Weekday_text;
    }

    [System.Serializable]
    public class Candidate
    {
        [FormerlySerializedAs("business_status")] public string Business_status;
        [FormerlySerializedAs("formatted_address")] public string Formatted_address;
        [FormerlySerializedAs("geometry")] public Geometry Geometry;
        [FormerlySerializedAs("icon")] public string Icon;
        [FormerlySerializedAs("name")] public string Name;
        [FormerlySerializedAs("photos")] public List<Photo> Photos;

        [FormerlySerializedAs("place_id")] public string Place_id;
        //public string plusCode;

        [FormerlySerializedAs("rating")] public string Rating;
        [FormerlySerializedAs("price_level")] public string Price_level;
        [FormerlySerializedAs("opening_hours")] public string Opening_hours;

        [FormerlySerializedAs("types")] public List<string> Types;
    }

    [System.Serializable]
    public class Geometry
    {
        [FormerlySerializedAs("location")] public Location Location;
        [FormerlySerializedAs("viewport")] public Viewport Viewport;
    }

    [System.Serializable]
    public class Photo
    {
        [FormerlySerializedAs("height")] public int Height;
        [FormerlySerializedAs("photo_reference")] public string Photo_reference;
        [FormerlySerializedAs("width")] public int Width;
    }

    [System.Serializable]
    public class Location
    {
        [FormerlySerializedAs("lat")] public float Lat;
        [FormerlySerializedAs("lng")] public float Lng;
    }

    [System.Serializable]
    public class Viewport
    {
        [FormerlySerializedAs("northeast")] public Location Northeast;
        [FormerlySerializedAs("southwest")] public Location Southwest;
    }

    public class PlaceInfo : MonoBehaviour
    {
        public string SearchQuery; // The search query for place information
        [FormerlySerializedAs("json")] public string Json; // JSON data received from the API
        [FormerlySerializedAs("placeIDString")] public string PlaceIDString; // Place ID extracted from JSON

        [FormerlySerializedAs("locationBias")] public bool LocationBias; // Flag for enabling location bias
        [FormerlySerializedAs("xml")] public bool Xml; // Flag for specifying JSON or XML format

        [FormerlySerializedAs("searchByPhone")] public bool SearchByPhone; // Flag for searching by phone number
        [FormerlySerializedAs("searchByPlaceID")] public bool SearchByPlaceID; // Flag for searching by Place ID
        [FormerlySerializedAs("searchNow")] public bool SearchNow; // Flag for testing purposes

        [FormerlySerializedAs("gpsCoordinates")] public Vector2 GpsCoordinates; // GPS coordinates for location bias
        [FormerlySerializedAs("range")] public float Range; // Range for location bias


        //fields to return in JSON
        [FormerlySerializedAs("placeName")] public bool PlaceName;
        [FormerlySerializedAs("placeID")] public bool PlaceID;
        [FormerlySerializedAs("plusCode")] public bool PlusCode;
        [FormerlySerializedAs("placeIcon")] public bool PlaceIcon;
        [FormerlySerializedAs("placeStatus")] public bool PlaceStatus;
        public bool FormattedAddress;
        [FormerlySerializedAs("photos")] public bool Photos;
        [FormerlySerializedAs("rating")] public bool Rating;
        [FormerlySerializedAs("priceLevel")] public bool PriceLevel;
        [FormerlySerializedAs("openingHours")] public bool OpeningHours;
        [FormerlySerializedAs("geometry")] public bool Geometry;
        [FormerlySerializedAs("types")] public bool Types;
        private bool m_jsonBypass;

        private bool m_gotPlaceID; // Flag indicating if Place ID is obtained
        [FormerlySerializedAs("outputtext")] public TMP_Text Outputtext; // Text field for displaying results

        [FormerlySerializedAs("apiKey")] [SerializeField] private string m_apiKey = ""; // API key
        [FormerlySerializedAs("locationDataScript")] [SerializeField] private LocationData m_locationDataScript; // Reference to LocationData script

        [FormerlySerializedAs("geospatialControllerScript")] [SerializeField]
        private GeospatialController m_geospatialControllerScript; // Reference to GeospatialController script

        [FormerlySerializedAs("locID")] [HideInInspector] public string LocID; // Location ID
        [FormerlySerializedAs("imageNotAvilalable")] public Sprite ImageNotAvilalable; // Placeholder image for missing images

        private string m_url = "https://maps.googleapis.com/maps/api/place/findplacefromtext/"; // API URL

        private Action<LocationInfo> m_onDataReceived; // Callback for data received
        private LocationInfo m_locationInfo; // Location info object
        [FormerlySerializedAs("candidates")] public List<string> Candidates = new List<string>(); // List of candidates for search

        /// <summary>
        /// Returns the location bias parameters for the URL, based on whether locationBias is enabled and the specified range and GPS coordinates.
        /// </summary>
        /// <returns>A string containing the location bias parameters for the URL.</returns>
        public string GetLocationBias()
        {
            string _locBias = "";
            if (LocationBias)
            {
                _locBias = "&locationbias=circle%3A" + Range + "%40" + GpsCoordinates.x + "%2C" + GpsCoordinates.y;
            }

            return _locBias;
        }


        /// <summary>
        /// Searches for place information using a specified Place ID.
        /// </summary>
        /// <param name="searchPlaceID">The Place ID to search for.</param>
        /// <returns>No expected outputs.</returns>
        public void SearchPlaceID(string searchPlaceID)
        {
            //"https://maps.googleapis.com/maps/api/place/details/json?placeid=ChIJy0PC1XVyAHwRyroJ1MW1D_I&key=AIzaSyBEbUyFEUTvxNagaexjUpvkNnrsVlyWbwM"
            string _urlPrefix = "https://maps.googleapis.com/maps/api/place/details/json?placeid=";

            string _urlMiddle = "&key=";

            string _fullUrl = _urlPrefix + searchPlaceID + _urlMiddle + m_apiKey;

            Debug.Log(_fullUrl);
            StartCoroutine(GetJson(_fullUrl));
        }

        /// <summary>
        /// Searches for place information using a specified Place ID and a location ID.
        /// </summary>
        /// <param name="searchPlaceID">The Place ID to search for.</param>
        /// <param name="locationID">The location ID associated with the search.</param>
        /// <returns>No expected outputs.</returns>
        public void SearchPlaceID(string searchPlaceID, string locationID)
        {
            //"https://maps.googleapis.com/maps/api/place/details/json?placeid=ChIJy0PC1XVyAHwRyroJ1MW1D_I&key=AIzaSyBEbUyFEUTvxNagaexjUpvkNnrsVlyWbwM"
            string _urlPrefix = "https://maps.googleapis.com/maps/api/place/details/json?placeid=";

            string _urlMiddle = "&key=";

            string _fullUrl = _urlPrefix + searchPlaceID + _urlMiddle + m_apiKey;

            Debug.Log(_fullUrl);
            StartCoroutine(GetJson2(_fullUrl, locationID));
        }

        /// <summary>
        /// Searches for place information using a specified search query and optional parameters.
        /// </summary>
        /// <param name="searchQuery">The search query for the place information.</param>
        /// <returns>No expected outputs.</returns>
        public void Search(string searchQuery)
        {
            string _fullUrl;
            string _format = "json";
            if (Xml) _format = "xml";
            string _queryType, 
                _input;
            if (SearchByPhone)
            {
                _queryType = "&inputtype=phonenumber";
                _input = "?input=%2b";
            }
            else
            {
                _queryType = "&inputtype=textquery";
                _input = "?input=";
            }

            string _feilds = Fields();


            _fullUrl = m_url + _format + _input + PrepareText(searchQuery) + _queryType + GetLocationBias() + _feilds + m_apiKey;

            Debug.Log(GetLocationBias());

            Debug.Log(_fullUrl);
            StartCoroutine(GetJson(_fullUrl));
        }

        /// <summary>
        /// Searches for place information using a specified search query and additional data.
        /// </summary>
        /// <param name="initLocationInfo">The initial LocationInfo object.</param>
        /// <param name="onDataReceivedCallback">Callback function to handle the retrieved location information.</param>
        /// <returns>No expected outputs.</returns>
        public void Search(LocationInfo initLocationInfo,
            Action<LocationInfo> onDataReceivedCallback)
        {
            m_locationInfo = initLocationInfo;
            string _searchQuery = m_locationInfo.Name;
            string _locationID = m_locationInfo.ID;

            m_onDataReceived = onDataReceivedCallback;
            string _fullUrl;
            string _format = "json";
            if (Xml) _format = "xml";
            string _queryType, _input;
            if (SearchByPhone)
            {
                _queryType = "&inputtype=phonenumber";
                _input = "?input=%2b";
            }
            else
            {
                _queryType = "&inputtype=textquery";
                _input = "?input=";
            }

            string _feilds = Fields();


            _fullUrl = m_url + _format + _input + PrepareText(_searchQuery) + _queryType + GetLocationBias() + _feilds + m_apiKey;

            Debug.Log(GetLocationBias());

            Debug.Log(_fullUrl);
            StartCoroutine(GetJson(_fullUrl, _locationID));
        }

        /// <summary>
        /// Constructs the fields parameter for the URL based on the selected fields.
        /// </summary>
        /// <returns>A string containing the fields parameter for the URL.</returns>
        public string Fields()
        {
            string _f = "&fields=";

            if (PlaceName) _f = _f + "name%2C";
            if (PlaceID) _f = _f + "place_id%2C";
            if (PlusCode) _f = _f + "plus_code%2C";
            if (PlaceIcon) _f = _f + "icon%2C";
            if (PlaceStatus) _f = _f + "business_status%2C";
            if (FormattedAddress) _f = _f + "formatted_address%2C";
            if (Photos) _f = _f + "photos%2C";
            if (Rating) _f = _f + "rating%2C";
            if (PriceLevel) _f = _f + "price_level%2C";
            if (OpeningHours) _f = _f + "opening_hours%2C";
            if (Geometry) _f = _f + "geometry%2C";
            if (Types) _f = _f + "types%2C";
            if (_f.Substring(_f.Length - 3, 3) == "%2C") _f = _f.Substring(0, _f.Length - 3);
            _f = _f + "&key=";
            Debug.Log("f is " + _f);
            // f = "https://maps.googleapis.com/maps/api/place/details/json?place_id="
            return _f;
        }

        /// <summary>
        /// Prepares a text string for use in the URL by encoding special characters.
        /// </summary>
        /// <param name="address">The input address or text to be prepared.</param>
        /// <returns>The prepared text string.</returns>
        string PrepareText(string address)
        {
            string _a;
            if (SearchByPhone) _a = address.Replace(" ", "");
            else _a = address.Replace(" ", "%20");
            //invalid url chars
            _a = _a.Replace(",", "").Replace(".", "").Replace("#", "").Replace("-", "").Replace("(", "").Replace(")", "")
                .Replace("?", "");

            return _a;
        }


        /// <summary>
        /// Coroutine that sends a UnityWebRequest to retrieve JSON data from the specified URL.
        /// </summary>
        /// <param name="searchUrl">The URL to send the request to.</param>
        /// <returns>No expected outputs.</returns>
        IEnumerator GetJson(string searchUrl)
        {
            UnityWebRequest _www = UnityWebRequest.Get(searchUrl);
            yield return _www.SendWebRequest();

            if (_www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(_www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(_www.downloadHandler.text);
                Json = _www.downloadHandler.text;
                if (!m_jsonBypass) PlaceIDString = GetPlaceID(Json); // get PlaceID
                m_jsonBypass = false;
                // Or retrieve results as binary data
                byte[] _results = _www.downloadHandler.data;
            }
        }

        /// <summary>
        /// Coroutine that sends a UnityWebRequest to retrieve JSON data from the specified URL and location ID.
        /// </summary>
        /// <param name="searchUrl">The URL to send the request to.</param>
        /// <param name="locationID">The location ID associated with the request.</param>
        /// <returns>No expected outputs.</returns>
        IEnumerator GetJson(string searchUrl, string locationID)
        {
            UnityWebRequest _www = UnityWebRequest.Get(searchUrl);
            yield return _www.SendWebRequest();

            if (_www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(_www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(_www.downloadHandler.text);
                Json = _www.downloadHandler.text;
                if (!m_jsonBypass) PlaceIDString = GetPlaceID(Json, locationID);
                m_jsonBypass = false;
                // Or retrieve results as binary data
                byte[] results = _www.downloadHandler.data;
            }
        }

        /// <summary>
        /// Coroutine that sends a UnityWebRequest to retrieve JSON data from the specified URL and location ID.
        /// </summary>
        /// <param name="searchUrl">The URL to send the request to.</param>
        /// <param name="locationID">The location ID associated with the request.</param>
        /// <returns>No expected outputs.</returns>
        IEnumerator GetJson2(string searchUrl, string locationID)
        {
            UnityWebRequest _www = UnityWebRequest.Get(searchUrl);
            yield return _www.SendWebRequest();

            if (_www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(_www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(_www.downloadHandler.text);
                Json = _www.downloadHandler.text;
                GotPlaceID(Json, locationID);
                // Or retrieve results as binary data
                byte[] results = _www.downloadHandler.data;
            }
        }

        /// <summary>
        /// Processes the retrieved JSON data and populates the locationInfo object with additional information.
        /// </summary>
        /// <param name="json">The JSON data to be processed.</param>
        /// <param name="locationID">The location ID associated with the retrieved data.</param>
        /// <returns>No expected outputs.</returns>
        private void GotPlaceID(string json, string locationID)
        {
            Debug.Log("locationID is " + locationID);
            Debug.Log(json);

            m_gotPlaceID = false;
            //ResultContainer sr = new ResultContainer();
            ResultContainer _sr = JsonUtility.FromJson<ResultContainer>(json);
            string _phone = _sr.Result.Formatted_phone_number;
            Debug.Log("phone " + _phone);

            if (!string.IsNullOrEmpty(_sr.Result.Website)) m_locationInfo.Website = _sr.Result.Website;
            else m_locationInfo.Website = "Website not found";

            if (!string.IsNullOrEmpty(_sr.Result.Formatted_phone_number))
                m_locationInfo.Phone = _sr.Result.Formatted_phone_number;
            else m_locationInfo.Phone = "Phone number not found";

            if (!string.IsNullOrEmpty(_sr.Result.Editorial_summary.Overview))
                m_locationInfo.Description = _sr.Result.Editorial_summary.Overview;
            else m_locationInfo.Description = "Summary not found";

            if (_sr.Result.Current_opening_hours.Weekday_text != null &&
                _sr.Result.Current_opening_hours.Weekday_text.Count > 0)
            {
                m_locationInfo.Hours = GetTodaysHours(_sr.Result.Current_opening_hours.Weekday_text);
            }
            else
            {
                m_locationInfo.Hours = string.Empty;
            }

            m_onDataReceived?.Invoke(m_locationInfo);
        }

        /// <summary>
        /// Processes the retrieved JSON data and returns the Place ID.
        /// </summary>
        /// <param name="json">The JSON data to be processed.</param>
        /// <returns>The retrieved Place ID or an empty string if not found.</returns>
        string GetPlaceID(string json)
        {
            //locationInfo = new LocationData.LocationInfo();

            Container _sr;
            _sr = JsonUtility.FromJson<Container>(json);

            string s = "";
            if (_sr.Status == "OK")
            {
                foreach (Candidate candidate in _sr.Candidates)
                {
                    Debug.Log($"LocID {LocID}");
                    Debug.Log("ID " + candidate.Place_id);
                    Debug.Log("Name " + candidate.Name);
                    Debug.Log("Address " + candidate.Formatted_address);
                    Debug.Log("Status " + candidate.Business_status);
                    Debug.Log("Rating " + candidate.Rating);
                    Debug.Log("Price Level " + candidate.Price_level);
                    Debug.Log("Hours " + candidate.Opening_hours);

                    if (candidate.Types[0] != null) Debug.Log("Types " + candidate.Types[0]);
                    Debug.Log("Geometry Location " + candidate.Geometry.Location.Lat + " , " +
                              candidate.Geometry.Location.Lng);
                    //Debug.Log("Geometry Location V2 " + candidate.geometry.location.gpsCoordinates.x + " , " + candidate.geometry.location.gpsCoordinates.y);
                    Debug.Log("Geometry Viewport Location " + candidate.Geometry.Viewport.Northeast.Lat + " , " +
                              candidate.Geometry.Viewport.Northeast.Lng);
                    Debug.Log("Reference Photo " + candidate.Photos[0].Photo_reference);
                }

                DisplayInfo(_sr);
            }
            else s = _sr.Status;

            return s;
        }


        /// <summary>
        /// Processes the retrieved JSON data and returns the Place ID while also setting up locationInfo for further processing.
        /// </summary>
        /// <param name="json">The JSON data to be processed.</param>
        /// <param name="locationID">The location ID associated with the retrieved data.</param>
        /// <returns>The retrieved Place ID or an empty string if not found.</returns>
        string GetPlaceID(string json, string locationID)
        {
            Debug.Log($"locationID {locationID}");
            //locationInfo = new LocationData.LocationInfo();
            Container _sr;
            _sr = JsonUtility.FromJson<Container>(json);

            string _s = "";
            if (_sr.Status == "OK")
            {
                foreach (var candidate in _sr.Candidates)
                {
                    Debug.Log("ID " + candidate.Place_id);
                    Debug.Log("Name " + candidate.Name);
                    Debug.Log("Address " + candidate.Formatted_address);
                    Debug.Log("Status " + candidate.Business_status);
                    Debug.Log("Rating " + candidate.Rating);
                    Debug.Log("Price Level " + candidate.Price_level);
                    Debug.Log("Hours " + candidate.Opening_hours);

                    Candidates.Add(locationID);

                    if (candidate.Types[0] != null) Debug.Log("Types " + candidate.Types[0]);
                    Debug.Log("Geometry Location " + candidate.Geometry.Location.Lat + " , " +
                              candidate.Geometry.Location.Lng);
                    //Debug.Log("Geometry Location V2 " + candidate.geometry.location.gpsCoordinates.x + " , " + candidate.geometry.location.gpsCoordinates.y);
                    Debug.Log("Geometry Viewport Location " + candidate.Geometry.Viewport.Northeast.Lat + " , " +
                              candidate.Geometry.Viewport.Northeast.Lng);
                }

                //This might break something, but it seems like a terrible way to cause recursive API searches.
                if (_sr.Candidates[0].Place_id != null)
                {
                    _s = _sr.Candidates[0].Place_id;
                    if (m_locationDataScript != null)
                    {
                        m_locationInfo.ID = locationID;

                        m_locationInfo.Name = _sr.Candidates[0].Name;
                        float _stars = 0;
                        float.TryParse(_sr.Candidates[0].Rating, out _stars);
                        m_locationInfo.Stars = _stars;
                        m_locationInfo.Website = "Website data not available";
                        m_locationInfo.Phone = "Phone number data not available";
                        m_locationInfo.Address = _sr.Candidates[0].Formatted_address;
                        m_locationInfo.Hours = _sr.Candidates[0].Opening_hours;
                        m_locationInfo.Description = _sr.Candidates[0].Name + " Location Data from Google Places API";
                        // locationInfo.category = (Category)sr.candidates[0].types;
                        if (_sr.Candidates[0].Photos != null && _sr.Candidates[0].Photos.Count > 0)
                        {
                            m_locationInfo.PhotoRef = _sr.Candidates[0].Photos[0].Photo_reference;
                            m_locationInfo.DispImage = null;
                        }

                        m_gotPlaceID = true;


                        SearchPlaceID(_sr.Candidates[0].Place_id, locationID);
                    }
                }

                DisplayInfo(_sr);
            }
            else _s = _sr.Status;

            return _s;
        }


        private void AddMissingData(string locationID, LocationInfo additionalInfo)
        {
            // LocationData.LocationInfo locInfo = locationDataScript.LocationDictionary[locationID];
            // if (additionalInfo.stars != 0) locInfo.stars = additionalInfo.stars;
            // if (string.IsNullOrEmpty(locInfo.address)) locInfo.address = additionalInfo.address;
            // if (string.IsNullOrEmpty(locInfo.website)) locInfo.website = additionalInfo.website;
            // if (string.IsNullOrEmpty(locInfo.phone)) locInfo.phone = additionalInfo.phone;
            // if (string.IsNullOrEmpty(locInfo.hours)) locInfo.hours = additionalInfo.hours;
            // if (string.IsNullOrEmpty(locInfo.description)) locInfo.description = additionalInfo.description;
            // if (string.IsNullOrEmpty(locInfo.photoRef)) locInfo.photoRef = additionalInfo.photoRef;

            /*public string name;
            public float stars;
            public string website;
            public string address;
            public string phone;
            public string hours;
            public string description;
            //public string type;
            public Category category;
            public string photoRef;
            public Sprite dispImage;*/


            // locationDataScript.LocationDictionary.Remove(locationID);
            // locationDataScript.LocationDictionary.Add(locationID, locInfo);
        }

        /// <summary>
        /// Constructs the image URL for a specified photo reference and maximum width.
        /// </summary>
        /// <param name="photoReference">The photo reference for the image.</param>
        /// <param name="maxWidth">The maximum width for the image.</param>
        /// <returns>The constructed image URL.</returns>
        public string ImgURLBuilder(string photoReference, float maxWidth)
        {
            string _photoURL = "https://maps.googleapis.com/maps/api/place/photo?maxwidth=" + maxWidth.ToString() +
                              "&photo_reference=" + photoReference + "&key=" + m_apiKey;
            Debug.Log(_photoURL);
            return _photoURL;
        }

        /// <summary>
        /// Displays place information from the retrieved JSON data in the output text field.
        /// </summary>
        /// <param name="sr">The Container object containing place information.</param>
        /// <returns>No expected outputs.</returns>
        public void DisplayInfo(Container sr)
        {
            string _placeID = sr.Candidates[0].Place_id;
            string _name = sr.Candidates[0].Name;
            string _formatted_address = sr.Candidates[0].Formatted_address;
            string _rating = sr.Candidates[0].Rating;
            string _priceLevel = sr.Candidates[0].Price_level;
            string _hours = sr.Candidates[0].Opening_hours;
            string _status = sr.Candidates[0].Business_status;
            string _types = sr.Candidates[0].Types[0];
            float _lat = sr.Candidates[0].Geometry.Location.Lat;
            float _lng = sr.Candidates[0].Geometry.Location.Lng;

            Outputtext.text = "placeID: " + _placeID +
                              " name " + _name +
                              " address " + _formatted_address +
                              " rating " + _rating +
                              " priceLevel " + _priceLevel +
                              " hours " + _hours +
                              " status " + _status +
                              " types " + _types +
                              " coords (" + _lat + "," + _lng + ")";
        }

        /// <summary>
        /// Retrieves the opening hours for the current day from a list of weekday_text entries.
        /// </summary>
        /// <param name="rowData">The list of weekday_text entries.</param>
        /// <returns>The opening hours for the current day.</returns>
        private string GetTodaysHours(List<string> rowData)
        {
            string _currentDay = DateTime.Today.DayOfWeek.ToString();
            string _timeRange = "";
            foreach (var item in rowData)
            {
                string[] _split = item.Split(':');
                string _day = _split[0].Trim();
                if (_currentDay.Equals(_day, StringComparison.OrdinalIgnoreCase))
                {
                    if (item.Contains("Closed"))
                    {
                        _timeRange = "Closed";
                    }
                    else
                    {
                        _timeRange = item.Remove(0, _day.Length + 1);
                    }
                }
            }

            return _timeRange;
        }
    }
}