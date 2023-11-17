using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class InternetConnectionChecker
{
    private const string ConnectionTestUrl = "http://www.google.com";

    /// <summary>
    /// Checks the internet connection using a head request to a specified URL.
    /// </summary>
    /// <param name="resultCallback">Callback to receive the connection status.</param>
    /// <returns>No expected outputs.</returns>
    public static IEnumerator CheckInternetConnection(System.Action<bool> resultCallback)
    {
        using (var request = UnityWebRequest.Head(ConnectionTestUrl))
        {
            yield return request.SendWebRequest();

            bool _isInternetConnected = !request.isNetworkError && !request.isHttpError;

            resultCallback?.Invoke(_isInternetConnected);
        }
    }
}