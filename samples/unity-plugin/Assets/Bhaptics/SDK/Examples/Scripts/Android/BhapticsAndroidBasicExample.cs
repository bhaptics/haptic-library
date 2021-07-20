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
        public Button ping;
        public Button toggle;
    }

    public BhapticsAndroidExampleButtons talButtons;
    public BhapticsAndroidExampleButtons suitButtons;
    public BhapticsAndroidExampleButtons armsLeftButtons;
    public BhapticsAndroidExampleButtons armsRightButtons;
    public Text pairedDevicesCount;











    void Awake()
    {
        BhapticsAndroidManager.AddRefreshAction(Refresh);
    }









    private void Refresh()
    {
        #region UI
        var head = BhapticsUtils.ToPositionType(HapticDeviceType.Tactal);
        talButtons.ping.interactable = BhapticsAndroidManager.GetConnectedDevices(head).Count > 0;

        var suit = BhapticsUtils.ToPositionType(HapticDeviceType.TactSuit);
        suitButtons.ping.interactable = BhapticsAndroidManager.GetConnectedDevices(suit).Count > 0;

        var leftArm = BhapticsUtils.ToPositionType(HapticDeviceType.Tactosy_arms, true);
        armsLeftButtons.ping.interactable = BhapticsAndroidManager.GetConnectedDevices(leftArm).Count > 0;
        armsLeftButtons.toggle.interactable = BhapticsAndroidManager.GetPairedDevices(leftArm).Count > 0;

        var rightArm = BhapticsUtils.ToPositionType(HapticDeviceType.Tactosy_arms, false);
        armsRightButtons.ping.interactable = BhapticsAndroidManager.GetConnectedDevices(rightArm).Count > 0;
        armsRightButtons.toggle.interactable = BhapticsAndroidManager.GetPairedDevices(rightArm).Count > 0;

        if (pairedDevicesCount != null)
        {
            pairedDevicesCount.text = BhapticsAndroidManager.GetDevices().Count.ToString();
        }
        #endregion
    }

    public void PingTactal()
    {
        PingPairedDevice(BhapticsUtils.ToPositionType(HapticDeviceType.Tactal));
    }

    public void PingTactSuit()
    {
        PingPairedDevice(BhapticsUtils.ToPositionType(HapticDeviceType.TactSuit));
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

    private void PingPairedDevice(PositionType deviceType)
    {
        BhapticsAndroidManager.Ping(deviceType);
    }
}
