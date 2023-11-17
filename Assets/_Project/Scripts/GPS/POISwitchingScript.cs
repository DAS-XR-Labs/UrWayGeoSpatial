using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

namespace DAS.Urway
{
    [Serializable]
    public enum UserPOI
    {
        OurayNonCommercialLocations = 0,
    }

    [ExecuteInEditMode]
    /// <summary>
    /// Manages the switching of Points of Interest (POIs) and related functionality.
    /// </summary>
    /// <param>No inputs required.</param>
    /// <returns>No expected outputs.</returns>
    public class POISwitchingScript : MonoBehaviour
    {
        [FormerlySerializedAs("userPOI")] public UserPOI UserPOI = UserPOI.OurayNonCommercialLocations;

        //public UserPOI UserPOI { 
        //    get { return userPOI; } 
        //    set { userPOI = value; ChoosePersonPOI(); } 
        //}
        [FormerlySerializedAs("selectedLocationDataSO")] public LocationDataSO SelectedLocationDataSO;
        [FormerlySerializedAs("locationDataSOs")] public List<LocationDataSO> LocationDataSOs = new List<LocationDataSO>();

        [FormerlySerializedAs("geospatialController")] [SerializeField] private GeospatialController m_geospatialController;
        [FormerlySerializedAs("locationData")] [SerializeField] private LocationData m_locationData;
        [FormerlySerializedAs("arrow")] [SerializeField] private GEOAnchor m_arrow;
        [FormerlySerializedAs("perimeterTrailSign")] [SerializeField] private GEOAnchor m_perimeterTrailSign;
        
        private float m_ourayLat = 38.0297431860574f;
        private float m_ourayLon = -107.672537688139f;
        private float m_ourayAltitude = 7675;

        private float m_ourayArrowLat = 38.0295630737291f;
        private float m_ourayArrowLon = -107.672762323138f;
        private float m_ourayArrowAltitude = 7675;

        private UserPOI m_oldUserPOI;

#if UNITY_EDITOR
        /// <summary>
        /// Update method that executes in the Unity Editor.
        /// Checks for changes in the selected Points of Interest (POI) and updates accordingly.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void Update()
        {
            if (m_oldUserPOI != UserPOI)
            {
                Debug.Log("Editor");
                ChoosePersonPOI();
                m_oldUserPOI = UserPOI;
            }
        }
#endif


        /// <summary>
        /// Sets the selected Points of Interest (POI) dynamically.
        /// </summary>
        /// <param name="value">The index value representing the selected POI.</param>
        /// <returns>No expected outputs.</returns>
        public void SetPOIDynamically(int value)
        {
            Debug.Log("Selected POI " + value);
            UserPOI = (UserPOI) value;
            Debug.Log("Selected POI " + UserPOI);
            ChoosePersonPOI();
        }


        /// <summary>
        /// Chooses the Points of Interest (POI) and sets dependencies accordingly.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void ChoosePersonPOI()
        {
            foreach (var SO in LocationDataSOs)
            {
                if (SO.name.Contains(UserPOI.ToString()))
                {
                    SelectedLocationDataSO = SO;
                    SetAllDependencies();
                    break;
                }
            }
        }

      

        /// <summary>
        /// Sets dependencies related to the selected Points of Interest (POI).
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        private void SetAllDependencies()
        {
            m_geospatialController.SetLocationDataSO(SelectedLocationDataSO);
            m_locationData.SetLocationDataSO(SelectedLocationDataSO);
            m_arrow.Altitude = SelectedLocationDataSO.DataSet[0].GeoData.Altitude;
            m_arrow.Latitude = SelectedLocationDataSO.DataSet[0].GeoData.Latitude;
            m_arrow.Longitude = SelectedLocationDataSO.DataSet[0].GeoData.Longitude;
            m_perimeterTrailSign.Altitude = SelectedLocationDataSO.DataSet[0].GeoData.Altitude;
            m_perimeterTrailSign.Latitude = SelectedLocationDataSO.DataSet[0].GeoData.Latitude;
            m_perimeterTrailSign.Longitude = SelectedLocationDataSO.DataSet[0].GeoData.Longitude;

            if (UserPOI == UserPOI.OurayNonCommercialLocations)
            {
                m_arrow.Altitude = m_ourayArrowAltitude;
                m_arrow.Latitude = m_ourayArrowLat;
                m_arrow.Longitude = m_ourayArrowLon;
                m_perimeterTrailSign.Altitude = m_ourayAltitude;
                m_perimeterTrailSign.Latitude = m_ourayLat;
                m_perimeterTrailSign.Longitude = m_ourayLon;
            }
        }
    }
}