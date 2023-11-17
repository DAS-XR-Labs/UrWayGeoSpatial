using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DAS.Urway
{
    [CustomEditor(typeof(LocationInfoPanel))]
    public class LocationInfoPanelEditor : Editor
    {
        /// <summary>
        /// Custom editor for the LocationInfoPanel component.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        override public void OnInspectorGUI()
        {
            LocationInfoPanel _tar = (LocationInfoPanel) target;
            if (GUILayout.Button("Flip"))
                _tar.Init();

            DrawDefaultInspector();
        }
    }
}