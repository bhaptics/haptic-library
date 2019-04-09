using System;
using System.Collections.Generic;

namespace Bhaptics.Tact
{

    public class Project
    {
        public Track[] Tracks { get; set; }
        public Layout Layout { get; set; }

        public override string ToString()
        {
            return "Project { Tracks=" + Tracks +
                   ", Layout=" + Layout + "}";
        }
    }

    public class HapticFeedbackFile
    {
        public Project Project;
    }

    public class Track
    {
        public HapticEffect[] Effects { get; set; }

        public override string ToString()
        {
            return "Track {  Effects=" + Effects + "}";
        }
    }

    public class HapticEffect
    {
        public int StartTime { get; set; }
        public int OffsetTime { get; set; }
        public Dictionary<string, HapticEffectMode> Modes {get; set; }

        public override string ToString()
        {
            return "HapticEffect { StartTime=" + StartTime +
                   ", OffsetTime=" + OffsetTime +
                   ", Modes=" + Modes + "}";
        }
    }

    public class Layout
    {
        public string Type { get; set; }
        public Dictionary<string, LayoutObject[]> Layouts { get; set; }
    }

    public class LayoutObject
    {
        public int Index { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }

    public enum PlaybackType
    {
        NONE, FADE_IN, FADE_OUT, FADE_IN_OUT
    }


    public class HapticEffectMode
    {
        public FeedbackMode Mode { get; set; }
        public DotMode DotMode { get; set; }
        public PathMode PathMode { get; set; }
    }

    public class DotMode
    {
        public bool DotConnected { get; set; }
        public DotModeObjectCollection[] Feedback { get; set; }
    }

    public class DotModeObjectCollection
    {
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public PlaybackType PlaybackType = PlaybackType.NONE;
        public DotModeObject[] PointList { get; set; }
    }

    public class DotModeObject
    {
        public int Index { get; set; }
        public float Intensity { get; set; }
        
    }

    public enum PathMovingPattern
    {
        CONST_SPEED, CONST_TDM
    }

    public class PathMode
    {
        public PathModeObjectCollection[] Feedback { get; set; }
    }

    public class PathModeObjectCollection
    {
        public PlaybackType PlaybackType = PlaybackType.NONE;
        public PathMovingPattern MovingPattern = PathMovingPattern.CONST_TDM;
        public PathModeObject[] PointList { get; set; }
    }

    public class PathModeObject
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Intensity { get; set; }
        public int Time { get; set; }
    }
}
