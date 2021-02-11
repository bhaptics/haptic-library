using Bhaptics.Tact.Unity;
using UnityEngine;
using UnityEngine.UI;


public class BhapticsTestScript : MonoBehaviour
{
    [SerializeField] private HapticClip[] tactClips;


    public Slider intensitySlider;
    public Slider durationSlider;
    public Slider offsetXSlider;
    public Slider offsetYSlider;

    public Toggle reflectToggle;

    public void Start()
    {
    }

    public void Play()
    {
        foreach (var hapticClip in tactClips)
        {
            var fileHapticClip = hapticClip as ArmsHapticClip;
            if (fileHapticClip != null)
            {
                fileHapticClip.IsReflect = reflectToggle.isOn;
            }

            hapticClip.Play(intensitySlider.value, durationSlider.value, offsetXSlider.value, offsetYSlider.value);
        }
    }
}