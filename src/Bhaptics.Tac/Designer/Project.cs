using System.Collections.Generic;

namespace Bhaptics.Tac.Designer
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

    public enum PlaybackType
    {
        NONE, FADE_IN, FADE_OUT, FADE_IN_OUT
    }

    public class HapticEffectMode
    {
        public int Texture;
        public FeedbackMode Mode { get; set; }
        public DotMode DotMode { get; set; }
        public PathMode PathMode { get; set; }
    }
}
