// Copyright 2022 Niantic, Inc. All Rights Reserved.


using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Niantic.ARDK.Utilities.Logging;

#if UNITY_ANDROID
using UnityEngine;
using UnityEngine.Android;
#endif

namespace Niantic.ARDK.Utilities.Permissions
{
  /// Permission types ARDK supports requesting from the user
  public enum ARDKPermission
  {
    Camera,
    Microphone,
    FineLocation,
    CoarseLocation,
    ExternalStorageRead,
    ExternalStorageWrite
  }

  public enum PermissionStatus
  {
    /// User has granted permission
    Granted,

    /// The user has denied the permission but not selected "Don't ask again."
    Denied,

    /// The user has denied permission and selected "Don't ask again,"
    /// or is restricted from granting the permission.
    DeniedAndCannotRequest,

        Unknown
    }


    /// <summary>
    /// Request a permission asynchronously.     
    /// Static class for requesting permissions at runtime. We recommend following Android's
    /// guideline for requesting permissions found here: https://developer.android.com/training/permissions/requesting
    /// @note
    ///   Only valid on Android. Permission requests will pop up automatically on iOS devices when an
    ///   app starts a certain service that requires an ungranted permission.
    /// </summary>
    /// <param name="permission">The ARDK permission to request.</param>
    /// <returns>The permission status after the request.</returns>
    public class PermissionRequester
    {
#if UNITY_ANDROID
    internal static readonly Dictionary<ARDKPermission, string> AndroidPermissionString =
      new Dictionary<ARDKPermission, string>
      {
        { ARDKPermission.Camera, Permission.Camera },
        { ARDKPermission.Microphone, Permission.Microphone },
        { ARDKPermission.FineLocation, Permission.FineLocation },
        { ARDKPermission.CoarseLocation, Permission.CoarseLocation },
        { ARDKPermission.ExternalStorageRead, Permission.ExternalStorageRead },
        { ARDKPermission.ExternalStorageWrite, Permission.ExternalStorageWrite },
      };


    /// @param permission The permission to request.
    /// <summary>
    /// Request a permission asynchronously for a permission that ARDK does not have an enum for.
    /// </summary>
    /// <param name="permissionName"> The name of the permission to request.
    /// Must be one of the constant values provided in the Android documentation.
    /// </param>
    /// <returns>The permission status after the request.</returns>
    public static async Task<PermissionStatus> RequestPermissionAsync(ARDKPermission permission)
    {
      return await RequestPermissionAsync(AndroidPermissionString[permission]);
    }

    /// <summary>
    /// Request a permission that ARDK does not have an enum for.
    /// </summary>
    /// <param name="permission">The ARDK permission to request.</param>
    /// <param name="callback">Method invoked once permission request popup has been closed.</param>
    /// </summary>
    /// <param name="permission">The ARDK permission to request.</param>
    /// <param name="callback">Method invoked once permission request popup has been closed.</param>
    public static Task<PermissionStatus> RequestPermissionAsync(string permissionName)
    {
      var t = new TaskCompletionSource<PermissionStatus>();
      RequestPermission(permissionName, (status) => t.TrySetResult(status));

      return t.Task;
    }
 
    /// <summary>
    /// Method invoked once permission request popup has been closed.
    /// Request a permission for a permission that ARDK does not have an enum for.
    /// </summary>
    /// <param name="permissionName">
    /// The name of the permission to request.
    /// Must be one of the constant values provided in the Android documentation.
    /// </param>
    /// <param name="callback">Method invoked once permission request popup has been closed.</param>
    public static void RequestPermission(ARDKPermission permission, Action<PermissionStatus> callback)
    {
      RequestPermission(AndroidPermissionString[permission], callback);
    }

    /// <summary>
    /// Method invoked once permission request popup has been closed.
    /// Request a permission for a permission that ARDK does not have an enum for.
    /// </summary>
    /// <param name="permissionName">
    /// The name of the permission to request.
    /// Must be one of the constant values provided in the Android documentation.
    /// The permission to request. Must be one of the constant values provided in the Android
    /// documentation here: https://developer.android.com/reference/android/Manifest.permission
    /// </param>
    /// <param name="callback">Method invoked once permission request popup has been closed.</param>
    public static void RequestPermission(string permissionName, Action<PermissionStatus> callback)
    {
      if (HasPermission(permissionName))
      {
        ARLog._Debug($"Requested permission for {permissionName} but it was already granted");
        callback.Invoke(PermissionStatus.Granted);
        return;
      }

      var callbacks = new PermissionCallbacks();
      callbacks.PermissionGranted += status => callback.Invoke(PermissionStatus.Granted);
      callbacks.PermissionDenied += status => callback.Invoke(PermissionStatus.Denied);
      callbacks.PermissionDeniedAndDontAskAgain += status => callback.Invoke(PermissionStatus.DeniedAndCannotRequest);

      ARLog._Debug("Requesting permission for: " + permissionName);
      Permission.RequestUserPermission(permissionName, callbacks);
    }

    /// <summary>
    /// Check the status of an Android permission.
    /// </summary>
    /// <param name="permission">The ARDK permission to check.</param>
    /// <returns>True if the permission has been granted.</returns>
    public static bool HasPermission(ARDKPermission permission)
    {
      return HasPermission(AndroidPermissionString[permission]);
    }

    /// <summary>
    /// Check the status of an Android permission.
    /// </summary>
    /// <param name="permission">The ARDK permission to check.</param>
    /// <returns>True if the permission has been granted. Will always return true in the Unity Editor.</returns>
    public static bool HasPermission(string permissionName)
    {
#if UNITY_EDITOR
      var granted = true;
#else
      var granted = Permission.HasUserAuthorizedPermission(permissionName);
#endif
      ARLog._DebugFormat("{0} Permission: {1}", false, permissionName, granted ? "Granted" : "Not Granted" );
      return granted;
    }
#endif
    }
}