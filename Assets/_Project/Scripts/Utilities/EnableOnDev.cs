using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


/// <summary>
/// This class enables a list of objects in development builds
/// </summary>
public class EnableOnDev : MonoBehaviour
{
    [FormerlySerializedAs("objects")] [SerializeField] private List<GameObject> m_objects = new List<GameObject>(); // The list of GameObjects to be enabled in development builds.
    [FormerlySerializedAs("image")] [SerializeField] private Image m_image;                                       // The Image component to be enabled in development builds.

    [FormerlySerializedAs("testDev")] public bool TestDev;                                                        // Flag to test development build mode.

    /// <summary>
    /// Unity Start method. Enables objects and image in development builds based on the testDev flag and DEVELOPMENT_BUILD macro.
    /// </summary>
    /// <param>No inputs required.</param>
    /// <returns>No expected outputs.</returns>
    void Start()
    {
        if(TestDev)
        {
            Debug.Log("TEST DEVELOPMENT_BUILD");
            foreach (GameObject obj in m_objects) { obj.SetActive(true); }
            if (m_image != null) m_image.enabled = true;
        }


#if DEVELOPMENT_BUILD
Debug.Log("DEVELOPMENT_BUILD");
foreach (GameObject obj in m_objects) { obj.SetActive(true); }
if (m_image != null) m_image.enabled = true;
        
#endif
    }

    /// <summary>
    /// Unity Update method.
    /// </summary>
    /// <param>No inputs required.</param>
    /// <returns>No expected outputs.</returns>
    void Update()
    {
        
    }


}
