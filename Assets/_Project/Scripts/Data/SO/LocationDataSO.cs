using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace DAS.Urway
{
    /// <summary>
    /// Class representing a set of location data.
    /// </summary>
    [System.Serializable]
    public class LocationDataSet
    {
        public string ID; // Unique identifier for the location.
        public MarkerInfo GeoData; // Geospatial information associated with the location.
        public LocationInfo Info; // Information about the location.
    }
    
    /// <summary>
    /// ScriptableObject class containing location data sets.
    /// </summary>
    [CreateAssetMenu(fileName = "LocationDataSet", menuName = "Scriptable Objects/Location Data")]
    public class LocationDataSO : ScriptableObject
    {
        [FormerlySerializedAs("dataSet")]
        [Searchable]
        [ListDrawerSettings(DraggableItems = true, Expanded = false, ShowIndexLabels = false, ShowPaging = false, ShowItemCount = true)]
        [SerializeField] private List<LocationDataSet> m_dataSet = new List<LocationDataSet>();
        public List<LocationDataSet> DataSet => m_dataSet;
    }
}