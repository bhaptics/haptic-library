using System.Collections;
using System.Collections.Generic;
using Bhaptics.Tact.Unity;
using UnityEngine;


public class BhapticsProfiler : MonoBehaviour
{
    [SerializeField] private int numOfTactClips = 1;

    public HapticClip[] tactClips;

    public bool hapticEnable;
    public int targetFrameRate = 60;

    void Awake()
    {
        Application.targetFrameRate = targetFrameRate;

        InvokeRepeating("TriggerPlay", 1f, 4f);
    }


    private void TriggerPlay()
    {
        if (hapticEnable)
        {
            BhapticsLogger.LogInfo("TriggerPlay");
            for (int i = 0; i < numOfTactClips; i++)
            {
                foreach (var clip in tactClips)
                {
                    clip.keyId = System.Guid.NewGuid().ToString() + i;
                    clip.Play();
                }
            }
        }
    }
}