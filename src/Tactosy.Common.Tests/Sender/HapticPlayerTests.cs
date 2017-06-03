using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tactosy.Common.Tests.Sender
{
    [TestClass()]
    public class HapticPlayerTests
    {
        private readonly string key = "testKey";
        private readonly byte[] bytes = {
            0, 0, 0, 0, 0,
            0, 0, 0, 0, 100,
            100, 0, 0, 0, 0,
            0, 0, 0, 0, 0};

        [TestMethod()]
        public void HapticPlayerTest()
        {
            HapticPlayer player = new HapticPlayer();

            HapticFeedback feedback = new HapticFeedback(PositionType.Left, bytes, FeedbackMode.DOT_MODE);

            player.Register(key, new BufferedHapticFeedback(feedback, 1000));
            player.Register(key, feedback, 1000);
            
            player.SubmitRegistered(key);

            while (player.IsPlaying(key))
            {   
            }
            Assert.IsNotNull(player);
        }
    }
}