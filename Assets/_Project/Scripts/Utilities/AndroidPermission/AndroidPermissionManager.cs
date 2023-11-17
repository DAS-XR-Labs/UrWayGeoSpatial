// Copyright 2022 Niantic, Inc. All Rights Reserved.


using System;
#if UNITY_ANDROID
using Niantic.ARDK.Utilities.Permissions;
using UnityEngine.Android;

namespace Niantic.ARDK.Extensions.Permissions
{
  /// <summary>
  /// Static helper for requesting permissions at runtime.
  /// </summary>
  /// <param>No inputs required.</param>
  /// <returns>No expected outputs.</returns>
  public static class AndroidPermissionManager
  {
    /// <summary>
    /// Request a single Android permission.
    /// </summary>
    /// <param name="permission">The Android permission to request.</param>
    /// <returns>No expected outputs.</returns>
    [Obsolete("Use PermissionRequester.RequestPermission(ARDKPermission permission, Action<PermissionStatus> callback) instead.")]
    public static void RequestPermission(ARDKPermission permission)
    {
      if (!Permission.HasUserAuthorizedPermission(PermissionRequester.AndroidPermissionString[permission]))
        Permission.RequestUserPermission(PermissionRequester.AndroidPermissionString[permission]);
    }

    /// <summary>
    /// Check if the application has a specific Android permission.
    /// </summary>
    /// <param name="permission">The Android permission to check.</param>
    /// <returns>True if the application has the permission, otherwise false.</returns>
    [Obsolete("Use PermissionRequester.HasPermission(permission) instead.")]
    public static bool HasPermission(ARDKPermission permission)
    {
      return PermissionRequester.HasPermission(permission);
    }
  }
}
#endif
