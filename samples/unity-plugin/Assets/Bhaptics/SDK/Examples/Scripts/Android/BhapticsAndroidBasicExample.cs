using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Bhaptics.Tact;
using Bhaptics.Tact.Unity;
using UnityEngine.Events;


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


    void Awake()
    {
        BhapticsAndroidManager.AddRefreshAction(Refresh);
    }


    private void Refresh()
    {
        #region Button UI
        if (scanStateText != null)
        {
            scanStateText.text = BhapticsAndroidManager.IsScanning() ? "Scanning" : "Scan Stopped";
        }

        var head = BhapticsUtils.ToPositionType(HapticDeviceType.Tactal);
        talButtons.pair.interactable = BhapticsAndroidManager.CanPairDevice(head);
        talButtons.ping.interactable = BhapticsAndroidManager.GetConnectedDevices(head).Count > 0;
        talButtons.unpair.interactable = BhapticsAndroidManager.GetPairedDevices(head).Count > 0;

        var suit = BhapticsUtils.ToPositionType(HapticDeviceType.TactSuit);
        suitButtons.pair.interactable = BhapticsAndroidManager.CanPairDevice(suit);
        suitButtons.ping.interactable = BhapticsAndroidManager.GetConnectedDevices(suit).Count > 0;
        suitButtons.unpair.interactable = BhapticsAndroidManager.GetPairedDevices(suit).Count > 0;

        var leftArm = BhapticsUtils.ToPositionType(HapticDeviceType.Tactosy_arms, true);
        armsLeftButtons.pair.interactable = BhapticsAndroidManager.CanPairDevice(leftArm);
        armsLeftButtons.ping.interactable = BhapticsAndroidManager.GetConnectedDevices(leftArm).Count > 0;
        armsLeftButtons.unpair.interactable = BhapticsAndroidManager.GetPairedDevices(leftArm).Count > 0;
        armsLeftButtons.toggle.interactable = BhapticsAndroidManager.GetPairedDevices(leftArm).Count > 0;

        var rightArm = BhapticsUtils.ToPositionType(HapticDeviceType.Tactosy_arms, false);
        armsRightButtons.pair.interactable = BhapticsAndroidManager.CanPairDevice(rightArm);
        armsRightButtons.ping.interactable = BhapticsAndroidManager.GetConnectedDevices(rightArm).Count > 0;
        armsRightButtons.unpair.interactable = BhapticsAndroidManager.GetPairedDevices(rightArm).Count > 0;
        armsRightButtons.toggle.interactable = BhapticsAndroidManager.GetPairedDevices(rightArm).Count > 0;
        #endregion
    }


    public void RequestPermission()
    {
        if (!BhapticsAndroidManager.CheckPermission())
        {
            BhapticsAndroidManager.RequestPermission();
        }
    }

    public void Scan()
    {
        if (!BhapticsAndroidManager.CheckPermission())
        {
            return;
        }

        BhapticsAndroidManager.Scan();
    }

    public void ScanStop()
    {
        if (!BhapticsAndroidManager.CheckPermission())
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
        var connectedDevices = BhapticsAndroidManager.GetConnectedDevices(BhapticsUtils.ToPositionType(HapticDeviceType.Tactosy_arms, isLeft));
        for (int i = 0; i < connectedDevices.Count; ++i)
        {
            BhapticsAndroidManager.TogglePosition(connectedDevices[i].Address);
        }
    }
















    private void PairHapticDevice(PositionType deviceType)
    {
        BhapticsAndroidManager.Pair(deviceType);
    }

    private void UnpairHapticDevice(PositionType deviceType)
    {
        BhapticsAndroidManager.Unpair(deviceType);
    }

    private void PingPairedDevice(PositionType deviceType)
    {
        BhapticsAndroidManager.Ping(deviceType);
    }
}
