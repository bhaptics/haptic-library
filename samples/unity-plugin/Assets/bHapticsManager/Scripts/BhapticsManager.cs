using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Bhaptics.Tact.Unity
{
    public class BhapticsManager : MonoBehaviour
    {
        private static BhapticsManager _manager;

        private VisualFeedback[] visualFeedbacks;

        [Tooltip("Show visual feedabck or not")]
        [SerializeField]
        public bool visualizeFeedbacks;

        public bool launchPlayerIfNotRunning = true;
        
        private readonly List<HapticFeedback> _changedFeedbacks = new List<HapticFeedback>();

        private static IHapticPlayer _hapticPlayer;
        private static bool _isTryLaunchApp;

        private bool isInBackgroundForAndroid= false;

        public static string GetFeedbackId(string key)
        {
            foreach (var file in TactFileAsset.Instance.FeedbackFiles)
            {
                if (file.Key == key)
                {
                    return file.Id;
                }
            }
            Debug.LogError("Cannot find feedback file with key : " + key);
            return "";
        }

        public static IHapticPlayer HapticPlayer
        {
            get
            {
                if (_hapticPlayer == null)
                {
                    if (!BhapticsUtils.IsPlayerInstalled())
                    {
                        _hapticPlayer = new DumbPlayer();
                    }
                    else
                    {
                        if (Application.platform == RuntimePlatform.Android)
                        {
                            _hapticPlayer = new AndroidHapticPlayer();
                        }
                        else
                        {
                            _hapticPlayer = new HapticPlayer2();
                        }
                    }
                }
                
                return _hapticPlayer;
            }
        }

#region Unity Method

        void AndroidStopScan()
        {
            Invoke("AndroidStartScan", 3f);
            AndroidHapticPlayer player = HapticPlayer as AndroidHapticPlayer;
            if (player != null)
            {
                player.StopScan();
            }
        }

        void AndroidStartScan()
        {
            Invoke("AndroidStopScan", 17f);

            AndroidHapticPlayer player = HapticPlayer as AndroidHapticPlayer;
            if (player != null)
            {
                player.StartScan();
            }
        }


        void Awake()
        {
            var uninstalledMessage = "bHaptics Player is not installed. Plugin is now disabled. Please download here." +
                                     "\nhttp://bhaptics.com/app.html#download";
            _manager = this;
            if (!BhapticsUtils.IsPlayerInstalled())
            {
                Debug.LogError(uninstalledMessage);
                return;
            }

            if (!BhapticsUtils.IsPlayerRunning())
            {
                Debug.Log("bHaptics Player is not running, try launching bHaptics Player.");
                BhapticsUtils.LaunchPlayer(launchPlayerIfNotRunning);
            }

            if (Application.platform == RuntimePlatform.Android)
            {
                Invoke("AndroidStopScan", 3f);
            }
        }

        void OnEnable()
        {
            if (!BhapticsUtils.IsPlayerInstalled())
            {
                gameObject.SetActive(false);
                return;
            }

            InitVisualFeedback();

            HapticPlayer.StatusReceived += OnStatusChanged;
            HapticPlayer.Enable();

            LoadFeedbackFile();
        }

        void OnDisable()
        {
            if (!BhapticsUtils.IsPlayerInstalled())
            {
                return;
            }
            HapticPlayer.TurnOff();
            HapticPlayer.Disable();
            HapticPlayer.StatusReceived -= OnStatusChanged;
        }
        
        // only for Android
        public void Received(string message)
        {
            
            var response = Bhaptics.fastJSON.JSON.ToObject<PlayerResponse>(message);

            try
            {
                object[] item = { response };
                HapticPlayer.GetType().GetMethod("Receive").Invoke(HapticPlayer, item);
            }
            catch (Exception e)
            {
                
                Debug.Log(e);
            }
        }

        void OnDestroy()
        {
            if (HapticPlayer != null)
            {
                HapticPlayer.Dispose();
            }
            _hapticPlayer = null;
        }

        void Update()
        {
            if (!visualizeFeedbacks)
            {
                return;
            }

            if (!BhapticsUtils.IsPlayerInstalled())
            {
                return;
            }

            if (Application.platform == RuntimePlatform.Android)
            {
                if (!Monitor.TryEnter(_changedFeedbacks))
                {
                    Debug.Log("failed to enter");
                    return;
                }
                try
                {
                    foreach (var feedback in _changedFeedbacks)
                    {
                        foreach (var vis in visualFeedbacks)
                        {
                            if (vis.Position == feedback.Position)
                            {
                                vis.UpdateFeedbacks(feedback);
                            }
                        }
                    }
                    _changedFeedbacks.Clear();
                }
                finally
                {
                    Monitor.Exit(_changedFeedbacks);
                }
            }
            else
            {
                foreach (var vis in visualFeedbacks)
                {
                    HapticApi.status status;
                    HapticApi.TryGetResponseForPosition(vis.Position, out status);

                    byte[] result = new byte[20];
                    for (int i = 0; i < result.Length; i++)
                    {
                        result[i] = (byte)status.values[i];
                    }

                    HapticFeedback feedback = new HapticFeedback(vis.Position, result);
                    vis.UpdateFeedbacks(feedback);
                }
            }
        }

        void OnApplicationPause(bool pauseState)
        {
            if (pauseState)
            {
                Debug.Log("background");
                isInBackgroundForAndroid = true;
                CancelInvoke("AndroidStopScan");
                CancelInvoke("AndroidStartScan");
                AndroidStopScan();
                OnDisable();
            }
            else
            {
                Debug.Log("foreground");
                isInBackgroundForAndroid = false;
                AndroidStartScan();
                
                OnEnable();
            }
        }

#endregion

        void InitVisualFeedback()
        {
            visualFeedbacks = GetComponentsInChildren<VisualFeedback>(true);

            var feedback = transform.GetChild(0);

            if (feedback != null)
            {
                feedback.gameObject.SetActive(visualizeFeedbacks);
            }
        }

        private void LoadFeedbackFile()
        {
            try
            {
                if (TactFileAsset.Instance == null)
                {
                    Debug.LogError("Failed to Read TactFileAsset");
                    return;
                }
                
                var files = TactFileAsset.Instance.FeedbackFiles;

                if (files == null)
                {
                    Debug.LogError("Failed to Get Feedback files");
                }

            }
            catch (Exception e)
            {
                Debug.LogError("LoadFeedbackFile() failed : " + e.Message);
            }
        }

        private void OnStatusChanged(PlayerResponse playerResponse)
        {
            if (visualizeFeedbacks == false)
            {
                return;
            }

            if (_changedFeedbacks == null)
            {
                return;
            }

            lock (_changedFeedbacks)
            {
                foreach (var status in playerResponse.Status)
                {
                    var pos = EnumParser.ToPositionType(status.Key);
                    var val = status.Value;

                    byte[] result = new byte[val.Length];
                    for (int i = 0; i < val.Length; i++)
                    {
                        result[i] = (byte)val[i];
                    }
                    var feedback = new HapticFeedback(pos, result);
                    _changedFeedbacks.Add(feedback);
                }
            }
        }
    }

    public class DumbPlayer : IHapticPlayer
    {
        public void Dispose()
        {
            // nothing to do
        }

        public void Enable()
        {
            // nothing to do
        }

        public void Disable()
        {
            // nothing to do
        }

        public bool IsActive(PositionType type)
        {
            // nothing to do
            return false;
        }

        public bool IsPlaying(string key)
        {
            // nothing to do
            return false;
        }

        public bool IsPlaying()
        {
            // nothing to do
            return false;
        }

        public void Register(string key, string path)
        {
            // nothing to do
        }

        public void RegisterTactFileStr(string key, string tactFileStr)
        {
            // nothing to do
        }

        public void RegisterTactFileStrReflected(string key, string tactFileStr)
        {
            // nothing
        }

        public void Submit(string key, PositionType position, byte[] motorBytes, int durationMillis)
        {
            // nothing to do
        }

        public void Submit(string key, PositionType position, List<DotPoint> points, int durationMillis)
        {
            // nothing to do
        }

        public void Submit(string key, PositionType position, DotPoint point, int durationMillis)
        {
            // nothing to do
        }

        public void Submit(string key, PositionType position, List<PathPoint> points, int durationMillis)
        {
            // nothing to do
        }

        public void Submit(string key, PositionType position, PathPoint point, int durationMillis)
        {
            // nothing to do
        }

        public void SubmitRegistered(string key, string altKey, ScaleOption option)
        {
            // nothing to do
        }

        public void SubmitRegistered(string key, ScaleOption option)
        {
            // nothing to do
        }

        public void SubmitRegisteredVestRotation(string key, RotationOption option)
        {
            // nothing to do
        }

        public void SubmitRegisteredVestRotation(string key, string altKey, RotationOption option)
        {
            // nothing to do
        }

        public void SubmitRegisteredVestRotation(string key, string altKey, RotationOption option, ScaleOption sOption)
        {
            // nothing to do
        }

        public void SubmitRegistered(string key)
        {
            // nothing to do
        }

        public void SubmitRegistered(string key, float duration)
        {
            // nothing to do
        }

        public void TurnOff(string key)
        {
            // nothing to do
        }

        public void TurnOff()
        {
            // nothing to do
        }

        public void Register(string key, Project project)
        {
            // nothing to do
        }

        public event Action<PlayerResponse> StatusReceived;
    }
}