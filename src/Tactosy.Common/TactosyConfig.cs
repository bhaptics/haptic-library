namespace Tactosy.Common
{
    public class TactosyConfig
    {
        private static readonly byte[] DefaultBytes =
        {
            100, 4, 0
        };

        /// <summary>
        /// Gets or sets the maximum voltage.
        /// </summary>
        /// <value>
        /// The maximum voltage.
        /// </value>
        public int MaxVoltage { get; set; }

        /// <summary>
        /// Gets or sets the texture.
        /// </summary>
        /// <value>
        /// The texture.
        /// </value>
        public int Texture { get; set; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public int Color { get; set; } // 0: green, 1 : blue, 2 : sky blue

        /// <summary>
        /// Gets or sets the type of the position.
        /// </summary>
        /// <value>
        /// The type of the position.
        /// </value>
        public PositionType PositionType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TactosyConfig"/> class.
        /// </summary>
        public TactosyConfig() : this(PositionType.Left, DefaultBytes)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TactosyConfig"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public TactosyConfig(PositionType type) : this(type, DefaultBytes)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TactosyConfig"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="configs">The configs.</param>
        public TactosyConfig(PositionType type, byte[] configs)
        {
            PositionType = type;
            MaxVoltage = configs[0];
            Texture = configs[1];
            Color = configs[2];
        }

        public override string ToString()
        {
            return "TactosyConfig {MaxVoltage=" + MaxVoltage +
                 ", Color=" + Color +
                 ", Texture=" + Texture + "}";
        }
    }
}
