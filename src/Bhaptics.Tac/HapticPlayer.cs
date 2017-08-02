using System;
using System.Collections.Generic;
using System.Diagnostics;
using Bhaptics.Tac.Sender;

namespace Bhaptics.Tac
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
        
        private void PlayFeedback(HapticFeedbackFrame feedback)
        {
            if (!_enable)
            {
                _sender.PlayFeedback(HapticFeedbackFrame.AsTurnOffFrame(feedback.Position));
                return;
            }

            if (feedback.DotPoints.Count > 0)
            {
                Debug.WriteLine(feedback.Position + ", " + feedback.DotPoints.Count);
            }

            // callback
            
            _sender.PlayFeedback(feedback);
        }

        private void TimerOnElapsed(object sender, EventArgs e)
        {
            if (_actives.Count == 0)
            {
                if (_currentTime > 0)
                {
                    _currentTime = 0;
                    PlayFeedback(HapticFeedbackFrame.AsTurnOffFrame(PositionType.Right));
                    PlayFeedback(HapticFeedbackFrame.AsTurnOffFrame(PositionType.Left));
                    PlayFeedback(HapticFeedbackFrame.AsTurnOffFrame(PositionType.VestFront));
                    PlayFeedback(HapticFeedbackFrame.AsTurnOffFrame(PositionType.VestBack));
                    PlayFeedback(HapticFeedbackFrame.AsTurnOffFrame(PositionType.Head));
                }

                return;
            }
            
            List<string> expired = new List<string>();

            Dictionary<PositionType, HapticFeedbackFrame> feedbackMap = new Dictionary<PositionType, HapticFeedbackFrame>();
            feedbackMap[PositionType.Left] = new HapticFeedbackFrame{Position = PositionType.Left};
            feedbackMap[PositionType.Right] = new HapticFeedbackFrame { Position = PositionType.Right };
            feedbackMap[PositionType.VestBack] = new HapticFeedbackFrame { Position = PositionType.VestBack };
            feedbackMap[PositionType.VestFront] = new HapticFeedbackFrame { Position = PositionType.VestFront };
            feedbackMap[PositionType.Head] = new HapticFeedbackFrame { Position = PositionType.Head };

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
                                feedbackMap[feedback.Position] = new HapticFeedbackFrame { Position = feedback.Position };
                            }
                            
                            feedbackMap[feedback.Position].DotPoints.AddRange(feedback.DotPoints);
                            feedbackMap[feedback.Position].Texture = feedback.Texture;
                            feedbackMap[feedback.Position].PathPoints.AddRange(feedback.PathPoints);
                        }
                    }
                }
            }

            foreach (var keyValue in feedbackMap)
            {
                var frame = keyValue.Value;
                PlayFeedback(frame);
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
            HapticFeedbackFile file = CommonUtils.ConvertToTactosyFile(path);
            BufferedHapticFeedback bufferedHapticFeedback = new BufferedHapticFeedback(file);
            Register(key, bufferedHapticFeedback);
        }

        public void Register(string key, BufferedHapticFeedback bufferedHapticFeedback)
        {
            _registered[key] = bufferedHapticFeedback;
        }
        
        public void Submit(string key, PositionType position, byte[] motorBytes, int durationMillis)
        {
            List<DotPoint> points = new List<DotPoint>();
            for (int i = 0; i < motorBytes.Length; i++)
            {
                if (motorBytes[i] > 0)
                {
                    points.Add(new DotPoint(i, motorBytes[i]));
                }
            }

            _actives[key] = new BufferedHapticFeedback(position, points, durationMillis);
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
            _actives[key] = new BufferedHapticFeedback(position, points, durationMillis);
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

        public void SubmitRegistered(string key, float duration)
        {
            if (!_registered.ContainsKey(key))
            {
                Debug.WriteLine("Key : " + key + " is not registered.");
                return;
            }

            if (duration < 0 || duration > 1)
            {
                Debug.WriteLine("ratio should be between [0, 1]");
                return;
            }
            var signal = _registered[key];
            signal.StartTime = _currentTime - (int) (signal.EndTime * duration / _interval) * _interval;
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
            PlayFeedback(HapticFeedbackFrame.AsTurnOffFrame(PositionType.All));

            _sender.FeedbackChangeReceived += SenderOnFeedbackChangeReceived;
        }

        private void SenderOnFeedbackChangeReceived(HapticFeedback hapticFeedback)
        {
            if (FeedbackChanged != null)
            {
                FeedbackChanged(hapticFeedback);
            }
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

            PlayFeedback(HapticFeedbackFrame.AsTurnOffFrame(PositionType.All));

            _sender.FeedbackChangeReceived -= SenderOnFeedbackChangeReceived;
        }

        public event FeedbackEvent.HapticFeedbackChangeEvent FeedbackChanged;

        #endregion
    }
}