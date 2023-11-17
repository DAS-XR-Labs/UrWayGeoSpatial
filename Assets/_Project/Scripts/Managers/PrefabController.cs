using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DAS.Urway
{
    /// <summary>
    /// Controls the components of a prefab.
    /// </summary>
    public class PrefabController : MonoBehaviour
    {
        [FormerlySerializedAs("starText")] public TMP_Text StarText;
        [FormerlySerializedAs("locationText")] public TMP_Text LocationText;
        [FormerlySerializedAs("bigIcon")] public Image BigIcon;

    }
}