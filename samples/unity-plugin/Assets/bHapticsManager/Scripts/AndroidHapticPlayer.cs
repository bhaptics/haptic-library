using System;
using System.Collections.Generic;
using Bhaptics.Tact;
using Bhaptics.Tact.Unity;
using UnityEngine;


public class AndroidHapticPlayer :IHapticPlayer
{
    private static AndroidJavaObject hapticPlayer;
    private readonly List<string> _activeKeys = new List<string>();
    private readonly List<PositionType> _activePosition = new List<PositionType>();
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

    public void Pair(string address)
    {
        if (hapticPlayer != null)
        {
            hapticPlayer.Call("pair", address);
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


    public List<BhapticsDevice> GetDeviceList()
    {
        if (hapticPlayer != null)
        {
           string result = hapticPlayer.Call<string>("getDeviceList"); 
            var devicesJson = JSON.Parse(result); 
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
                return deviceList;
            }
        }
        return null;
    }


    public bool IsScanning()
    {
        if (hapticPlayer != null)
        {
            return hapticPlayer.Call<bool>("isScanning");
           
        }
        return false;
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

    public bool IsActive(PositionType type)
    {
        lock (_activePosition)
        {
            return _activePosition.Contains(type);
        }
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
        hapticPlayer.Call("submit", request.ToJsonObject().ToString());
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
        if (submitRequest == null)
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

    public void SubmitRegistered(string key, float ratio)
    {
        var request = new SubmitRequest()
        {
            Key = key,
            Type = "key",
            Parameters = new Dictionary<string, object>
            {
                { "ratio", ratio}
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

            lock (_activePosition)
            {
                _activePosition.Clear();
                _activePosition.AddRange(response.ConnectedPositions);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        
    }
}