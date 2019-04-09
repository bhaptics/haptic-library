using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bhaptics.Tact
{
    public class PlayerRequest
    {

        public List<RegisterRequest> Register;
        public List<SubmitRequest> Submit;

        public static PlayerRequest Create()
        {
            return new PlayerRequest
            {
                Register = new List<RegisterRequest>(),
                Submit = new List<SubmitRequest>()
            };
        }
    }

    public class RegisterRequest
    {
        public string Key { get; set; }
        public Project Project { get; set; }
    }

    public class SubmitRequest
    {
        public string Type { get; set; }
        public string Key { get; set; }
        public Dictionary<string, object> Parameters { get; set; } // durationRatio
        public Frame Frame { get; set; }
    }

    public class RotationOption
    {
        public float OffsetAngleX { get; set; }
        public float OffsetY { get; set; }

        public RotationOption(float offsetAngleX, float offsetY)
        {
            OffsetAngleX = offsetAngleX;
            OffsetY = offsetY;
        }
    }

    public class ScaleOption
    {
        public float Intensity { get; set; }
        public float Duration { get; set; }

        public ScaleOption(float intensity, float duration)
        {
            Intensity = intensity;
            Duration = duration;
        }
    }

    public class PlayerResponse
    {
        public List<string> RegisteredKeys { get; set; }
        public List<string> ActiveKeys { get; set; }
        public int ConnectedDeviceCount { get; set; }
        public List<PositionType> ConnectedPositions { get; set; }
        public Dictionary<string, int[]> Status { get; set; }
        
    }

    public class Frame
    {
        public int DurationMillis { get; set; }
        public PositionType Position { get; set; }
        public List<PathPoint> PathPoints { get; set; }
        public List<DotPoint> DotPoints { get; set; }
    }
}
