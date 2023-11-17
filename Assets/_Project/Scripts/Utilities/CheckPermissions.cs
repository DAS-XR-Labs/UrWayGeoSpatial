using TMPro;
using UnityEngine;
using UnityEngine.Android;
using System.Collections;
using UnityEngine.Serialization;

public class CheckPermissions : MonoBehaviour
{
    [FormerlySerializedAs("message")] public TextMeshProUGUI Message;                 // TextMeshProUGUI component to display permission-related messages.
    [FormerlySerializedAs("checkCameraPermission")] public bool CheckCameraPermission;              // Flag to check camera permission.
    [FormerlySerializedAs("checkCoarseLocationPermission")] public bool CheckCoarseLocationPermission;      // Flag to check coarse location permission.
    [FormerlySerializedAs("checkFineLocationPermission")] public bool CheckFineLocationPermission;        // Flag to check fine location permission.

    private bool m_checkingPermissions;               // Flag to check permission.

    private string m_originalMessage;                 // String to save the original message to


    /// <summary>
    /// Handles the PermissionDeniedAndDontAskAgain callback for a permission.
    /// </summary>
    /// <param name="permissionName">The name of the permission that was denied.</param>
    /// <returns>No expected outputs.</returns>
    internal void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permissionName)
    {
        Debug.Log($"{permissionName} PermissionDeniedAndDontAskAgain");
    }

    /// <summary>
    /// Handles the PermissionGranted callback for a permission.
    /// </summary>
    /// <param name="permissionName">The name of the permission that was granted.</param>
    /// <returns>No expected outputs.</returns>
    internal void PermissionCallbacks_PermissionGranted(string permissionName)
    {
        Debug.Log($"{permissionName} PermissionCallbacks_PermissionGranted");
    }

    /// <summary>
    /// Handles the PermissionDenied callback for a permission.
    /// </summary>
    /// <param name="permissionName">The name of the permission that was denied.</param>
    /// <returns>No expected outputs.</returns>
    internal void PermissionCallbacks_PermissionDenied(string permissionName)
    {
        Debug.Log($"{permissionName} PermissionCallbacks_PermissionDenied");
    }

    /// <summary>
    /// Unity Start method. Checks permissions based on flags and updates the message accordingly.
    /// </summary>
    /// <param>No inputs required.</param>
    /// <returns>No expected outputs.</returns>
    void Start()
    {
        m_originalMessage = Message.text;
        m_checkingPermissions = true;
        string _s = "";
        if (CheckCameraPermission) 
        { 
            if (!PermissionCheck(Permission.Camera)) 
            {
                _s = "Camera ";
                Debug.LogError("Please Allow Camera Permissions"); 
            } 
        }
        if (CheckCoarseLocationPermission)
        {
            if (!PermissionCheck(Permission.CoarseLocation))
            {
                _s = _s + "c.Location ";
                Debug.LogError("Please Allow Coarse Location Permissions");
            }
        }

        if (CheckFineLocationPermission)
        {
            if (!PermissionCheck(Permission.FineLocation))
            {
                _s = _s + "f.Location ";
                Debug.LogError("Please Allow Fine Location Permissions");
            }

        }

        if (!_s.Equals("")) Message.text = "Please Allow " + _s + " Permissions";
    }

    /// <summary>
    /// Unity Update method. Continuously checks and updates permissions-related states.
    /// </summary>
    /// <param>No inputs required.</param>
    /// <returns>No expected outputs.</returns>
    private void Update()
    {


        if (m_checkingPermissions)
        {
            if (CheckCameraPermission) if (PermissionCheck(Permission.Camera)) CheckCameraPermission = false;
            if (CheckCoarseLocationPermission) if (PermissionCheck(Permission.CoarseLocation)) CheckCoarseLocationPermission = false;
            if (CheckFineLocationPermission) if (!PermissionCheck(Permission.FineLocation)) CheckFineLocationPermission = false;
            if (!CheckCameraPermission
                && !CheckCoarseLocationPermission
                && !CheckFineLocationPermission)
            {
                Message.text = m_originalMessage;
                m_checkingPermissions = false;
            }
        }

    }

    /// <summary>
    /// Checks if a specified permission is granted.
    /// </summary>
    /// <param name="pType">The permission type to check.</param>
    /// <returns>True if the permission is granted, false otherwise.</returns>
    private bool PermissionCheck(string pType)
    {
        if (Permission.HasUserAuthorizedPermission(pType))
        {
            // The user authorized use of the camera.
            return true;
        }
        else
        {
            bool useCallbacks = false;
            if (!useCallbacks)
            {
                // We do not have permission to use the camera.
                // Ask for permission or proceed without the functionality enabled.
                
                useCallbacks = false;
                Permission.RequestUserPermission(pType);
            }
            else
            {
                var callbacks = new PermissionCallbacks();
                callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
                callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
                callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedAndDontAskAgain;
                Permission.RequestUserPermission(pType, callbacks);
            }
            return false;
        }

    }
}