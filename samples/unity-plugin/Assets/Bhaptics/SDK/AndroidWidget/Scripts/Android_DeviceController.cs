using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bhaptics.Tact.Unity
{
    [Serializable]
    public class PositonIconSetting
    {
        public Sprite connect;
        public Sprite disconnect;
    }

    [Serializable]
    public class IconSetting
    {

        [Header("[Setting Icons]")]
        public PositonIconSetting Vest;
        public PositonIconSetting Head;
        public PositonIconSetting Arm;
        public PositonIconSetting Foot;
        public PositonIconSetting Hand;
    }

    public class Android_DeviceController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image icon;

        [SerializeField] private IconSetting widgetSetting;

        [Header("Connect Menu")]
        [SerializeField] private GameObject ConnectMenu;
        [SerializeField] private Button pingButton;
        [SerializeField] private Button lButton;
        [SerializeField] private Button rButton;

        [Header("Disconnect Menu")] 
        [SerializeField] private GameObject DisconnectMenu;

        private static Color SelectColor = new Color(82f/255, 103f/255, 249f/255);
        private static Color DisableColor = new Color(82f/255, 84f/255, 102f/255);


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

            if (d.IsConnected)
            {
                RenderConnectMenu();
            }
            else
            {
                RenderDisconnectMenu();
            }
        }

        private void RenderConnectMenu()
        {
            ConnectMenu.gameObject.SetActive(true);
            DisconnectMenu.gameObject.SetActive(false);

            UpdateButtons();
        }

        private void RenderDisconnectMenu()
        {
            ConnectMenu.gameObject.SetActive(false);
            DisconnectMenu.gameObject.SetActive(true);
        }

        private void UpdateButtons()
        {
            if (IsLeft(device.Position) || IsRight(device.Position))
            {
                pingButton.gameObject.SetActive(false);
                lButton.gameObject.SetActive(true);

                rButton.gameObject.SetActive(true);
                if (IsLeft(device.Position))
                {
                    lButton.image.color = SelectColor;
                    rButton.image.color = DisableColor;
                }
                else
                {
                    lButton.image.color = DisableColor;
                    rButton.image.color = SelectColor;
                }
            }
            else
            {
                pingButton.gameObject.SetActive(true);
                pingButton.image.color = SelectColor;
                lButton.gameObject.SetActive(false);
                rButton.gameObject.SetActive(false);
            }
        }

        private void UpdateIcon(HapticDevice d)
        {
            switch (d.Position)
            {
                case PositionType.Vest:
                    icon.sprite = GetSprite(widgetSetting.Vest, d.IsConnected);
                    break;
                case PositionType.FootL:
                case PositionType.FootR:
                    icon.sprite = GetSprite(widgetSetting.Foot, d.IsConnected);
                    break;
                case PositionType.HandL:
                case PositionType.HandR:
                    icon.sprite = GetSprite(widgetSetting.Hand, d.IsConnected);
                    break;
                case PositionType.ForearmL:
                case PositionType.ForearmR:
                    icon.sprite = GetSprite(widgetSetting.Arm, d.IsConnected);
                    break;
                case PositionType.Head:
                    icon.sprite = GetSprite(widgetSetting.Head, d.IsConnected);
                    break;

                default:
                    icon.sprite = null;
                    break;
            }
        }

        private Sprite GetSprite(PositonIconSetting icon, bool connected)
        {
            return connected ? icon.connect : icon.disconnect;
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