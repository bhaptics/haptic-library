using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tactosy.Common.Tests
{
    [TestClass()]
    public class PointTests
    {
        [TestMethod()]
        public void PointTest()
        {
            PathPoint point = new PathPoint(2f, 0.5f, 100);
            
            Debug.WriteLine(point);

            DotPoint point2 = new DotPoint(1, 100);
            
            Debug.WriteLine(point2);
        }
    }
}