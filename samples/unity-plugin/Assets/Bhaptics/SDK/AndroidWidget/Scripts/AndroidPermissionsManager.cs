using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bhaptics.Tact.Unity
{
    public class AndroidPermissionsManager
    {
        private const string STORAGE_PERMISSION = "android.permission.WRITE_EXTERNAL_STORAGE";
        private const string BLUETOOTH_PERMISSION = "android.permission.ACCESS_FINE_LOCATION";
        private static AndroidJavaObject androidObject;
        private static IntPtr permissionObjectPtr;
        private static IntPtr IsPermissionGrantedPtr;
        private static IntPtr RequestPermissionAsyncPtr;
        

        private static readonly Dictionary<string, bool> permissionCache = new Dictionary<string, bool>();
        private static AndroidJavaObject GetPermissionsService()
        {
            try
            {
                if (androidObject == null)
                {
                    androidObject =
                        new AndroidJavaObject("com.bhaptics.bhapticsunity.permissions.UnityAndroidPermissions");
                    permissionObjectPtr = androidObject.GetRawObject();
                    IsPermissionGrantedPtr = AndroidJNIHelper.GetMethodID(androidObject.GetRawClass(), "IsPermissionGranted");
                    RequestPermissionAsyncPtr = AndroidJNIHelper.GetMethodID(androidObject.GetRawClass(), "RequestPermissionAsync");
                }


                return androidObject;
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("GetPermissionsService() " + e.Message);
                return null;
            }
        }

        private static bool IsPermissionGranted(string permissionName)
        {
            if (GetPermissionsService() == null)
            {
                return false;
            }

            return AndroidUtils.CallNativeBoolMethod(
                permissionObjectPtr, IsPermissionGrantedPtr, new object[] {permissionName});
        }

        private static void RequestPermission(string[] permissionNames)
        {
            if (GetPermissionsService() == null)
            {
                return;
            }

            AndroidUtils.CallNativeVoidMethod(
                permissionObjectPtr, RequestPermissionAsyncPtr, new object[] {permissionNames});
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