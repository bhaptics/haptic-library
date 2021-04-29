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


        public static bool pcAndoidTestMode = false;

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
                pcAndoidTestMode = true;
            }
#endif
        }
        public static void Ping(PositionType pos)
        {
            var connectedDevices = GetConnectedDevices(pos);
            foreach (var pairedDevice in connectedDevices)
            {
                Ping(pairedDevice);
            }
        }

        #region Connection Related Functions

        public static void Pair(PositionType deviceType)
        {
            var devices = GetDevices();
            int index = -1;

            for (int i = 0; i < devices.Count; i++)
            {
                if (AndroidUtils.CanPair(devices[i], deviceType))
                {
                    index = i;
                    break;
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
            if (androidHapticPlayer == null)
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

        public static List<HapticDevice> GetDevices()
        {
            var androidHapticPlayer = BhapticsManager.GetHaptic() as AndroidHaptic;
            if (androidHapticPlayer == null)
            {
                var device =new HapticDevice()
                {
                    Position = PositionType.Vest,
                    IsConnected = true,
                    IsPaired =  true,
                    Address = "aaaa",
                    DeviceName = "Tactot",
                    Candidates = new PositionType[] { PositionType.Vest },
                };
                var device2 =new HapticDevice()
                {
                    Position = PositionType.ForearmL,
                    IsConnected = false,
                    IsPaired =  false,
                    Address = "aaaa22",
                    DeviceName = "Tactosy",
                    Candidates = new PositionType[] {PositionType.ForearmR, PositionType.ForearmL},
                };
                var list = new List<HapticDevice>();
                list.Add(device);
                list.Add(device2);
                // TODO DEBUGGING USAGE.
                return list;
            }

            return androidHapticPlayer.GetDevices();
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


        public static bool CheckPermission()
        {
            var androidHapticPlayer = BhapticsManager.GetHaptic() as AndroidHaptic;
            if (androidHapticPlayer == null)
            {
                if (pcAndoidTestMode)
                {
                    return true;
                }

                return false;
            }

            return androidHapticPlayer.CheckPermission();
        }

        public static void RequestPermission()
        {

            var androidHapticPlayer = BhapticsManager.GetHaptic() as AndroidHaptic;
            if (androidHapticPlayer == null)
            {
                return;
            }

            androidHapticPlayer.RequestPermission();
        }

        #endregion


        #region Callback Functions from native code

        public void PermissionGranted(string s)
        {
            Scan();
        }
#endregion
    }
}