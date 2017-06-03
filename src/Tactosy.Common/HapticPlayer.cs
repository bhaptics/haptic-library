using System;
using System.Collections.Generic;
using System.Diagnostics;
using Tactosy.Common.Sender;

namespace Tactosy.Common
{
    /// <summary>
    /// HapticPlayer
    /// </summary>
    /// <seealso cref="IHapticPlayer" />
    public class HapticPlayer : IHapticPlayer
    {
        private int _currentTime = 0;
        private int _motorSize = 20;
        private readonly int _interval;
        private bool _enable = false;
        
        // .NET 3.5 doesnot support System.Collections.Concurrent so let's use lock....
        private readonly Dictionary<string, BufferedHapticFeedback> _registered;
        private readonly Dictionary<string, BufferedHapticFeedback> _actives;
        private readonly ITimer _timer;
        private readonly ISender _sender;

        public HapticPlayer(ISender sender, ITimer timer, int interval = 20)
        {
            if (interval <= 0)
            {
                throw new HapticException("Interval should be positive : " + interval);
            }

            if (timer == null)
            {
                throw new HapticException("Timer should not be null");
            }

            _registered = new Dictionary<string, BufferedHapticFeedback>();
            _actives = new Dictionary<string, BufferedHapticFeedback>();
            _sender = sender;
            _timer = timer;
            _timer.Elapsed += TimerOnElapsed;

            _interval = interval;

            Enable();
        }

        public HapticPlayer() : this(new WebSocketSender(), new MultimediaTimer())
        {
        }
        
        private void PlayFeedback(HapticFeedback feedback)
        {
            if (!_enable)
            {
                feedback.Mode = FeedbackMode.DOT_MODE;
                feedback.Values = new byte[_motorSize];
            }

            // callback
            _sender.PlayFeedback(feedback);
            if (FeedbackChanged != null)
            {
                FeedbackChanged(feedback);
            }
        }

