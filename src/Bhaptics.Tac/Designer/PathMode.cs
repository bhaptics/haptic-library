using System.Collections.Generic;

namespace Bhaptics.Tac.Designer
{
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
