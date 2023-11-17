using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace DAS.Urway.Deals
{
    /// <summary>
    /// Manages the retrieval and processing of deal data.
    /// </summary>
    public class DealsManager : MonoBehaviour
    {
        public Action OnDataUpdated; // Action that gets invoked when data is updated.

        [FormerlySerializedAs("isEnabled")] [SerializeField]
        public bool IsEnabled;       // Determines if the loading process should start.

        private const string m_endpoint = "https://us-central1-ar-tourism-ad158.cloudfunctions.net/deals";   // The API endpoint for retrieving deal data
        [FormerlySerializedAs("mockDeals")] [SerializeField] public List<DealData> MockDeals;   // List of mock deal data
        private RootData m_deals;                             // Root data for deals

        public List<DealData> Deals => m_deals.Data;          // List of deals

        /// <summary>
        /// Starts the process of loading deals if enabled.
        /// </summary>
        /// <param name="isEnabled">Determines if the loading process should start.</param>
        /// <returns>No expected outputs.</returns>
        private void Start()
        {
            if (IsEnabled)
            {
                LoadDeals();
            }
        }

        /// <summary>
        /// Initiates the loading of deals by starting a coroutine.
        /// </summary>
        /// <param name="isEnabled">No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void LoadDeals()
        {
            StartCoroutine(LoaddealsCoroutine());
        }

        /// <summary>
        /// Coroutine for loading deals asynchronously using UnityWebRequest.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private IEnumerator LoaddealsCoroutine()
        {
            UnityWebRequest _request = UnityWebRequest.Get(m_endpoint);
            yield return _request.SendWebRequest();

            if (_request.result == UnityWebRequest.Result.Success)
            {
                string _jsonResponse = _request.downloadHandler.text;
                ProcessResponse(_jsonResponse);

            }
            else
            {
                Debug.LogError("Request failed with error: " + _request.error);
            }
        }

        /// <summary>
        /// Processes the JSON response from the API and updates the deals.
        /// </summary>
        /// <param name="jsonResponse">The JSON response received from the API.</param>
        /// <returns>No expected outputs.</returns>
        private void ProcessResponse(string jsonResponse)
        {
            m_deals = JsonUtility.FromJson<RootData>(jsonResponse);
            AddMockData();
            OnDataUpdated?.Invoke();
        }

        /// <summary>
        /// Adds mock deal data to the list of deals.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void AddMockData()
        {
            foreach (var deal in MockDeals)
            {
                m_deals.Data.Add(deal); 
            }
            
        }

        /// <summary>
        /// Retrieves a deal based on the provided location coordinates.
        /// </summary>
        /// <param name="latitude">Latitude of the location.</param>
        /// <param name="longitude">Longitude of the location.</param>
        /// <returns>The deal associated with the provided location coordinates.</returns>
        public DealData GetDealByLocation(double latitude, double longitude)
        {
            DealData _result = m_deals.Data.Find(x => x.Location.Lat == latitude && x.Location.Lng == longitude);
            return _result;
        }

    }
}