using UnityEngine;
using UnityEngine.Serialization;

namespace DAS.Urway
{
    /// <summary>
    /// Represents a geographic anchor with latitude, longitude, and altitude.
    /// </summary>
    /// <param>No inputs required.</param>
    /// <returns>No expected outputs.</returns>
    public class GEOAnchor : MonoBehaviour
    {
        [FormerlySerializedAs("latitude")] [SerializeField] private double m_latitude; // Latitude of the geographic anchor.
        [FormerlySerializedAs("longitude")] [SerializeField] private double m_longitude; // Longitude of the geographic anchor.
        [FormerlySerializedAs("altitude")] [SerializeField] private double m_altitude; // Altitude of the geographic anchor.

        public double Latitude
        {
            get { return m_latitude; }
            set { m_latitude = value; }
        } // Gets or sets the latitude of the geographic anchor.

        public double Longitude
        {
            get { return m_longitude; }
            set { m_longitude = value; }
        } // Gets or sets the longitude of the geographic anchor.

        public double Altitude
        {
            get { return m_altitude; }
            set { m_altitude = value; }
        } // Gets or sets the altitude of the geographic anchor.
    }
}