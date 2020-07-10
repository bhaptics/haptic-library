using Bhaptics.Tact.Unity;
using UnityEngine;

public class BhapticsManager
{
    private static IHaptic Haptic;

    public static bool Init = false;


    public static IHaptic GetHaptic()
    {
        if (Haptic == null)
        {
            Init = true;
            if (Application.platform == RuntimePlatform.Android)
            {
                BhapticsLogger.LogInfo("Android initialized.");
                Haptic = new AndroidHaptic();
            }
            else
            {
                BhapticsLogger.LogInfo("Initialized.");
                Haptic = new BhapticsHaptic();
            }
        }

        return Haptic;
    }

    public static void Initialize()
    {
        GetHaptic();
    }

    public static void Dispose()
    {
        if (Haptic != null)
        {
            Init = false;
            Haptic.TurnOff();
            BhapticsLogger.LogInfo("Dispose() bHaptics plugin.");
            Haptic.Dispose();
            Haptic = null;
        }
    }
}
