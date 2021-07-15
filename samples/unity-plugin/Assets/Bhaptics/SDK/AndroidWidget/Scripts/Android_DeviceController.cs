using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bhaptics.Tact.Unity
{
    public class Android_DeviceController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image icon;
        [SerializeField] private Button pingButton;
        [SerializeField] private Button lButton;
        [SerializeField] private Button rButton;

        [SerializeField] private Bhaptics_Widget_Setting widgetSetting;


        private HapticDevice device;

        void Awake()
        {
            if (pingButton != null)
            {
                pingButton.onClick.AddListener(Ping);
            }
            if (lButton != null)
            {
                lButton.onClick.AddListener(ToLeft);
            }
            if (rButton != null)
            {
                rButton.onClick.AddListener(ToRight);
            }
        }


        public void RefreshDevice(HapticDevice d)
        {
            device = d;
            if (device == null)
            {
                gameObject.SetActive(false);
                return;
            }

            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            UpdateIcon(d);
        }

        private void UpdateIcon(HapticDevice d)
        {
            switch (d.Position)
            {
                case PositionType.Vest:
                    icon.sprite = GetSprite(widgetSetting.SettingTactot, d.IsConnected);
                    break;
                case PositionType.FootL:
                case PositionType.FootR:
                    icon.sprite = GetSprite(widgetSetting.SettingTactosyFoot, d.IsConnected);
                    break;
                case PositionType.HandL:
                case PositionType.HandR:
                    icon.sprite = GetSprite(widgetSetting.SettingTactosyHand, d.IsConnected);
                    break;
                case PositionType.ForearmL:
                case PositionType.ForearmR:
                    icon.sprite = GetSprite(widgetSetting.SettingTactosyArm, d.IsConnected);
                    break;
                case PositionType.Head:
                    icon.sprite = GetSprite(widgetSetting.SettingTactal, d.IsConnected);
                    break;

                default:
                    icon.sprite = null;
                    break;
            }
        }

        private Sprite GetSprite(SettingDeviceIcon icon, bool connected)
        {
            return connected ? icon.pairImage : icon.unpairImage;
        }

        private void Ping()
        {
            if (device == null)
            {
                return;
            }

            BhapticsAndroidManager.Ping(device);
        }


        private void ToLeft()
        {
            if (device == null)
            {
                return;
            }

            if (IsRight(device.Position))
            {
                BhapticsAndroidManager.TogglePosition(device.Address);
            }
        }
        private void ToRight()
        {
            if (device == null)
            {
                return;
            }
            if (IsLeft(device.Position))
            {
                BhapticsAndroidManager.TogglePosition(device.Address);
            }
        }

        private static bool IsLeft(PositionType pos)
        {
            switch (pos)
            {
                case PositionType.FootL:
                case PositionType.HandL:
                case PositionType.ForearmL:
                case PositionType.Left:
                case PositionType.GloveLeft:
                    return true;

            }
            return false;
        }

        private static bool IsRight(PositionType pos)
        {
            switch (pos)
            {
                case PositionType.FootR:
                case PositionType.HandR:
                case PositionType.ForearmR:
                case PositionType.Right:
                case PositionType.GloveRight:
                    return true;

            }
            return false;
        }
    }

}