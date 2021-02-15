using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bhaptics.Tact.Unity
{
    public class AndroidPermissionsManager
    {
        private const string STORAGE_PERMISSION = "android.permission.WRITE_EXTERNAL_STORAGE";
        private const string BLUETOOTH_PERMISSION = "android.permission.ACCESS_FINE_LOCATION";
        private static AndroidJavaObject activity;
        private static AndroidJavaObject permissionService;

        private static readonly Dictionary<string, bool> permissionCache = new Dictionary<string, bool>();

        private static AndroidJavaObject GetActivity()
        {
            if (activity == null)
            {
                var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }
            return activity;
        }

        private static AndroidJavaObject GetPermissionsService()
        {
            return permissionService ??
                (permissionService = new AndroidJavaObject("com.bhaptics.bhapticsunity.permissions.UnityAndroidPermissions"));
        }

        private static bool IsPermissionGranted(string permissionName)
        {
            return GetPermissionsService().Call<bool>("IsPermissionGranted", GetActivity(), permissionName);
        }

        private static void RequestPermission(string[] permissionNames)
        {
            GetPermissionsService().Call("RequestPermissionAsync", GetActivity(),
                permissionNames);
        }


        public static bool CheckFilePermissions()
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                return true;
            }

            if (permissionCache.ContainsKey(STORAGE_PERMISSION))
            {
                return permissionCache[STORAGE_PERMISSION];
            }

            bool hasPermission = IsPermissionGranted(STORAGE_PERMISSION);
            permissionCache[STORAGE_PERMISSION] = hasPermission;

            return hasPermission;
        }

        public static bool CheckBluetoothPermissions()
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                return true;
            }

            if (permissionCache.ContainsKey(BLUETOOTH_PERMISSION))
            {
                return permissionCache[BLUETOOTH_PERMISSION];
            }

            bool hasPermission = IsPermissionGranted(BLUETOOTH_PERMISSION);
            permissionCache[BLUETOOTH_PERMISSION] = hasPermission;

            return hasPermission;
        }

        public static void RequestPermission()
        {
            permissionCache.Clear();
            RequestPermission(
                new[] { STORAGE_PERMISSION, BLUETOOTH_PERMISSION });
        }
    }
}