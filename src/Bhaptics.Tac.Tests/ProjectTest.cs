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
            var key = "feedbackWithKey";
            var hapticPlayer = new HapticPlayer((connected) =>
            {
                Debug.WriteLine("Connected");
            });
            hapticPlayer.Register(key, "BowShoot.tact");

            hapticPlayer.StatusReceived += feedback =>
            {
                if (feedback.ActiveKeys.Count <= 0)
                {
                    return;
                }
                Debug.WriteLine($"Active feedbacks : {string.Join(",", feedback.ActiveKeys.ToArray())}");
            };

            Thread.Sleep(100);
            hapticPlayer.SubmitRegistered(key);
            Thread.Sleep(1000);

            hapticPlayer.Disable();
            hapticPlayer.SubmitRegistered(key);
            Thread.Sleep(1000);
            hapticPlayer.Enable();

            hapticPlayer.SubmitRegistered(key, 0.5f, 5f);
            Thread.Sleep(2000);

            hapticPlayer.SubmitRegistered(key, 2f, 0.3f);
            Thread.Sleep(500);

            hapticPlayer.SubmitRegistered(key, 0.8f);
            Thread.Sleep(500);

            var points = new List<PathPoint>();
            points.Add(new PathPoint(0.5f, 0.5f, 100));
            points.Add(new PathPoint(0f, 0f, 50));
            points.Add(new PathPoint(1f, 1f, 50));
            hapticPlayer.Submit("feedbackWithPathPoints", PositionType.Right, points, 1000);
            Thread.Sleep(950);

            var dots = new List<DotPoint>();
            dots.Add(new DotPoint(1, 50));
            dots.Add(new DotPoint(2, 50));
            hapticPlayer.Submit("feedbackWithDotPoints", PositionType.Right, dots, 100);
            Thread.Sleep(1000);
            hapticPlayer.Dispose();
        }
    }
}