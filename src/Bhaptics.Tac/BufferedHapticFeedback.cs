using System;
using System.Collections.Generic;

namespace Bhaptics.Tac
{
    public class BufferedHapticFeedback
    {
        public Dictionary<int, HapticFeedbackFrame[]> HapticFeedback;
        private int interval = 20;
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        
        private BufferedHapticFeedback()
        {
        }

        public BufferedHapticFeedback(string jsonContent)
        {
            try
            {
                HapticFeedbackFile hapticFeedbackFile = CommonUtils.ConvertJsonStringToTactosyFile(jsonContent);
                if (hapticFeedbackFile == null)
                {
                    throw new HapticException("tactosy file exception - returned null");
                }

                Initialize(hapticFeedbackFile);
            }
            catch (Exception e)
            {
                throw new HapticException("tactosy file exception unexpected", e);
            }
        }

        public BufferedHapticFeedback(HapticFeedbackFile hapticFeedbackFile)
        {
            if (hapticFeedbackFile == null)
            {
                throw new HapticException("tactosy file is null");
            }

            Initialize(hapticFeedbackFile);
        }


        private void Initialize(HapticFeedbackFile hapticFeedbackFile)
        {
            StartTime = -1;
            EndTime = hapticFeedbackFile.DurationMillis;
            var feedback = hapticFeedbackFile.Feedback;
            HapticFeedback = new Dictionary<int, HapticFeedbackFrame[]>();

            foreach (var keyValuePair in feedback)
            {
                var time = keyValuePair.Key;
                var feed = keyValuePair.Value;

                HapticFeedbackFrame[] feedbackFrames = new HapticFeedbackFrame[feed.Length];

                for (int i = 0; i < feed.Length; i++)
                {
                    if (feed[i].Mode == FeedbackMode.DOT_MODE)
                    {
                        List<DotPoint> points = new List<DotPoint>();
                        var values = feed[i].Values;
                        for (int index = 0; index < values.Length; index++)
                        {
                            if (values[index] > 0)
                            {
                                points.Add(new DotPoint(index, values[index]));
                            }
                        }
                        
                        feedbackFrames[i] = HapticFeedbackFrame.AsFeedbackFrame(feed[i].Position, points, feed[i].Texture);
                    }
                    else if (feed[i].Mode == FeedbackMode.PATH_MODE)
                    {
                        // This is legacy
                        List<PathPoint> points = new List<PathPoint>();
                        var values = feed[i].Values;
                        var size = values[0];
                        for (int index = 0; index < size; index++)
                        {
                            float x = values[3 * index + 1] / 40f;
                            float y = values[3 * index + 2] / 30f;
                            int intensity = values[3 * index + 3];

                            points.Add(new PathPoint(x, y, intensity));
                        }

                        feedbackFrames[i] = HapticFeedbackFrame.AsFeedbackFrame(feed[i].Position, points);
                    }

                }


                HapticFeedback[time] = feedbackFrames;
            }
        }

        public BufferedHapticFeedback(PositionType position, List<DotPoint> points, int durationMillis,
            int texture = 0)
        {
            if (durationMillis <= 0)
            {
                throw new HapticException("durationMillis should be positive");
            }

            int i;
            HapticFeedback = new Dictionary<int, HapticFeedbackFrame[]>();
            for (i = 0; i < durationMillis / interval; i++)
            {
                HapticFeedbackFrame[] feedbacks;
                if (position == PositionType.All)
                {
                    HapticFeedbackFrame left = HapticFeedbackFrame.AsFeedbackFrame(PositionType.Left, points, texture);
                    HapticFeedbackFrame right = HapticFeedbackFrame.AsFeedbackFrame(PositionType.Right, points, texture);
                    HapticFeedbackFrame vFront = HapticFeedbackFrame.AsFeedbackFrame(PositionType.VestFront, points, texture);
                    HapticFeedbackFrame vBack = HapticFeedbackFrame.AsFeedbackFrame(PositionType.VestBack, points, texture);
                    HapticFeedbackFrame head = HapticFeedbackFrame.AsFeedbackFrame(PositionType.Head, points, texture);
                    feedbacks = new[] { left, right, vFront, vBack, head };
                }
                else
                {
                    feedbacks = new[] { HapticFeedbackFrame.AsFeedbackFrame(position, points, texture) };
                }

                HapticFeedback[i * interval] = feedbacks;
            }
            StartTime = -1;
            HapticFeedbackFrame[] f;
            if (position == PositionType.All)
            {
                HapticFeedbackFrame left = HapticFeedbackFrame.AsTurnOffFrame(PositionType.Left);
                HapticFeedbackFrame right = HapticFeedbackFrame.AsTurnOffFrame(PositionType.Right);
                HapticFeedbackFrame vFront = HapticFeedbackFrame.AsTurnOffFrame(PositionType.VestFront);
                HapticFeedbackFrame vBack = HapticFeedbackFrame.AsTurnOffFrame(PositionType.VestBack);
                HapticFeedbackFrame head = HapticFeedbackFrame.AsTurnOffFrame(PositionType.Head);
                f = new[] { left, right, vFront, vBack, head };
            }
            else
            {
                f = new[] { HapticFeedbackFrame.AsTurnOffFrame(position) };

            }

            HapticFeedback[i * interval] = f;
            EndTime = i * interval;
        }

