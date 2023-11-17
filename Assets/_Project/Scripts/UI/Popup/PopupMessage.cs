using System;
using UnityEngine.Serialization;

namespace DAS.Urway
{
    /// <summary>
    /// Defines the types of popup messages.
    /// </summary>
    public enum PopupMessageType
    {
        NoInternet,
        GeospatialAPINotSupported,
        GeospatialLocalizationTimeout,
        DataLoadingError
    }

    /// <summary>
    /// Represents a popup message with specific details.
    /// </summary>
    [Serializable]
    public class PopupMessage
    {
        [FormerlySerializedAs("popupMessageType")] public PopupMessageType PopupMessageType; // The type of the popup message.
        [FormerlySerializedAs("title")] public string Title; // The title of the popup message.
        [FormerlySerializedAs("text")] public string Text; // The text content of the popup message.
    }
}