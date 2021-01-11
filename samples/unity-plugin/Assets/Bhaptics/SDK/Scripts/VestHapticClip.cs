using UnityEngine;

namespace Bhaptics.Tact.Unity
{
    public class VestHapticClip : FileHapticClip
    {
        [SerializeField, Range(0f, 360f)] protected float TactFileAngleX;
        [SerializeField, Range(-0.5f, 0.5f)] protected float TactFileOffsetY;





        #region Play method
        public override void Play()
        {
            Play(Intensity, Duration, this.TactFileAngleX, this.TactFileOffsetY, "");
        }

        public override void Play(string identifier)
        {
            Play(Intensity, Duration, this.TactFileAngleX, this.TactFileOffsetY, identifier);
        }

        public override void Play(float intensity, string identifier = "")
        {
            Play(intensity, Duration, this.TactFileAngleX, this.TactFileOffsetY, identifier);
        }

        public override void Play(float intensity, float duration, string identifier = "")
        {
            Play(intensity, duration, this.TactFileAngleX, this.TactFileOffsetY, identifier);
        }

        public override void Play(float intensity, float duration, float vestRotationAngleX, string identifier = "")
        {
            Play(intensity, duration, vestRotationAngleX, this.TactFileOffsetY, identifier);
        }

        public override void Play(float intensity, float duration, float vestRotationAngleX, float vestRotationOffsetY, string identifier = "")
        {
            if (!BhapticsManager.Init)
            {
                BhapticsManager.Initialize();
                //return;
            }

            var hapticPlayer = BhapticsManager.GetHaptic();

            if (hapticPlayer == null)
            {
                return;
            }

            if (!hapticPlayer.IsFeedbackRegistered(assetId))
            {
                hapticPlayer.RegisterTactFileStr(assetId, JsonValue);
            }

            hapticPlayer.SubmitRegistered(assetId, keyId + identifier,
                new RotationOption(
                    vestRotationAngleX + this.TactFileAngleX,
                    vestRotationOffsetY + this.TactFileOffsetY), new ScaleOption(intensity, duration));
        }
        #endregion

        public override void ResetValues()
        {
            base.ResetValues();
            this.TactFileAngleX = 0f;
            this.TactFileOffsetY = 0f;
        }
    }
}
