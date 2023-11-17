using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Toggles the active state of a GameObject.
/// </summary>
public class ToggleObject : MonoBehaviour
{
    [FormerlySerializedAs("obj")] [SerializeField] private GameObject _obj;                          // The GameObject to be toggled.

    /// <summary>
    /// Toggles the active state of the GameObject.
    /// </summary>
    /// <param>No inputs required.</param>
    /// <returns>No expected outputs.</returns>
    public void ToggleObj()
    {
        if (_obj.activeSelf) _obj.SetActive(false);
        else _obj.SetActive(true);
    }
}
