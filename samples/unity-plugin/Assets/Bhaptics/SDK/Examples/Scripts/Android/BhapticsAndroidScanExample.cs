using Bhaptics.Tact.Unity;
using UnityEngine;



public class BhapticsAndroidScanExample : MonoBehaviour
{
    void Start()
    {
        InvokeRepeating("CheckScanning", 1f, 5f);
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            CheckPermission();
        }
    }

    private void CheckScanning()
    {
        if (!AndroidPermissionsManager.CheckBluetoothPermissions())
        {
            return;
        }

        BhapticsAndroidManager.Scan();
    }

    public void CheckPermission()
    {
        if (!AndroidPermissionsManager.CheckBluetoothPermissions())
        {
            AndroidPermissionsManager.RequestPermission();
        }
    }
}
