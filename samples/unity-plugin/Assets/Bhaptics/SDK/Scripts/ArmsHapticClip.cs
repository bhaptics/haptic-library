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

        var haptic = BhapticsManager.GetHaptic();

        if (IsReflect)
        {
            var reflectIdentifier = assetId + "Reflect";

            if (!haptic.IsFeedbackRegistered(reflectIdentifier))
            {
                haptic.RegisterTactFileStrReflected(reflectIdentifier, JsonValue);
            }

            haptic.SubmitRegistered(reflectIdentifier, keyId, new ScaleOption(intensity, duration));
        }
        else
        {
            if (!haptic.IsFeedbackRegistered(assetId))
            {
                haptic.RegisterTactFileStr(assetId, JsonValue);
            }

            haptic.SubmitRegistered(assetId, keyId, new ScaleOption(intensity, duration));
        }
    }

    public override void ResetValues()
    {
        base.ResetValues();
        IsReflect = false;
    }

}

