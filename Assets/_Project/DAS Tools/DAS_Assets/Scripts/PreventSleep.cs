using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DAS.Urway
{
    public class PreventSleep : MonoBehaviour
    {
        private void Awake()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}