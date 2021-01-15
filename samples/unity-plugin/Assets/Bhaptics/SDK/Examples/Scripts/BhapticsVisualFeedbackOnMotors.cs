using UnityEngine;
using Bhaptics.Tact;


public class BhapticsVisualFeedbackOnMotors : MonoBehaviour
{
    [SerializeField] public PositionType tactPositionType;
    [SerializeField] private GameObject visualMotorsObject;
    [SerializeField] private Gradient hapticColor;

    private GameObject[] visualMotors;

    void Start()
    {
        if (visualMotorsObject == null)
        {
            BhapticsLogger.LogError("BhapticsVisualFeedbackOnMotors.cs / visualMotorsObject is null");
            return;
        }
        visualMotors = new GameObject[visualMotorsObject.transform.childCount];
        for (int i = 0; i < visualMotorsObject.transform.childCount; ++i)
        {
            visualMotors[i] = visualMotorsObject.transform.GetChild(i).gameObject;
        }
    }

    void Update()
    {
        var haptic = BhapticsManager.GetHaptic();

        if (haptic != null)
        {
            var feedback = haptic.GetCurrentFeedback(tactPositionType);

            ShowFeedbackEffect(feedback);
        }
    }


    private void ShowFeedbackEffect(int[] feedback)
    {
        if (visualMotors == null)
        {
            return;
        }

        for (int i = 0; i < visualMotors.Length; i++)
        {
            var motor = visualMotors[i];
            var power = feedback[i] / 100f;
            var meshRenderer = motor.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material.color = hapticColor.Evaluate(power);
            }
        }
    }
}
