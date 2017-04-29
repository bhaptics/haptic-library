using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tactosy.Common.Tests
{
    [TestClass()]
    public class TactosyFeedbackTests
    {
        [TestMethod()]
        public void TactosyFeedbackTest()
        {
            var tactosyPlayer = TactosyPlayerHelper.Instance();
            byte[] bytes =
            {
                0, 0, 0, 0, 0,
                0, 0, 0, 0, 0,
                0, 0, 100, 0, 0,
                0, 0, 0, 0, 0
            };

            tactosyPlayer.SendSignal("a", PositionType.VestFront, bytes, 2000);

            while (tactosyPlayer.IsPlaying("a"))
            {

            }
        }

        [TestMethod()]
        public void TactosyFeedbackParsingTest()
        {
            byte[] tactosyBytes =
            {
                2, 1, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0,
                0, 0, 0, 0, 0,
                0, 0, 0, 0, 0
            };
            TactosyFeedback feedback = new TactosyFeedback(tactosyBytes);
            
            Assert.AreEqual(feedback.Position, PositionType.Left);
            Assert.AreEqual(feedback.Mode, FeedbackMode.DOT_MODE);


            byte[] vestBack =
            {
                1, 202, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0,
                0, 0, 0, 0, 0,
                0, 0, 0, 0, 0
            };
            TactosyFeedback vestBackFeedback = new TactosyFeedback(vestBack);

            Assert.AreEqual(vestBackFeedback.Position, PositionType.VestBack);
            Assert.AreEqual(vestBackFeedback.Mode, FeedbackMode.PATH_MODE);
        }
    }
}