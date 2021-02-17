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

        private static readonly RotationOption DefaultRotationOption = new RotationOption(0, 0);


        private readonly object syncLock = new object();
        private Dictionary<PositionType, int[]> updatedList = new Dictionary<PositionType, int[]>();

        private readonly IntPtr AndroidJavaObjectPtr;
        private readonly IntPtr SubmitRegisteredPtr;

        public AndroidHaptic()
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            androidJavaObject = new AndroidJavaObject("com.bhaptics.bhapticsunity.BhapticsManagerWrapper", currentActivity);

            AndroidJavaObjectPtr = androidJavaObject.GetRawObject();
            SubmitRegisteredPtr = AndroidJNIHelper.GetMethodID(androidJavaObject.GetRawClass(), "submitRegistered");


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
            if (androidJavaObject != null)
            {
                try
                {
                    return androidJavaObject.Call<bool>("isPlaying", key);
                }
                catch (Exception e)
                {
                    BhapticsLogger.LogError("isPlaying() : {0}", e.Message);
                }
            }

            return false;
        }

        public bool IsFeedbackRegistered(string key)
        {
            if (registeredCache.Contains(key))
            {
                return true;
            }

            if (androidJavaObject != null)
            {
                try
                {
                    var res = androidJavaObject.Call<bool>("isRegistered", key);
                    if (res)
                    {
                        registeredCache.Add(key);
                    }

                    return res;
                }
                catch (Exception e)
                {
                    BhapticsLogger.LogError("isRegistered() : {0}", e.Message);
                }
            }

            return false;
        }

        public bool IsPlaying()
        {
            if (androidJavaObject != null)
            {
                try
                {
                    return androidJavaObject.Call<bool>("isAnythingPlaying");
                }
                catch (Exception e)
                {
                    BhapticsLogger.LogError("isAnythingPlaying() : {0}", e.Message);
                }
            }

            return false;
        }

        public void RegisterTactFileStr(string key, string tactFileStr)
        {
            Register(key, tactFileStr);
        }

        public void RegisterTactFileStrReflected(string key, string tactFileStr)
        {
            RegisterReflected(key, tactFileStr);
        }

        public void Submit(string key, PositionType position, List<DotPoint> points, int durationMillis)
        {
            if (!AndroidPermissionsManager.CheckBluetoothPermissions())
            {
                return;
            }

            if (androidJavaObject != null)
            {
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
        }

        public void Submit(string key, PositionType position, List<PathPoint> points, int durationMillis)
        {
            if (!AndroidPermissionsManager.CheckBluetoothPermissions())
            {
                return;
            }
            if (androidJavaObject != null)
            {
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

            if (androidJavaObject != null)
            {
                try
                {
                    androidJavaObject.Call("submitRegisteredWithTime",
                        key, startTimeMillis);
                }
                catch (Exception e)
                {
                    BhapticsLogger.LogError("TurnOff() : {0}", e.Message);
                }
            }
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
            if (!AndroidPermissionsManager.CheckBluetoothPermissions())
            {
                return;
            }

            if (androidJavaObject != null)
            {
                SubmitRegisteredParams[0] = key;
                SubmitRegisteredParams[1] = altKey;
                SubmitRegisteredParams[2] = intensity;
                SubmitRegisteredParams[3] = duration;
                SubmitRegisteredParams[4] = offsetAngleX;
                SubmitRegisteredParams[5] = offsetY;

                jvalue[] args = AndroidJNIHelper.CreateJNIArgArray(SubmitRegisteredParams);
                try
                {
                    AndroidJNI.CallVoidMethod(AndroidJavaObjectPtr, SubmitRegisteredPtr, args);

                    //androidJavaObject.Call("submitRegistered", _params);
                }
                catch (Exception e)
                {
                    BhapticsLogger.LogError("SubmitRequest() : {0}", e.Message);
                }
                finally
                {
                    AndroidJNIHelper.DeleteJNIArgArray(SubmitRegisteredParams, args);
                }
            }
        }

        private void Register(string key, string tactFileString)
        {
            if (!AndroidPermissionsManager.CheckBluetoothPermissions())
            {
                return;
            }
            if (androidJavaObject == null)
            {
                return;
            }


            androidJavaObject.Call("register", key, tactFileString);
        }

        private void RegisterReflected(string key, string tactFileString)
        {
            if (!AndroidPermissionsManager.CheckBluetoothPermissions())
            {
                return;
            }
            if (androidJavaObject == null)
            {
                return;
            }


            androidJavaObject.Call("registerReflected", key, tactFileString);
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
            if (androidJavaObject != null)
            {
                androidJavaObject.Call("unpair", address);
            }
        }

        public void UnpairAll()
        {
            if (androidJavaObject != null)
            {
                androidJavaObject.Call("unpairAll");
            }
        }


        public void StartScan()
        {
            if (androidJavaObject != null)
            {
                BhapticsLogger.LogDebug("StartScan()");
                androidJavaObject.Call("scan");
            }
        }

        public void StopScan()
        {
            if (androidJavaObject != null)
            {
                androidJavaObject.Call("stopScan");
            }
        }

        public bool IsScanning()
        {
            if (androidJavaObject != null)
            {
                return androidJavaObject.Call<bool>("isScanning");
            }

            return false;
        }

        public void TogglePosition(string address)
        {
            if (androidJavaObject != null)
            {
                androidJavaObject.Call("togglePosition", address);
            }
        }

        public void PingAll()
        {
            if (androidJavaObject != null)
            {
                androidJavaObject.Call("pingAll");
            }
        }

        public void Ping(string address)
        {
            if (androidJavaObject != null)
            {
                androidJavaObject.Call("ping", address);
            }
        }

        public void CheckChange()
        {
            lock (syncLock)
            {
                updatedList.Clear();
            }
        }
    }
}