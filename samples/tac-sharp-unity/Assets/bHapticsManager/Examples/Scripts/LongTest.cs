using UnityEngine;

namespace Bhaptics.Tac.Unity
{
    public class LongTest : MonoBehaviour
    {
        private IHapticPlayer _player;

        // Use this for initialization
        void Start()
        {
            _player = BhapticsManager.HapticPlayer;
            InvokeRepeating("TriggerBowShoot", .1f, 3f);
            InvokeRepeating("TriggerElectricFront", 1f, 3f);
        }

        void TriggerBowShoot()
        {
            _player.SubmitRegistered("BowShoot");
        }

        void TriggerElectricFront()
        {
            _player.SubmitRegistered("ElectricFront");
        }
    }
}