using System;
using System.Collections;
using UnityEngine;

namespace Bhaptics.Tac.Unity
{
    
    public class UnityTimer : MonoBehaviour, ITimer
    {
        private float _interval = 0.02f;
        public event EventHandler Elapsed;
        private Coroutine timerCoroutine;
        private float time = 0f;
        private int count;

        private int maxCountOnce = 5;

        void OnEnable()
        {
            time = 0;
            count = 0;
        }

        void OnDisable()
        {
            time = 0;
            count = 0;
        }

        private IEnumerator TimerLoop()
        {
            while (true)
            {
                if (time > 5f)
                {
                    time = 0;
                    count = 0;
                }

                time += Time.deltaTime;

                var expectedCount = (int) (time / _interval);
                var repeatCount = 0;
                while (count < expectedCount)
                {
                    if (Elapsed != null)
                    {
                        try
                        {
                            Elapsed(this, new EventArgs());
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e);
                        }
                    }
                    count++;
                    repeatCount++;
                    
                    if (repeatCount > maxCountOnce)
                    {
                        time = 0;
                        count = 0;
                        break;
                    }
                }

                yield return null;
            }
        }

        public void StartTimer()
        {
            if (timerCoroutine != null)
            {
                return;
            }
            time = 0;
            count = 0;
            timerCoroutine = StartCoroutine(TimerLoop());
        }

        public void StopTimer()
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }


        public void Dispose()
        {
            StopTimer();
        }
    }
}
