using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ImageDownload : MonoBehaviour
{
    private string m_downloadURL;               // The URL to download the image from.
    private bool m_download;                    // Flag to initiate image download.
    private Sprite m_sprite;                   // The downloaded sprite.

    public string DownloadURL
    {
        get => m_downloadURL;
        set { m_downloadURL = value; }
    }

    public bool Download
    {
        get => m_download;
        set { m_download = value; }
    }

    public Sprite Sprite
    {
        get => m_sprite;
        set => m_sprite = value;
    }

    /// <summary>
    /// Unity Update method.
    /// </summary>
    /// <param>No inputs required.</param>
    /// <returns>No expected outputs.</returns>
    void Update()
    {
        if (Download)
        {
            Download = false;
            StartCoroutine(DownloadURL);
         }
    }

    /// <summary>
    /// Unity Start method. Sets download to false
    /// </summary>
    /// <param>No inputs required.</param>
    /// <returns>No expected outputs.</returns>
    private void Start()
    {
        Download = false;
    }

    /// <summary>
    /// Coroutine to download and set the player image.
    /// </summary>
    /// <param name="url">The URL of the image to download.</param>
    /// <returns>No expected outputs.</returns>
    private IEnumerator GetPlayerImage(string url)
    {
    UnityWebRequest _www = UnityWebRequestTexture.GetTexture(url);
    yield return _www.SendWebRequest();

    Texture2D _myTexture = DownloadHandlerTexture.GetContent(_www);

    Rect _rec = new Rect(0, 0, _myTexture.width, _myTexture.height);
    Sprite _spriteToUse = Sprite.Create(_myTexture, _rec, new Vector2(0.5f, 0.5f), 100);

    Sprite = _spriteToUse;
    }
}
