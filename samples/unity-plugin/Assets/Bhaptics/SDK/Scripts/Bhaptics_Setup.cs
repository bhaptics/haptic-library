using Bhaptics.Tact.Unity;
using UnityEngine;

public class Bhaptics_Setup : MonoBehaviour
{
    [SerializeField] public BhapticsConfig Config;


    public static Bhaptics_Setup instance;

    void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this.gameObject);
            return;
        }

        instance = this;

        Initialize();

        DontDestroyOnLoad(this);
    }


    void OnApplicationQuit()
    {
        BhapticsManager.Dispose();
    }

    private void Initialize()
    {
        BhapticsManager.Initialize();

        if (Config == null)
        {
            BhapticsLogger.LogError("BHapticsConfig is not setup!");
            return;
        }

        if (Application.platform != RuntimePlatform.Android)
        {
            if (Config.launchPlayerIfNotRunning 
                && BhapticsUtils.IsPlayerInstalled() 
                && !BhapticsUtils.IsPlayerRunning())
            {
                BhapticsLogger.LogInfo("Try launching bhaptics player.");
                BhapticsUtils.LaunchPlayer(true);
            }

        }

#if UNITY_ANDROID


        if (Config.AndroidManagerPrefab == null)
        {
            BhapticsLogger.LogError("[bhaptics] AndroidManagerPrefab is not setup!");
            return;
        }

        var go = Instantiate(Config.AndroidManagerPrefab, transform);
        go.alwaysScanDisconnectedDevice = Config.AlwaysScanDisconnectedDevice;
#endif

    }
}
