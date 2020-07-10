using System;
using UnityEngine;
using UnityEngine.UI;


namespace Bhaptics.Tact.Unity
{
    [Serializable]
    public class SettingDeviceIcon
    {
        public Sprite pairImage;
        public Sprite unpairImage;
        public Sprite scanImage;
    }


    // [CreateAssetMenu(fileName = "WidgetSetting", menuName = "Bhaptics/Create Android Widget Setting")]
    public class Bhaptics_Widget_Setting : ScriptableObject
    {

        [Header("[Setting Icons]")]
        public SettingDeviceIcon SettingTactot;
        public SettingDeviceIcon SettingTactal;
        public SettingDeviceIcon SettingTactosyArm;
        public SettingDeviceIcon SettingTactosyFoot;
        public SettingDeviceIcon SettingTactosyHand;


        public Sprite GetPairedDeviceSprite(HapticDevice device)
        {
            string deviceType = device.DeviceName;
            var isConnect = device.IsConnected;
            if (deviceType.StartsWith("TactosyH"))
            {
                return isConnect ? SettingTactosyHand.pairImage : SettingTactosyHand.unpairImage;
            }

            if (deviceType.StartsWith("TactosyF"))
            {
                return isConnect ? SettingTactosyFoot.pairImage : SettingTactosyFoot.unpairImage;
            }

            if (deviceType.StartsWith("Tactosy"))
            {
                return isConnect ? SettingTactosyArm.pairImage : SettingTactosyArm.unpairImage;
            }

            if (deviceType.StartsWith("Tactal"))
            {
                return isConnect ? SettingTactal.pairImage : SettingTactal.unpairImage;
            }

            if (deviceType.StartsWith("Tactot"))
            {
                return isConnect ? SettingTactot.pairImage : SettingTactot.unpairImage;
            }

            return null;
        }

        public Sprite GetScannedDeviceSprite(string deviceType)
        {
            if (deviceType.StartsWith("TactosyH"))
            {
                return SettingTactosyHand.scanImage;
            }

            if (deviceType.StartsWith("TactosyF"))
            {
                return SettingTactosyFoot.scanImage;
            }

            if (deviceType.StartsWith("Tactosy"))
            {
                return SettingTactosyArm.scanImage;
            }

            if (deviceType.StartsWith("Tactal"))
            {
                return SettingTactal.scanImage;
            }

            if (deviceType.StartsWith("Tactot"))
            {
                return SettingTactot.scanImage;
            }

            return null;
        }
    }
}