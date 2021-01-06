using Bhaptics.Tact.Unity;
using UnityEngine;

public class BhapticsVisualizer : MonoBehaviour
{
    private VisualFeedback[] visualFeedback;




    void Awake()
    {
        visualFeedback = GetComponentsInChildren<VisualFeedback>();
    }

    void Update()
    {
        var hapticPlayer = BhapticsManager.GetHaptic();

        if (hapticPlayer == null)
        {
            return;
        }

        foreach (var vis in visualFeedback)
        {
            var feedback = hapticPlayer.GetCurrentFeedback(BhapticsUtils.ToPositionType(vis.devicePos));
    
            vis.UpdateFeedback(feedback);
        }
    }
}