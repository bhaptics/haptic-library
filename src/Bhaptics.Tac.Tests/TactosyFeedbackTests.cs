using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bhaptics.Tac.Tests
{
    [TestClass()]
    public class TactosyFeedbackTests
    {
        [TestMethod()]
        public void HapticFeedbackTest()
        {
            var feedback = new HapticFeedback(PositionType.Racket, new List<PathPoint>());
            Debug.WriteLine(feedback);
        }
    }
}