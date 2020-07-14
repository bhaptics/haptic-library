using System;
using System.Collections;
using UnityEngine;

namespace Bhaptics.Tact.Unity
{
    public class HapticSource : MonoBehaviour
    {
        private Coroutine currentCoroutine, loopCoroutine;

        private bool isLooping = false;

        public bool playOnAwake = false;
        public bool loop = false;
        public float loopDelaySeconds = 0f;
        public HapticClip clip;

        public void PlayLoop()
        {
            if (clip == null)
            {
                BhapticsLogger.LogInfo("clip is null.");
                return;
            }

            isLooping = true;

            loopCoroutine = StartCoroutine(PlayLoopCoroutine());
        }

        public void Play()
        {
            PlayTactClip();
        }

        public void PlayDelayed(float delaySecond = 0)
        {
            if (clip == null)
            {
                BhapticsLogger.LogInfo("clip is null.");
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

            if (clip == null)
            {
                return;
            }

            clip.Stop();
        }


        public bool IsPlaying()
        {
            if (clip == null)
            {
                return false;
            }

            return clip.IsPlaying();
        }


        private IEnumerator PlayCoroutine(float delaySecond)
        {
            yield return new WaitForSeconds(delaySecond);

            PlayTactClip();
            yield return null;
        }

        private void PlayTactClip()
        {
            if (clip == null)
            {
                BhapticsLogger.LogInfo("clip is null");
                return;
            }

            clip.Play();
        }

        private IEnumerator PlayLoopCoroutine()
        {
            while (isLooping)
            {
                if (!clip.IsPlaying())
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
            if (clip != null)
            {
                clip.keyId = Guid.NewGuid().ToString();
            }

            BhapticsManager.GetHaptic();



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

            if (playOnAwake)
            {

                if (loop)
                {
                    PlayLoop();
                }
                else
                {
                    PlayTactClip();
                }
            }
        }
    }

}
