using System.Diagnostics;
using System.Threading;
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
            var player = new HapticPlayer();

            // Register with DotPoint
            var head = "head";
            
            // Register with file
            string sleeve = "test2";
            player.Register(sleeve, "test2.tact"); 
            string vest = "vest1";
            player.Register(vest, "vest1.tact");

            player.StatusReceived += feedback =>
            {
                Debug.WriteLine($"{string.Join(",", feedback.ActiveKeys)}");
            };

            // Play two feeabck together
            player.Submit(head, PositionType.Head, new DotPoint(0, 100), 500);
            player.SubmitRegistered(sleeve);
            player.SubmitRegistered(vest);
            // Waiting for response
            Thread.Sleep(200);
            while (player.IsPlaying(vest))
            {
            }

            player.SubmitRegistered(sleeve);

            // Waiting for response
            Thread.Sleep(200);

            while (player.IsPlaying(sleeve))
            {
            }

            player.Dispose();
        }
    }
}