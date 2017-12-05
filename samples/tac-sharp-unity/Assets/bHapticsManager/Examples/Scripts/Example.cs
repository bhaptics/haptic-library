using Bhaptics.Tac.Sender;
using UnityEngine;

namespace Bhaptics.Tac.Unity
{
    public class Example : MonoBehaviour
    {
        private IHapticPlayer _player;

        private int _motorIndex = 0;

        void Start()
        {
            _player = BhapticsManager.HapticPlayer;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _player.Submit("FootL", PositionType.FootL, new DotPoint(_motorIndex, 100), 1000);
                _player.Submit("FootR", PositionType.FootR, new DotPoint(_motorIndex, 100), 1000);
                _player.Submit("left", PositionType.Left, new DotPoint(_motorIndex, 100), 1000);
                _player.Submit("right", PositionType.Right, new DotPoint(_motorIndex, 100), 1000);
                _player.Submit("head", PositionType.Head, new DotPoint(_motorIndex, 100), 1000);
                _player.Submit("vestfront", PositionType.VestFront, new DotPoint(_motorIndex, 100), 1000);
                _player.Submit("vestback", PositionType.VestBack, new DotPoint(_motorIndex, 100), 1000);
                _player.Submit("racket", PositionType.Racket, new DotPoint(_motorIndex, 100), 1000);
                _motorIndex++;
                if (_motorIndex >= 20)
                {
                    _motorIndex = 0;
                }
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                _player.SubmitRegistered("BowShoot", new TransformOption(0.5f, .5f));
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                _player.SubmitRegistered("BowShoot", new TransformOption(0.7f, 0f));
            }
        }
    }
}
