using UnityEngine;
using System.Collections;

namespace Bhaptics.Tac.Unity
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
