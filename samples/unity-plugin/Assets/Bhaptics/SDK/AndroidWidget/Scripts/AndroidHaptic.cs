using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bhaptics.Tact.Unity
{
    public class AndroidHaptic : IHaptic
    {
        private static AndroidJavaObject hapticPlayer;

        private List<HapticDevice> deviceList = new List<HapticDevice>();

        private readonly List<string> activeKeys = new List<string>();
        private readonly List<string> registered = new List<string>();
        private Dictionary<string, int[]> status = new Dictionary<string, int[]>();

        public AndroidHaptic()
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            hapticPlayer = new AndroidJavaObject("com.bhaptics.bhapticsunity.BhapticsManagerWrapper", currentActivity);
            TurnOnVisualization();
            if (AndroidPermissionsManager.CheckBluetoothPermissions())
            {
                deviceList = GetDevices(true);
                StartScan();
            }
        }

        public bool IsActive(PositionType type)
        {
            foreach (var device in deviceList)
            {
                if (device.Position == type
                    && device.IsConnected)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsPlaying(string key)
        {
            lock (activeKeys)
            {
                return activeKeys.Contains(key);
            }
        }

        public bool IsFeedbackRegistered(string key)
        {
            lock (registered)
            {
                return registered.Contains(key);
            }
        }

        public bool IsPlaying()
        {
            lock (activeKeys)
            {
                return activeKeys.Count > 0;
            }
        }

        public void RegisterTactFileStr(string key, string tactFileStr)
        {
            var file = CommonUtils.ConvertJsonStringToTactosyFile(tactFileStr);
            Register(key, file.Project);
        }

        public void RegisterTactFileStrReflected(string key, string tactFileStr)
        {
            var project = BhapticsUtils.ReflectLeftRight(tactFileStr);
            Register(key, project);
        }

        public void Submit(string key, PositionType position, List<DotPoint> points, int durationMillis)
        {
            var frame = new Frame();
            frame.DotPoints = points;
            frame.PathPoints = new List<PathPoint>();
            frame.Position = position;
            frame.DurationMillis = durationMillis;
            Submit(key, frame);
        }

        public void Submit(string key, PositionType position, List<PathPoint> points, int durationMillis)
        {
            var frame = new Frame();
            frame.DotPoints = new List<DotPoint>();
            frame.PathPoints = points;
            frame.Position = position;
            frame.DurationMillis = durationMillis;
            Submit(key, frame);
        }

        public void SubmitRegistered(string key, string altKey, ScaleOption option)
        {
            var request = new SubmitRequest()
            {
                Key = key,
                Type = "key",
                Parameters = new Dictionary<string, object>
                {
                    {"scaleOption", option},
                    {"altKey", altKey}
                }
            };
            SubmitRequest(request);
        }

        public void SubmitRegistered(string key, string altKey, RotationOption rOption, ScaleOption sOption)
        {
            var request = new SubmitRequest()
            {
                Key = key,
                Type = "key",
                Parameters = new Dictionary<string, object>
                {
                    {"rotationOption", rOption},
                    {"scaleOption", sOption},
                    {"altKey", altKey}
                }
            };
            SubmitRequest(request);
        }

        public void SubmitRegistered(string key)
        {
            var request = new SubmitRequest()
            {
                Key = key,
                Type = "key"
            };
            SubmitRequest(request);
        }

        public void SubmitRegistered(string key, int startTimeMillis)
        {
            var request = new SubmitRequest()
            {
                Key = key,
                Type = "key",
                Parameters = new Dictionary<string, object>
                {
                    {"startTimeMillis", (int) startTimeMillis + ""}
                }
            };
            SubmitRequest(request);
        }

        public void TurnOff(string key)
        {
            var req = new SubmitRequest
            {
                Key = key,
                Type = "turnOff"
            };
            SubmitRequest(req);
        }

        public void TurnOff()
        {
            var req = new SubmitRequest
            {
                Key = "",
                Type = "turnOffAll"
            };
            SubmitRequest(req);
        }

        public void Dispose()
        {
            if (hapticPlayer != null)
            {
                hapticPlayer.Call("quit");
                hapticPlayer = null;
            }
        }

        private void Submit(string key, Frame req)
        {
            var submitRequest = new SubmitRequest
            {
                Frame = req,
                Key = key,
                Type = "frame"
            };
            SubmitRequest(submitRequest);
        }

        private void SubmitRequest(SubmitRequest submitRequest)
        {
            if (!AndroidPermissionsManager.CheckBluetoothPermissions())
            {
                return;
            }

            var request = PlayerRequest.Create();
            request.Submit.Add(submitRequest);
            if (hapticPlayer != null)
            {
                try
                {
                    hapticPlayer.Call("submit", request.ToJsonObject().ToString());
                }
                catch (Exception e)
                {
                    BhapticsLogger.LogError("SubmitRequest() : {0}", e.Message);
                }
            }
        }

        private void Register(string key, Project project)
        {
            if (!AndroidPermissionsManager.CheckBluetoothPermissions())
            {
                return;
            }

            var req = new RegisterRequest
            {
                Key = key,
                Project = project
            };
            var registerRequests = new List<RegisterRequest> {req};
            var request = PlayerRequest.Create();
            request.Register = registerRequests;
            if (hapticPlayer == null)
            {
                return;
            }


            hapticPlayer.Call("register", request.ToJsonObject().ToString());
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
                string result = hapticPlayer.Call<string>("getDeviceList");
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
            try
            {
                lock (activeKeys)
                {
                    activeKeys.Clear();
                    activeKeys.AddRange(response.ActiveKeys);
                }

                lock (registered)
                {
                    registered.Clear();
                    registered.AddRange(response.RegisteredKeys);
                }

                lock (status)
                {
                    status = response.Status;
                }
            }
            catch (Exception e)
            {
                BhapticsLogger.LogInfo("Receive: {0}", e.Message);
            }
        }

        public void Pair(string address, string position)
        {
            if (hapticPlayer != null)
            {
                if (position != "")
                {
                    hapticPlayer.Call("pair", address, position);
                }
                else
                {
                    hapticPlayer.Call("pair", address);
                }
            }
        }

        public void Unpair(string address)
        {
            if (hapticPlayer != null)
            {
                hapticPlayer.Call("unpair", address);
            }
        }

        public void UnpairAll()
        {
            if (hapticPlayer != null)
            {
                hapticPlayer.Call("unpairAll");
            }
        }


        public void StartScan()
        {
            if (hapticPlayer != null)
            {
                BhapticsLogger.LogDebug("StartScan()");
                hapticPlayer.Call("scan");
            }
        }

        public void StopScan()
        {
            if (hapticPlayer != null)
            {
                hapticPlayer.Call("stopScan");
            }
        }

        public bool IsScanning()
        {
            if (hapticPlayer != null)
            {
                return hapticPlayer.Call<bool>("isScanning");
            }

            return false;
        }

        public void TogglePosition(string address)
        {
            if (hapticPlayer != null)
            {
                hapticPlayer.Call("togglePosition", address);
            }
        }

        public void TurnOnVisualization()
        {
            if (hapticPlayer != null)
            {
                hapticPlayer.Call("turnOnVisualization");
            }
        }

        public void PingAll()
        {
            if (hapticPlayer != null)
            {
                hapticPlayer.Call("pingAll");
            }
        }

        public void Ping(string address)
        {
            if (hapticPlayer != null)
            {
                hapticPlayer.Call("ping", address);
            }
        }
    }
}