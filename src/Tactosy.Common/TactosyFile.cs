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

        public Dictionary<int, HapticFeedback[]> feedback;
    }

    public class TactosyFeedbackBridge
    {
        public string position { get; set; }
        public string mode { get; set; }
        public byte[] values { get; set; }

        public static HapticFeedback AsTactosyFeedback(TactosyFeedbackBridge bridge)
        {
            return new HapticFeedback(EnumParser.ToPositionType(bridge.position), 
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
            file.feedback = new Dictionary<int, HapticFeedback[]>();

            foreach (var feed in bridge.feedback)
            {
                var key = int.Parse(feed.Key);
                HapticFeedback[] feedbacks = new HapticFeedback[feed.Value.Length];
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
