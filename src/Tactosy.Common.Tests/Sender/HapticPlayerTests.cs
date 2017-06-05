using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tactosy.Common.Tests.Sender
{
    [TestClass()]
    public class HapticPlayerTests
    {
        private readonly string key = "testKey";
        [TestMethod()]
        public void HapticPlayerTest()
        {
            HapticPlayer player = new HapticPlayer();

            var points = new List<DotPoint> {new DotPoint(2, 100), new DotPoint(3, 100)};
            player.Register(key, new BufferedHapticFeedback(PositionType.Left, points, 2000));
            
            player.SubmitRegistered(key);

            while (player.IsPlaying(key))
            {   
            }
            Assert.IsNotNull(player);
        }
    }
}