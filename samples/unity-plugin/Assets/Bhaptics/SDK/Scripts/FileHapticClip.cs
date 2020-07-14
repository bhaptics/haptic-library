using System;
using UnityEngine;


namespace Bhaptics.Tact.Unity
{
    public enum HapticClipType
    {
        None,
        Tactal, Tactot, Tactosy_arms, Tactosy_hands, Tactosy_feet
    }

    public class FileHapticClip : HapticClip
    {
        [Range(0.2f, 5f)] public float Intensity = 1f;
        [Range(0.2f, 5f)] public float Duration = 1f;

        public HapticClipType ClipType;
        
        // [HideInInspector] 
        // public string Name;
        // [HideInInspector] 
        public string JsonValue;


        public override void Play()
        {
            Play(Intensity, Duration, 0f, 0f);
        }

        public override void Play(float intensity)
        {
            Play(intensity, Duration, 0f, 0f);
        }

        public override void Play(float intensity, float duration)
        {
            Play(intensity, duration, 0f, 0f);
        }

        public override void Play(float intensity, float duration, float vestRotationAngleX)
        {
            Play(intensity, duration, vestRotationAngleX, 0f);
        }

        public override void Play(Vector3 contactPos, Collider targetCollider)
        {
            Play(contactPos, targetCollider.bounds.center, targetCollider.transform.forward, targetCollider.bounds.size.y);
        }

        public override void Play(Vector3 contactPos, Vector3 targetPos, Vector3 targetForward, float targetHeight)
        {
            Vector3 targetDir = contactPos - targetPos;
            var angle = BhapticsUtils.Angle(targetDir, targetForward);
            var offsetY = (contactPos.y - targetPos.y) / targetHeight;

            Play(this.Intensity, this.Duration, angle, offsetY);
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

            haptic.SubmitRegistered(assetId, keyId, new ScaleOption(intensity, duration));
        }

        public override void Stop()
        {
            var haptic = BhapticsManager.GetHaptic();
            haptic.TurnOff();
        }

        public override bool IsPlaying()
        {
            var haptic = BhapticsManager.GetHaptic();
            return haptic.IsPlaying(keyId);
        }

        public override void ResetValues()
        {
            base.ResetValues();
            Intensity = 1f;
            Duration = 1f;
        }
    }
}