using Bhaptics.Tact;
using Bhaptics.Tact.Unity;

public class TactosyTactClip : TactFileClip
{
    public bool IsReflectTactosy;


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

        else if (IsReflectTactosy && (ClipType == TactClipType.Tactosy_arms || ClipType == TactClipType.Tactosy_hands || ClipType == TactClipType.Tactosy_feet))
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
            haptic.SubmitRegistered(assetId, keyId, new ScaleOption(intensity, duration));
        }
    }

    public override void ResetValues()
    {
        base.ResetValues();
        IsReflectTactosy = false;
    }

}