        private void TimerOnElapsed(object sender, EventArgs e)
        {
            if (_actives.Count == 0)
            {
                if (_currentTime > 0)
                {
                    _currentTime = 0;

                    PlayFeedback(new HapticFeedback(PositionType.Right, new byte[_motorSize], FeedbackMode.DOT_MODE));
                    PlayFeedback(new HapticFeedback(PositionType.Left, new byte[_motorSize], FeedbackMode.DOT_MODE));
                    PlayFeedback(new HapticFeedback(PositionType.VestFront, new byte[_motorSize], FeedbackMode.DOT_MODE));
                    PlayFeedback(new HapticFeedback(PositionType.VestBack, new byte[_motorSize], FeedbackMode.DOT_MODE));
                    PlayFeedback(new HapticFeedback(PositionType.Head, new byte[_motorSize], FeedbackMode.DOT_MODE));
                }

                return;
            }
            
            List<string> expired = new List<string>();

            Dictionary<PositionType, Dictionary<FeedbackMode, List<HapticFeedback>>> feedbackMap = new Dictionary<PositionType, Dictionary<FeedbackMode, List<HapticFeedback>>>();
            feedbackMap[PositionType.Left] = new Dictionary<FeedbackMode, List<HapticFeedback>>();
            feedbackMap[PositionType.Right] = new Dictionary<FeedbackMode, List<HapticFeedback>>();
            feedbackMap[PositionType.VestBack] = new Dictionary<FeedbackMode, List<HapticFeedback>>();
            feedbackMap[PositionType.VestFront] = new Dictionary<FeedbackMode, List<HapticFeedback>>();
            feedbackMap[PositionType.Head] = new Dictionary<FeedbackMode, List<HapticFeedback>>();


            foreach (KeyValuePair<string, BufferedHapticFeedback> keyPair in _actives)
            {
                BufferedHapticFeedback data = keyPair.Value;
                if (data.StartTime > _currentTime || data.StartTime == -1)
                {
                    data.StartTime = _currentTime;
                }
                int timePast = _currentTime - data.StartTime;

                if (timePast > data.EndTime)
                {
                    expired.Add(keyPair.Key);
                }
                else
                {
                    if (data.HapticFeedback.ContainsKey(timePast))
                    {
                        var hapticFeedbackData = data.HapticFeedback[timePast];
                        foreach (var feedback in hapticFeedbackData)
                        {
                            if (!feedbackMap.ContainsKey(feedback.Position))
                            {
                                feedbackMap[feedback.Position] = new Dictionary<FeedbackMode, List<HapticFeedback>>();
                            }

                            if (!feedbackMap[feedback.Position].ContainsKey(feedback.Mode))
                            {
                                feedbackMap[feedback.Position][feedback.Mode] = new List<HapticFeedback>();
                            }

                            feedbackMap[feedback.Position][feedback.Mode].Add(feedback);
                        }
                    }
                }
            }

            foreach (var keyValue in feedbackMap)
            {
                var modeDictionary = keyValue.Value;
                var pos = keyValue.Key;

                bool isDotMode = false;
                bool isPathMode = false;

                byte[] dotMode = new byte[_motorSize];
                byte[] pathMode = new byte[_motorSize];

                foreach (var feedbackKeyValue in modeDictionary)
                {
                    var feedbackMode = feedbackKeyValue.Key;

                    if (feedbackMode == FeedbackMode.DOT_MODE)
                    {
                        isDotMode = true;
                        foreach (var feedback in feedbackKeyValue.Value)
                        {
                            for (int i = 0; i < _motorSize; i++)
                            {
                                dotMode[i] += feedback.Values[i];
                            }
                        }
                    }
                    else if (feedbackMode == FeedbackMode.PATH_MODE)
                    {
                        isPathMode = true;
                        foreach (var feedback in feedbackKeyValue.Value)
                        {
                            int prevSize = pathMode[0];

                            byte[] data = feedback.Values;
                            int size = data[0];
                            if (prevSize + size > 6)
                            {
                                continue;
                            }

                            pathMode[0] = (byte) (prevSize + size);

                            for (int i = prevSize; i < prevSize + size; i++)
                            {
                                pathMode[3 * i + 1] = data[3 * (i - prevSize) + 1];
                                pathMode[3 * i + 2] = data[3 * (i - prevSize) + 2];
                                pathMode[3 * i + 3] = data[3 * (i - prevSize) + 3];
                            }
                        }
                    }
                }

                if (isDotMode)
                {
                    PlayFeedback(new HapticFeedback(pos, dotMode, FeedbackMode.DOT_MODE));
                }

                if (isPathMode)
                {
                    PlayFeedback(new HapticFeedback(pos, pathMode, FeedbackMode.PATH_MODE));
                }

                if (!isDotMode && !isPathMode)
                {
                    PlayFeedback(new HapticFeedback(pos, new byte[_motorSize], FeedbackMode.DOT_MODE));
                }
            }

            foreach (string key in expired)
            {
                _actives.Remove(key);
            }

            _currentTime += _interval;
        }

        #region public methods
        public bool IsPlaying(string key)
        {
            return _actives.ContainsKey(key);
        }
        
        public bool IsPlaying()
        {
            return _actives.Count > 0;
        }
        
        public void Register(string key, string path)
        {
            TactosyFile file = TactosyUtils.ConvertToTactosyFile(path);
            BufferedHapticFeedback bufferedHapticFeedback = new BufferedHapticFeedback(file);
            Register(key, bufferedHapticFeedback);
        }

        public void Register(string key, BufferedHapticFeedback bufferedHapticFeedback)
        {
            _registered[key] = bufferedHapticFeedback;
        }

        public void Register(string key, HapticFeedback feedback, int durationMillis)
        {
            _registered[key] = new BufferedHapticFeedback(feedback, durationMillis);
        }
        
        public void Submit(string key, PositionType position, byte[] motorBytes, int durationMillis)
        {
            HapticFeedback feedback = new HapticFeedback(position, motorBytes, FeedbackMode.DOT_MODE);
            _actives[key] = new BufferedHapticFeedback(feedback, durationMillis, _interval);
        }

        public void Submit(string key, PositionType position, List<DotPoint> points, int durationMillis)
        {
            byte[] bytes = new byte[_motorSize];

            foreach (var dotPoint in points)
            {
                bytes[dotPoint.Index] = (byte)dotPoint.Intensity;
            }

            Submit(key, position, bytes, durationMillis);
        }

