using UnityEngine;

namespace Bhaptics.Tac.Unity
{
    public class Example : MonoBehaviour
    {
        public TactSource[] Sources;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                foreach (var tactClip in Sources)
                {
                    tactClip.Play();
                }
            }
        }
    }
}
