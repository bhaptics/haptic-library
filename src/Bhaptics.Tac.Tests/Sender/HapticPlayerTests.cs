using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bhaptics.Tac.Tests.Sender
{
    [TestClass()]
    public class HapticPlayerTests
    {
        private readonly string key = "testKey";
        [TestMethod()]
        public void HapticPlayerTest()
        {
            var player = new HapticPlayer();
            
            player.Register(key, "BowShoot.tact");
            player.SubmitRegistered(key);
            // waiting for submission
            Thread.Sleep(100);

            while (player.IsPlaying(key))
            {   
            }
            Assert.IsNotNull(player);
            player.Dispose();
            
        }
    }
}