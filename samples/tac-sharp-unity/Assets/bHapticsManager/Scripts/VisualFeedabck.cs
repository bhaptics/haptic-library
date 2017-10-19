using UnityEngine;
using System.Collections;

namespace Bhaptics.Tac.Unity
{
    public class VisualFeedabck : MonoBehaviour
    {
        [SerializeField] private int column, row;
        [SerializeField] private GameObject motorPrefab;
        [SerializeField] private float distance = 1f;
        public PositionType Position = PositionType.Left;

        private GameObject[] motors;

        void Start()
        {
            if (motorPrefab != null && column > 0 && row > 0)
            {
                motors = new GameObject[column * row];
                for (var r = 0; r < row; r++)
                {
                    for (var c = 0; c < column; c++)
                    {
                        var dot = Instantiate(motorPrefab);
                        dot.transform.parent = transform;
                        dot.transform.localPosition = new Vector3(c * distance, r * distance, 0);
                        motors[(row - r - 1) * column + c] = dot;
                    }
                }

                UpdateFeedbacks(new HapticFeedback(Position, new byte[column * row]));
            }
        }

        public void UpdateFeedbacks(HapticFeedback feedback)
        {
            if (motors == null)
            {
                return;
            }

            for (int i = 0; i < row * column; i++)
            {
                var motor = motors[i];

                if (motor == null)
                {
                    return;
                }
                var scale = feedback.Values[i] / 100f;
                
                motor.transform.localScale = new Vector3(0.2f + (scale * (.8f)), 0.2f + (scale * (.8f)), 1f);

                var rd = motor.GetComponentInChildren<Renderer>();
                if (rd != null)
                {
                    rd.material.color = new Color(0.8f + scale * 0.2f, 0.8f + scale * 0.01f, 0.8f - scale * 0.79f, 0.2f - 0.2f * scale);
                }
            }
        }
    }
}
