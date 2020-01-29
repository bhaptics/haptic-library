using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bhaptics.Tact.Unity
{ 
    public class AndroidWidget_DeviceManager : MonoBehaviour
    {
        public static AndroidWidget_DeviceManager Instance;
        [SerializeField] private bool alwaysScanDisconnectedDevice;
        private AndroidWidget_UIManager uiManager; 

        [HideInInspector] public bool IsScanning; 

        private void Awake()
        {
#if !UNITY_ANDROID
            return;
#endif

            if (Instance == null)
            {
                uiManager = GetComponent<AndroidWidget_UIManager>();
                Instance = this; 
            } 
        }

        private void Start()
        {
#if !UNITY_ANDROID
            return;
#endif
            Scan();
        }

        private void OnEnable()
        {
#if !UNITY_ANDROID
            return;
#endif

            if (alwaysScanDisconnectedDevice)
            {
                InvokeRepeating("CheckDisconnectedDevice", 0.5f, 0.5f);
            }
        }
        private void OnDisable()
        {
#if !UNITY_ANDROID
            return;
#endif

            if (alwaysScanDisconnectedDevice)
            {
                CancelInvoke();
            }
        }

        private void OnApplicationQuit()
        {
#if !UNITY_ANDROID
            return;
#endif
            Quit();
        }


        public void ForceUpdateDeviceList()
        {
            var androidHapticPlayer = BhapticsManager.HapticPlayer as AndroidHapticPlayer;
            if (androidHapticPlayer == null)
            {
                return;
            }
            androidHapticPlayer.GetDeviceList();
            IsScanning = androidHapticPlayer.IsScanning();
            RefreshDeviceListUi();
        }

        public void UpdateDeviceUI(List<BhapticsDevice> devices)
        {
            var androidHapticPlayer = BhapticsManager.HapticPlayer as AndroidHapticPlayer;
            if (androidHapticPlayer == null)
            {
                return;
            }
            androidHapticPlayer.UpdateDeviceList(devices);
            RefreshDeviceListUi();
        }

        private void RefreshDeviceListUi()
        {
            var androidHapticPlayer = BhapticsManager.HapticPlayer as AndroidHapticPlayer;
            if (androidHapticPlayer == null)
            {
                return;
            }
            uiManager.Refresh(androidHapticPlayer.GetDeviceList(), IsScanning);
        }

        public void UpdateScanning(bool isScanning)
        {
            IsScanning = isScanning;
            RefreshDeviceListUi();
        }

        public void Pair(string address, string position = "")
        {
            var androidHapticPlayer = BhapticsManager.HapticPlayer as AndroidHapticPlayer;
            if (androidHapticPlayer == null)
            {
                return;
            }

            androidHapticPlayer.Pair(address, position);
        }

        public void Unpair(string address)
        {
            var androidHapticPlayer = BhapticsManager.HapticPlayer as AndroidHapticPlayer;
            if (androidHapticPlayer == null)
            {
                return;
            }

            androidHapticPlayer.Unpair(address);
        }

        public void UnpairAll()
        {
            var androidHapticPlayer = BhapticsManager.HapticPlayer as AndroidHapticPlayer;
            if (androidHapticPlayer == null)
            {
                return;
            }
            androidHapticPlayer.UnpairAll();
        }

        public void Scan()
        {
            var androidHapticPlayer = BhapticsManager.HapticPlayer as AndroidHapticPlayer;
            if (androidHapticPlayer == null)
            {
                return;
            } 
            androidHapticPlayer.StartScan();
        }


        public void ScanStop()
        {
            var androidHapticPlayer = BhapticsManager.HapticPlayer as AndroidHapticPlayer;
            if (androidHapticPlayer == null)
            {
                return;
            }

            androidHapticPlayer.StopScan();
        }

        public void TogglePosition(string address)
        {
            var androidHapticPlayer = BhapticsManager.HapticPlayer as AndroidHapticPlayer;
            if (androidHapticPlayer == null)
            {
                return;
            }

            androidHapticPlayer.TogglePosition(address);
        }

        public void Ping(string address)
        {
            var androidHapticPlayer = BhapticsManager.HapticPlayer as AndroidHapticPlayer;
            if (androidHapticPlayer == null)
            {
                return;
            }

            androidHapticPlayer.Ping(address);
        }

        public void PingAll()
        {
            var androidHapticPlayer = BhapticsManager.HapticPlayer as AndroidHapticPlayer;
            if (androidHapticPlayer == null)
            {
                return;
            }

            androidHapticPlayer.PingAll();
        }

        public void Quit()
        {
            var androidHapticPlayer = BhapticsManager.HapticPlayer as AndroidHapticPlayer;
            if (androidHapticPlayer == null)
            {
                return;
            }

            androidHapticPlayer.Quit();
        }

        public void ScanButton()
        {
            if (IsScanning)
            {
                ScanStop();
            }
            else
            {
                Scan();
            }
        }

#region Callback Functions from native code

        public void OnChangeResponse(string message)
        {
            if (message == "")
            {
                return;
            }
            var response = PlayerResponse.ToObject(message);
            try
            {
                var androidHapticPlayer = BhapticsManager.HapticPlayer as AndroidHapticPlayer;
                if (androidHapticPlayer == null)
                {
                    return;
                }
                androidHapticPlayer.Receive(response);
            }
            catch (Exception e)
            {
                Debug.Log(message + " : " + e.Message);
            }
        }

        public void ScanStatusChanged(string message)
        {
            var isScanning = JSON.Parse((message));
            UpdateScanning(isScanning.AsBool);
        }

        public void OnDeviceUpdate(string message)
        {
            var androidHapticPlayer = BhapticsManager.HapticPlayer as AndroidHapticPlayer;
            if (androidHapticPlayer == null)
            {
                return;
            }
            List<BhapticsDevice> deviceList = new List<BhapticsDevice>();

            var devicesJson = JSON.Parse(message);
            if (devicesJson.IsArray)
            {
                var arr = devicesJson.AsArray;

                foreach (var deviceJson in arr.Children)
                {
                    var device = new BhapticsDevice();
                    device.IsPaired = deviceJson["IsPaired"];
                    device.Address = deviceJson["Address"];
                    device.Battery = deviceJson["Battery"];
                    device.ConnectionStatus = deviceJson["ConnectionStatus"];
                    device.DeviceName = deviceJson["DeviceName"];
                    device.Position = deviceJson["Position"];
                    device.Rssi = deviceJson["Rssi"];
                    deviceList.Add(device);
                }
                androidHapticPlayer.UpdateDeviceList(deviceList);
                UpdateDeviceUI(deviceList);
            }
        }

        public void OnConnect(string address)
        {
            var androidHapticPlayer = BhapticsManager.HapticPlayer as AndroidHapticPlayer;
            if (androidHapticPlayer == null)
            {
                return;
            }
            androidHapticPlayer.Connected(address);
        }
        public void OnDisconnect(string address)
        {
            var androidHapticPlayer = BhapticsManager.HapticPlayer as AndroidHapticPlayer;
            if (androidHapticPlayer == null)
            {
                return;
            }
            androidHapticPlayer.Disconnected(address);
        }
#endregion



#region Check for disconnected devices
            public void CheckDisconnectedDevice()
        {
            var androidHapticPlayer = BhapticsManager.HapticPlayer as AndroidHapticPlayer;
            if (androidHapticPlayer == null)
            {
                return;
            }
            var devices = androidHapticPlayer.GetDeviceList();
            if (devices != null)
            {
                for (int i = 0; i < devices.Count; i++)
                {
                    if (devices[i].IsPaired && !IsScanning && AndroidWidget_CompareDeviceString.convertConnectionStatus(devices[i].ConnectionStatus) == 2)
                    {
                        Scan();
                        break;
                    }
                }
            }
        }
#endregion
    }



}