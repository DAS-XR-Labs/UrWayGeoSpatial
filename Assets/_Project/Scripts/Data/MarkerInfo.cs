
using UnityEngine.Serialization;

namespace DAS.Urway
{
    [System.Serializable]
    public class MarkerInfo
    {
        [FormerlySerializedAs("locationID")] public string LocationID;
        [FormerlySerializedAs("locationName")] public string LocationName;
        [FormerlySerializedAs("latitude")] public double Latitude;
        [FormerlySerializedAs("longitude")] public double Longitude;
        [FormerlySerializedAs("altitude")] public double Altitude;
        [FormerlySerializedAs("offset")] public float Offset;
        [FormerlySerializedAs("type")] public AnchorType Type;
    }
}