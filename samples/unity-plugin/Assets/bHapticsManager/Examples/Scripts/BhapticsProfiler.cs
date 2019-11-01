using System.Collections;
using System.Collections.Generic;
using Bhaptics.Tact.Unity;
using UnityEngine;

namespace Bhaptics.Tact.Unity
{
    public class BhapticsProfiler : MonoBehaviour
    {

        [SerializeField] private int numOfTactSource = 1;

        public BhapticsManager manager;

        public GameObject tactSources;

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
            else
            {
                if (manager != null)
                {
                    manager.gameObject.SetActive(false);
                }
            }
        }


        private void TriggerPlay()
        {
            for (int i = 0; i < numOfTactSource; i++)
            {
                if (instantiate == null)
                {
                    instantiate = Instantiate(this.tactSources);

                }


                var sources = instantiate.GetComponents<TactSource>();
                foreach (var tactSource in sources)
                {
                    tactSource.SetKey(tactSource.FeedbackType + "-" + tactSource.FeedbackFile.Id + i);
                    tactSource.Play();
                }
            }
        }

    }
}
