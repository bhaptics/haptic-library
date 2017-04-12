using System;
using System.Collections.Generic;

namespace Tactosy.Common
{
    public class TactosyEvent
    {
        public delegate void ValueChangeEvent(TactosyFeedback feedback);
    }

    /// <summary>
    /// ITactosyManager
    /// </summary>
    public interface ITactosyPlayer
    {
        /// <summary>
        /// Determines whether the specified key is playing.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if the specified key is playing; otherwise, <c>false</c>.
        /// </returns>
        bool IsPlaying(string key);
        /// <summary>
        /// Determines whether this instance is playing.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is playing; otherwise, <c>false</c>.
        /// </returns>
        bool IsPlaying();

        /// <summary>
        /// Registers the feedback.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="path">The path.</param>
        void RegisterFeedback(string key, string path );
        /// <summary>
        /// Registers the feedback.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="tactosyFile">The tactosy file.</param>
        void RegisterFeedback(string key, FeedbackSignal tactosyFile);

        /// <summary>
        /// Sends the signal.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="position">The position.</param>
        /// <param name="motorBytes">The motor bytes.</param>
        /// <param name="durationMillis">The duration millis.</param>
        void SendSignal(string key, PositionType position, byte[] motorBytes, int durationMillis);
        /// <summary>
        /// Sends the signal.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="position">The position.</param>
        /// <param name="points">The points.</param>
        /// <param name="durationMillis">The duration millis.</param>
        void SendSignal(string key, PositionType position, List<Point> points, int durationMillis);
        /// <summary>
        /// Sends the signal.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="intensity">The intensity.</param>
        /// <param name="duration">The duration.</param>
        void SendSignal(string key, float intensity, float duration);
        /// <summary>
        /// Sends the signal.
        /// </summary>
        /// <param name="key">The key.</param>
        void SendSignal(string key);


        /// <summary>
        /// Sends the signal.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="ratio">The start ratio of the feedback.</param>
        void SendSignal(string key, float ratio);

        /// <summary>
        /// Turns the off.
        /// </summary>
        /// <param name="key">The key.</param>
        void TurnOff(string key);
        /// <summary>
        /// Turns the off.
        /// </summary>
        void TurnOff();

        /// <summary>
        /// Starts this instance.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops this instance.
        /// </summary>
        void Stop();

        event TactosyEvent.ValueChangeEvent ValueChanged;
    }
}
