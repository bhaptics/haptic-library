using System.Collections.Generic;

namespace Tactosy.Common
{
    /// <summary>
    /// TactosyFile mapping with exported tactosy file from tactosy studio
    /// </summary>
    public class TactosyFile
    {
        public int intervalMillis;
        public int size;
        public int durationMillis;

        public Dictionary<int, TactosyFeedback[]> feedback;
    }

    public class TactosyFeedbackBridge
    {
        public string position { get; set; }
        public string mode { get; set; }
        public byte[] values { get; set; }

        public static TactosyFeedback AsTactosyFeedback(TactosyFeedbackBridge bridge)
        {
            return new TactosyFeedback(EnumParser.ToPositionType(bridge.position), 
                bridge.values, EnumParser.ToMode(bridge.mode));
        }
    }

    public class TactosyFileBridge
    {
        public int intervalMillis;
        public int size;
        public int durationMillis;

        public Dictionary<string, TactosyFeedbackBridge[]> feedback;

        public static TactosyFile AsTactosyFile(TactosyFileBridge bridge)
        {
            if (bridge == null)
            {
                return null;
            }

            TactosyFile file = new TactosyFile();
            file.intervalMillis = bridge.intervalMillis;
            file.durationMillis = bridge.durationMillis;
            file.size = bridge.size;
            file.feedback = new Dictionary<int, TactosyFeedback[]>();

            foreach (var feed in bridge.feedback)
            {
                var key = int.Parse(feed.Key);
                TactosyFeedback[] feedbacks = new TactosyFeedback[feed.Value.Length];
                for (int i = 0; i < feed.Value.Length; i++)
                {
                    feedbacks[i] = TactosyFeedbackBridge.AsTactosyFeedback(feed.Value[i]);
                }

                file.feedback[key] = feedbacks;
            }
            return file;
        }
    }
}
