using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bhaptics.Tact.Unity;


public class BhapticsHapticClipExample : MonoBehaviour
{
    public HapticClip hapticClip;







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
