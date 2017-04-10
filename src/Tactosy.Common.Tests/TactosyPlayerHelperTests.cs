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
            tactosyPlayer.RegisterFeedback(key, new FeedbackSignal(new TactosyFeedback(PositionType.All, bytes, FeedbackMode.DOT_MODE), 10000));

            tactosyPlayer.SendSignal(key, 0.3f);


            while (tactosyPlayer.IsPlaying(key))
            {

            }
        }
    }
}