using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Bhaptics.Tact.Unity;
using UnityEngine;

public class TactSource : MonoBehaviour
{
    private Coroutine currentCoroutine, loopCoroutine;

    private bool isLooping = false;

    public bool playOnAwake = false;
    public bool loop = false;
    public float loopDelaySeconds = 0f;
    public TactClip tactClip;

    public void PlayLoop()
    {
        isLooping = true;
        loopCoroutine = StartCoroutine(PlayLoopCoroutine());
    }

    public void Play()
    {
        PlayTactClip();
    }

    public void PlayDelayed(float delaySecond = 0)
    {
        if (tactClip == null)
        {
            Debug.LogFormat("[bhaptics] tactClip is null.");
            return;
        }

        currentCoroutine = StartCoroutine(PlayCoroutine(delaySecond));
    }

    public void Stop()
    {
        if (loopCoroutine != null)
        {
            isLooping = false;
            StopCoroutine(loopCoroutine);
            loopCoroutine = null;
        }

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        if (tactClip == null)
        {
            return;
        }

        tactClip.Stop();
    }



    private IEnumerator PlayCoroutine(float delaySecond)
    {
        yield return new WaitForSeconds(delaySecond);

        PlayTactClip();
        yield return null;
    }

    private void PlayTactClip()
    {
        if (tactClip == null)
        {
            Debug.LogFormat("tactClip is null");
            return;
        }

        tactClip.Play();
    }

    private IEnumerator PlayLoopCoroutine()
    {
        while (isLooping)
        {
            if (!tactClip.IsPlaying())
            {
                yield return new WaitForSeconds(loopDelaySeconds);
                PlayTactClip();
            }

            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }

    void Awake()
    {
        if (tactClip != null)
        {
            /////// TODO 
            tactClip.instanceId = GetInstanceID() + "";
        }

        if (playOnAwake)
        {
            BhapticsManager.GetHaptic();

            if (loop)
            {
                PlayLoop();
            }
            else
            {
                PlayTactClip();
            }
        }


        var findObjectOfType = FindObjectOfType<Bhaptics_Setup>();
        
        if (findObjectOfType == null)
        {
            var go = new GameObject("[bhaptics]");
            go.SetActive(false);
            var setup = go.AddComponent<Bhaptics_Setup>();

            var config = Resources.Load<BhapticsConfig>("BhapticsConfig");

            if (config == null)
            {
                BhapticsLogger.LogError("Cannot find 'BhapticsConfig' in the Resources folder.");
            }

            setup.Config = config;

            go.SetActive(true);
        }
    }
}
