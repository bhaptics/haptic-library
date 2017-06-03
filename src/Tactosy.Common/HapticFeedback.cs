using System;
using System.Collections.Generic;

namespace Tactosy.Common
{
    public enum Texture
    {
        Soft = 0, Middle = 2, Rough = 4  
    }

    public class DotPoint
    {
        public DotPoint(int index, int intensity)
        {
            Index = index;
            Intensity = intensity;
        }

        public int Index { get; set; }
        public int Intensity { get; set; }
    }

    public class PathPoint
    {
        public PathPoint(float x, float y, int intensity)
        {
            X = x;
            Y = y;
            Intensity = intensity;
        }

        public float X { get; set; }
        public float Y { get; set; }
        public int Intensity { get; set; }
    }

    public class HapticFeedback
    {
        #region public members
        public PositionType Position { get; set; }
        public FeedbackMode Mode { get; set; }
        public byte[] Values { get; set; }
        #endregion

        #region Constructor
        
//        public HapticFeedback(byte[] bytes)
//        {
//            // refer https://github.com/bhaptics/tactosy-sharp
//            if (bytes.Length != 22)
//            {
//                throw new HapticException("Illegal argument size");
//            }
//
//            Values = TactosyUtils.SubArray(bytes, 2, 20);
//            try
//            {
//                Mode = (FeedbackMode)bytes[0];
//            }
//            catch (Exception)
//            {
//                // turn off
//                Values = new byte[20];
//                Mode = FeedbackMode.DOT_MODE;
//            }
//
//            try
//            {
//                Position = (PositionType) bytes[1];
//            }
//            catch (Exception)
//            {
//                throw new HapticException("Undefined Position Type");
//            }
//        }

        public HapticFeedback(PositionType position, byte[] values, FeedbackMode mode)
        {
            Position = position;
            Values = values;
            Mode = mode;
        }

        public HapticFeedback(PositionType position, List<PathPoint> points, Texture texture = Texture.Soft)
        {
            Position = position;
            Mode = FeedbackMode.PATH_MODE;
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
                ", Values=" + TactosyUtils.ConvertByteArrayToString(Values) + "}";
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
