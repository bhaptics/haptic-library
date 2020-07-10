using System.Collections;
using System.Collections.Generic;
using Bhaptics.Tact;
using Bhaptics.Tact.Unity;
using UnityEngine;

namespace Bhaptics.Tact.Unity
{
    public class VestHapticClip : FileHapticClip
    {
        [Range(0f, 360f)] public float VestRotationAngleX;
        [Range(-0.5f, 0.5f)] public float VestRotationOffsetY;
        [Range(0f, 360f)] public float TactFileAngleX;
        [Range(-0.5f, 0.5f)] public float TactFileOffsetY;

        public override void Play()
        {
            Play(Intensity, Duration, VestRotationAngleX, VestRotationOffsetY);
        }

        public override void Play(float intensity)
        {
            Play(intensity, Duration, VestRotationAngleX, VestRotationOffsetY);
        }

        public override void Play(float intensity, float duration)
        {
            Play(intensity, duration, this.VestRotationAngleX, this.VestRotationOffsetY);
        }

        public override void Play(float intensity, float duration, float vestRotationAngleX)
        {
            Play(intensity, duration, vestRotationAngleX, this.VestRotationOffsetY);
        }

        public override void Play(float intensity, float duration, float vestRotationAngleX, float vestRotationOffsetY)
        {
            if (!BhapticsManager.Init)
            {
                BhapticsManager.Initialize();
                return;
            }

            var haptic = BhapticsManager.GetHaptic();
            if (!haptic.IsFeedbackRegistered(assetId))
            {
                haptic.RegisterTactFileStr(assetId, JsonValue);
            }

            haptic.SubmitRegistered(assetId, keyId,
                new RotationOption(
                    vestRotationAngleX + TactFileAngleX,
                    vestRotationOffsetY + TactFileOffsetY), new ScaleOption(intensity, duration));

        }

        public override void ResetValues()
        {
            base.ResetValues();
            VestRotationAngleX = 0f;
            VestRotationOffsetY = 0f;
            TactFileAngleX = 0f;
            TactFileOffsetY = 0f;
        }

    }
}
