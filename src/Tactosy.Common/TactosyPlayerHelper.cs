using System;
using System.Collections.Generic;
using Tactosy.Common.Sender;

namespace Tactosy.Common
{
    public class TactosyPlayerHelper
    {
        private static ITactosyPlayer _player;
        private static ISender _sender;
        private static ITimer _timer;

        public static ITactosyPlayer Instance()
        {
            if (_player == null)
            {
                _sender = new WebSocketSender();
                _timer = new MultimediaTimer()
                {
                    Interval = 20
                };
                _player = new TactosyPlayer(_sender, _timer);
            }

            return _player;
        }

        public static void Play(IFeedback feedback)
        {
            var player = Instance();

            if (feedback.Type == FeebackType.RegisteredFeedback)
            {
                player.SendSignal(feedback.Key);
            }
            else if (feedback.Type == FeebackType.DotMode)
            {
                var dot = feedback as DotmodeFeedback;

                if (dot == null)
                {
                    throw new ArgumentException("Dotmode argument exception");
                }

                player.SendSignal(dot.Key, dot.Position, dot.Values, dot.Duration);
            }
            else if (feedback.Type == FeebackType.PathMode)
            {
                var path = feedback as PathModeFeedback;
                
                if (path == null)
                {
                    throw new ArgumentException("Pathmode argument exception");
                }
                player.SendSignal(path.Key, path.Position, path.Points, path.Duration);
            }
        }

        public static IFeedback GetDotFeedback(string name, PositionType position, int intensity, int duration)
        {
            byte val = (byte)intensity;
            byte[] values =
            {
                0, 0, 0, 0, 0, val, val, val, val, val,
                val, val, val, val, val, 0, 0, 0, 0, 0
            };

            var soldierFeedback = new DotmodeFeedback(name, position, values, duration);

            return soldierFeedback;
        }
    }

    public enum FeebackType
    {
        DotMode, PathMode, RegisteredFeedback
    }

    public interface IFeedback
    {
        FeebackType Type { get; set; }
        string Key { get; }
    }

    public class DotmodeFeedback : IFeedback
    {
        public DotmodeFeedback(string key, PositionType position, byte[] values, int duration)
        {
            Values = values;
            Position = position;
            Key = key;
            Type = FeebackType.DotMode;
            Duration = duration;
        }

        public byte[] Values { get; set; }
        public PositionType Position { get; set; }
        public FeebackType Type { get; set; }
        public string Key { get; set; }
        public int Duration { get; set; }
    }

    public class PathModeFeedback : IFeedback
    {
        public PathModeFeedback(string key, PositionType position, List<Point> points, int duration)
        {
            Points = points;
            Position = position;
            Key = key;
            Type = FeebackType.PathMode;
            Duration = duration;
        }

        public List<Point> Points { get; set; }
        public PositionType Position { get; set; }
        public FeebackType Type { get; set; }
        public string Key { get; set; }
        public int Duration { get; set; }
    }

    public class RegisteredFeedback : IFeedback
    {
        public RegisteredFeedback(string key)
        {
            Key = key;
            Type = FeebackType.RegisteredFeedback;
        }
        public FeebackType Type { get; set; }
        public string Key { get; set; }
    }
}
