using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DAS.Urway.ScavengerHuntMode
{
    /// <summary>
    /// Represents information about a scavenger hunt.
    /// </summary>
    [System.Serializable]
    public class ScavengerHuntInfo
    {
        /// <summary>
        /// The name of the scavenger hunt item.
        /// </summary>
        public string Name;

        /// <summary>
        /// Description of the scavenger hunt item.
        /// </summary>
        [Title("Description", bold: false)] [HideLabel] [MultiLineProperty(7)]
        public string Description;

        /// <summary>
        /// The sticker associated with the scavenger hunt item.
        /// </summary>
        [TableColumnWidth(57, Resizable = false)] [PreviewField(Alignment = ObjectFieldAlignment.Left)]
        public Sprite Sticker;

        public ScavengerHuntInfo()
        {
            Description = string.Empty;
        }
    }

    /// <summary>
    /// Represents a data set for a scavenger hunt.
    /// </summary>
    [System.Serializable]
    public class ScavengerHuntDataSet
    {
        /// <summary>
        /// The ID of the data set.
        /// </summary>
        public string ID;

        [BoxGroup("Data")]
        public MarkerInfo GeoData;

        [BoxGroup("Data")]
        public ScavengerHuntInfo Info;
    }

    /// <summary>
    /// Scriptable Object for storing scavenger hunt location data.
    /// </summary>
    [CreateAssetMenu(fileName = "ScavengerHuntDataSet", menuName = "Scriptable Objects/ScavengerHuntDataSet")]
    public class ScavengerHuntLocationDataSO : ScriptableObject
    {
        [Searchable]
        [TableList(ShowIndexLabels = true, AlwaysExpanded = false, ShowPaging = true)]
        [SerializeField]
        private List<ScavengerHuntDataSet> dataSet = new List<ScavengerHuntDataSet>();

        /// <summary>
        /// Gets the list of scavenger hunt data sets.
        /// </summary>
        public List<ScavengerHuntDataSet> DataSet => dataSet;
    }
}
