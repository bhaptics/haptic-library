using System.Collections.Generic;

namespace Tactosy.Common
{
    /// <summary>
    /// TactosyFile mapping with exported tactosy file from tactosy studio
    /// </summary>
    public class TactosyFile
    {
        public int intervalMillis;
        public int size;
        public int durationMillis;

        public Dictionary<int, TactosyFeedback[]> feedback;
    }
}
