using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bhaptics.Tac.Tests
{
    [TestClass()]
    public class HapticPlayerTests
    {
        [TestMethod()]
        public void PlayTest()
        {
            // Initialize hapticPlayer
            var hapticPlayer = new HapticPlayer();

            // Register with DotPoint
            var key = "headFeedabck";
            
            // Register with file
            string key2 = "tactosyLeftFeedback";
            hapticPlayer.Register(key2, "test2.tact");        

            // Register with json string
            string key3 = "tactosyFeedback";
            hapticPlayer.Register(key3, "test3.tact");

            hapticPlayer.StatusReceived += feedback =>
            {
                Debug.WriteLine($"{string.Join(",", feedback.ActiveKeys)}");

            };

            // Play two feeabck together
            hapticPlayer.SubmitRegistered(key);
            hapticPlayer.SubmitRegistered(key3);
            while (hapticPlayer.IsPlaying(key))
            {
            }

            hapticPlayer.SubmitRegistered(key2);
            while (hapticPlayer.IsPlaying(key2))
            {
            }
        }
    }
}