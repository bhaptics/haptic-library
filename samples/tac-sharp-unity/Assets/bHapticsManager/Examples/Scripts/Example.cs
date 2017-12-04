using UnityEngine;

namespace Bhaptics.Tac.Unity
{
    public class Example : MonoBehaviour
    {
        [SerializeField]
        private BhapticsManager _manager;
        private HapticPlayer _player;

        private int _motorIndex = 0;
        
        void Update()
        {
            if (_manager == null)
            {
                Debug.LogError("BhapticsManger is not set");
                return;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                BhapticsManager.HapticPlayer.Submit("FootL", PositionType.FootL, new DotPoint(_motorIndex, 100), 1000);
                BhapticsManager.HapticPlayer.Submit("FootR", PositionType.FootR, new DotPoint(_motorIndex, 100), 1000);
                BhapticsManager.HapticPlayer.Submit("left", PositionType.Left, new DotPoint(_motorIndex, 100), 1000);
                BhapticsManager.HapticPlayer.Submit("right", PositionType.Right, new DotPoint(_motorIndex, 100), 1000);
                BhapticsManager.HapticPlayer.Submit("head", PositionType.Head, new DotPoint(_motorIndex, 100), 1000);
                BhapticsManager.HapticPlayer.Submit("vestfront", PositionType.VestFront, new DotPoint(_motorIndex, 100), 1000);
                BhapticsManager.HapticPlayer.Submit("vestback", PositionType.VestBack, new DotPoint(_motorIndex, 100), 1000);
                BhapticsManager.HapticPlayer.Submit("racket", PositionType.Racket, new DotPoint(_motorIndex, 100), 1000);
                _motorIndex++;
                if (_motorIndex >= 20)
                {
                    _motorIndex = 0;
                }
            }
        }
    }
}
