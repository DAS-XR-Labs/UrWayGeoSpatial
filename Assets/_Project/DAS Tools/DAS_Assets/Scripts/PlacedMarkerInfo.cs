using UnityEngine;

namespace DAS.Urway
{
    public class PlacedMarkerInfo : MonoBehaviour
    {
        public string locationName;
        public double latitude;
        public double longitude;
        public double altitude;
        public string locationID;
        public Category category;
        public string description;

        public void SetData(MarkerInfo info, Category category, string description)
        {
            locationName = info.LocationName;
            latitude = info.Latitude;
            longitude = info.Longitude;
            altitude = info.Altitude;
            locationID = info.LocationID;
            this.category = category;
            this.description = description;
        }
    }
}