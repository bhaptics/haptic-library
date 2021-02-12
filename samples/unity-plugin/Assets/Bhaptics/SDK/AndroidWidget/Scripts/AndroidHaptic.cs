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


        private readonly object syncObject = new object();

        private readonly List<string> activeKeys = new List<string>();
        private readonly List<string> registered = new List<string>();
        private Dictionary<string, int[]> status = new Dictionary<string, int[]>();

        public AndroidHaptic()
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            androidJavaObject = new AndroidJavaObject("com.bhaptics.bhapticsunity.BhapticsManagerWrapper", currentActivity);
            TurnOnVisualization();
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
            lock (syncObject)
            {
                return activeKeys.Contains(key);
            }
        }

        public bool IsFeedbackRegistered(string key)
        {
            lock (syncObject)
            {
                return registered.Contains(key);
            }
        }

        public bool IsPlaying()
        {
            lock (syncObject)
            {
                return activeKeys.Count > 0;
            }
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
            var frame = new Frame
            {
                DotPoints = points,
                PathPoints = new List<PathPoint>(),
                Position = position,
                DurationMillis = durationMillis
            };
            Submit(key, frame);
        }

        public void Submit(string key, PositionType position, List<PathPoint> points, int durationMillis)
        {
            var frame = new Frame
            {
                DotPoints = new List<DotPoint>(),
                PathPoints = points,
                Position = position,
                DurationMillis = durationMillis
            };
            Submit(key, frame);
        }

        public void SubmitRegistered(string key, string altKey, ScaleOption option)
        {
            SubmitRegistered(key, altKey, new RotationOption(0, 0), option);
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

        private void Submit(string key, Frame req)
        {
            var androidFrame = new AndroidFrame
            {
                durationMillis = req.DurationMillis,
                position = req.Position.ToString(),
                dotPoints = new AndroidDotPoint[req.DotPoints.Count],
                pathPoints = new AndroidPathPoint[req.PathPoints.Count]
            };
            for (var i = 0; i < req.DotPoints.Count; i++)
            {
                var p = req.DotPoints[i];
                androidFrame.dotPoints[i] = new AndroidDotPoint() {index = p.Index, intensity = p.Intensity};
            }
            for (var i = 0; i < req.PathPoints.Count; i++)
            {
                var p = req.PathPoints[i];
                androidFrame.pathPoints[i] = new AndroidPathPoint()
                {
                    x = p.X, y = p.Y, motorCount = p.MotorCount, intensity = p.Intensity
                };
            }


            var submitRequest = new AndroidFrameRequest
            {
                frame = androidFrame,
                type = "frame",
                key = key,
            };

            var json = JsonUtility.ToJson(submitRequest);
            SubmitRequest(json);
        }

        private void SubmitRequest(string submitRequest)
        {
            if (!AndroidPermissionsManager.CheckBluetoothPermissions())
            {
                return;
            }

            if (androidJavaObject != null)
            {
                try
                {
                    androidJavaObject.Call("submit", submitRequest);
                }
                catch (Exception e)
                {
                    BhapticsLogger.LogError("SubmitRequest() : {0}", e.Message);
                }
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
                try
                {
                    androidJavaObject.Call("submitRegistered", 
                        key, altKey, intensity, duration, offsetAngleX, offsetY);
                }
                catch (Exception e)
                {
                    BhapticsLogger.LogError("SubmitRequest() : {0}", e.Message);
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

            if (status.ContainsKey(pos.ToString()))
            {
                return status[pos.ToString()];
            }

            return new int[20];
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

        public void Receive(PlayerResponse response)
        {
            if (Monitor.TryEnter(syncObject, 5))
            {
                try
                {
                    activeKeys.Clear();
                    activeKeys.AddRange(response.ActiveKeys);
                    registered.Clear();
                    registered.AddRange(response.RegisteredKeys);
                    status = response.Status;
                }
                finally
                {
                    Monitor.Exit(syncObject);
                }
            }
            else
            {
                // failed to get lock: throw exceptions, log messages, get angry etc.
                BhapticsLogger.LogInfo("Receive update failed.");
            }
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

        public void TurnOnVisualization()
        {
            if (androidJavaObject != null)
            {
                androidJavaObject.Call("turnOnVisualization");
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
    }
}