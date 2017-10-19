using Bhaptics.Tac.Designer;

namespace Bhaptics.Tac
{
    /// <summary>
    /// TactFile mapping with exported tact file from bHaptics Designer
    /// </summary>
    public class HapticFeedbackFile
    {
        public int IntervalMillis;
        public int Size;
        public int DurationMillis;
        public Project Project;
    }
}
