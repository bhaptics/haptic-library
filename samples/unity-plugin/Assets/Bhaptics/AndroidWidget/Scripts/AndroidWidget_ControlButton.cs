using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bhaptics.Tact.Unity
{
    public class AndroidWidget_ControlButton : MonoBehaviour
    {

        [SerializeField] private PositionType DeviceType;

        [Header("Images")] [SerializeField] private Sprite defaultImage;
        [SerializeField] private Sprite pairImage;
        [SerializeField] private Sprite defaultHoverImage;
        [SerializeField] private Sprite pairHoverImage;


        [Header("UI")] [SerializeField] private Image canPairImage;
        [SerializeField] private GameObject unPairButton;
        [SerializeField] private Transform pairDeviceCount;

        private Button button;
        
        void Awake()
        {
            button = GetComponent<Button>();
            unPairButton.GetComponent<Button>().onClick.AddListener(OnUnpairDevice);
            button.onClick.AddListener(OnClickDevice);
            BhapticsAndroidManager.AddRefresh(Refresh);

            BhapticsLogger.LogDebug("start");
        }

        private void OnEnable()
        {
            InvokeRepeating("BlinkCanPair", 0f, 0.1f);
            BhapticsAndroidManager.AddRefresh(Refresh);
        }

        private void OnDisable()
        {
            CancelInvoke();
            BhapticsAndroidManager.RemoveRefresh(Refresh);
        }


        public void Refresh()
        {
            BhapticsLogger.LogDebug("Refresh()");

            var connectedDevices = BhapticsAndroidManager.GetConnectedDevices(DeviceType);
            if (connectedDevices.Count > 0)
            {
                button.image.sprite = pairImage;
                unPairButton.SetActive(true);
                var spriteState = button.spriteState;
                spriteState.highlightedSprite = pairHoverImage;
                button.spriteState = spriteState;
                canPairImage.gameObject.SetActive(false);

                for (int i = 0; i < pairDeviceCount.childCount; i++)
                {
                    if (!pairDeviceCount.GetChild(i).gameObject.activeSelf)
                    {
                        break;
                    }

                    pairDeviceCount.GetChild(i).gameObject.SetActive(false);
                }

                for (int i = 0; i < connectedDevices.Count; i++)
                {
                    if (pairDeviceCount.GetChild(i) != null)
                    {
                        pairDeviceCount.GetChild(i).gameObject.SetActive(true);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                BhapticsLogger.LogDebug("button + ", " + defaultImage");
                button.image.sprite = defaultImage;
                unPairButton.SetActive(false);
                var spriteState = button.spriteState;
                spriteState.highlightedSprite = defaultHoverImage;
                button.spriteState = spriteState;
                canPairImage.gameObject.SetActive(BhapticsAndroidManager.CanPairDevice(DeviceType));
                

                for (int i = 0; i < pairDeviceCount.childCount; i++)
                {
                    pairDeviceCount.GetChild(i).gameObject.SetActive(false);
                }
            }
        }


        private void BlinkCanPair()
        {
            canPairImage.enabled = !canPairImage.enabled;
        }


        public void OnPairDevice()
        {
            var devices = BhapticsAndroidManager.GetDevices();
            int rssi = -9999;
            int index = -1;

            for(int i = 0; i < devices.Count; i++)
            {
                if (AndroidUtils.CanPair(devices[i], DeviceType))
                {
                    if(rssi < devices[i].Rssi)
                    {
                        rssi = devices[i].Rssi;
                        index = i;
                    }
                }
            }

            if (index != -1)
            {
                
                if(DeviceType == PositionType.Vest)
                {
                    BhapticsAndroidManager.Pair(devices[index].Address);
                }
                else
                {
                    BhapticsAndroidManager.Pair(devices[index].Address, DeviceType.ToString());
                }
            }
        }


        private void OnUnpairDevice()
        {
            var pairedDevices = BhapticsAndroidManager.GetPairedDevices(DeviceType);
            foreach (var pairedDevice in pairedDevices)
            {
                if (pairedDevice.IsConnected)
                {
                    BhapticsAndroidManager.Unpair(pairedDevice.Address);
                }
            }
        }

        public void OnPingDevice()
        {
            BhapticsAndroidManager.Ping(DeviceType);
        }

        public void OnClickDevice()
        {
            var connectedDevices = BhapticsAndroidManager.GetConnectedDevices(DeviceType);

            if (connectedDevices.Count > 0)
            {
                OnPingDevice();
            }
            else
            {
                OnPairDevice();
            }
        }
    }
}
