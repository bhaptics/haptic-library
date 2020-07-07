using System.Collections;
using System.Collections.Generic;
using Bhaptics.Tact.Unity;
using UnityEngine;

namespace Bhaptics.Tact.Unity
{
    public class BhapticsProfiler : MonoBehaviour
    {

        [SerializeField] private int numOfTactSource = 1;

        public TactClip[] tactClips;

        private GameObject instantiate;

        public bool hapticEnable;
        public int targetFrameRate = 60;

        void Awake()
        {
            Application.targetFrameRate = targetFrameRate;

            if (hapticEnable)
            {
                InvokeRepeating("TriggerPlay", 1f, 4f);
            }
        }


        private void TriggerPlay()
        {
            for (int i = 0; i < numOfTactSource; i++)
            {
                foreach (var tactSource in tactClips)
                {
                    tactSource.Identifier = ("TEST "+ i);
                    tactSource.Play();
                }
            }
        }

    }
}
