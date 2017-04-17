using System;
using System.Collections.Generic;
using System.Diagnostics;
using Tactosy.Common.Sender;

namespace Tactosy.Common
{
    /// <summary>
    /// TactosyManager
    /// </summary>
    /// <seealso cref="ITactosyPlayer" />
    public class TactosyPlayer : ITactosyPlayer
    {
        /// <summary>
        /// The current time
        /// </summary>
        private int _currentTime = 0;

        /// <summary>
        /// The motor size
        /// </summary>
        private int _motorSize = 20;

        /// <summary>
        /// The interval
        /// </summary>
        private readonly int _interval;

        /// <summary>
        /// The enable
        /// </summary>
        private bool _enable = true;
        
        // .NET 3.5 doesnot support System.Collections.Concurrent so let's use lock....
        /// <summary>
        /// The registered signals
        /// </summary>
        private readonly Dictionary<string, FeedbackSignal> _registeredSignals;

        /// <summary>
        /// The active signals
        /// </summary>
        private readonly Dictionary<string, FeedbackSignal> _activeSignals;

        /// <summary>
        /// The timer
        /// </summary>
        private readonly ITimer _timer;

        /// <summary>
        /// The sender
        /// </summary>
        private readonly ISender _sender;
        

        /// <summary>
        /// Initializes a new instance of the <see cref="TactosyPlayer"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="timer">The timer.</param>
        /// <param name="interval">The interval.</param>
        /// <exception cref="Exception">
        /// Interval should be positive : " + interval
        /// or
        /// Timer should not be null
        /// </exception>
        public TactosyPlayer(ISender sender, ITimer timer, int interval = 20)
        {
            if (interval <= 0)
            {
                throw new Exception("Interval should be positive : " + interval);
            }

            if (timer == null)
            {
                throw new Exception("Timer should not be null");
            }

            _registeredSignals = new Dictionary<string, FeedbackSignal>();
            _activeSignals = new Dictionary<string, FeedbackSignal>();
            _sender = sender;
            _timer = timer;
            _timer.Elapsed += TimerOnElapsed;

            _interval = interval;

            StartPlayer();
        }

        public TactosyPlayer() : this(new WebSocketSender(), new MultimediaTimer())
        {
        }

        private void StartPlayer()
        {
            _activeSignals.Clear();
            if (_timer == null)
            {
                return;
            }

            _timer.StartTimer();
        }

        private void StopPlayer()
        {
            _activeSignals.Clear();
            if (_timer == null)
            {
                return;
            }

            _timer.StopTimer();
        }

        /// <summary>
        /// Plays the feedback.
        /// </summary>
        /// <param name="feedback">The feedback.</param>
        private void PlayFeedback(TactosyFeedback feedback)
        {
            if (!_enable)
            {
                feedback.Mode = FeedbackMode.DOT_MODE;
                feedback.Values = new byte[_motorSize];
            }

            // callback
            _sender.PlayFeedback(feedback);
            if (ValueChanged != null)
            {
                ValueChanged(feedback);
            }
        }

