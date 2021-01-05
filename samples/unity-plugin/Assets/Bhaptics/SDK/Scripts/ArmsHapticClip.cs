using Bhaptics.Tact;
using Bhaptics.Tact.Unity;

public class ArmsHapticClip : FileHapticClip
{
    public bool IsReflect;


    public override void Play(float intensity, float duration, float vestRotationAngleX, float vestRotationOffsetY)
    {
        if (!BhapticsManager.Init)
        {
            BhapticsManager.Initialize();
            return;
        }

        var hapticPlayer = BhapticsManager.GetHaptic();

        if (hapticPlayer == null)
        {
            return;
        }

        if (IsReflect)
        {
            var reflectIdentifier = assetId + "Reflect";

            if (!hapticPlayer.IsFeedbackRegistered(reflectIdentifier))
            {
                hapticPlayer.RegisterTactFileStrReflected(reflectIdentifier, JsonValue);
            }

            hapticPlayer.SubmitRegistered(reflectIdentifier, keyId, new ScaleOption(intensity, duration));
        }
        else
        {
            if (!hapticPlayer.IsFeedbackRegistered(assetId))
            {
                hapticPlayer.RegisterTactFileStr(assetId, JsonValue);
            }

            hapticPlayer.SubmitRegistered(assetId, keyId, new ScaleOption(intensity, duration));
        }
    }

    public override void ResetValues()
    {
        base.ResetValues();
        IsReflect = false;
    }

}

