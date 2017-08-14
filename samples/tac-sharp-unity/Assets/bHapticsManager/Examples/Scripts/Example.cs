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
                _manager.HapticPlayer().Submit("left", PositionType.Left, new DotPoint(_motorIndex, 100), 1000);
                _manager.HapticPlayer().Submit("right", PositionType.Right, new DotPoint(_motorIndex, 100), 1000);
                _manager.HapticPlayer().Submit("head", PositionType.Head, new DotPoint(_motorIndex, 100), 1000);
                _manager.HapticPlayer().Submit("vestfront", PositionType.VestFront, new DotPoint(_motorIndex, 100), 1000);
                _manager.HapticPlayer().Submit("vestback", PositionType.VestBack, new DotPoint(_motorIndex, 100), 1000);
                _manager.HapticPlayer().Submit("racket", PositionType.Racket, new DotPoint(_motorIndex, 100), 1000);
                _motorIndex++;
                if (_motorIndex >= 20)
                {
                    _motorIndex = 0;
                }
            }
        }
    }
}
