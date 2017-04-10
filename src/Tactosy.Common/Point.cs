namespace Tactosy.Common
{
    public class Point
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="intensity">The intensity.</param>
        public Point(float x, float y, float intensity)
        {
            X = x;
            Y = y;
            Intensity = intensity;
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
    }

    public class CoordinatePoint
    {
        public CoordinatePoint(int x, int y, float intensity)
        {
            X = x;
            Y = y;
            Intensity = intensity;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public float Intensity { get; set; }
    }
}
