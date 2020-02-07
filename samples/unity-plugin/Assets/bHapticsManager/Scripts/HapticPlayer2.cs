using System;
using System.Collections.Generic;

namespace Bhaptics.Tact.Unity
{
    public class HapticPlayer2 : IHapticPlayer
    {

        public HapticPlayer2(string appId, string appName)
        {
            HapticApi.Initialise(appId, appName);
        }

        public void Dispose()
        {
            HapticApi.Destroy();
        }

        public void Enable()
        {
            HapticApi.EnableFeedback();
        }

        public void Disable()
        {
            HapticApi.DisableFeedback();
        }

        public bool IsActive(PositionType type)
        {
            return HapticApi.IsDevicePlaying(type);
        }

        public bool IsPlaying(string key)
        {
            return HapticApi.IsPlayingKey(key);
        }

        public bool IsPlaying()
        {
            return HapticApi.IsPlaying();
        }

        public void Register(string key, Project project)
        {
            HapticApi.RegisterFeedback(key, project.ToJsonObject().ToString());
        }

        public void RegisterTactFileStr(string key, string tactFileStr)
        {
            HapticApi.RegisterFeedbackFromTactFile(key, tactFileStr);
        }

        public void RegisterTactFileStrReflected(string key, string tactFileStr)
        {
            HapticApi.RegisterFeedbackFromTactFileReflected(key, tactFileStr);
        }

        public void Submit(string key, PositionType position, byte[] motorBytes, int durationMillis)
        {
            HapticApi.SubmitByteArray(key, position, motorBytes, motorBytes.Length, durationMillis);
        }

        public void Submit(string key, PositionType position, List<DotPoint> points, int durationMillis)
        {
            byte[] bytes = new byte[20];
            for (var i = 0; i < points.Count; i++)
            {
                DotPoint point = points[i];
                bytes[point.Index] = (byte) point.Intensity;
            }

            HapticApi.SubmitByteArray(key, position, bytes, 20, durationMillis);
        }

        public void Submit(string key, PositionType position, DotPoint point, int durationMillis)
        {
            Submit(key, position, new List<DotPoint>() { point }, durationMillis);
        }

        public void Submit(string key, PositionType position, List<PathPoint> points, int durationMillis)
        {
            HapticApi.point[] pts = new HapticApi.point[points.Count];
            for (var i = 0; i < points.Count; i++)
            {
                pts[i].intensity = points[i].Intensity;
                pts[i].motorCount = points[i].MotorCount;
                pts[i].x = points[i].X;
                pts[i].y = points[i].Y;
            }

            HapticApi.SubmitPathArray(key, position, pts, pts.Length, durationMillis);
        }

        public void Submit(string key, PositionType position, PathPoint point, int durationMillis)
        {
            Submit(key, position, new List<PathPoint>() {point}, durationMillis);
        }

        public void SubmitRegistered(string key, ScaleOption option)
        {
            SubmitRegistered(key, key, option);
        }

        public void SubmitRegistered(string key, string altKey, ScaleOption option)
        {
            HapticApi.SubmitRegisteredWithOption(key, altKey, option.Intensity, option.Duration, 1f, 1f);
        }

        public void SubmitRegisteredVestRotation(string key, RotationOption option)
        {
            HapticApi.SubmitRegisteredWithOption(key, key, 1f, 1f, option.OffsetAngleX, option.OffsetY);
        }

        public void SubmitRegisteredVestRotation(string key, string altKey, RotationOption option)
        {
            SubmitRegisteredVestRotation(key, altKey, option, new ScaleOption(1f, 1f));
        }

        public void SubmitRegisteredVestRotation(string key, string altKey, RotationOption rOption, ScaleOption sOption)
        {
            HapticApi.SubmitRegisteredWithOption(key, altKey, sOption.Intensity, sOption.Duration, rOption.OffsetAngleX,
                rOption.OffsetY);
        }

        public void SubmitRegistered(string key)
        {
            HapticApi.SubmitRegistered(key);
        }

        public void SubmitRegistered(string key, float startTimeMillis)
        {
            HapticApi.SubmitRegisteredStartMillis(key, (int)startTimeMillis);
        }

        public void TurnOff(string key)
        {
            HapticApi.TurnOffKey(key);
        }

        public void TurnOff()
        {
            HapticApi.TurnOff();
        }

        public event Action<PlayerResponse> StatusReceived;
    }
}
