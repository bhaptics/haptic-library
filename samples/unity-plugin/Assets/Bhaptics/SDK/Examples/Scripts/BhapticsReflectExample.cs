using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bhaptics.Tact.Unity;


public class BhapticsReflectExample : MonoBehaviour
{
    public ArmsHapticClip armsHapticClip;


    private bool isReflect;



    public void Play()
    {
        if (armsHapticClip != null)
        {
            armsHapticClip.Play();
        }
    }

    public void Stop()
    {
        if (armsHapticClip != null)
        {
            armsHapticClip.Stop();
        }
    }

    public void OnClickToggle(UnityEngine.UI.Toggle toggle)
    {
        if (toggle != null)
        {
            isReflect = toggle.isOn;
        }

        if (armsHapticClip != null)
        {
            armsHapticClip.IsReflect = isReflect;
        }
    }
}