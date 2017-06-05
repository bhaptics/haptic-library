using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tactosy.Common.Sender;

namespace Tactosy.Common.Tests
{
    [TestClass()]
    public class HapticPlayerTests
    {
        [TestMethod()]
        public void PlayTest()
        {
            var tactosyPlayer = new HapticPlayer(new WebSocketSender(), new MultimediaTimer { Interval = 20});
            var key = "a";
            var points = new List<DotPoint>() {new DotPoint(1, 100)};
            tactosyPlayer.Register(key, new BufferedHapticFeedback(PositionType.Head, points,  1000));

            string filePath = "test2.tactosy";
            string json = File.ReadAllText(filePath);
            string key2 = "b";
            tactosyPlayer.Register(key2, new BufferedHapticFeedback(json));

            tactosyPlayer.FeedbackChanged += feedback =>
            {
                if (feedback.Position == PositionType.Left)
                {
                    Debug.WriteLine($"{feedback.Position} {TactosyUtils.ConvertByteArrayToString(feedback.Values)}");
                }
            };


            tactosyPlayer.SubmitRegistered(key);

            while (tactosyPlayer.IsPlaying(key))
            {

            }

            tactosyPlayer.SubmitRegistered(key2);

            while (tactosyPlayer.IsPlaying(key2))
            {

            }
        }
    }
}