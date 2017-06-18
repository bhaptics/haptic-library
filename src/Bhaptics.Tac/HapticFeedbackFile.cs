using System.Collections.Generic;

namespace Bhaptics.Tac
{
    /// <summary>
    /// TactosyFile mapping with exported tactosy file from tactosy studio
    /// </summary>
    public class HapticFeedbackFile
    {
        public int intervalMillis;
        public int size;
        public int durationMillis;

        public Dictionary<int, HapticFeedback[]> feedback;
    }

    public class HapticFeedbackBridge
    {
        public string position { get; set; }
        public string mode { get; set; }
        public byte[] values { get; set; }

        public static HapticFeedback AsTactosyFeedback(HapticFeedbackBridge bridge)
        {
            return new HapticFeedback(EnumParser.ToPositionType(bridge.position), 
                bridge.values, EnumParser.ToMode(bridge.mode));
        }
    }

    public class HapticFeedbackFileBridge
    {
        public int intervalMillis;
        public int size;
        public int durationMillis;

        public Dictionary<string, HapticFeedbackBridge[]> feedback;

        public static HapticFeedbackFile AsTactosyFile(HapticFeedbackFileBridge bridge)
        {
            if (bridge == null)
            {
                return null;
            }

            HapticFeedbackFile file = new HapticFeedbackFile();
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
                    feedbacks[i] = HapticFeedbackBridge.AsTactosyFeedback(feed.Value[i]);
                }

                file.feedback[key] = feedbacks;
            }
            return file;
        }
    }
}
