using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace DAS.Urway
{
    public class POITracker : MonoBehaviour
    {
        [FormerlySerializedAs("markers")] public List<RepresentationObject>
            Markers =
                new List<RepresentationObject>(); // The list of representation markers associated with Points of Interest (POIs).

        /// <summary>
        /// Clears the list of representation markers when the component is disabled.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void OnDisable()
        {
            Markers.Clear();
        }
    }
}