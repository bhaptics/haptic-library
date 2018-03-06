using UnityEngine;

namespace Bhaptics.Tac.Unity
{
    public class Example : MonoBehaviour
    {
        public TactSource[] Sources;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                foreach (var tactSource in Sources)
                {
                    tactSource.Play();
                }
            }
        }
    }
}