        private void TimerOnElapsed(object sender, EventArgs e)
        {
            if (_activeSignals.Count == 0)
            {
                if (_currentTime > 0)
                {
                    _currentTime = 0;

                    PlayFeedback(new TactosyFeedback(PositionType.Right, new byte[_motorSize], FeedbackMode.DOT_MODE));
                    PlayFeedback(new TactosyFeedback(PositionType.Left, new byte[_motorSize], FeedbackMode.DOT_MODE));
                }

                return;
            }

            int[] dotModeSignalLeft = new int[_motorSize];
            int[] dotModeSignalRight = new int[_motorSize];
            int[] pathModeSignalLeft = new int[_motorSize];
            int[] pathModeSignalRight = new int[_motorSize];

            List<string> expiredSignals = new List<string>();


            bool dotModeLeftActive = false;
            bool dotModeRightActive = false;
            bool pathModeActiveLeft = false;
            bool pathModeActiveRight = false;
            
            foreach (KeyValuePair<string, FeedbackSignal> keyPair in _activeSignals)
            {
                FeedbackSignal signalData = keyPair.Value;
                if (signalData.StartTime > _currentTime || signalData.StartTime == -1)
                {
                    signalData.StartTime = _currentTime;
                }
                int timePast = _currentTime - signalData.StartTime;

                if (timePast > signalData.EndTime)
                {
                    expiredSignals.Add(keyPair.Key);
                }
                else
                {
                    if (signalData.HapticFeedback.ContainsKey(timePast))
                    {
                        var hapticFeedbackData = signalData.HapticFeedback[timePast];
                        foreach (var feedback in hapticFeedbackData)
                        {
                            if (feedback.Mode == FeedbackMode.PATH_MODE && feedback.Position == PositionType.Left)
                            {
                                int prevSize = pathModeSignalLeft[0];

                                byte[] data = feedback.Values;
                                int size = data[0];
                                if (prevSize + size > 6)
                                {
                                    continue;
                                }

                                pathModeSignalLeft[0] = prevSize + size;

                                for (int i = prevSize; i < prevSize + size; i++)
                                {
                                    pathModeSignalLeft[3 * i + 1] = data[3 * (i - prevSize) + 1];
                                    pathModeSignalLeft[3 * i + 2] = data[3 * (i - prevSize) + 2];
                                    pathModeSignalLeft[3 * i + 3] = data[3 * (i - prevSize) + 3];
                                }

                                pathModeActiveLeft = true;
                            }
                            else if (feedback.Mode == FeedbackMode.PATH_MODE && feedback.Position == PositionType.Right)
                            {
                                int prevSize = pathModeSignalRight[0];

                                byte[] data = feedback.Values;
                                int size = data[0];
                                if (prevSize + size > 6)
                                {
                                    continue;
                                }

                                pathModeSignalRight[0] = prevSize + size;

                                for (int i = prevSize; i < prevSize + size; i++)
                                {
                                    pathModeSignalRight[3 * i + 1] = data[3 * (i - prevSize) + 1];
                                    pathModeSignalRight[3 * i + 2] = data[3 * (i - prevSize) + 2];
                                    pathModeSignalRight[3 * i + 3] = data[3 * (i - prevSize) + 3];
                                }

                                pathModeActiveRight = true;
                            }
                            else if (feedback.Mode == FeedbackMode.DOT_MODE && feedback.Position == PositionType.Left)
                            {
                                for (int i = 0; i < _motorSize; i++)
                                {
                                    dotModeSignalLeft[i] += feedback.Values[i];
                                }

                                dotModeLeftActive = true;
                            }
                            else if (feedback.Mode == FeedbackMode.DOT_MODE && feedback.Position == PositionType.Right)
                            {
                                for (int i = 0; i < _motorSize; i++)
                                {
                                    dotModeSignalRight[i] += feedback.Values[i];
                                }
                                dotModeRightActive = true;
                            }
                        }
                    }
                }
            }

            foreach (string key in expiredSignals)
            {
                _activeSignals.Remove(key);
            }

            if (dotModeLeftActive)
            {
                byte[] data = new byte[_motorSize];
                for (int i = 0; i < _motorSize; i++)
                {
                    data[i] = (byte)dotModeSignalLeft[i];
                }

                TactosyFeedback feedback = new TactosyFeedback(PositionType.Left, data, FeedbackMode.DOT_MODE);
                PlayFeedback(feedback);
            }
            else if (pathModeActiveLeft)
            {
                byte[] data = new byte[_motorSize];
                for (int i = 0; i < _motorSize; i++)
                {
                    data[i] = (byte)pathModeSignalLeft[i];
                }
                TactosyFeedback feedback = new TactosyFeedback(PositionType.Left, data, FeedbackMode.PATH_MODE);
                PlayFeedback(feedback);
            }
            else
            {
                TactosyFeedback feedback = new TactosyFeedback(PositionType.Left, new byte[_motorSize], FeedbackMode.DOT_MODE);
                PlayFeedback(feedback);
            }

            if (dotModeRightActive)
            {
                byte[] data = new byte[_motorSize];
                for (int i = 0; i < _motorSize; i++)
                {
                    data[i] = (byte)dotModeSignalRight[i];
                }

                TactosyFeedback feedback = new TactosyFeedback(PositionType.Right, data, FeedbackMode.DOT_MODE);
                PlayFeedback(feedback);
            }
            else if (pathModeActiveRight)
            {
                byte[] data = new byte[_motorSize];
                for (int i = 0; i < _motorSize; i++)
                {
                    data[i] = (byte)pathModeSignalRight[i];
                }
                TactosyFeedback feedback = new TactosyFeedback(PositionType.Right, data, FeedbackMode.PATH_MODE);
                PlayFeedback(feedback);
            }
            else
            {
                TactosyFeedback feedback = new TactosyFeedback(PositionType.Right, new byte[_motorSize], FeedbackMode.DOT_MODE);
                PlayFeedback(feedback);
            }

            _currentTime += _interval;
        }

