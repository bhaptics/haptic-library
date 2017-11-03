using System.Collections.Generic;

namespace Bhaptics.Tac.Designer
{
    public class Layout
    {
        public string Type { get; set; }
        public Dictionary<string, LayoutObject[]> Layouts { get; set; }
    }

    public class LayoutObject
    {
        public int Index { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }
}
