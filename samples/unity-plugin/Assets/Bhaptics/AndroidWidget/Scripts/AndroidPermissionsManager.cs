using System;
using UnityEngine;

public class AndroidPermissionsManager
{
    private const string STORAGE_PERMISSION = "android.permission.WRITE_EXTERNAL_STORAGE";
    private const string BLUETOOTH_PERMISSION = "android.permission.ACCESS_FINE_LOCATION";
    private static AndroidJavaObject activity;
    private static AndroidJavaObject permissionService;

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

    private static void RequestPermission(string permissionName)
    {
        RequestPermission(new[] {permissionName});
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

        return IsPermissionGranted(STORAGE_PERMISSION);
    }

    public static bool CheckBluetoothPermissions()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            return true;
        }

        return IsPermissionGranted(BLUETOOTH_PERMISSION);
    }

    public static void RequestPermission()
    {
        RequestPermission(
            new[] { STORAGE_PERMISSION, BLUETOOTH_PERMISSION });
    }
}
