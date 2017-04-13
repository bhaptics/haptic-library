using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signal
{
    public int intervalMillis { get; set; }
    public int size { get; set; }
    public int durationMillis { get; set; }
    public Feedback feedback { get; set; }

    public class Feedback
    {
    }

    public class FeedbackInfo
    {
        public string position { get; set; }
        public string mode { get; set; }
        public List<int> values { get; set; }
    }
    
}

