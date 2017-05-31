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
            Point point = new Point(2f, 0.5f, -1f);
            
            Debug.WriteLine(point);

            IndexPoint point2 = new IndexPoint(1, -1f);
            
            Debug.WriteLine(point2);
        }
    }
}