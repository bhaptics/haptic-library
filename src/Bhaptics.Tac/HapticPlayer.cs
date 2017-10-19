using System;
using System.Collections.Generic;
using System.Diagnostics;
using Bhaptics.Tac.Designer;
using Bhaptics.Tac.Sender;

namespace Bhaptics.Tac
{
    public class HapticPlayer : IHapticPlayer
    {
        private readonly DefaultWebSocketSender _sender;
        private readonly List<string> _activeKeys = new List<string>();
        public event FeedbackEvent.StatusReceivedEvent StatusReceived;

        public HapticPlayer(FeedbackEvent.ConnectionEvent connectionChanged, bool tryReconnect = true)
        {
            _sender = new DefaultWebSocketSender();
            _sender.StatusReceived += feedback =>
            {
                StatusReceived?.Invoke(feedback);

                lock (_activeKeys)
                {
                    _activeKeys.Clear();
                    _activeKeys.AddRange(feedback.ActiveKeys);
                }
            };
            _sender.ConnectionChanged += (isConn) =>
            {
                connectionChanged?.Invoke(isConn);
            };
            _sender.Initialize(tryReconnect);
        }

        public HapticPlayer(bool tryReconnect = true) : this(null, tryReconnect)
        {
        }

        public void Dispose()
        {
            _sender.StoreTurnOff();
            _sender.Dispose();
        }

        public void Enable()
        {
            _sender.Enable();
        }

        public void Disable()
        {
            _sender.Disable();
        }

        public bool IsPlaying(string key)
        {

            lock (_activeKeys)
            {
                return _activeKeys.Contains(key);
            }
        }

        public bool IsPlaying()
        {
            return _activeKeys.Count > 0;
        }

        public void Register(string key, string path)
        {
            HapticFeedbackFile file = CommonUtils.ConvertToTactosyFile(path);
            Register(key, file.Project);
        }

        public void Register(string key, Project project)
        {
            _sender.Register(key, project);
        }

        public void Submit(string key, PositionType position, 
            byte[] motorBytes, int durationMillis)
        {
            var points = new List<DotPoint>();
            for (int i = 0; i < motorBytes.Length; i++)
            {
                if (motorBytes[i] > 0)
                {
                    points.Add(new DotPoint(i, motorBytes[i]));
                }
            }

            Submit(key, position, points, durationMillis);
        }

        public void Submit(string key, 
            PositionType position, 
            List<DotPoint> points, 
            int durationMillis)
        {
            var frame = Frame.AsDotPointFrame(points, position, durationMillis);
            _sender.Store(key, frame);
        }

        public void Submit(string key, PositionType position, DotPoint point, int durationMillis)
        {
            Submit(key, position, new List<DotPoint> { point }, durationMillis);
        }

        public void Submit(string key, PositionType position, List<PathPoint> points, int durationMillis)
        {
            var frame = Frame.AsPathPointFrame(points, position, durationMillis);
            _sender.Store(key, frame);
        }

        public void Submit(string key, PositionType position, PathPoint point, int durationMillis)
        {
            Submit(key, position, new List<PathPoint> { point }, durationMillis);
        }

        public void SubmitRegistered(string key, float intensity, float duration)
        {
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

            throw new NotImplementedException();
        }

        public void SubmitRegistered(string key)
        {
            _sender.Store(key);
//            SubmitRegistered(key, 1f, 1f);
        }

        public void SubmitRegistered(string key, float duration)
        {
            if (duration < 0 || duration > 1)
            {
                Debug.WriteLine("ratio should be between [0, 1]");
                return;
            }
            SubmitRegistered(key, 1f, duration);
        }

        public void TurnOff(string key)
        {
            _sender.StoreTurnOff(key);
        }

        public void TurnOff()
        {
            _sender.StoreTurnOff();
        }
    }
}
