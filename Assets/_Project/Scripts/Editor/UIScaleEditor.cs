using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DAS.Urway
{
    [CustomEditor(typeof(UIScale))]
    public class UIScaleEditor : Editor
    {
        /// <summary>
        /// Custom editor for the UIScale component.
        /// </summary>
        /// <param>No inputs required.</param>
        /// <returns>No expected outputs.</returns>
        override public void OnInspectorGUI()
        {
            UIScale _tar = (UIScale) target;
            if (GUILayout.Button("Flip"))
                _tar.SwitchOrientation();
            DrawDefaultInspector();
        }
    }
}