using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Bhaptics.Tact.Unity
{ 
    public class BhapticsAndroidManager : MonoBehaviour
    {
        private static BhapticsAndroidManager Instance;

        [HideInInspector] public bool alwaysScanDisconnectedDevice;

        private List<UnityAction> refreshActions = new List<UnityAction>();





        void Awake()
        {
            if (Instance != null)
            {
                DestroyImmediate(this);
                return;
            }

            Instance = this;
            name = "[bHapticsAndroidManager]";
        }

        void Start()
        {
#if UNITY_ANDROID
            if (Application.platform != RuntimePlatform.Android)
            {
                // only for debugging
                BhapticsLogger.LogDebug("InvokeRefresh for debuggin usage.");

                InvokeRepeating("InvokeRefresh", 1f, 1f);
            }
#endif
        }

        void OnEnable()
        {
            if (alwaysScanDisconnectedDevice)
            {
                InvokeRepeating("CheckIfNeededToScan", 0.5f, 0.5f);
            }
        }

        void OnDisable()
        {
            if (alwaysScanDisconnectedDevice)
            {
                CancelInvoke();
            }
        }








        public static void ForceUpdateDeviceList()
        {
            GetDevices(true);
            RefreshDeviceListUi();
        }

        public static void Ping(PositionType pos)
        {
            var connectedDevices = GetConnectedDevices(pos);
            foreach (var pairedDevice in connectedDevices)
            {
                Ping(pairedDevice);
            }
        }

        private static void OnUpdateDevicesChange(List<HapticDevice> devices)
        {
            var androidHapticPlayer = BhapticsManager.GetHaptic() as AndroidHaptic;
            if (androidHapticPlayer == null)
            {
                return;
            }
            androidHapticPlayer.UpdateDeviceList(devices);
        }

        private static void RefreshDeviceListUi()
        {
            var androidHapticPlayer = BhapticsManager.GetHaptic() as AndroidHaptic;
            if (androidHapticPlayer == null)
            {
                return;
            }

            if (Instance != null)
            {
                Instance.InvokeRefresh();
            }
        }

        private void UpdateScanning(bool isScanning)
        {
            RefreshDeviceListUi();
        }


        #region Connection Related Functions

        public static void Pair(PositionType deviceType)
        {
            var devices = GetDevices();
            int rssi = -9999;
            int index = -1;

            for (int i = 0; i < devices.Count; i++)
            {
                if (AndroidUtils.CanPair(devices[i], deviceType))
                {
                    if (rssi < devices[i].Rssi)
                    {
                        rssi = devices[i].Rssi;
                        index = i;
                    }
                }
            }

            if (index != -1)
            {

                if (deviceType == PositionType.Vest)
                {
                    Pair(devices[index].Address);
                }
                else
                {
                    Pair(devices[index].Address, deviceType.ToString());
                }
            }
        }

        public static void Pair(string address, string position = "")
        {
            var androidHapticPlayer = BhapticsManager.GetHaptic() as AndroidHaptic;
            if (androidHapticPlayer == null)
            {
                return;
            }
            androidHapticPlayer.Pair(address, position);
        }

        public static void Unpair(PositionType deviceType)
        {
            var devices = GetPairedDevices(deviceType);
            for (int i = 0; i < devices.Count; ++i)
            {
                if (devices[i].Position == deviceType)
                {
                    Unpair(devices[i].Address);
                }
            }
        }

        public static void Unpair(string address)
        {
            var androidHapticPlayer = BhapticsManager.GetHaptic() as AndroidHaptic;
            if (androidHapticPlayer == null)
            {
                return;
            }

            androidHapticPlayer.Unpair(address);
        }

        public static void UnpairAll()
        {
            var androidHapticPlayer = BhapticsManager.GetHaptic() as AndroidHaptic;
            if (androidHapticPlayer == null)
            {
                return;
            }
            androidHapticPlayer.UnpairAll();
        }

        public static void Scan()
        {
            var androidHapticPlayer = BhapticsManager.GetHaptic() as AndroidHaptic;
            if (androidHapticPlayer == null || !AndroidPermissionsManager.CheckBluetoothPermissions())
            {
                return;
            }

            if (!androidHapticPlayer.IsScanning())
            {
                androidHapticPlayer.StartScan();
            }
        }

        public static void ScanStop()
        {
            var androidHapticPlayer = BhapticsManager.GetHaptic() as AndroidHaptic;
            if (androidHapticPlayer == null)
            {
                return;
            }

            androidHapticPlayer.StopScan();
        }

        public static void TogglePosition(string address)
        {
            var androidHapticPlayer = BhapticsManager.GetHaptic() as AndroidHaptic;
            if (androidHapticPlayer == null)
            {
                return;
            }

            androidHapticPlayer.TogglePosition(address);
        }

        public static void Ping(HapticDevice device)
        {
            var androidHapticPlayer = BhapticsManager.GetHaptic() as AndroidHaptic;
            if (androidHapticPlayer == null)
            {
                return;
            }

            androidHapticPlayer.Ping(device.Address);
        }

        public static void PingAll()
        {
            var androidHapticPlayer = BhapticsManager.GetHaptic() as AndroidHaptic;
            if (androidHapticPlayer == null)
            {
                return;
            }

            androidHapticPlayer.PingAll();
        }

        public static bool IsScanning()
        {
            var androidHapticPlayer = BhapticsManager.GetHaptic() as AndroidHaptic;
            if (androidHapticPlayer == null)
            {
                return false;
            }

            return androidHapticPlayer.IsScanning();
        }

        public static bool CanPairDevice(PositionType position)
        {
            var deviceList = GetDevices();
            foreach (var device in deviceList)
            {
                if (AndroidUtils.CanPair(device, position))
                {
                    return true;
                }
            }
            return false;
        }

        public static List<HapticDevice> GetDevices(bool force = false)
        {
            var androidHapticPlayer = BhapticsManager.GetHaptic() as AndroidHaptic;
            if (androidHapticPlayer == null)
            {
                var device =new HapticDevice()
                {
                    Position = PositionType.Vest,
                    IsConnected = false,
                    IsPaired =  true,
                    Address = "aaaa",
                    Battery = 100,
                    Rssi = -100,
                    DeviceName = "Tactot",
                    Candidates = new PositionType[] { PositionType.Vest },
                    ConnectionStatus = "Disconnected",
                };
                var device2 =new HapticDevice()
                {
                    Position = PositionType.ForearmL,
                    IsConnected = false,
                    IsPaired =  false,
                    Address = "aaaa22",
                    Battery = 100,
                    Rssi = -100,
                    DeviceName = "Tactosy",
                    ConnectionStatus = "Disconnected",
                    Candidates = new PositionType[] {PositionType.ForearmR, PositionType.ForearmL},
                };
                var list = new List<HapticDevice>();
                // list.Add(device);
                // list.Add(device2);
                // TODO DEBUGGING USAGE.
                return list;
            }

            return androidHapticPlayer.GetDevices(force);
        }

        public static List<HapticDevice> GetConnectedDevices(PositionType pos)
        {
            var pairedDeviceList = new List<HapticDevice>();
            var devices = GetDevices();
            foreach (var device in devices)
            {
                if (device.IsPaired && device.Position == pos && device.IsConnected)
                {
                    pairedDeviceList.Add(device);
                }
            }

            return pairedDeviceList;
        }

        public static bool IsNeededToScan()
        {
            var devices = GetDevices();
            if (IsScanning())
            {
                return false;
            }

            if (devices != null)
            {
                for (int i = 0; i < devices.Count; i++)
                {
                    if (devices[i].IsPaired && AndroidUtils.ConvertConnectionStatus(devices[i].ConnectionStatus) == 2)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static List<HapticDevice> GetPairedDevices(PositionType pos)
        {
            var res = new List<HapticDevice>();
            var devices = GetDevices();
            foreach (var device in devices)
            {
                if (device.IsPaired && device.Position == pos)
                {
                    res.Add(device);
                }
            }

            return res;
        }

        #endregion


        #region Callback Functions from native code

        public void OnChange()
        {
            try
            {
                var androidHapticPlayer = BhapticsManager.GetHaptic() as AndroidHaptic;
                if (androidHapticPlayer == null)
                {
                    return;
                }

                androidHapticPlayer.CheckChange();
            }
            catch (Exception e)
            {
                BhapticsLogger.LogInfo("OnChange {0}", e.Message);
            }
        }

        public void ScanStatusChanged(string message)
        {
            var isScanning = JSON.Parse((message));
            UpdateScanning(isScanning.AsBool);
        }

        public void OnDeviceUpdate(string message)
        {
            var deviceList = AndroidUtils.ConvertToBhapticsDevices(message);
            OnUpdateDevicesChange(deviceList);
            RefreshDeviceListUi();

        }

        public void OnConnect(string address)
        {
            // nothing to do
        }

        public void OnDisconnect(string address)
        {
            // nothing to do
        }
        public void PermissionGranted(string permission)
        {
            Scan();
        }
#endregion

#region Callback Functions from UI update 
        public static void AddRefresh(UnityAction call)
        {
            if (Instance == null)
            {
                return;
            }

            int index = Instance.GetListenerIndex(call);
            if (index == -1)
            {
                Instance.refreshActions.Add(call);
            }
        }

        public static void RemoveRefresh(UnityAction call)
        {
            if (Instance == null)
            {
                return;
            }

            int index = Instance.GetListenerIndex(call);
            if (index != -1)
            {
                Instance.refreshActions.RemoveAt(index);
            }
        }

        private int GetListenerIndex(UnityAction call)
        {
            for (int i = 0; i < Instance.refreshActions.Count; i++)
            {
                if (refreshActions[i] == call)
                {
                    return i;
                }
            }
            return -1;
        }

        private void InvokeRefresh()
        {
            for (int i = 0; i < refreshActions.Count; i++)
            {
                refreshActions[i].Invoke();
            }
        }
#endregion



#region Check for Disconnected devices
        private void CheckIfNeededToScan()
        {
            if (IsNeededToScan())
            {
                Scan();
            }
        }
#endregion
    }
}