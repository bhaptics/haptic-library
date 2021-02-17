using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Bhaptics.Tact.Unity
{
    public class AndroidHaptic : IHaptic
    {
        private static AndroidJavaObject androidJavaObject;

        private List<HapticDevice> deviceList = new List<HapticDevice>();

        private List<string> registeredCache = new List<string>();


        private static readonly object[] SubmitRegisteredParams = new object[6];
        private static readonly int[] Empty = new int[20];
        private static readonly object[] EmptyParams = new object[0];

        private static readonly RotationOption DefaultRotationOption = new RotationOption(0, 0);


        private readonly object syncLock = new object();
        private Dictionary<PositionType, int[]> updatedList = new Dictionary<PositionType, int[]>();

        private readonly IntPtr AndroidJavaObjectPtr;
        private readonly IntPtr SubmitRegisteredPtr;
        private readonly IntPtr SubmitRegisteredWithTimePtr;
        private readonly IntPtr RegisterPtr;
        private readonly IntPtr RegisterReflectedPtr;
        private readonly IntPtr ScanPtr;
        private readonly IntPtr StopScanPtr;
        private readonly IntPtr PingAllPtr;
        private readonly IntPtr PingPtr;
        private readonly IntPtr UnpairPtr;
        private readonly IntPtr UnpairAllPtr;

        // bool methods
        private readonly IntPtr IsRegisteredPtr;
        private readonly IntPtr IsPlayingPtr;
        private readonly IntPtr IsPlayingAnythingPtr;
        private readonly IntPtr IsScanningPtr;


        public AndroidHaptic()
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            androidJavaObject =
                new AndroidJavaObject("com.bhaptics.bhapticsunity.BhapticsManagerWrapper", currentActivity);

            AndroidJavaObjectPtr = androidJavaObject.GetRawObject();
            SubmitRegisteredPtr = AndroidJNIHelper.GetMethodID(androidJavaObject.GetRawClass(), "submitRegistered");
            SubmitRegisteredWithTimePtr =
                AndroidJNIHelper.GetMethodID(androidJavaObject.GetRawClass(), "submitRegisteredWithTime");
            RegisterPtr = AndroidJNIHelper.GetMethodID(androidJavaObject.GetRawClass(), "register");
            RegisterReflectedPtr = AndroidJNIHelper.GetMethodID(androidJavaObject.GetRawClass(), "registerReflected");
            ScanPtr = AndroidJNIHelper.GetMethodID(androidJavaObject.GetRawClass(), "scan");
            StopScanPtr = AndroidJNIHelper.GetMethodID(androidJavaObject.GetRawClass(), "stopScan");
            PingAllPtr = AndroidJNIHelper.GetMethodID(androidJavaObject.GetRawClass(), "pingAll");
            PingPtr = AndroidJNIHelper.GetMethodID(androidJavaObject.GetRawClass(), "ping");
            UnpairPtr = AndroidJNIHelper.GetMethodID(androidJavaObject.GetRawClass(), "unpair");
            UnpairAllPtr = AndroidJNIHelper.GetMethodID(androidJavaObject.GetRawClass(), "unpairAll");

            IsRegisteredPtr = AndroidJNIHelper.GetMethodID(androidJavaObject.GetRawClass(), "isRegistered");
            IsPlayingPtr = AndroidJNIHelper.GetMethodID(androidJavaObject.GetRawClass(), "isPlaying");
            IsPlayingAnythingPtr = AndroidJNIHelper.GetMethodID(androidJavaObject.GetRawClass(), "isAnythingPlaying");
            IsScanningPtr = AndroidJNIHelper.GetMethodID(androidJavaObject.GetRawClass(), "isScanning");


            if (AndroidPermissionsManager.CheckBluetoothPermissions())
            {
                deviceList = GetDevices(true);
                StartScan();
            }
        }

        public bool IsConnect(PositionType type)
        {
            foreach (var device in deviceList)
            {
                if (device.Position == type && device.IsConnected)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsConnect(HapticDeviceType type, bool isLeft = true)
        {
            foreach (var device in deviceList)
            {
                if (device.Position == BhapticsUtils.ToPositionType(type, isLeft) && device.IsConnected)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsPlaying(string key)
        {

            if (androidJavaObject == null)
            {
                return false;
            }

            return CallNativeBoolMethod(IsPlayingPtr, new object[] {key});
        }

        public bool IsFeedbackRegistered(string key)
        {
            if (androidJavaObject == null)
            {
                return false;
            }

            if (registeredCache.Contains(key))
            {
                return true;
            }

            var res = CallNativeBoolMethod(IsRegisteredPtr, new object[] {key});
            if (res)
            {
                registeredCache.Add(key);
            }

            return res;
        }

        public bool IsPlaying()
        {
            if (androidJavaObject == null)
            {
                return false;
            }

            return CallNativeBoolMethod(IsPlayingAnythingPtr, EmptyParams);
        }

        public void RegisterTactFileStr(string key, string tactFileStr)
        {
            if (androidJavaObject == null)
            {
                return;
            }

            CallNativeVoidMethod(RegisterPtr, new object[] {key, tactFileStr});
        }

        public void RegisterTactFileStrReflected(string key, string tactFileStr)
        {
            if (androidJavaObject == null)
            {
                return;
            }

            CallNativeVoidMethod(RegisterReflectedPtr, new object[] {key, tactFileStr});
        }

        public void Submit(string key, PositionType position, List<DotPoint> points, int durationMillis)
        {
            if (androidJavaObject == null)
            {
                return;
            }

            if (!AndroidPermissionsManager.CheckBluetoothPermissions())
            {
                return;
            }

            try
            {
                int[] indexes = new int[points.Count];
                int[] intensity = new int[points.Count];
                for (var i = 0; i < points.Count; i++)
                {
                    indexes[i] = points[i].Index;
                    intensity[i] = points[i].Intensity;
                }

                androidJavaObject.Call("submitDot",
                    key, position.ToString(), indexes, intensity, durationMillis);
            }
            catch (Exception e)
            {
                BhapticsLogger.LogError("submitDot() : {0}", e.Message);
            }
        }

        public void Submit(string key, PositionType position, List<PathPoint> points, int durationMillis)
        {
            if (androidJavaObject == null)
            {
                return;
            }

            if (!AndroidPermissionsManager.CheckBluetoothPermissions())
            {
                return;
            }

            try
            {
                float[] x = new float[points.Count];
                float[] y = new float[points.Count];
                int[] intensity = new int[points.Count];
                for (var i = 0; i < points.Count; i++)
                {
                    x[i] = points[i].X;
                    y[i] = points[i].Y;
                    intensity[i] = points[i].Intensity;
                }

                androidJavaObject.Call("submitPath",
                    key, position.ToString(), x, y, intensity, durationMillis);
            }
            catch (Exception e)
            {
                BhapticsLogger.LogError("submitPath() : {0}", e.Message);
            }
        }

        public void SubmitRegistered(string key, string altKey, ScaleOption option)
        {
            SubmitRegistered(key, altKey, DefaultRotationOption, option);
        }

        public void SubmitRegistered(string key, string altKey, RotationOption rOption, ScaleOption sOption)
        {
            SubmitRequest(key, altKey, sOption.Intensity, sOption.Duration, rOption.OffsetAngleX, rOption.OffsetY);
        }

        public void SubmitRegistered(string key)
        {
            SubmitRequest(key, key, 1, 1, 0, 0);
        }

        public void SubmitRegistered(string key, int startTimeMillis)
        {
            if (!AndroidPermissionsManager.CheckBluetoothPermissions())
            {
                return;
            }

            if (androidJavaObject == null)
            {
                return;
            }

            CallNativeVoidMethod(SubmitRegisteredWithTimePtr, new object[] {startTimeMillis});
        }

        public void TurnOff(string key)
        {
            if (!AndroidPermissionsManager.CheckBluetoothPermissions())
            {
                return;
            }

            if (androidJavaObject != null)
            {
                try
                {
                    androidJavaObject.Call("turnOff",
                        key);
                }
                catch (Exception e)
                {
                    BhapticsLogger.LogError("TurnOff() : {0}", e.Message);
                }
            }
        }

        public void TurnOff()
        {
            if (!AndroidPermissionsManager.CheckBluetoothPermissions())
            {
                return;
            }

            if (androidJavaObject != null)
            {
                try
                {
                    androidJavaObject.Call("turnOffAll");
                }
                catch (Exception e)
                {
                    BhapticsLogger.LogError("turnOffAll() : {0}", e.Message);
                }
            }
        }

        public void Dispose()
        {
            if (androidJavaObject != null)
            {
                androidJavaObject.Call("quit");
                androidJavaObject = null;
            }
        }

        private void SubmitRequest(string key, string altKey,
            float intensity, float duration, float offsetAngleX, float offsetY)
        {
            if (androidJavaObject == null)
            {
                return;
            }

            if (!AndroidPermissionsManager.CheckBluetoothPermissions())
            {
                return;
            }

            SubmitRegisteredParams[0] = key;
            SubmitRegisteredParams[1] = altKey;
            SubmitRegisteredParams[2] = intensity;
            SubmitRegisteredParams[3] = duration;
            SubmitRegisteredParams[4] = offsetAngleX;
            SubmitRegisteredParams[5] = offsetY;

            CallNativeVoidMethod(SubmitRegisteredPtr, SubmitRegisteredParams);
        }

        public int[] GetCurrentFeedback(PositionType pos)
        {
            if (androidJavaObject == null)
            {
                return Empty;
            }

            lock (syncLock)
            {
                if (updatedList.ContainsKey(pos))
                {
                    return updatedList[pos];
                }

                byte[] result = androidJavaObject.Call<byte[]>("getPositionStatus", pos.ToString());
                int[] res = Array.ConvertAll(result, System.Convert.ToInt32);
                updatedList[pos] = res;

                return res;
            }
        }


        public List<HapticDevice> GetDevices(bool force = false)
        {
            if (force)
            {
                string result = androidJavaObject.Call<string>("getDeviceList");
                deviceList = AndroidUtils.ConvertToBhapticsDevices(result);
            }

            return deviceList;
        }

        public void UpdateDeviceList(List<HapticDevice> devices)
        {
            deviceList = devices;
        }

        public void Pair(string address, string position)
        {
            if (androidJavaObject != null)
            {
                if (position != "")
                {
                    androidJavaObject.Call("pair", address, position);
                }
                else
                {
                    androidJavaObject.Call("pair", address);
                }
            }
        }

        public void Unpair(string address)
        {
            if (androidJavaObject == null)
            {
                return;
            }

            CallNativeVoidMethod(UnpairPtr, new object[] {address});
        }

        public void UnpairAll()
        {
            if (androidJavaObject == null)
            {
                return;
            }

            CallNativeVoidMethod(UnpairAllPtr, EmptyParams);
        }


        public void StartScan()
        {
            if (androidJavaObject == null)
            {
                return;
            }

            if (!AndroidPermissionsManager.CheckBluetoothPermissions())
            {
                return;
            }

            CallNativeVoidMethod(ScanPtr, EmptyParams);
        }

        public void StopScan()
        {
            if (androidJavaObject == null)
            {
                return;
            }

            if (!AndroidPermissionsManager.CheckBluetoothPermissions())
            {
                return;
            }

            CallNativeVoidMethod(StopScanPtr, EmptyParams);
        }

        public bool IsScanning()
        {
            if (androidJavaObject == null)
            {
                return false;
            }

            return CallNativeBoolMethod(IsScanningPtr, EmptyParams);
        }

        public void TogglePosition(string address)
        {
            if (androidJavaObject == null)
            {
                return;
            }


            if (androidJavaObject != null)
            {
                androidJavaObject.Call("togglePosition", address);
            }
        }

        public void PingAll()
        {
            if (androidJavaObject == null)
            {
                return;
            }

            CallNativeVoidMethod(PingAllPtr, EmptyParams);
        }

        public void Ping(string address)
        {
            if (androidJavaObject == null)
            {
                return;
            }

            CallNativeVoidMethod(PingPtr, new object[] {address});
        }

        public void CheckChange()
        {
            lock (syncLock)
            {
                updatedList.Clear();
            }
        }


        private void CallNativeVoidMethod(IntPtr methodPtr, object[] param)
        {
            if (androidJavaObject == null)
            {
                return;
            }

            AndroidUtils.CallNativeVoidMethod(AndroidJavaObjectPtr, methodPtr, param);
        }


        private bool CallNativeBoolMethod(IntPtr methodPtr, object[] param)
        {
            if (androidJavaObject == null)
            {
                return false;
            }

            return AndroidUtils.CallNativeBoolMethod(AndroidJavaObjectPtr, methodPtr, param);
        }
    }
}