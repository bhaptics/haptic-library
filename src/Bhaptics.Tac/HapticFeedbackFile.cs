using System;
using System.Collections.Generic;
using System.Diagnostics;
using Bhaptics.Tac.Designer;

namespace Bhaptics.Tac
{
    /// <summary>
    /// TactosyFile mapping with exported tactosy file from tactosy studio
    /// </summary>
    public class HapticFeedbackFile
    {
        public int IntervalMillis;
        public int Size;
        public int DurationMillis;

        public Dictionary<int, HapticFeedback[]> Feedback;

        public Project Project;
    }

    public class HapticFeedbackBridge
    {
        public string Position { get; set; }
        public string Mode { get; set; }
        public int[] Values { get; set; }

        public static HapticFeedback AsTactosyFeedback(HapticFeedbackBridge bridge)
        {
            byte[] result = new byte[bridge.Values.Length * sizeof(int)];
            Buffer.BlockCopy(bridge.Values, 0, result, 0, result.Length);

            return new HapticFeedback(EnumParser.ToPositionType(bridge.Position),
                result, EnumParser.ToMode(bridge.Mode));
        }
    }

    public class HapticFeedbackFileBridge
    {
        public int IntervalMillis;
        public int Size;
        public int DurationMillis;

        public Dictionary<string, HapticFeedbackBridge[]> Feedback;
        public Project Project;

        public static HapticFeedbackFile AsTactosyFile(HapticFeedbackFileBridge bridge)
        {
            if (bridge == null)
            {
                return null;
            }

            HapticFeedbackFile file = new HapticFeedbackFile();
            file.IntervalMillis = bridge.IntervalMillis;
            file.DurationMillis = bridge.DurationMillis;
            file.Size = bridge.Size;
            file.Feedback = new Dictionary<int, HapticFeedback[]>();

            foreach (var feed in bridge.Feedback)
            {
                var key = int.Parse(feed.Key);
                HapticFeedback[] feedbacks = new HapticFeedback[feed.Value.Length];
                for (int i = 0; i < feed.Value.Length; i++)
                {
                    feedbacks[i] = HapticFeedbackBridge.AsTactosyFeedback(feed.Value[i]);
                }

                file.Feedback[key] = feedbacks;
            }
            var project = bridge.Project;

            if (project != null)
            {
                Debug.WriteLine($"{project}");
            }

            file.Project = bridge.Project;
            return file;
        }
    }
}
