using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Bhaptics.Tact.Unity;


public class BhapticsRotateExample : MonoBehaviour
{
    public VestHapticClip vestHapticClip;
    [Space]
    public Text angleXText;
    public Text offsetYText;



    private float angleX = 0f, offsetY = 0f;





    public void PlayWithRotate()
    {
        if (vestHapticClip != null)
        {
            vestHapticClip.Play(vestHapticClip.Intensity, vestHapticClip.Duration, angleX, offsetY);
        }
    }

    public void Stop()
    {
        if (vestHapticClip != null)
        {
            vestHapticClip.Stop();
        }
    }

    public void OnAngleXChanged(Slider slider)
    {
        if (slider != null)
        {
            angleX = slider.value * 360f;
            angleXText.text = Mathf.RoundToInt(angleX).ToString();
        }
    }

    public void OnOffsetYChanged(Slider slider)
    {
        if (slider != null)
        {
            offsetY = slider.value - 0.5f;
            offsetYText.text = (Mathf.RoundToInt(offsetY * 100f) / 100f).ToString();
        }
    }
}
