using UnityEngine;
using System.Collections.Generic;
using System.IO;
using TactosyCommon.Models;

namespace TactosyCommon.Unity
{
    public class FeedbackSignal
    {
        public Dictionary<int, TactosyFeedback[]> HapticFeedback;
        
        public int StartTime { get; set; }
        public int EndTime { get; set; }

        private string json;

        public FeedbackSignal(TactosyFeedback feedback, int durationMillis, int interval)
        {
            int i;
            HapticFeedback = new Dictionary<int, TactosyFeedback[]>();
            for (i = 0; i < durationMillis/interval; i++)
            {
                TactosyFeedback[] feedbacks;
                if (feedback.Position == PositionType.All)
                {
                    TactosyFeedback left = new TactosyFeedback(PositionType.Left, feedback.Values, feedback.Mode);
                    TactosyFeedback right = new TactosyFeedback(PositionType.Right, feedback.Values, feedback.Mode);
                    feedbacks = new[] {left, right};
                }
                else
                {
                    feedbacks = new[] { feedback };
                }
                
                HapticFeedback[i*interval] = feedbacks;
            }
            StartTime = -1;
            TactosyFeedback[] f;
            if (feedback.Position == PositionType.All)
            {

                TactosyFeedback left = new TactosyFeedback(PositionType.Left, new byte[20], feedback.Mode);
                TactosyFeedback right = new TactosyFeedback(PositionType.Right, new byte[20], feedback.Mode);
                f = new[] { left, right };
            }
            else
            {
                f = new[] {new TactosyFeedback(feedback.Position, new byte[20], feedback.Mode)};

            }

            HapticFeedback[i * interval] = f;
            EndTime = i * interval;
        }

        private FeedbackSignal()
        {
        }

        public FeedbackSignal(string path)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                WWW www = new WWW(path);

                while (!www.isDone)
                {
                }

                json = www.text;
            }
            else
            {
                json = File.ReadAllText(path);          
            }
            

            TactosyFile tactosyFile = TactosyUtils.ConvertJsonStringToTactosyFile(json);
            EndTime = tactosyFile.durationMillis;
            HapticFeedback = tactosyFile.feedback;
        }

        public static FeedbackSignal Copy(FeedbackSignal signal, int interval, float intensityRatio, float durationRatio)
        {
            FeedbackSignal feedbackSignal = new FeedbackSignal();
            feedbackSignal.EndTime = (int)(signal.EndTime * durationRatio / interval * interval) + interval;
            feedbackSignal.StartTime = -1;
            feedbackSignal.HapticFeedback = new Dictionary<int, TactosyFeedback[]>();
            int time;
            for (time = 0; time < feedbackSignal.EndTime; time += interval)
            {
                int keyTime = (int) (time/durationRatio)/interval*interval;

                if (signal.HapticFeedback.ContainsKey(keyTime))
                {
                    TactosyFeedback[] tactosyFeedbacks = signal.HapticFeedback[keyTime];

                    TactosyFeedback[] copiedFeedbacks = new TactosyFeedback[tactosyFeedbacks.Length];

                    for (int i = 0; i < tactosyFeedbacks.Length; i++)
                    {
                        TactosyFeedback tactosyFeedback = tactosyFeedbacks[i];
                        byte[] values = new byte[tactosyFeedback.Values.Length];
                        if (tactosyFeedback.Mode == FeedbackMode.DOT_MODE)
                        {
                            for (int valueIndex = 0; valueIndex < tactosyFeedback.Values.Length; valueIndex++)
                            {
                                int val = (int) (tactosyFeedback.Values[valueIndex]*intensityRatio);
                                if (val > 100)
                                {
                                    val = 100;
                                }
                                else if (val < 0)
                                {
                                    val = 0;
                                }

                                values[valueIndex] = (byte) val;
                            }
                        }
                        else if (tactosyFeedback.Mode == FeedbackMode.PATH_MODE)
                        {
                            for (int valueIndex = 0; valueIndex < tactosyFeedback.Values.Length; valueIndex++)
                            {
                                values[valueIndex] = tactosyFeedback.Values[valueIndex];
                            }

                            for (int index = 0; index < 6; index++)
                            {
                                int realIndex = 3 + index*3;
                                int val = (int) (tactosyFeedback.Values[realIndex]*intensityRatio);
                                if (val > 100)
                                {
                                    val = 100;
                                }
                                else if (val < 0)
                                {
                                    val = 0;
                                }

                                values[realIndex] = (byte) val;
                            }
                        }
                        TactosyFeedback feedback = new TactosyFeedback(tactosyFeedback.Position, values,
                            tactosyFeedback.Mode);
                        copiedFeedbacks[i] = feedback;
                        feedbackSignal.HapticFeedback[time] = copiedFeedbacks;
                    }
                }
            }

            return feedbackSignal;
        }
    }
}
