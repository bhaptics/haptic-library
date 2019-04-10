using UnityEngine;
using System.Collections;

namespace Bhaptics.Tact.Unity
{
    public class ExamplePrev : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                BhapticsManager.HapticPlayer.SubmitRegistered(BhapticsManager.GetFeedbackId("BowShoot"));
            }
        }
    }
}
