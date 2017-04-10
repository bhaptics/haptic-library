using System.Collections.Generic;

namespace Tactosy.Common
{
    public class FeedbackItem
    {
        public FeedbackItem(int time, Dictionary<int, HapticFeedbackData> feedback)
        {
            Time = time;

            Feedback = feedback;
        }

        public int Time { get; set; }

        public Dictionary<int, HapticFeedbackData> Feedback { get; set; }
    }

    public class HapticFeedbackData
    {
        public int Time { get; set; }
        public int[] Values { get; set; }
        public FeedbackMode Mode { get; set; }
    }

    public enum FeedbackMode
    {
        PATH_MODE, DOT_MODE
    }
}
