using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tactosy.Common.Tests
{
    [TestClass()]
    public class TactosyFeedbackTests
    {
        [TestMethod()]
        public void TactosyFeedbackTest()
        {
            byte[] bytes = new byte[22];
            bytes[1] = 4;
            TactosyFeedback feedback = new TactosyFeedback(bytes);

            Debug.WriteLine(feedback);
        }
    }
}