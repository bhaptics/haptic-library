using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bhaptics.Tact.Unity
{
    public interface IHaptic
    {
        bool IsActive(PositionType type);

        bool IsPlaying(string key);
        bool IsFeedbackRegistered(string key);

        bool IsPlaying();

        void RegisterTactFileStr(string key, string tactFileStr);

        void RegisterTactFileStrReflected(string key, string tactFileStr);

        void Submit(string key, PositionType position, List<DotPoint> points, int durationMillis);

        void Submit(string key, PositionType position, List<PathPoint> points, int durationMillis);

        void SubmitRegistered(string key, string altKey, ScaleOption option);

        void SubmitRegistered(
            string key,
            string altKey,
            RotationOption rOption,
            ScaleOption sOption);

        void SubmitRegistered(string key);

        void SubmitRegistered(string key, int startTimeMillis);

        void TurnOff(string key);

        void TurnOff();

        void Dispose();

        int[] GetCurrentFeedback(PositionType pos);
    }


    public class BhapticsHaptic : IHaptic
    {
        public BhapticsHaptic()
        {
            HapticApi.Initialise(Application.identifier, Application.productName);
        }

        public bool IsActive(PositionType type)
        {
            return HapticApi.IsDevicePlaying(type);
        }

        public bool IsPlaying(string key)
        {
            return HapticApi.IsPlayingKey(key);
        }

        public bool IsFeedbackRegistered(string key)
        {
            return HapticApi.IsFeedbackRegistered(key);
        }

        public bool IsPlaying()
        {
            return HapticApi.IsPlaying();
        }

        public void RegisterTactFileStr(string key, string tactFileStr)
        {
            HapticApi.RegisterFeedbackFromTactFile(key, tactFileStr);
        }

        public void RegisterTactFileStrReflected(string key, string tactFileStr)
        {
            HapticApi.RegisterFeedbackFromTactFileReflected(key, tactFileStr);
        }

        public void Submit(string key, PositionType position, List<DotPoint> points, int durationMillis)
        {
            byte[] bytes = new byte[20];
            for (var i = 0; i < points.Count; i++)
            {
                DotPoint point = points[i];
                bytes[point.Index] = (byte)point.Intensity;
            }

            HapticApi.SubmitByteArray(key, position, bytes, 20, durationMillis);
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

        public void SubmitRegistered(string key, string altKey, ScaleOption option)
        {
            HapticApi.SubmitRegisteredWithOption(key, altKey, option.Intensity, option.Duration, 1f, 1f);
        }

        public void SubmitRegistered(string key, string altKey, RotationOption rOption, ScaleOption sOption)
        {
            HapticApi.SubmitRegisteredWithOption(key, altKey, sOption.Intensity, sOption.Duration, rOption.OffsetAngleX,
                rOption.OffsetY);
        }

        public void SubmitRegistered(string key)
        {
            HapticApi.SubmitRegistered(key);
        }

        public void SubmitRegistered(string key, int startTimeMillis)
        {
            HapticApi.SubmitRegisteredStartMillis(key, startTimeMillis);
        }

        public void TurnOff(string key)
        {
            HapticApi.TurnOffKey(key);
        }

        public void TurnOff()
        {
            HapticApi.TurnOff();
        }

        public void Dispose()
        {
            HapticApi.Destroy();
        }

        public int[] GetCurrentFeedback(PositionType pos)
        {
            HapticApi.status status;
            HapticApi.TryGetResponseForPosition(pos, out status);

            return status.values;
        }
    }
}
