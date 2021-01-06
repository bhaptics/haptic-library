using UnityEngine;

namespace Bhaptics.Tact.Unity
{
    public class BhapticsIsActiveIsPlayingExample : MonoBehaviour
    {
        public HapticClip[] Sources;

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
