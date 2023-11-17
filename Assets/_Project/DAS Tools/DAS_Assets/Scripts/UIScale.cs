using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DAS.Urway
{
    public class UIScale : MonoBehaviour
    {
        public float scaleFactor = 1;
        public bool portrait = true;
        public Camera cam;

        float camHeight;
        float canvasDistance;

        private void Awake()
        {
            cam = Camera.main;
        }

        private void Update()
        {
            Scale();
        }

        public void SwitchOrientation()
        {
            portrait = !portrait;
        }

        private void Scale()
        {

            switch (Input.deviceOrientation)
            {
                case DeviceOrientation.Portrait:
                    portrait = true;
                    break;
                case DeviceOrientation.PortraitUpsideDown:
                    portrait = true;
                    break;
                case DeviceOrientation.LandscapeLeft:
                    portrait = false;
                    break;
                case DeviceOrientation.LandscapeRight:
                    portrait = false;
                    break;
                default:
                    portrait = true;
                    break;
            }

            if (cam.orthographic)
            {
                camHeight = cam.orthographicSize * 2;
            }
            else
            {
                canvasDistance = Vector3.Distance(cam.transform.position, transform.position);
                camHeight = 2 * canvasDistance * Mathf.Tan(Mathf.Deg2Rad * (cam.fieldOfView * 0.5f));
            }

            float scale;

            if (portrait)
                scale = (camHeight / Screen.width) * scaleFactor;
            else
                scale = (camHeight / Screen.height) * scaleFactor * 2;

            transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}