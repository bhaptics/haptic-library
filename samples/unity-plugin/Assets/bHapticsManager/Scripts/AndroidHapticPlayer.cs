using System;
using System.Collections.Generic;
using Bhaptics.Tact;
using Bhaptics.Tact.Unity;
using UnityEngine;


public class AndroidHapticPlayer :IHapticPlayer
{
    private static AndroidJavaObject hapticPlayer;

    private readonly List<string> _activeKeys = new List<string>();

    private HashSet<string> registered = new HashSet<string>();

    private List<BhapticsDevice> deviceList = null;
    public event Action<string> OnConnect, OnDisconnect;

    public void Dispose()
    {
    }

    public void Enable()
    {
        if (hapticPlayer != null)
        {
            return;
        }
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        Debug.Log("Enable() activity : " + currentActivity);
        hapticPlayer = new AndroidJavaObject("com.bhaptics.bhapticsunity.BhapticsManagerWrapper", currentActivity);

        if (Application.platform == RuntimePlatform.Android)
        {
            if (BhapticsManager.Instance.visualizeFeedback)
            {
                TurnOnVisualization();
            }
        }
    }

    public void StopScan()
    {
        if (hapticPlayer != null)
        {
            hapticPlayer.Call("stopScan");
        }
    }

    public void StartScan()
    {   
        if (hapticPlayer != null)
        {
            hapticPlayer.Call("scan");
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

    public void TogglePosition(string address)
    {
        if (hapticPlayer != null)
        {
            hapticPlayer.Call("togglePosition", address);
        }
    }

    public void Ping(string address)
    {
        if (hapticPlayer != null)
        {
            hapticPlayer.Call("ping", address);
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

    public void Disable()
    {
    }

    public void Quit()
    {
        if (hapticPlayer != null)
        {
            hapticPlayer.Call("quit");
        }
    }

    public bool IsActive(PositionType type)
    {
        foreach(var device in GetDeviceList())
        {
            if(device.Position == type.ToString() && AndroidWidget_CompareDeviceString.convertConnectionStatus(device.ConnectionStatus) == 0)
            {
                return true;
            }            
        }
        return false;
    }

    public bool IsPlaying(string key)
    {
        lock (_activeKeys)
        {
            return _activeKeys.Contains(key);
        }
    }

    public bool IsPlaying()
    {
        return _activeKeys.Count > 0;
    }

    public void Register(string key, string path)
    {
        throw new System.NotImplementedException();
    }

    public void Register(string key, Project project)
    {
        var req = new RegisterRequest
        {
            Key = key,
            Project = project
        };
        var registerRequests = new List<RegisterRequest> {req};
        var request = PlayerRequest.Create();
        request.Register = registerRequests;
        if (hapticPlayer == null) {
            return;
        }
        hapticPlayer.Call("register", request.ToJsonObject().ToString());
    }

    public void RegisterTactFileStr(string key, string tactFileStr)
    {
        if (registered.Contains(key))
        {
            return;
        }

        var file = CommonUtils.ConvertJsonStringToTactosyFile(tactFileStr);
        Register(key, file.Project);
        registered.Add(key);
    }

    public void RegisterTactFileStrReflected(string key, string tactFileStr)
    {
        var project = BhapticsUtils.ReflectLeftRight(tactFileStr);
        Register(key, project);
    }

    public void Submit(string key, PositionType position, byte[] motorBytes, int durationMillis)
    {
        var points = new List<DotPoint>();
        for (int i = 0; i < motorBytes.Length; i++)
        {
            if (motorBytes[i] > 0)
            {
                points.Add(new DotPoint(i, motorBytes[i]));
            }
        }

        Submit(key, position, points, durationMillis);
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

    public void Submit(string key, PositionType position, DotPoint point, int durationMillis)
    {
        Submit(key, position, new List<DotPoint>() { point }, durationMillis);
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

    public void Submit(string key, PositionType position, PathPoint point, int durationMillis)
    {
        Submit(key, position, new List<PathPoint>() {point}, durationMillis);
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
                Debug.Log("SubmitRequest() : " + e.Message);
            }
        }
    }

    public void SubmitRegistered(string key, ScaleOption option)
    {
        var request = new SubmitRequest()
        {
            Key = key,
            Type = "key",
            Parameters = new Dictionary<string, object>
            {
                { "scaleOption", option}
            }
        };
        SubmitRequest(request);
    }

    public void SubmitRegistered(string key, string altKey, ScaleOption option)
    {
        var request = new SubmitRequest()
        {
            Key = key,
            Type = "key",
            Parameters = new Dictionary<string, object>
            {
                { "scaleOption", option},
                { "altKey", altKey}
            }
        };
        SubmitRequest(request);
    }

    public void SubmitRegisteredVestRotation(string key, RotationOption option)
    {
        var request = new SubmitRequest()
        {
            Key = key,
            Type = "key",
            Parameters = new Dictionary<string, object>
            {
                { "rotationOption", option}
            }
        };
        SubmitRequest(request);
    }

    public void SubmitRegisteredVestRotation(string key, string altKey, RotationOption option)
    {
        var request = new SubmitRequest()
        {
            Key = key,
            Type = "key",
            Parameters = new Dictionary<string, object>
            {
                { "rotationOption", option},
                { "altKey", altKey}
            }
        };
        SubmitRequest(request);
    }

    public void SubmitRegisteredVestRotation(string key, string altKey, RotationOption rOption, ScaleOption sOption)
    {
        var request = new SubmitRequest()
        {
            Key = key,
            Type = "key",
            Parameters = new Dictionary<string, object>
            {
                { "rotationOption", rOption},
                { "scaleOption", sOption},
                { "altKey", altKey}
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

    public void SubmitRegistered(string key, float startTimeMillis)
    {
        var request = new SubmitRequest()
        {
            Key = key,
            Type = "key",
            Parameters = new Dictionary<string, object>
            {
                { "startTimeMillis", (int)startTimeMillis + ""}
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

    #region Callback function;
    public void Connected(string address)
    {
        if (OnConnect != null)
        {
            OnConnect(address);
        }
    }
    public void Disconnected(string address)
    {
        if (OnDisconnect != null)
        {
            OnDisconnect(address);
        }
    }
    public void UpdateDeviceList(List<BhapticsDevice> _deviceList)
    {
        deviceList = _deviceList;
    }
    #endregion




    public event Action<PlayerResponse> StatusReceived;
    public void Receive(PlayerResponse response)
    {
        try
        {
            if (StatusReceived != null)
            {
                StatusReceived(response);
            }

            lock (_activeKeys)
            {
                _activeKeys.Clear();
                _activeKeys.AddRange(response.ActiveKeys);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public List<BhapticsDevice> GetDeviceList()
    {
        if (hapticPlayer == null)
        {
            return new List<BhapticsDevice>();
        }

        if (deviceList == null)
        {
            string result = hapticPlayer.Call<string>("getDeviceList");
            var devicesJson = JSON.Parse(result);
            if (devicesJson.IsArray)
            {
                deviceList = new List<BhapticsDevice>();
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
            }
        }

        return deviceList;
    }
}