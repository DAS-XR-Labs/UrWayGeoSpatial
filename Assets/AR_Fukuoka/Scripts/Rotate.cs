using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public GameObject spinningImage;
    public GameObject locationText;
    public bool imageSpinning = true;

    public Transform cameraTransform;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if(spinningImage != null)
        {
            if (imageSpinning)
            {
                spinningImage.transform.Rotate(0,30 * Time.deltaTime, 0,Space.World);
            }
            else
            {
                spinningImage.transform.LookAt(cameraTransform);
            }
        }
        if(locationText != null)
        {
            locationText.transform.LookAt(cameraTransform);
        }
    }
}
