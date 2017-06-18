using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Bhaptics.Tac.Sender;
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
            var hapticPlayer = new HapticPlayer(new WebSocketSender(), new MultimediaTimer { Interval = 20});

            // Register with DotPoint
            var key = "headFeedabck";
            var points = new List<DotPoint>() {new DotPoint(1, 100)};
            hapticPlayer.Register(key, new BufferedHapticFeedback(PositionType.Head, points,  1000));
            
            // Register with file
            HapticFeedbackFile file = CommonUtils.ConvertJsonStringToTactosyFile(File.ReadAllText("test2.tactosy"));
            string key2 = "tactosyLeftFeedback";
            hapticPlayer.Register(key2, new BufferedHapticFeedback(file));        

            // Register with json string
            string key3 = "tactosyFeedback";
            hapticPlayer.Register(key3, new BufferedHapticFeedback(File.ReadAllText("test3.tactosy")));

            hapticPlayer.FeedbackChanged += feedback =>
            {
                if (feedback.Position == PositionType.Left)
                {
                    Debug.WriteLine($"{feedback.Position} {CommonUtils.ConvertByteArrayToString(feedback.Values)}");
                }
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