using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bhaptics.Tact.Unity
{
    public class AndroidWidget_ControlButton : MonoBehaviour
    {

        [SerializeField] private TactDeviceType DeviceType;

        [Header("Images")] [SerializeField] private Sprite defaultImage;
        [SerializeField] private Sprite pairImage;
        [SerializeField] private Sprite defaultHoverImage;
        [SerializeField] private Sprite pairHoverImage;


        [Header("UI")] [SerializeField] private Image canPairImage;
        [SerializeField] private GameObject unPairButton;
        [SerializeField] private Transform pairDeviceCount;

        private Button button;
        private bool canPair;

        private bool isLeft;
        
        void Start()
        {
            isLeft = DeviceType.ToString().Contains("Left"); 
            button = GetComponent<Button>(); 
        }

        private void OnEnable()
        {
            InvokeRepeating("RefreshUI", 0f, 0.1f);
            InvokeRepeating("CanPairBlink", 0f, 0.1f);
        }

        private void OnDisable()
        {
            CancelInvoke();
        }


        private void RefreshUI()
        {
            var pairedDevices = GetPariedDevice();
            if (pairedDevices.Count > 0)
            {
                button.image.sprite = pairImage;
                button.onClick.RemoveListener(OnPairDevice);
                button.onClick.RemoveListener(OnPingDevice);
                button.onClick.AddListener(OnPingDevice);
                unPairButton.SetActive(true);
                unPairButton.GetComponent<Button>().onClick.AddListener(OnUnpairDevice);
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

                for (int i = 0; i < pairedDevices.Count; i++)
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
                button.image.sprite = defaultImage;
                button.onClick.RemoveListener(OnPairDevice);
                button.onClick.RemoveListener(OnPingDevice);
                button.onClick.AddListener(OnPairDevice);
                unPairButton.GetComponent<Button>().onClick.RemoveListener(OnUnpairDevice);
                unPairButton.SetActive(false);
                var spriteState = button.spriteState;
                spriteState.highlightedSprite = defaultHoverImage;
                button.spriteState = spriteState;
                canPairImage.gameObject.SetActive(CanPairedDevice());
                

                for (int i = 0; i < pairDeviceCount.childCount; i++)
                {
                    pairDeviceCount.GetChild(i).gameObject.SetActive(false);
                }
            }
        }


        private void CanPairBlink()
        {
            canPairImage.enabled = !canPairImage.enabled;
        }


        public void OnPairDevice()
        {
            var androidHapticPlayer = BhapticsManager.HapticPlayer as AndroidHapticPlayer;
            if(androidHapticPlayer == null)
            {
                return;
            }

            var devices = androidHapticPlayer.GetDeviceList();
            int rssi = -9999;
            int index = -1;

            for(int i = 0; i < devices.Count; i++)
            {
                string pairDevice = AndroidWidget_CompareDeviceString.GetDeviceNameString(DeviceType);
                if (devices[i].DeviceName.StartsWith(pairDevice) &&
                    AndroidWidget_CompareDeviceString.convertConnectionStatus(devices[i].ConnectionStatus) == 2)
                {
                    if(rssi < int.Parse(devices[i].Rssi))
                    {
                        rssi = int.Parse(devices[i].Rssi);
                        index = i;
                    }
                    else
                    {
                        continue;
                    } 
                }
            }

            if (index != -1)
            {
                
                if(DeviceType == TactDeviceType.TactosyLeft)
                {
                    AndroidWidget_DeviceManager.Instance.Pair(devices[index].Address, "ForearmL");
                }
                else if(DeviceType == TactDeviceType.TactosyRight)
                {
                    AndroidWidget_DeviceManager.Instance.Pair(devices[index].Address, "ForearmR");
                }
                else
                {
                    AndroidWidget_DeviceManager.Instance.Pair(devices[index].Address);
                }


            }
        }


        private void OnUnpairDevice()
        {
            var pairedDevices = GetPariedDevice();
            foreach (var pairedDevice in pairedDevices)
            {
                if (AndroidWidget_CompareDeviceString.convertConnectionStatus(pairedDevice.ConnectionStatus) == 0 ||
                    (AndroidWidget_CompareDeviceString.convertConnectionStatus(pairedDevice.ConnectionStatus) == 2 &&
                     pairedDevice.IsPaired))
                    AndroidWidget_DeviceManager.Instance.Unpair(pairedDevice.Address);
            }
        }

        public void OnPingDevice()
        {
            var pairedDevices = GetPariedDevice();
            foreach (var pairedDevice in pairedDevices)
            {
                AndroidWidget_DeviceManager.Instance.Ping(pairedDevice.Address);
            }
        }

        private List<BhapticsDevice> GetPariedDevice()
        {
            var androidHapticPlayer = BhapticsManager.HapticPlayer as AndroidHapticPlayer;
            if (androidHapticPlayer == null)
            {
                return new List<BhapticsDevice>();
            }

            List<BhapticsDevice> pairedDeviceList = new List<BhapticsDevice>();
            var deviceList = androidHapticPlayer.GetDeviceList();
            string position = AndroidWidget_CompareDeviceString.GetPositionString(DeviceType);
            foreach (var device in deviceList)
            {
                if (device.IsPaired && device.Position.StartsWith(position) &&
                    AndroidWidget_CompareDeviceString.convertConnectionStatus(device.ConnectionStatus) == 0)
                {
                    pairedDeviceList.Add(device);
                }
            }

            return pairedDeviceList;
        }

        private bool CanPairedDevice()
        {
            var androidHapticPlayer = BhapticsManager.HapticPlayer as AndroidHapticPlayer;
            if (androidHapticPlayer == null)
            {
                return false;
            }

            var deviceList = androidHapticPlayer.GetDeviceList();
            string position = AndroidWidget_CompareDeviceString.GetDeviceNameString(DeviceType);
            foreach (var device in deviceList)
            { 
                if(position == "Tactosy")
                {
                    if(!device.IsPaired && (device.DeviceName.StartsWith("Tactosy_") || device.DeviceName.StartsWith("Tactosy2")))
                    {
                        return true;
                    } 
                    continue; 
                }

                if (!device.IsPaired && device.DeviceName.StartsWith(position))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