        public BufferedHapticFeedback(PositionType position, List<PathPoint> points, int durationMillis, int texture = 0)
        {
            if (durationMillis <= 0)
            {
                throw new HapticException("durationMillis should be positive");
            }

            int i;
            HapticFeedback = new Dictionary<int, HapticFeedbackFrame[]>();
            for (i = 0; i < durationMillis / interval; i++)
            {
                HapticFeedbackFrame[] feedbacks;
                if (position == PositionType.All)
                {
                    HapticFeedbackFrame left =  HapticFeedbackFrame.AsFeedbackFrame(PositionType.Left, points, texture);
                    HapticFeedbackFrame right =  HapticFeedbackFrame.AsFeedbackFrame(PositionType.Right, points, texture);
                    HapticFeedbackFrame vFront =  HapticFeedbackFrame.AsFeedbackFrame(PositionType.VestFront, points, texture);
                    HapticFeedbackFrame vBack =  HapticFeedbackFrame.AsFeedbackFrame(PositionType.VestBack, points, texture);
                    HapticFeedbackFrame head =  HapticFeedbackFrame.AsFeedbackFrame(PositionType.Head, points, texture);
                    feedbacks = new[] { left, right, vFront, vBack, head };
                }
                else
                {
                    feedbacks = new[] { HapticFeedbackFrame.AsFeedbackFrame(position, points, texture) };
                }

                HapticFeedback[i * interval] = feedbacks;
            }
            StartTime = -1;
            HapticFeedbackFrame[] f;
            if (position == PositionType.All)
            {
                HapticFeedbackFrame left = HapticFeedbackFrame.AsTurnOffFrame(PositionType.Left);
                HapticFeedbackFrame right = HapticFeedbackFrame.AsTurnOffFrame(PositionType.Right);
                HapticFeedbackFrame vFront = HapticFeedbackFrame.AsTurnOffFrame(PositionType.VestFront);
                HapticFeedbackFrame vBack = HapticFeedbackFrame.AsTurnOffFrame(PositionType.VestBack);
                HapticFeedbackFrame head = HapticFeedbackFrame.AsTurnOffFrame(PositionType.Head);
                f = new[] { left, right, vFront, vBack, head };
            }
            else
            {
                f = new[] { HapticFeedbackFrame.AsTurnOffFrame(position) };

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
                HapticFeedback = new Dictionary<int, HapticFeedbackFrame[]>()
            };

            int time;
            for (time = 0; time < bufferedHapticFeedback.EndTime; time += interval)
            {
                int keyTime = (int)(time / durationRatio) / interval * interval;

                if (bFeedback.HapticFeedback.ContainsKey(keyTime))
                {
                    HapticFeedbackFrame[] hapticFeedbacks = bFeedback.HapticFeedback[keyTime];

                    HapticFeedbackFrame[] copiedFeedbacks = new HapticFeedbackFrame[hapticFeedbacks.Length];

                    for (int i = 0; i < hapticFeedbacks.Length; i++)
                    {
                        HapticFeedbackFrame hapticFeedback = hapticFeedbacks[i];

                        HapticFeedbackFrame feedback = new HapticFeedbackFrame();
                        feedback.Position = hapticFeedback.Position;
                        feedback.Texture = hapticFeedback.Texture;
                        copiedFeedbacks[i] = feedback;

                        foreach (var point in hapticFeedback.DotPoints)
                        {
                            int val = (int)(point.Intensity * intensityRatio);
                            if (val > 100)
                            {
                                val = 100;
                            }
                            else if (val < 0)
                            {
                                val = 0;
                            }
                            var pt = new DotPoint(point.Index, val);
                            copiedFeedbacks[i].DotPoints.Add(pt);
                        }

                        foreach (var point in hapticFeedback.PathPoints)
                        {
                            int val = (int)(point.Intensity * intensityRatio);
                            if (val > 100)
                            {
                                val = 100;
                            }
                            else if (val < 0)
                            {
                                val = 0;
                            }
                            var pt = new PathPoint(point.X, point.Y, val);
                            copiedFeedbacks[i].PathPoints.Add(pt);
                        }

                        bufferedHapticFeedback.HapticFeedback[time] = copiedFeedbacks;
                    }
                }
            }

            return bufferedHapticFeedback;
        }

    }
}