        #region public methods
        /// <summary>
        /// Determines whether the specified key is playing.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if the specified key is playing; otherwise, <c>false</c>.
        /// </returns>
        public bool IsPlaying(string key)
        {
            return _activeSignals.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether this instance is playing.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is playing; otherwise, <c>false</c>.
        /// </returns>
        public bool IsPlaying()
        {
            return _activeSignals.Count > 0;
        }

        /// <summary>
        /// Registers the feedback.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="path">The path.</param>
        public void RegisterFeedback(string key, string path)
        {
            TactosyFile file = TactosyUtils.ConvertToTactosyFile(path);
            FeedbackSignal feedbackSignal = new FeedbackSignal(file);
            RegisterFeedback(key, feedbackSignal);
        }

        /// <summary>
        /// Registers the feedback.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="feedbackSignal">The feedback signal.</param>
        public void RegisterFeedback(string key, FeedbackSignal feedbackSignal)
        {
            _registeredSignals[key] = feedbackSignal;
        }

        /// <summary>
        /// Sends the signal.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="position">The position.</param>
        /// <param name="motorBytes">The motor bytes.</param>
        /// <param name="durationMillis">The duration millis.</param>
        public void SendSignal(string key, PositionType position, byte[] motorBytes, int durationMillis)
        {
            TactosyFeedback feedback = new TactosyFeedback(position, motorBytes, FeedbackMode.DOT_MODE);
            _activeSignals[key] = new FeedbackSignal(feedback, durationMillis, _interval);
        }

        /// <summary>
        /// Sends the signal.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="position">The position.</param>
        /// <param name="points">The points.</param>
        /// <param name="durationMillis">The duration millis.</param>
        public void SendSignal(string key, PositionType position, List<Point> points, int durationMillis)
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
                bytes[3 * i + 3] = (byte)TactosyUtils.Min(100f, TactosyUtils.Max(0f, (float)points[i].Intensity * 100f)); // z
            }

            TactosyFeedback feedback = new TactosyFeedback(position, bytes, FeedbackMode.PATH_MODE);
            _activeSignals[key] = new FeedbackSignal(feedback, durationMillis, _interval);
        }

        /// <summary>
        /// Sends the signal.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="intensity">The intensity.</param>
        /// <param name="duration">The duration.</param>
        public void SendSignal(string key, float intensity, float duration)
        {
            if (!_registeredSignals.ContainsKey(key))
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


            FeedbackSignal signal = _registeredSignals[key];

            FeedbackSignal copiedFeedbackSignal = FeedbackSignal.Copy(signal, _interval, intensity, duration);

            _activeSignals[key] = copiedFeedbackSignal;
        }

        /// <summary>
        /// Sends the signal.
        /// </summary>
        /// <param name="key">The key.</param>
        public void SendSignal(string key)
        {
            if (!_registeredSignals.ContainsKey(key))
            {
                Debug.WriteLine("Key : " + key + " is not registered.");

                return;
            }
            var signal = _registeredSignals[key];

            signal.StartTime = -1;
            if (!_activeSignals.ContainsKey(key))
            {
                _activeSignals[key] = signal;
            }
        }

        public void SendSignal(string key, float ratio)
        {
            if (!_registeredSignals.ContainsKey(key))
            {
                Debug.WriteLine("Key : " + key + " is not registered.");

                return;
            }

            if (ratio < 0 || ratio > 1)
            {
                Debug.WriteLine("ratio should be between [0, 1]");
                return;
            }
            var signal = _registeredSignals[key];
            signal.StartTime = _currentTime - (int) (signal.EndTime * ratio / _interval) * _interval;
            if (!_activeSignals.ContainsKey(key))
            {
                _activeSignals[key] = signal;
            }
        }

        /// <summary>
        /// Turns off specified feedback with key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void TurnOff(string key)
        {
            if (!_activeSignals.ContainsKey(key))
            {
                Debug.WriteLine("feedback with key( " + key + " ) is not playing.");
                return;
            }

            _activeSignals.Remove(key);
        }

        /// <summary>
        /// Turns off.
        /// </summary>
        public void TurnOff()
        {
            _activeSignals.Clear();
        }

        public void Start()
        {

            if (_enable)
            {
                Debug.WriteLine("Already Started.");
                return;
            }

            StartPlayer();
            _enable = true;
            PlayFeedback(new TactosyFeedback(PositionType.All, new byte[20], FeedbackMode.DOT_MODE));
        }

        public void Stop()
        {
            if (!_enable)
            {
                Debug.WriteLine("Already Stopped.");
                return;
            }

            StopPlayer();
            _enable = false;
            PlayFeedback(new TactosyFeedback(PositionType.All, new byte[20], FeedbackMode.DOT_MODE));
        }

        public event TactosyEvent.ValueChangeEvent ValueChanged;

        #endregion
    }
}