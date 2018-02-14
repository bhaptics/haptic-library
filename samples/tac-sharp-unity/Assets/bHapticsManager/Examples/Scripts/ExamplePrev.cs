using UnityEngine;
using System.Collections;

namespace Bhaptics.Tac.Unity
{
    public class ExamplePrev : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                BhapticsManager.HapticPlayer.SubmitRegistered(BhapticsManager.GetFeedbackId("BowShoot"));
            }
        }
    }
}
