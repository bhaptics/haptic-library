using System;
using System.Collections.Generic;

namespace Tactosy.Common
{
    public class TactosyFeedback
    {
        #region private members
        private int gridSize = 20;
        private int column = 5;
        private int row = 4;
        private int resolution = 10;
        private static int defaultTexture = 0;
        #endregion

        #region public members
        public PositionType Position { get; set; }
        public FeedbackMode Mode { get; set; }
        public byte[] Values { get; set; }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TactosyFeedback"/> class.
        /// </summary>
        public TactosyFeedback()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TactosyFeedback"/> class.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <exception cref="Exception">
        /// Illegal argument size
        /// or
        /// Undefined Position Type
        /// </exception>
        public TactosyFeedback(byte[] bytes)
        {
            // refer https://github.com/bhaptics-corp/tactosy-win
            if (bytes.Length != 22)
            {
                throw new Exception("Illegal argument size");
            }

            Values = TactosyUtils.SubArray(bytes, 2, 20);
            
            if (bytes[0] == 0)
            {
                // turn off
                Values = new byte[20];
                Mode = FeedbackMode.DOT_MODE;
            } else if (bytes[0] == 1)
            {
                Mode = FeedbackMode.PATH_MODE; ;
            } else if (bytes[0] == 2)
            {
                Mode = FeedbackMode.DOT_MODE;
            }


            if (bytes[1] == 1)
            {
                Position = PositionType.Left;
            } else if (bytes[1] == 2)
            {
                Position = PositionType.Right;
            }
            else if (bytes[1] == 0)
            {
                Position = PositionType.All;
            } else
            {
                throw new Exception("Undefined Position Type");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TactosyFeedback"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="values">The values.</param>
        /// <param name="mode">The mode.</param>
        public TactosyFeedback(PositionType position, byte[] values, FeedbackMode mode)
        {
            Position = position;
            Values = values;
            Mode = mode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TactosyFeedback"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="points">The points.</param>
        /// <param name="texture">The texture.</param>
        public TactosyFeedback(PositionType position, List<Point> points, int texture)
        {
            Position = position;
            Mode = FeedbackMode.PATH_MODE;

            Values = new byte[gridSize];
            Values[0] = (byte)points.Count;

            if (points.Count > 6)
            {
                // undefined points
                return;
            }

            for (int i = 0; i < points.Count; i++)
            {
                Values[3 * i + 1] = (byte)(points[i].X * (column - 1) * resolution);
                Values[3 * i + 2] = (byte)(points[i].Y * (row - 1) * resolution);
                Values[3 * i + 3] = (byte)(points[i].Intensity * 100);
            }

            Values[19] = (byte) texture;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TactosyFeedback"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="points">The points.</param>
        public TactosyFeedback(PositionType position, List<Point> points): this(position, points, defaultTexture)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TactosyFeedback"/> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="points">The points.</param>
        public TactosyFeedback(PositionType position, List<CoordinatePoint> points)
        {
            Position = position;
            Mode = FeedbackMode.DOT_MODE;

            Values = new byte[gridSize];
            foreach (CoordinatePoint coordinatePoint in points)
            {
                int idx = coordinatePoint.Y * column + coordinatePoint.X;
                Values[idx] = (byte)(coordinatePoint.Intensity * 100);
            }
        }
        #endregion

        public override string ToString()
        {
            return "TactosyDevice {Position=" + Position +
                 ", Mode=" + Mode +
                ", Values=" + TactosyUtils.ConvertByteArrayToString(Values) + "}";
        }

        #region static method
        /// <summary>
        /// feedback to the bytes.
        /// </summary>
        /// <param name="feedback">The feedback.</param>
        /// <returns></returns>
        public static byte[] ToBytes(TactosyFeedback feedback)
        {
            byte[] bytes = new byte[22];
            if (feedback.Mode == FeedbackMode.PATH_MODE)
            {
                bytes[0] = 1;
            } else if (feedback.Mode == FeedbackMode.DOT_MODE)
            {
                bytes[0] = 2;
            }

            if (feedback.Position == PositionType.Left)
            {
                bytes[1] = 1;
            } else if (feedback.Position == PositionType.Right)
            {
                bytes[1] = 2;
            }

            Buffer.BlockCopy(feedback.Values,0, bytes, 2, 20);

            return bytes;
        }
        #endregion
    }

    public enum PositionType
    {
        All = 0, Left = 1, Right = 2,
        VestFront =3, VestBack=4,
        GloveLeft =5, GloveRight=6,
        Custom1 =251, Custom2 = 252, Custom3 = 253, Custom4 = 254
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
