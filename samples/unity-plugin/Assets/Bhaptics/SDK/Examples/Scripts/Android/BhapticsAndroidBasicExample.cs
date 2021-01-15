using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Bhaptics.Tact;
using Bhaptics.Tact.Unity;



// This example script does not handle multiple pair, such as using two Tactot or two Tactosy_arms_left.
public class BhapticsAndroidBasicExample : MonoBehaviour
{
    [System.Serializable]
    public struct BhapticsAndroidExampleButtons
    {
        public Button pair;
        public Button ping;
        public Button unpair;
        public Button toggle;
    }


    public Text scanStateText;
    public BhapticsAndroidExampleButtons talButtons;
    public BhapticsAndroidExampleButtons suitButtons;
    public BhapticsAndroidExampleButtons armsLeftButtons;
    public BhapticsAndroidExampleButtons armsRightButtons;







    void Update()
    {
        #region Button UI
        if (scanStateText != null)
        {
            scanStateText.text = BhapticsAndroidManager.IsScanning() ? "Scanning" : "Scan Stopped";
        }

        talButtons.pair.interactable = BhapticsAndroidManager.CanPairDevice(BhapticsUtils.ToPositionType(HapticDeviceType.Tactal));
        talButtons.ping.interactable = BhapticsAndroidManager.GetConnectedDevices(BhapticsUtils.ToPositionType(HapticDeviceType.Tactal)).Count > 0;
        talButtons.unpair.interactable = BhapticsAndroidManager.GetPairedDevices(BhapticsUtils.ToPositionType(HapticDeviceType.Tactal)).Count > 0;

        suitButtons.pair.interactable = BhapticsAndroidManager.CanPairDevice(BhapticsUtils.ToPositionType(HapticDeviceType.TactSuit));
        suitButtons.ping.interactable = BhapticsAndroidManager.GetConnectedDevices(BhapticsUtils.ToPositionType(HapticDeviceType.TactSuit)).Count > 0;
        suitButtons.unpair.interactable = BhapticsAndroidManager.GetPairedDevices(BhapticsUtils.ToPositionType(HapticDeviceType.TactSuit)).Count > 0;

        armsLeftButtons.pair.interactable = BhapticsAndroidManager.CanPairDevice(BhapticsUtils.ToPositionType(HapticDeviceType.Tactosy_arms, true));
        armsLeftButtons.ping.interactable = BhapticsAndroidManager.GetConnectedDevices(BhapticsUtils.ToPositionType(HapticDeviceType.Tactosy_arms, true)).Count > 0;
        armsLeftButtons.unpair.interactable = BhapticsAndroidManager.GetPairedDevices(BhapticsUtils.ToPositionType(HapticDeviceType.Tactosy_arms, true)).Count > 0;
        armsLeftButtons.toggle.interactable = BhapticsAndroidManager.GetPairedDevices(BhapticsUtils.ToPositionType(HapticDeviceType.Tactosy_arms, true)).Count > 0;

        armsRightButtons.pair.interactable = BhapticsAndroidManager.CanPairDevice(BhapticsUtils.ToPositionType(HapticDeviceType.Tactosy_arms, false));
        armsRightButtons.ping.interactable = BhapticsAndroidManager.GetConnectedDevices(BhapticsUtils.ToPositionType(HapticDeviceType.Tactosy_arms, false)).Count > 0;
        armsRightButtons.unpair.interactable = BhapticsAndroidManager.GetPairedDevices(BhapticsUtils.ToPositionType(HapticDeviceType.Tactosy_arms, false)).Count > 0;
        armsRightButtons.toggle.interactable = BhapticsAndroidManager.GetPairedDevices(BhapticsUtils.ToPositionType(HapticDeviceType.Tactosy_arms, false)).Count > 0;
        #endregion
    }







    public void RequestPermission()
    {
        if (!AndroidPermissionsManager.CheckBluetoothPermissions())
        {
            AndroidPermissionsManager.RequestPermission();
        }
    }

    public void Scan()
    {
        if (!AndroidPermissionsManager.CheckBluetoothPermissions())
        {
            return;
        }

        BhapticsAndroidManager.Scan();
    }

    public void ScanStop()
    {
        if (!AndroidPermissionsManager.CheckBluetoothPermissions())
        {
            return;
        }

        BhapticsAndroidManager.ScanStop();
    }


    public void PairTactal()
    {
        PairHapticDevice(BhapticsUtils.ToPositionType(HapticDeviceType.Tactal));
    }

    public void UnpairTactal()
    {
        UnpairHapticDevice(BhapticsUtils.ToPositionType(HapticDeviceType.Tactal));
    }

    public void PingTactal()
    {
        PingPairedDevice(BhapticsUtils.ToPositionType(HapticDeviceType.Tactal));
    }




    public void PairTactSuit()
    {
        PairHapticDevice(BhapticsUtils.ToPositionType(HapticDeviceType.TactSuit));
    }

    public void UnpairTactSuit()
    {
        UnpairHapticDevice(BhapticsUtils.ToPositionType(HapticDeviceType.TactSuit));
    }

    public void PingTactSuit()
    {
        PingPairedDevice(BhapticsUtils.ToPositionType(HapticDeviceType.TactSuit));
    }



    public void PairTactosyArms(bool isLeft)
    {
        PairHapticDevice(BhapticsUtils.ToPositionType(HapticDeviceType.Tactosy_arms, isLeft));
    }

    public void UnpairTactosyArms(bool isLeft)
    {
        UnpairHapticDevice(BhapticsUtils.ToPositionType(HapticDeviceType.Tactosy_arms, isLeft));
    }

    public void PingTactosyArms(bool isLeft)
    {
        PingPairedDevice(BhapticsUtils.ToPositionType(HapticDeviceType.Tactosy_arms, isLeft));
    }

    public void ToggleTactosyArms(bool isLeft)
    {
        if (!AndroidPermissionsManager.CheckBluetoothPermissions())
        {
            return;
        }

        var connectedDevices = BhapticsAndroidManager.GetConnectedDevices(BhapticsUtils.ToPositionType(HapticDeviceType.Tactosy_arms, isLeft));
        for (int i = 0; i < connectedDevices.Count; ++i)
        {
            BhapticsAndroidManager.TogglePosition(connectedDevices[i].Address);
        }
    }
















    private void PairHapticDevice(PositionType deviceType)
    {
        if (!AndroidPermissionsManager.CheckBluetoothPermissions())
        {
            return;
        }

        BhapticsAndroidManager.Pair(deviceType);
    }

    private void UnpairHapticDevice(PositionType deviceType)
    {
        if (!AndroidPermissionsManager.CheckBluetoothPermissions())
        {
            return;
        }

        BhapticsAndroidManager.Unpair(deviceType);
    }

    private void PingPairedDevice(PositionType deviceType)
    {
        if (!AndroidPermissionsManager.CheckBluetoothPermissions())
        {
            return;
        }

        BhapticsAndroidManager.Ping(deviceType);
    }
}
