using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DAS.Urway
{
    [CustomEditor(typeof(DistanceFilterPanel))]
    public class DistanceFilterPanelEditor : Editor
    {
        /// <summary>
        /// Custom editor for the DistanceFilterPanel component.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        override public void OnInspectorGUI()
        {
            DistanceFilterPanel _tar = (DistanceFilterPanel) target;
            if (GUILayout.Button("Flip"))
                _tar.RectCheck();

            DrawDefaultInspector();
        }
    }
}