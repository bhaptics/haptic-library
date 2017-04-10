using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tactosy.Common.Tests.Sender
{
    [TestClass()]
    public class WebSocketSenderTests
    {
        [TestMethod()]
        public void WebSocketSenderTest()
        {
            TactosyPlayer player = new TactosyPlayer();
            string key = "testKey";
            byte[] bytes = {
                0, 0, 0, 0, 0,
                0, 0, 0, 0, 100,
                100, 0, 0, 0, 0,
                0, 0, 0, 0, 0};
            player.Start();
            player.RegisterFeedback(key, new FeedbackSignal(new TactosyFeedback(PositionType.Left, bytes, FeedbackMode.DOT_MODE), 1000));
            player.SendSignal(key);

            while (player.IsPlaying(key))
            {
                
            }
            Assert.IsNotNull(player);
        }
    }
}