using System;
using System.Collections;
using Tactosy.Common;
using UnityEngine;

namespace Tactosy.Unity
{
    public class UnityTimer : MonoBehaviour, ITimer
    {
        private int _interval = 20;
        public event EventHandler Elapsed;
        private Coroutine timerCoroutine;

        void OnTimeUpdate()
        {
            if (Elapsed != null)
            {
                Elapsed(this, new EventArgs());
            }
        }

        IEnumerator StageTransition()
        {
            while (true)
            {

                if (Elapsed != null)
                {
                    try
                    {
                        Elapsed(this, new EventArgs());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                yield return new WaitForSeconds(_interval*0.001f);
            }
        }

        public void StartTimer()
        {
            if (timerCoroutine != null)
            {
                return;
            }

            timerCoroutine = StartCoroutine(StageTransition());
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
