using System;

namespace Tactosy.Common
{
    public class Point
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> class.
        /// </summary>
        /// <param name="x">The x. [0, 1]</param>
        /// <param name="y">The y. [0, 1]</param>
        /// <param name="intensity">The intensity. [0, 1]</param>
        public Point(float x, float y, float intensity)
        {
            X = Math.Min(1f, Math.Max(0f, x));
            Y = Math.Min(1f, Math.Max(0f, y));
            Intensity = Math.Min(1f, Math.Max(0f, intensity));
        }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>
        /// The x.
        /// </value>
        public float X { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>
        /// The y.
        /// </value>
        public float Y { get; set; }

        /// <summary>
        /// Gets or sets the intensity.
        /// </summary>
        /// <value>
        /// The intensity.
        /// </value>
        public float Intensity { get; set; }

        public override string ToString()
        {
            return "Point {X=" + X +
                          ", Y=" + Y +
                          ", Intensity=" + Intensity + "}"; ;
        }
    }

    public class IndexPoint
    {
        public IndexPoint(int index, float intensity)
        {
            if (index < 0 && index > 40)
            {
                throw new TactosyException("Invalid index");
            }

            Index = index;
            Intensity = Math.Min(1f, Math.Max(0f, intensity));
        }

        public int Index { get; set; }
        public float Intensity { get; set; }
        
        public override string ToString()
        {
            return "IndexPoint {Index=" + Index +
                   ", Intensity=" + Intensity + "}";
        }
    }

    public class Point1D
    {
        public Point1D(float x, float intensity)
        {
            X = Math.Min(1f, Math.Max(0f, x));
            Intensity = Math.Min(1f, Math.Max(0f, intensity));
        }

        public float X { get; set; }
        public float Intensity { get; set; }

        public override string ToString()
        {
            return "IndexPoint {X=" + X +
                   ", Intensity=" + Intensity + "}";
        }
    }
}
