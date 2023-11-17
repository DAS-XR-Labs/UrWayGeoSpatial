using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


namespace DAS.Urway
{
    [Serializable]
    public class LocationDataCache
    {
        public List<LocationDataSet> cachedData = new List<LocationDataSet>();
    }

    [System.Serializable]
    public struct LocationInfo
    {
        public string ID; // Unique identifier for the location
        [FormerlySerializedAs("placeID")] public string PlaceID; // Identifier for the place
        [FormerlySerializedAs("name")] public string Name; // Name of the location
        [FormerlySerializedAs("stars")] public float Stars; // Rating of the location
        [FormerlySerializedAs("website")] public string Website; // Website URL of the location
        [FormerlySerializedAs("address")] public string Address; // Address of the location
        [FormerlySerializedAs("phone")] public string Phone; // Phone number of the location
        [FormerlySerializedAs("hours")] public string Hours; // Operating hours of the location
        [FormerlySerializedAs("description")] public string Description; // Description of the location

        //public string type; // Type of the location (commented out)
        [FormerlySerializedAs("category")] public Category Category; // Category of the location
        [FormerlySerializedAs("extraCategoryFilter")] public Category[] ExtraCategoryFilter; // Additional category filters for the location
        [FormerlySerializedAs("photoRef")] public string PhotoRef; // Reference to a photo of the location
        [FormerlySerializedAs("dispImage")] public Sprite DispImage; // Display image for the location

        private bool m_isCached; // Flag indicating if the data is cached

        public bool IsCached
        {
            get => m_isCached;
            set => m_isCached = value;
        }
    }

    public class LocationData : MonoBehaviour
    {
        protected readonly string CACHED_DATA_KEY = "locationDataSet"; // Key for cached data in PlayerPrefs

        [FormerlySerializedAs("locationDataContainer")] [SerializeField] private LocationDataSO m_locationDataContainer; // ScriptableObject for location data

        [SerializeField]
        private Dictionary<string, LocationDataSet>
            m_locationDictionary = new Dictionary<string, LocationDataSet>(); // Dictionary to store location data

        [FormerlySerializedAs("placeInfo")] [SerializeField] private PlaceInfo m_placeInfo; // Variable for place information

        private LocationDataCache m_locationDataCache; // Cached location data storage

        public Dictionary<string, LocationDataSet> LocationDictionary =>
            m_locationDictionary; // Dictionary to store location data

        /// <summary>
        /// Sets the LocationDataSO (Location Data Scriptable Object) and triggers the loading of location data from it.
        /// </summary>
        /// <param name="locationDataSO">The LocationDataSO to be set.</param>
        /// <returns>No expected outputs.</returns>
        public void SetLocationDataSO(LocationDataSO locationDataSO)
        {
            this.m_locationDataContainer = locationDataSO;

            Clear();

            LoadLocationData();
        }

        /// <summary>
        /// Clears the locationDictionary dictionary, removing all previously loaded location data.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void Clear()
        {
            m_locationDictionary.Clear();
        }

        /// <summary>
        /// Loads location data from the LocationDataSO into the locationDictionary dictionary.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void LoadLocationData()
        {
            foreach (var locationData in m_locationDataContainer.DataSet)
            {
                if (!m_locationDictionary.ContainsKey(locationData.ID))
                {
                    m_locationDictionary.Add(locationData.ID, locationData);
                }
            }

            LoadCachedData();
        }

        /// <summary>
        /// Called when the script is started. Initiates the loading of location data.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void Start()
        {
            LoadLocationData();
        }

        /// <summary>
        /// Retrieves location information for a specified location by name and ID, either from the cache or by fetching it from the server.
        /// </summary>
        /// <param name="name">Name of the location.</param>
        /// <param name="id">ID of the location.</param>
        /// <param name="callback">Callback function to handle the retrieved location information.</param>
        /// <returns>No expected outputs.</returns>
        public void GetLocationInfo(string name, string id, LocationInfoController.DataLoadedCallback callback)
        {
            var _targetData = m_locationDictionary[id];
            LocationDataSet _cachedData = null;
            if (m_locationDataCache != null)
            {
                _cachedData = m_locationDataCache.cachedData.Find(x => x.ID == id);
            }
            else
            {
                m_locationDataCache = new LocationDataCache();
            }

            if (_cachedData == null)
            {
                m_placeInfo.GpsCoordinates.x = (float) _targetData.GeoData.Latitude;
                m_placeInfo.GpsCoordinates.y = (float) _targetData.GeoData.Longitude;
                m_placeInfo.LocationBias = true;
                m_placeInfo.Search(_targetData.Info, info =>
                {
                    _targetData.Info = info;
                    m_locationDataCache.cachedData.Add(_targetData);
                    callback(_targetData.Info);
                    SaveData();
                });
            }
            else
            {
                Debug.Log("Location data from cache" + name);
                callback(_cachedData.Info);
            }
        }

        /// <summary>
        /// Serializes and saves the cached location data to the PlayerPrefs using JSON.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void SaveData()
        {
            string _json = JsonUtility.ToJson(m_locationDataCache);
            PlayerPrefs.SetString(CACHED_DATA_KEY, _json);
        }

        /// <summary>
        /// Loads cached location data from PlayerPrefs, deserializes it from JSON, and populates the locationDataCache object.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void LoadCachedData()
        {
            if (PlayerPrefs.HasKey(CACHED_DATA_KEY))
            {
                string _data = PlayerPrefs.GetString(CACHED_DATA_KEY);
                m_locationDataCache = JsonUtility.FromJson<LocationDataCache>(_data);
            }
        }

        /// <summary>
        /// Called when the script is disabled. Calls the SaveData() function to save cached data before the script is deactivated.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnDisable()
        {
            SaveData();
        }
    }
}