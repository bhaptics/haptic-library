using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bhaptics.Tac.Tests
{
    [TestClass()]
    public class PointTests
    {
        [TestMethod()]
        public void PointTest()
        {
            PathPoint point = new PathPoint(2f, 0.5f, 100);
            
            Assert.IsTrue(point.X <= 1f);
            Assert.IsTrue(Math.Abs(point.Y - 0.5f) <= 0.01f);
            Debug.WriteLine(point);

            DotPoint point2 = new DotPoint(1, 100);
            
            Debug.WriteLine(point2);
        }
    }
}