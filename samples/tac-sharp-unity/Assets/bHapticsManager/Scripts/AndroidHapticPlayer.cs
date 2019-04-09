using System;
using System.Collections.Generic;
using Bhaptics.fastJSON;
using Bhaptics.Tact;
using Bhaptics.Tact.Unity;
using UnityEngine;


public class AndroidHapticPlayer :IHapticPlayer
{
    private AndroidJavaObject hapticPlayer;
    private readonly JSONParameters DEFAULT_PARAM = new JSONParameters
    {
        EnableAnonymousTypes = true,
        UsingGlobalTypes = false,
        UseValuesOfEnums = false,
        SerializeToLowerCaseNames = false,
        UseExtensions = true
    };

    private readonly List<string> _activeKeys = new List<string>();
    private readonly List<PositionType> _activePosition = new List<PositionType>();
    public void Dispose()
    {
        Debug.Log("dispose() ");
        if (hapticPlayer != null)
        {
            hapticPlayer.Call("dispose");
            hapticPlayer = null;
        }
    }

    public void Enable()
    {
        if (hapticPlayer != null)
        {
            Debug.LogError("AndroidHapticPlayer not null");
            return;
        }

        AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = player.GetStatic<AndroidJavaObject>("currentActivity");
        Debug.Log("Enable() activity : " + currentActivity);
        hapticPlayer = new AndroidJavaObject("com.bhaptics.tact.unity.HapticPlayerWrapper", currentActivity);
        hapticPlayer.Call("start");

        Debug.Log("Enable() StopScan");
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
            hapticPlayer.Call("startScan");
        }
    }

    public void Disable()
    {
        Debug.Log("Disable() ");
        //        Dispose();
        if (hapticPlayer != null)
        {
            hapticPlayer.Call("stop");
            hapticPlayer = null;
        }
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
        hapticPlayer.Call("submit", JSON.ToJSON(request, DEFAULT_PARAM));
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
        var request = PlayerRequest.Create();
        request.Submit.Add(submitRequest);
        if (hapticPlayer != null)
        {
            hapticPlayer.Call("submit", JSON.ToJSON(request, DEFAULT_PARAM));
        }
        else
        {
            Debug.Log("hapticPlayer is null.");
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