using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bhaptics.Tact;
using Bhaptics.Tact.Unity;
using UnityEngine;

public class BhapticsVisualizer : MonoBehaviour {

    private VisualFeedback[] visualFeedback;

    void Awake()
    {
        visualFeedback = GetComponentsInChildren<VisualFeedback>();
    }

    void Update()
    {
        foreach (var vis in visualFeedback)
        {
            var feedback = BhapticsManager.GetHaptic().GetCurrentFeedback(vis.Position);
            vis.UpdateFeedback(feedback);
        }
    }
}
