using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tactosy.Common.Tests
{
    [TestClass()]
    public class TactosyPlayerHelperTests
    {
        [TestMethod()]
        public void PlayTest()
        {
            var tactosyPlayer = TactosyPlayerHelper.Instance();
            byte[] bytes =
            {
                0, 0, 0, 0, 0,
                0, 0, 0, 0, 0,
                0, 0, 100, 0, 0,
                0, 0, 0, 0, 0
            };
            var key = "a";
            tactosyPlayer.RegisterFeedback(key, new FeedbackSignal(new TactosyFeedback(PositionType.Head, bytes, FeedbackMode.DOT_MODE), 1000));

            string filePath = "test.tactosy";
            string json = File.ReadAllText(filePath);   
            tactosyPlayer.RegisterFeedback("tt", new FeedbackSignal(json));


            tactosyPlayer.SendSignal(key);


            while (tactosyPlayer.IsPlaying(key))
            {

            }
        }
    }
}