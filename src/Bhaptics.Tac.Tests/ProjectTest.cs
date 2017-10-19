using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bhaptics.Tac.Tests
{
    [TestClass()]
    public class ProjectTests
    {
        [TestMethod()]
        public void ParseTest()
        {
            var hapticPlayer = new HapticPlayer((connected) =>
            {
                Debug.WriteLine("Connected");
            });
            hapticPlayer.Register("test", "BowShoot.tact");

            hapticPlayer.StatusReceived += feedback =>
            {
                Debug.WriteLine($"{string.Join(",", feedback.ActiveKeys.ToArray())}");
            };

            Thread.Sleep(100);
            hapticPlayer.SubmitRegistered("test");
            Thread.Sleep(1000);
            hapticPlayer.Disable();
            hapticPlayer.SubmitRegistered("test");

            Thread.Sleep(1000);
            hapticPlayer.Enable();
            var points = new List<PathPoint>();
            points.Add(new PathPoint(0.5f, 0.5f, 100));
            points.Add(new PathPoint(0f, 0f, 50));
            points.Add(new PathPoint(1f, 1f, 50));
            hapticPlayer.Submit("test2", PositionType.Right, points, 1000);
            Thread.Sleep(1000);

            var dots = new List<DotPoint>();
            dots.Add(new DotPoint(1, 50));
            dots.Add(new DotPoint(2, 50));
            hapticPlayer.Submit("test3", PositionType.Right, dots, 100);
            Thread.Sleep(1000);
            hapticPlayer.Dispose();
        }
    }
}