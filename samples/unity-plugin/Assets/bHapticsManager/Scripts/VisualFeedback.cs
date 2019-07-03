using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Bhaptics.Tact.Unity
{
    public class VisualFeedback : MonoBehaviour
    {
        public PositionType Position = PositionType.Left;
        [SerializeField] private GameObject motorPrefab;
        [SerializeField] private int column, row;
        [SerializeField] private float intervalDistance = 0.4f;



        private GameObject[] motors;



        [SerializeField] private float offsetX;
        [SerializeField] private float offsetY;




        void Start()
        {
            if (motorPrefab != null && column > 0 && row > 0)
            {
                float c_distance, r_distance;
                c_distance = intervalDistance;
                r_distance = intervalDistance;
                if (gameObject.tag == "Racket")
                {
                    c_distance += 0.2f;
                }
                else if (gameObject.tag == "Shooes")
                {
                    c_distance -= 0.1f;
                }
                motors = new GameObject[column * row];
                var motorsGameObject = new GameObject("Motors");
                motorsGameObject.transform.parent = transform;
                motorsGameObject.transform.localPosition = Vector3.zero;
                motorsGameObject.transform.localRotation = Quaternion.identity;
                motorsGameObject.transform.localScale = Vector3.one;

                for (var r = 0; r < row; r++)
                {
                    for (var c = 0; c < column; c++)
                    {
                        var dot = (GameObject)Instantiate(motorPrefab, Vector3.zero, Quaternion.identity);
                        dot.transform.parent = motorsGameObject.transform;
                        dot.transform.localPosition = new Vector3(offsetX + c * c_distance, offsetY + r * r_distance, 0f);
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

                motor.transform.localScale = new Vector3(0.2f + (scale * (.25f)), 0.2f + (scale * (.25f)), 0.2f + (scale * (.25f)));

                var im = motor.GetComponent<Image>();
                if (im != null)
                {
                    im.color = new Color(0.8f + scale * 0.2f, 0.8f + scale * 0.01f, 0.8f - scale * 0.79f, 1f);
                }
            }
        }
    }
}