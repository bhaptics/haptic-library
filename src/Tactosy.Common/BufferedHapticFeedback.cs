using System;
using System.Collections.Generic;

namespace Tactosy.Common
{
    public class BufferedHapticFeedback
    {
        public Dictionary<int, HapticFeedback[]> HapticFeedback;

        public int StartTime { get; set; }
        public int EndTime { get; set; }
        
        private BufferedHapticFeedback()
        {
        }

        public BufferedHapticFeedback(string jsonContent)
        {
            try
            {
                TactosyFile tactosyFile = TactosyUtils.ConvertJsonStringToTactosyFile(jsonContent);
                if (tactosyFile == null)
                {
                    throw new HapticException("tactosy file exception - returned null");
                }

                EndTime = tactosyFile.durationMillis;
                HapticFeedback = tactosyFile.feedback;
            }
            catch (Exception e)
            {
                throw new HapticException("tactosy file exception unexpected", e);
            }
        }

        public BufferedHapticFeedback(TactosyFile tactosyFile)
        {
            if (tactosyFile == null)
            {
                throw new HapticException("tactosy file is null");
            }

            StartTime = -1;
            EndTime = tactosyFile.durationMillis;
            HapticFeedback = tactosyFile.feedback;
        }

        public BufferedHapticFeedback(HapticFeedback feedback, int durationMillis, int interval = 20)
        {
            if (feedback == null)
            {
                throw new HapticException("feedback is null");
            }

            if (durationMillis <= 0)
            {
                throw new HapticException("durationMillis should be positive");
            }

            if (interval <= 0)
            {
                throw new HapticException("interval should be positive");
            }

            int i;
            HapticFeedback = new Dictionary<int, HapticFeedback[]>();
            for (i = 0; i < durationMillis / interval; i++)
            {
                HapticFeedback[] feedbacks;
                if (feedback.Position == PositionType.All)
                {
                    HapticFeedback left = new HapticFeedback(PositionType.Left, feedback.Values, feedback.Mode);
                    HapticFeedback right = new HapticFeedback(PositionType.Right, feedback.Values, feedback.Mode);
                    HapticFeedback vFront = new HapticFeedback(PositionType.VestFront, feedback.Values, feedback.Mode);
                    HapticFeedback vBack = new HapticFeedback(PositionType.VestBack, feedback.Values, feedback.Mode);
                    HapticFeedback head = new HapticFeedback(PositionType.Head, feedback.Values, feedback.Mode);
                    feedbacks = new[] { left, right, vFront, vBack, head };
                }
                else
                {
                    feedbacks = new[] { feedback };
                }

                HapticFeedback[i * interval] = feedbacks;
            }
            StartTime = -1;
            HapticFeedback[] f;
            if (feedback.Position == PositionType.All)
            {

                HapticFeedback left = new HapticFeedback(PositionType.Left, new byte[20], feedback.Mode);
                HapticFeedback right = new HapticFeedback(PositionType.Right, new byte[20], feedback.Mode);
                HapticFeedback vFront = new HapticFeedback(PositionType.VestFront, new byte[20], feedback.Mode);
                HapticFeedback vBack = new HapticFeedback(PositionType.VestBack, new byte[20], feedback.Mode);
                HapticFeedback head = new HapticFeedback(PositionType.Head, new byte[20], feedback.Mode);
                f = new[] { left, right, vFront, vBack, head };
            }
            else
            {
                f = new[] { new HapticFeedback(feedback.Position, new byte[20], feedback.Mode) };

            }

            HapticFeedback[i * interval] = f;
            EndTime = i * interval;
        }

        public static BufferedHapticFeedback Copy(BufferedHapticFeedback bFeedback, int interval, float intensityRatio, float durationRatio)
        {
            BufferedHapticFeedback bufferedHapticFeedback = new BufferedHapticFeedback
            {
                EndTime = (int) (bFeedback.EndTime * durationRatio / interval * interval) + interval,
                StartTime = -1,
                HapticFeedback = new Dictionary<int, HapticFeedback[]>()
            };

            int time;
            for (time = 0; time < bufferedHapticFeedback.EndTime; time += interval)
            {
                int keyTime = (int)(time / durationRatio) / interval * interval;

                if (bFeedback.HapticFeedback.ContainsKey(keyTime))
                {
                    HapticFeedback[] hapticFeedbacks = bFeedback.HapticFeedback[keyTime];

                    HapticFeedback[] copiedFeedbacks = new HapticFeedback[hapticFeedbacks.Length];

                    for (int i = 0; i < hapticFeedbacks.Length; i++)
                    {
                        HapticFeedback hapticFeedback = hapticFeedbacks[i];
                        byte[] values = new byte[hapticFeedback.Values.Length];
                        if (hapticFeedback.Mode == FeedbackMode.DOT_MODE)
                        {
                            for (int valueIndex = 0; valueIndex < hapticFeedback.Values.Length; valueIndex++)
                            {
                                int val = (int)(hapticFeedback.Values[valueIndex] * intensityRatio);
                                if (val > 100)
                                {
                                    val = 100;
                                }
                                else if (val < 0)
                                {
                                    val = 0;
                                }

                                values[valueIndex] = (byte)val;
                            }
                        }
                        else if (hapticFeedback.Mode == FeedbackMode.PATH_MODE)
                        {
                            for (int valueIndex = 0; valueIndex < hapticFeedback.Values.Length; valueIndex++)
                            {
                                values[valueIndex] = hapticFeedback.Values[valueIndex];
                            }

                            for (int index = 0; index < 6; index++)
                            {
                                int realIndex = 3 + index * 3;
                                int val = (int)(hapticFeedback.Values[realIndex] * intensityRatio);
                                if (val > 100)
                                {
                                    val = 100;
                                }
                                else if (val < 0)
                                {
                                    val = 0;
                                }

                                values[realIndex] = (byte)val;
                            }
                        }
                        HapticFeedback feedback = new HapticFeedback(hapticFeedback.Position, values,
                            hapticFeedback.Mode);
                        copiedFeedbacks[i] = feedback;
                        bufferedHapticFeedback.HapticFeedback[time] = copiedFeedbacks;
                    }
                }
            }

            return bufferedHapticFeedback;
        }

    }
}
