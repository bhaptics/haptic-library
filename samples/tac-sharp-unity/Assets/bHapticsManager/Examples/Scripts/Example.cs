using System.Collections;
using System.Collections.Generic;
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
                _player.Submit("handr", PositionType.HandR, new PathPoint(0.5f, 0.5f, 100), 1000);
                _player.Submit("handl", PositionType.HandL, new PathPoint(0.5f, 0.5f, 100), 1000);
                _player.Submit("footr", PositionType.FootR, new PathPoint(0.5f, 0.5f, 100), 1000);
                _player.Submit("footl", PositionType.FootL, new PathPoint(0.5f, 0.5f, 100), 1000);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(Explode(PositionType.HandR, 0.5f, 0.5f));
                StartCoroutine(Explode(PositionType.HandL, 0.5f, 0.5f));
                StartCoroutine(Explode(PositionType.FootR, 0.5f, 0.5f));
                StartCoroutine(Explode(PositionType.FootL, 0.5f, 0.5f));
            }
        }

        IEnumerator Explode(PositionType type, float x, float y)
        {

            float offset = 0.0001f;
            int fade = 10;

            int intentity = 100;
            for (int i = 0; i < 5; i++)
            {
                List<PathPoint> explosion = GetFireballPoint(x, y, offset, intentity, fade);

                yield return new WaitForSeconds(0.05f);
                fade += 20;
                _player.Submit("Explode" + type, type, explosion, 80);
                offset += .2f;
            }
        }

        List<PathPoint> GetFireballPoint(
            float pX, float pY, float offset, int intentity, int fade)
        {
            List<PathPoint> explosion = new List<PathPoint>();
            explosion.Add(new PathPoint(pX, pY, intentity));
            explosion.Add(new PathPoint(pX + offset, pY + offset, intentity - fade));
            explosion.Add(new PathPoint(pX - offset, pY + offset, intentity - fade));
            explosion.Add(new PathPoint(pX - offset, pY - offset, intentity - fade));
            explosion.Add(new PathPoint(pX + offset, pY - offset, intentity - fade));
            explosion.Add(new PathPoint(pX + offset, pY, intentity - fade));
            explosion.Add(new PathPoint(pX, pY + offset, intentity - fade));
            explosion.Add(new PathPoint(pX - offset, pY, intentity - fade));
            explosion.Add(new PathPoint(pX, pY - offset, intentity - fade));


            foreach (var pathPoint in explosion)
            {
                if (pathPoint.X > 1f)
                {
                    pathPoint.X = 1;
                }
                else if (pathPoint.X < 0)
                {
                    pathPoint.X = 0;
                }
                if (pathPoint.Y > 1f)
                {
                    pathPoint.Y = 1;
                }
                else if (pathPoint.Y < 0)
                {
                    pathPoint.Y = 0;
                }
            }

            return explosion;
        }


    }
}
