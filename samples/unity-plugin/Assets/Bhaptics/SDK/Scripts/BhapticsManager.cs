using Bhaptics.Tact.Unity;
using UnityEngine;

public class BhapticsManager
{
    private static IHaptic Haptic;
    private static bool init;

    public static bool Init
    {
        get
        {
            return init;
        }
    }


    public static IHaptic GetHaptic()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Init)
            {
                return Haptic;
            }

            return null;
        }

        if (!Init)
        {
            Initialize();
        }

        return Haptic;
    }

    public static void Initialize()
    {
        if (Haptic == null)
        {
            try
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    Haptic = new AndroidHaptic();
                    BhapticsLogger.LogInfo("Android initialized.");
                }
                else
                {
                    Haptic = new BhapticsHaptic();
                    BhapticsLogger.LogInfo("Initialized.");
                }

                init = true;
            }
            catch (System.Exception e)
            {
                BhapticsLogger.LogError("BhapticsManager.cs - GetHaptic() / " + e.Message);
            }
        }
    }

    public static void Dispose()
    {
        if (Haptic != null)
        {
            Haptic.TurnOff();
            init = false;
            BhapticsLogger.LogInfo("Dispose() bHaptics plugin.");
            Haptic.Dispose();
            Haptic = null;
        }
    }
}
