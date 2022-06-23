using Bhaptics.Tact.Unity;
using UnityEngine;


public class Bhaptics_Setup : MonoBehaviour
{
    public static Bhaptics_Setup Instance;


    [SerializeField] private BhapticsConfig config;
    [Header("[ Ping On Start ]")]
    [SerializeField] private bool pingOnStart = true;
    [SerializeField] private HapticClip[] hapticClipsOnStart;

    

    public BhapticsConfig Config
    {
        get
        {
            return config;
        }
        set
        {
            config = value;
        }
    }




    void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(this.gameObject);
            return;
        }

        Instance = this;

        Initialize();

        DontDestroyOnLoad(this);
    }

    void Start()
    {
        if (pingOnStart)
        {
            Ping();
        }
    }

    void OnApplicationQuit()
    {
        BhapticsManager.Dispose();
    }

    private void Initialize()
    {
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
#endif
    }

    private void Ping()
    {
        for (int i = 0; i < hapticClipsOnStart.Length; ++i)
        {
            if (hapticClipsOnStart[i] == null)
            {
                continue;
            }

            hapticClipsOnStart[i].Play();
        }
    }
}
