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

            InvokeRepeating("TriggerPlay", 1f, 4f);
        }


        private void TriggerPlay()
        {
            if (hapticEnable)
            {
                BhapticsLogger.LogInfo("TriggerPlay");
                for (int i = 0; i < numOfTactSource; i++)
                {
                    foreach (var tactSource in tactClips)
                    {
                        tactSource.Identifier = tactSource + ("TEST " + i);
                        tactSource.Play();
                    }
                }
            }
        }

    }
}
