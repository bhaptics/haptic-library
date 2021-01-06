using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Bhaptics.Tact.Unity
{
    public class VisualFeedback : MonoBehaviour
    {
        public Pos devicePos = Pos.Head;
        public Transform motorContainer;
        public Gradient motorFeedbackGradient;


        private Transform[] motors;
        private float motorScaleMultiplier = 1.5f;




        void Start()
        {
            if (motorContainer == null)
            {
                BhapticsLogger.LogError("VisualFeedback.cs - Start() / motorContainer is null");

                return;
            }

            var tmpMotorContainer = Instantiate(motorContainer, motorContainer.parent);
            tmpMotorContainer.localPosition = motorContainer.localPosition;
            tmpMotorContainer.localRotation = motorContainer.localRotation;

            var tmpList = new List<Transform>();

            for (int i = 0; i < motorContainer.childCount; ++i)
            {
                tmpList.Add(motorContainer.GetChild(i));
            }

            motors = tmpList.ToArray();

            UpdateFeedback(new HapticFeedback(BhapticsUtils.ToPositionType(devicePos), new byte[motors.Length]));
        }






        public void UpdateFeedback(HapticFeedback feedback)
        {
            if (motors == null)
            {
                return;
            }

            for (int i = 0; i < motors.Length; i++)
            {
                var motor = motors[i];

                if (motor == null)
                {
                    return;
                }
                var scale = feedback.Values[i] / 100f;

                motor.transform.localScale = Vector3.one * scale * motorScaleMultiplier;

                var motorImage = motor.GetComponent<Image>();

                if (motorImage != null)
                {
                    motorImage.color = motorFeedbackGradient.Evaluate(scale);
                }
            }
        }

        public void UpdateFeedback(int[] feedbackValues)
        {
            if (motors == null)
            {
                return;
            }
            for (int i = 0; i < motors.Length; i++)
            {
                var motor = motors[i];

                if (motor == null)
                {
                    return;
                }

                var scale = feedbackValues[i] / 100f;

                motor.transform.localScale = Vector3.one * scale * motorScaleMultiplier;

                var motorImage = motor.GetComponent<Image>();

                if (motorImage != null)
                {
                    motorImage.color = motorFeedbackGradient.Evaluate(scale);
                }
            }
        }
    }
}