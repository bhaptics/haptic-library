using System.Collections.Generic;

namespace Bhaptics.Tac.Designer
{
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
}
