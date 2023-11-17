using System;
using System.Collections.Generic;
using DAS.Urway;
using Google.XR.ARCoreExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
namespace AR_Fukuoka
{
    public class PlacingObjAtLatLngAlt : MonoBehaviour
    {
        [SerializeField] AREarthManager EarthManager;
        [SerializeField] VpsInitializer Initializer;
        [SerializeField] Text OutputText;
        [SerializeField] double HeadingThreshold = 25;
        [SerializeField] double HorizontalThreshold = 20;

        [SerializeField] List<MarkerInfo> infoArray;
        [SerializeField] double Heading;
        [SerializeField] GameObject ContentPrefab;
        [SerializeField] ARAnchorManager AnchorManager;
        GameObject displayObject;
        bool initialized = false;
        List<Marker> CreatedPoints = new List<Marker>();

        [Flags]
        public enum POITypes
        {
            None = 0,
            Mountain = 1 << 1,
            Shops = 1 << 2,
            General = 1 << 3
        }

        [System.Serializable]
        struct MarkerInfo
        {
            public string locationName;
            public double latitude;
            public double longitude;
            public double altitude;
            public POITypes type;
        }

        struct Marker
        {
            public POITypes type;
            public GameObject markerObject;
        }

        void Update()
        {
            if (!Initializer.IsReady || EarthManager.EarthTrackingState != TrackingState.Tracking)
            {
                return;
            }
            string status = "";
            GeospatialPose pose = EarthManager.CameraGeospatialPose;
            if (pose.HeadingAccuracy > HeadingThreshold ||
                  pose.HorizontalAccuracy > HorizontalThreshold)
            {
                status = "低精度：周辺を見回してください";
            }
            else
            {
                status = "高精度：High Tracking Accuracy";
                if (!initialized)
                {
                    initialized = true;
                    foreach(MarkerInfo info in infoArray)
                    {
                        SpawnObject(pose, ContentPrefab, info);
                    }
                }
            }
            ShowTrackingInfo(status, pose);
        }

        void SpawnObject(GeospatialPose pose,GameObject prefab, MarkerInfo info)
        {
            info.altitude = pose.Altitude - 1.5f;

            //Create a rotation quaternion that has the +Z axis pointing in the same direction as the heading value (heading=0 means north direction)
            //https://developers.google.com/ar/develop/unity-arf/geospatial/developer-guide-android#place_a_geospatial_anchor
            Quaternion quaternion = Quaternion.AngleAxis(180f - (float)Heading, Vector3.up);

            ARGeospatialAnchor anchor = AnchorManager.AddAnchor(info.latitude, info.longitude, info.altitude, quaternion);
            prefab.GetComponentInChildren<TMP_Text>().text = info.locationName;
            prefab.GetComponentInChildren<GPSCoordsOnScreenSpace>().locationName = info.locationName;

            if (anchor != null)
            {
                displayObject = Instantiate(prefab, anchor.transform);
                Marker marker = new Marker()
                {
                    markerObject = displayObject,
                    type = info.type
                };
                CreatedPoints.Add(marker);
                Debug.Log(CreatedPoints.Count);
            }
        }
        void ShowTrackingInfo(string status, GeospatialPose pose)
        {
            if (OutputText == null) return;
            OutputText.text = string.Format(
               "\n" +
               "Latitude/Longitude: {0}°, {1}°\n" +
               "Horizontal Accuracy: {2}m\n" +
               "Altitude: {3}m\n" +
               "Vertical Accuracy: {4}m\n" +
               "Heading: {5}°\n" +
               "Heading Accuracy: {6}°\n" +
               "{7} \n"
               ,
               pose.Latitude.ToString("F6"),  //{0}
               pose.Longitude.ToString("F6"), //{1}
               pose.HorizontalAccuracy.ToString("F6"), //{2}
               pose.Altitude.ToString("F2"),  //{3}
               pose.VerticalAccuracy.ToString("F2"),  //{4}
               pose.Heading.ToString("F1"),   //{5}
               pose.HeadingAccuracy.ToString("F1"),   //{6}
               status //{7}
           );
        }

        public void ShowMarker(int markerType)
        {
            Debug.Log(markerType);
            Debug.Log(CreatedPoints.Count);
            for (int i = 0; i < CreatedPoints.Count; i++)
            {
                Debug.Log(CreatedPoints[i].markerObject.GetComponentInChildren<GPSCoordsOnScreenSpace>().locationName);
                if(markerType == -1)
                {
                    CreatedPoints[i].markerObject.SetActive(true);
                }
                else
                {
                    if (CreatedPoints[i].type == (POITypes)markerType)
                    {
                        CreatedPoints[i].markerObject.SetActive(true);
                    }
                    else
                    {
                        CreatedPoints[i].markerObject.SetActive(false);
                    }
                }
            }
        }
    }
}