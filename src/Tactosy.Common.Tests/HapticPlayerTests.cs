using System;
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
            byte[] bytes =
            {
                0, 100, 0, 0, 0,
                0, 0, 0, 0, 0,
                0, 0, 100, 0, 0,
                0, 0, 0, 0, 0
            };
            var key = "a";
            var feedback = new HapticFeedback(PositionType.Head, bytes, FeedbackMode.DOT_MODE);
            tactosyPlayer.Register(key, new BufferedHapticFeedback(feedback, 1000));

            string filePath = "test2.tactosy";
            string json = File.ReadAllText(filePath);
            string key2 = "b";
            tactosyPlayer.Register(key2, new BufferedHapticFeedback(json));

            tactosyPlayer.SubmitRegistered(key2);


            while (tactosyPlayer.IsPlaying(key2))
            {

            }
        }
    }
}