using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DAS.Urway
{
    public class EnableOnButtonClick : MonoBehaviour
    {
        public GameObject objectToEnable;

        public void EnableObject()
        {
            Debug.Log("@Doing an enable");
            objectToEnable.SetActive(true);
        }

    }
}