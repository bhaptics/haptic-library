using UnityEngine;
using UnityEngine.Assertions;


namespace Bhaptics.Tact.Unity
{
    public enum TactClipType
    {
        None,
        Tactal, Tactot, Tactosy_arms, Tactosy_hands, Tactosy_feet
    }

    public class TactClip : ScriptableObject
    {
        [Range(0.2f, 5f)] public float Intensity = 1f;
        [Range(0.2f, 5f)] public float Duration = 1f;
        public bool IsReflectTactosy;
        [Range(0f, 360f)] public float VestRotationAngleX;
        [Range(-0.5f, 0.5f)] public float VestRotationOffsetY;
        [Range(0f, 360f)] public float TactFileAngleX;
        [Range(-0.5f, 0.5f)] public float TactFileOffsetY;

        public TactClipType ClipType;
        

        // [HideInInspector] 
        public string Identifier;
        // [HideInInspector] 
        public string Name;
        // [HideInInspector] 
        public string JsonValue;
        // [HideInInspector] 
        public string Path;

        public string currentIdentifier = "";

        public string instanceId = "";




        public void Play()
        {
            Play(Intensity, Duration, VestRotationAngleX, VestRotationOffsetY);
        }

        public void Play(float intensity)
        {
            Play(intensity, Duration, VestRotationAngleX, VestRotationOffsetY);
        }

        public void Play(float intensity, float duration)
        {
            Play(intensity, duration, this.VestRotationAngleX, this.VestRotationOffsetY);
        }

        public void Play(float intensity, float duration, float vestRotationAngleX)
        {
            Play(intensity, duration, vestRotationAngleX, this.VestRotationOffsetY);
        }

        public void Play(Vector3 contactPos, Collider targetCollider)
        {
            Play(contactPos, targetCollider.bounds.center, targetCollider.transform.forward, targetCollider.bounds.size.y);
        }

        public void Play(Vector3 contactPos, Vector3 targetPos, Vector3 targetForward, float targetHeight)
        {
            Vector3 targetDir = contactPos - targetPos;
            var angle = BhapticsUtils.Angle(targetDir, targetForward);
            var offsetY = (contactPos.y - targetPos.y) / targetHeight;

            Play(this.Intensity, this.Duration, angle, offsetY);
        }

        public void Play(float intensity, float duration, float vestRotationAngleX, float vestRotationOffsetY)
        {
            if (!BhapticsManager.Init)
            {
                BhapticsManager.Initialize();
                return;
            }

            var haptic = BhapticsManager.GetHaptic();
            if (!haptic.IsFeedbackRegistered(currentIdentifier))
            {
                currentIdentifier = Identifier;
                haptic.RegisterTactFileStr(currentIdentifier, JsonValue);
            }
            
            if (ClipType == TactClipType.Tactot)
            {
                haptic.SubmitRegistered(currentIdentifier, instanceId,
                    new RotationOption(vestRotationAngleX + TactFileAngleX, vestRotationOffsetY + TactFileOffsetY), new ScaleOption(intensity, duration));
            }
            else if (IsReflectTactosy && (ClipType == TactClipType.Tactosy_arms || ClipType == TactClipType.Tactosy_hands || ClipType == TactClipType.Tactosy_feet))
            {
                var reflectIdentifier = Identifier + "Reflect";

                if (!haptic.IsFeedbackRegistered(reflectIdentifier))
                {
                    haptic.RegisterTactFileStrReflected(reflectIdentifier, JsonValue);
                }

                haptic.SubmitRegistered(reflectIdentifier, instanceId, new ScaleOption(intensity, duration));
            }
            else
            {
                haptic.SubmitRegistered(currentIdentifier, instanceId, new ScaleOption(intensity, duration));
            }
        }

        public void Stop()
        {
            var haptic = BhapticsManager.GetHaptic();
            haptic.TurnOff();
        }

        public bool IsPlaying()
        {
            var haptic = BhapticsManager.GetHaptic();
            return haptic.IsPlaying(instanceId);
        }

        public void ResetValues()
        {
            Intensity = 1f;
            Duration = 1f;
            IsReflectTactosy = false;
            VestRotationAngleX = 0f;
            VestRotationOffsetY = 0f;
            TactFileAngleX = 0f;
            TactFileOffsetY = 0f;
        }
    }
}