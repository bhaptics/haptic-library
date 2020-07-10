using System;
using System.Collections;
using System.Collections.Generic;
using Bhaptics.Tact.Unity;
using UnityEngine;

namespace Bhaptics.Tact.Unity
{
    public class HapticClip : ScriptableObject
    {
        [NonSerialized] protected string assetId = System.Guid.NewGuid().ToString();

        [NonSerialized] public string keyId = System.Guid.NewGuid().ToString();


        public virtual void Play()
        {
            Play(1f, 1f, 0f, 0f);
        }

        public virtual void Play(float intensity)
        {
            Play(intensity, 1f, 0f, 0f);
        }

        public virtual void Play(float intensity, float duration)
        {
            Play(intensity, duration, 0f, 0f);
        }

        public virtual void Play(float intensity, float duration, float vestRotationAngleX)
        {
            Play(intensity, duration, vestRotationAngleX, 0f);
        }

        public virtual void Play(Vector3 contactPos, Collider targetCollider)
        {
            Play(contactPos, targetCollider.bounds.center, targetCollider.transform.forward,
                targetCollider.bounds.size.y);
        }

        public virtual void Play(Vector3 contactPos, Vector3 targetPos, Vector3 targetForward, float targetHeight)
        {
            Vector3 targetDir = contactPos - targetPos;
            var angle = BhapticsUtils.Angle(targetDir, targetForward);
            var offsetY = (contactPos.y - targetPos.y) / targetHeight;

            Play(1f, 1f, angle, offsetY);
        }

        public virtual void Play(float intensity, float duration, float vestRotationAngleX, float vestRotationOffsetY)
        {
        }

        public virtual void Stop()
        {
            var haptic = BhapticsManager.GetHaptic();
            haptic.TurnOff();
        }

        public virtual bool IsPlaying()
        {
            var haptic = BhapticsManager.GetHaptic();
            return haptic.IsPlaying(keyId);
        }

        public virtual void ResetValues()
        {

        }
    }
}
