using System;

namespace Bhaptics.Tact
{
    public class CommonUtils
    {
        public static long GetCurrentMillis()
        {
            long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            return milliseconds;
        }

        public static T Clamp<T>(T aValue, T aMin, T aMax) where T : IComparable<T>
        {
            var result = aValue;
            if (aValue.CompareTo(aMax) > 0)
                result = aMax;
            else if (aValue.CompareTo(aMin) < 0)
                result = aMin;
            return result;
        }

        public static HapticFeedbackFile ConvertJsonStringToTactosyFile(string json)
        {
            return Parse(json);
        }

        private static HapticFeedbackFile Parse(string json)
        {
#if NETFX_CORE
            return HapticFeedbackFile.ToHapticFeedbackFile(json);
#else
            var obj = fastJSON.JSON.ToObject<HapticFeedbackFile>(json);

            return obj;
#endif
        }

        public static string TypeRight = "Right";
        public static string TypeLeft = "Left";
        public static string TypeVest = "Vest";
        public static string TypeTactosy = "Tactosy";

        public static Project ReflectLeftRight(string projectStr)
        {
            var feedbackFile = ConvertJsonStringToTactosyFile(projectStr);
            var project = feedbackFile.Project;

            foreach (var projectTrack in project.Tracks)
            {
                foreach (var projectTrackEffect in projectTrack.Effects)
                {
                    HapticEffectMode right = null, left = null;
                    if (projectTrackEffect.Modes.ContainsKey(TypeRight))
                    {
                        right = projectTrackEffect.Modes[TypeRight];
                    }

                    if (projectTrackEffect.Modes.ContainsKey(TypeLeft))
                    {
                        left = projectTrackEffect.Modes[TypeLeft];
                    }

                    projectTrackEffect.Modes[TypeLeft] = right;
                    projectTrackEffect.Modes[TypeRight] = left;
                }
            }

            return project;
        }
    }
}
