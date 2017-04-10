using System;

namespace Tactosy.Common
{
    public class TactosyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TactosyException"/> class.
        /// </summary>
        public TactosyException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TactosyException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public TactosyException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TactosyException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public TactosyException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}
