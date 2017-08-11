using System;
using System.Collections.Generic;

namespace Bhaptics.Tac
{
    public class DotPoint
    {
        public DotPoint(int index, int intensity)
        {
            if (index < 0)
            {
                throw new HapticException("Invalid argument index : " + index);
            }
            Intensity = CommonUtils.Clamp(intensity, 0, 100);

            Index = index;
        }

        public int Index { get; set; }
        public int Intensity { get; set; }

        public override string ToString()
        {
            return "DotPoint { Index=" + Index +
                   ", Intensity=" + Intensity + "}";
        }
    }

    public class PathPoint
    {
        public PathPoint(float x, float y, int intensity)
        {
            X = CommonUtils.Clamp(x, 0f, 1f);
            Y = CommonUtils.Clamp(y, 0f, 1f);
            Intensity = CommonUtils.Clamp(intensity, 0, 100);
        }

        public float X { get; set; }
        public float Y { get; set; }
        public int Intensity { get; set; }

        public override string ToString()
        {
            return "PathPoint { X=" + X +
                   ", Y=" + Y +
                   ", Intensity=" + Intensity + "}";
        }
    }

    public class HapticFeedback
    {
        #region public members
        public PositionType Position { get; set; }
        public FeedbackMode Mode { get; set; }
        public byte[] Values { get; set; }
        public int Texture { get; set; }
        #endregion

        #region Constructor
        public HapticFeedback(PositionType position, byte[] values, FeedbackMode mode, int texture = 0)
        {
            Position = position;
            Values = values;
            Mode = mode;
            Texture = texture;
        }

        public HapticFeedback(PositionType position, List<PathPoint> points, int texture = 0)
        {
            Position = position;
            Mode = FeedbackMode.PATH_MODE;
            Texture = texture;
            Values = new byte[20];
            Values[0] = (byte)(points.Count > 6 ? 6 : points.Count);

            for (int i = 0; i < Values[0]; i++)
            {
                var point = points[i];
                Values[3 * i + 1] = (byte)(point.X * 100f);
                Values[3 * i + 2] = (byte)(point.Y * 100f);
                Values[3 * i + 3] = (byte)point.Intensity;
            }
        }

        #endregion

        public override string ToString()
        {
            return "HapticFeedback {Position=" + Position +
                 ", Mode=" + Mode +
                ", Values=" + CommonUtils.ConvertByteArrayToString(Values) + "}";
        }

        #region static method
        public static byte[] ToBytes(HapticFeedback feedback)
        {
            byte[] bytes = new byte[22];
            bytes[0] = (byte) feedback.Mode;
            bytes[1] = (byte) feedback.Position;

            Buffer.BlockCopy(feedback.Values,0, bytes, 2, 20);

            return bytes;
        }
        #endregion
    }

    public enum PositionType
    {
        All = 0, Left = 1, Right = 2,
        Vest = 3,
        Head = 4,
        Racket = 5,
        VestFront =201, VestBack=202,
        GloveLeft =203, GloveRight=204,
        Custom1 =251, Custom2 = 252, Custom3 = 253, Custom4 = 254
    }

    public class FeedbackItem
    {
        public FeedbackItem(int time, Dictionary<int, HapticFeedbackData> feedback)
        {
            Time = time;

            Feedback = feedback;
        }

        public int Time { get; set; }

        public Dictionary<int, HapticFeedbackData> Feedback { get; set; }
    }

    public class HapticFeedbackData
    {
        public int Time { get; set; }
        public int[] Values { get; set; }
        public FeedbackMode Mode { get; set; }
    }

    public enum FeedbackMode
    {
        PATH_MODE = 1,
        DOT_MODE = 2
    }

    public class EnumParser
    {
        private static Dictionary<string, PositionType> _positionMappings;
        private static Dictionary<string, FeedbackMode> _modeMappings;

        public static PositionType ToPositionType(string str)
        {
            if (_positionMappings == null)
            {
                _positionMappings = new Dictionary<string, PositionType>();

                foreach (PositionType positionType in Enum.GetValues(typeof(PositionType)))
                {
                    _positionMappings[positionType.ToString()] = positionType;
                }
            }

            PositionType type;
            if (_positionMappings.TryGetValue(str, out type))
            {
                return type;
            }

            return PositionType.All;
        }

        public static FeedbackMode ToMode(string str)
        {
            if (_modeMappings == null)
            {
                _modeMappings = new Dictionary<string, FeedbackMode>();

                foreach (FeedbackMode positionType in Enum.GetValues(typeof(FeedbackMode)))
                {
                    _modeMappings[positionType.ToString()] = positionType;
                }
            }

            FeedbackMode type;
            if (_modeMappings.TryGetValue(str, out type))
            {
                return type;
            }

            return FeedbackMode.DOT_MODE;
        }
    }
}
