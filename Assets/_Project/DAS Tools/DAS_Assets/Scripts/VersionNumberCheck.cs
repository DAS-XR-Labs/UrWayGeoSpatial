using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace DAS.Urway
{
    public class VersionNumberCheck : MonoBehaviour
    {
        [SerializeField] private TMP_Text versionText;

        // Start is called before the first frame update
        void Start()
        {
            versionText.text = "v. " + Application.version;
        }
    }
}