using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bhaptics.Tact.Unity
{ 
    public class AndroidWidget_DeviceManager : MonoBehaviour
    {
        public static AndroidWidget_DeviceManager Instance;
        [SerializeField] private bool alwaysScanUnConnectedDevice;


        private List<BhapticsDevice> devices = new List<BhapticsDevice>();
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
            if (alwaysScanUnConnectedDevice)
            {
                InvokeRepeating("CheckUnconnectedDevice", 0.5f, 0.5f);
            }
        }
        private void OnDisable()
        {
            if (alwaysScanUnConnectedDevice)
            {
                CancelInvoke();
            }
        }


        private bool ScanTest()
        {
            var androidHapticPlayer = BhapticsManager.HapticPlayer as AndroidHapticPlayer;
            if (androidHapticPlayer == null)
            {
                return false;
            }
            return androidHapticPlayer.IsScanning();
        }
         
        public void ForceUpdateDeviceList()
        {
            var androidHapticPlayer = BhapticsManager.HapticPlayer as AndroidHapticPlayer;
            if (androidHapticPlayer == null)
            {
                return;
            }
            devices = androidHapticPlayer.GetDeviceList();
            IsScanning = androidHapticPlayer.IsScanning();
            RefreshDeviceListUi();
        }

        public void UpdateDevices(List<BhapticsDevice> devices)
        {
            this.devices = devices;
            RefreshDeviceListUi();
        }

        private void RefreshDeviceListUi()
        {
            uiManager.Refresh(devices, IsScanning);
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


        public List<BhapticsDevice> GetDeviceList()
        {
            return devices;
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
            var devicesJson = JSON.Parse(message);

            if (devicesJson.IsArray)
            {
                var deviceList = new List<BhapticsDevice>();
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
                UpdateDevices(deviceList);
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


            #region Check for unconnected devices
            public void CheckUnconnectedDevice()
        {
            var androidHapticPlayer = BhapticsManager.HapticPlayer as AndroidHapticPlayer;
            if (androidHapticPlayer == null)
            {
                return;
            }

            var checkDevices = androidHapticPlayer.GetDeviceList();
            for (int i = 0; i < checkDevices.Count; i++)
            {
                if (checkDevices[i].IsPaired && AndroidWidget_CompareDeviceString.convertConnectionStatus(checkDevices[i].ConnectionStatus) == 2 &&
                    !IsScanning)
                {
                    Scan();
                }
            }
        }
#endregion
    }



}