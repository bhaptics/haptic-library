using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Bhaptics.Tact.Unity;


public class BhapticsHapticClipExample : MonoBehaviour
{
    public HapticClip hapticClip;
    public Image isEnabledImage;


    void Update()
    {
        if (isEnabledImage != null)
        {
            isEnabledImage.color = BhapticsManager.Init ? Color.green : Color.red;
        }
    }


    public void Enable()
    {
        BhapticsManager.Initialize();
    }

    public void Disable()
    {
        BhapticsManager.Dispose();
    }

    public void Play()
    {
        if (hapticClip != null)
        {
            hapticClip.Play();
        }
    }

    public void Stop()
    {
        if (hapticClip != null)
        {
            hapticClip.Stop();
        }
    }
}