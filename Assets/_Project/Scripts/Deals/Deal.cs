using System.Collections.Generic;
using UnityEngine.Serialization;

namespace DAS.Urway.Deals
{
    /// <summary>
    /// Class representing the latitude and longitude of a location.
    /// </summary>
    [System.Serializable]
    public class Location
    {
        [FormerlySerializedAs("lat")] public double Lat;  // Location Latitude
        [FormerlySerializedAs("lng")] public double Lng;  // Location Longitude
    }
    /// <summary>
    /// Class representing information about the deal.
    /// </summary>
    [System.Serializable]
    public class DealData
    {
        [FormerlySerializedAs("company_name")] public string Company_name;     // The name of the company offering the deal
        [FormerlySerializedAs("deal_description")] public string Deal_description; // Description of the deal
        [FormerlySerializedAs("location")] public Location Location;       // Location of the deal
        [FormerlySerializedAs("deal_count")] public int Deal_count;          // The total number of deals
    }

    /// <summary>
    /// Class representing root data containing success status and deal data list.
    /// </summary>
    [System.Serializable]
    public class RootData
    {
        [FormerlySerializedAs("success")] public bool Success;        // Flag for Success
        [FormerlySerializedAs("data")] public List<DealData> Data; // List of deal data
    }
}