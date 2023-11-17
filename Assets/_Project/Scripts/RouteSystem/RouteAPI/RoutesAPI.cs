using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace DAS.Urway.Routes
{
    public class RoutesAPI : MonoBehaviour
    {
        // Replace with your Google API Key
        [SerializeField] private string m_apiKey = "";
        [SerializeField] private PolylineQuality m_polylineQuality;

        private string m_url = "https://routes.googleapis.com/directions/v2:computeRoutes";


        public void GetRoute(Location startPoint, Location endPoint, Action<Route> onComplete, Action onError = null)
        {
            StartCoroutine(SendDirectionsRequest(startPoint, endPoint, onComplete, onError));
        }

        private IEnumerator SendDirectionsRequest(Location startPoint, Location endPoint, Action<Route> onComplete,
            Action onError = null)
        {
            // Location _startPoint = new Location(37.419734f, -122.0827784f);
            //Location _endPoint = new Location(37.417670f, -122.079595f);
            RouteRequest _routeRequest = new RouteRequest(startPoint, endPoint, m_polylineQuality);

            string _requestBody = JsonUtility.ToJson(_routeRequest);

            UnityWebRequest _request = new UnityWebRequest(m_url, "POST");
            byte[] _bodyRaw = System.Text.Encoding.UTF8.GetBytes(_requestBody);
            _request.uploadHandler = (UploadHandler) new UploadHandlerRaw(_bodyRaw);
            _request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();

            _request.SetRequestHeader("Content-Type", "application/json");
            _request.SetRequestHeader("X-Goog-Api-Key", m_apiKey);
            _request.SetRequestHeader("X-Goog-FieldMask", "routes.legs");

            yield return _request.SendWebRequest();

            if (_request.result == UnityWebRequest.Result.Success)
            {
                RouteResponse _response = JsonUtility.FromJson<RouteResponse>(_request.downloadHandler.text);

                if (_response != null && _response.routes.Count > 0)
                {
                    onComplete?.Invoke(_response.routes[0]);
                    Debug.Log("Route "+_request.downloadHandler.text);

                }
                else
                {
                    Debug.LogError("Response parsing error: ");
                    onError?.Invoke();
                }
            }
            else
            {
                Debug.LogError("Error: " + _request.error);
                onError?.Invoke();
            }
        }

        [ContextMenu("Test")]
        public void Test()
        {
            Location _startPoint = new Location(37.419734f, -122.0827784f);
            Location _endPoint = new Location(37.417670f, -122.079595f);

            GetRoute(_startPoint, _endPoint, (r) => { Debug.Log("Route " + r.legs[0].steps.Count); });
        }
    }
}