using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestButtonCick : MonoBehaviour
{
    /// <summary>
    /// Simulates a button click by invoking its onClick event.
    /// </summary>
    /// <param>No inputs required.</param>
    /// <returns>No expected outputs.</returns>
    [ContextMenu("Click")]
    public void Click()
    {
        gameObject.GetComponent<Button>().onClick.Invoke();
    }
}
