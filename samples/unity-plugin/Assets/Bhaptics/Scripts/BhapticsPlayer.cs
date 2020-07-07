using System;
using System.Collections;
using System.Collections.Generic;
using Bhaptics.Tact;
using Bhaptics.Tact.Unity;
using UnityEngine;


public interface IBhapticsPlayer
{
    void Register(string key, string tactFileStr);

    
    void TurnOff();
    void TurnOff(string key);

    void Submit(string key, PositionType position, List<DotPoint> points, int durationMillis);
    void Submit(string key, PositionType position, List<PathPoint> points, int durationMillis);
    void SubmitRegistered(string key, string altKey, ScaleOption option);
    void SubmitRegisteredVestRotation(string key, string altKey, RotationOption option, ScaleOption sOption);
    void SubmitRegistered(string key);
    void SubmitRegistered(string key, float ratio);
    event Action<PlayerResponse> StatusReceived;

}

public class BhapticsPlayer : IBhapticsPlayer
{
    public void Register(string key, string tactFileStr)
    {
        HapticApi.RegisterFeedbackFromTactFile(key, tactFileStr);
    }

    public void TurnOff()
    {
        HapticApi.TurnOff();
    }

    public void TurnOff(string key)
    {
        throw new NotImplementedException();
    }

    public void Submit(string key, PositionType position, List<DotPoint> points, int durationMillis)
    {
        throw new NotImplementedException();
    }

    public void Submit(string key, PositionType position, List<PathPoint> points, int durationMillis)
    {
        throw new NotImplementedException();
    }

    public void SubmitRegistered(string key, string altKey, ScaleOption option)
    {
        throw new NotImplementedException();
    }

    public void SubmitRegisteredVestRotation(string key, string altKey, RotationOption option, ScaleOption sOption)
    {
        throw new NotImplementedException();
    }

    public void SubmitRegistered(string key)
    {
        throw new NotImplementedException();
    }

    public void SubmitRegistered(string key, float ratio)
    {
        throw new NotImplementedException();
    }

    public event Action<PlayerResponse> StatusReceived;
}
