using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DAS.Urway
{
    public class GPSCoordsRepresentation : MonoBehaviour
    {
        public static GPSCoordsRepresentation instance;

        [SerializeField] private Canvas coordRepresentationImagePrefab;
        string locationName = "";

        private void Awake()
        {
            instance = this;
        }

        public void AssignImageRepresentation(GPSCoordsOnScreenSpace gpsCoord)
        {
            locationName = gpsCoord.locationName;
            coordRepresentationImagePrefab.GetComponentInChildren<TMP_Text>().text = gpsCoord.locationName;
            coordRepresentationImagePrefab.GetComponentInChildren<UIScale>().cam = gpsCoord.CurrentCamera;
            coordRepresentationImagePrefab.GetComponentInChildren<Rotate>().cameraTransform =
                gpsCoord.CurrentCamera.transform;
            Canvas newGPSCoordObject = Instantiate(coordRepresentationImagePrefab, transform);
        }
    }
}