        public void Submit(string key, PositionType position, DotPoint point, int durationMillis)
        {
            Submit(key, position, new List<DotPoint> {point}, durationMillis);
        }
        
        public void Submit(string key, PositionType position, List<PathPoint> points, int durationMillis)
        {
            if (points.Count > 6 || points.Count <= 0)
            {
                Debug.WriteLine("number of points should be [1~6]");
                return;
            }

            byte[] bytes = new byte[_motorSize];
            bytes[0] = (byte)points.Count;

            for (var i = 0; i < points.Count; i++)
            {
                bytes[3 * i + 1] = (byte)TactosyUtils.Min(40f, TactosyUtils.Max(0f, points[i].X * 40f)); // x
                bytes[3 * i + 2] = (byte)TactosyUtils.Min(30f, TactosyUtils.Max(0f, (float)points[i].Y * 40f)); // y
                bytes[3 * i + 3] = (byte)TactosyUtils.Min(100f, TactosyUtils.Max(0f, points[i].Intensity)); // z
            }

            HapticFeedback feedback = new HapticFeedback(position, bytes, FeedbackMode.PATH_MODE);
            _actives[key] = new BufferedHapticFeedback(feedback, durationMillis, _interval);
        }

        public void Submit(string key, PositionType position, PathPoint point, int durationMillis)
        {
            Submit(key, position, new List<PathPoint> {point}, durationMillis);
        }
        
        public void SubmitRegistered(string key, float intensity, float duration)
        {
            if (!_registered.ContainsKey(key))
            {
                Debug.WriteLine("Key : " + key + " is not registered.");
                return;
            }

            if (duration < 0.01f || duration > 100f)
            {
                Debug.WriteLine("not allowed duration " + duration);
                return;
            }

            if (intensity < 0.01f || intensity > 100f)
            {
                Debug.WriteLine("not allowed intensity " + duration);
                return;
            }

            BufferedHapticFeedback signal = _registered[key];
            BufferedHapticFeedback feedback = BufferedHapticFeedback.Copy(signal, _interval, intensity, duration);

            _actives[key] = feedback;
        }
        
        public void SubmitRegistered(string key)
        {
            if (!_registered.ContainsKey(key))
            {
                Debug.WriteLine("Key : " + key + " is not registered.");
                return;
            }
            var signal = _registered[key];

            signal.StartTime = -1;
            if (!_actives.ContainsKey(key))
            {
                _actives[key] = signal;
            }
        }

        public void SubmitRegistered(string key, float ratio)
        {
            if (!_registered.ContainsKey(key))
            {
                Debug.WriteLine("Key : " + key + " is not registered.");
                return;
            }

            if (ratio < 0 || ratio > 1)
            {
                Debug.WriteLine("ratio should be between [0, 1]");
                return;
            }
            var signal = _registered[key];
            signal.StartTime = _currentTime - (int) (signal.EndTime * ratio / _interval) * _interval;
            if (!_actives.ContainsKey(key))
            {
                _actives[key] = signal;
            }
        }
        
        public void TurnOff(string key)
        {
            if (!_actives.ContainsKey(key))
            {
                Debug.WriteLine("feedback with key( " + key + " ) is not playing.");
                return;
            }

            _actives.Remove(key);
        }
        
        public void TurnOff()
        {
            _actives.Clear();
        }

        public void Enable()
        {
            if (_enable)
            {
                Debug.WriteLine("Already Started.");
                return;
            }
            _actives.Clear();
            if (_timer == null)
            {
                return;
            }

            _timer.StartTimer();
            _enable = true;
            PlayFeedback(new HapticFeedback(PositionType.All, new byte[20], FeedbackMode.DOT_MODE));
        }

        public void Disable()
        {
            if (!_enable)
            {
                Debug.WriteLine("Already Stopped.");
                return;
            }

            _actives.Clear();
            if (_timer == null)
            {
                return;
            }

            _timer.StopTimer();
            _enable = false;
            PlayFeedback(new HapticFeedback(PositionType.All, new byte[20], FeedbackMode.DOT_MODE));
        }

        public event FeedbackEvent.HapticFeedbackChangeEvent FeedbackChanged;

        #endregion
    }
}