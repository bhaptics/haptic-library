using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Bhaptics.Tac.Sender;
using UnityEngine;

namespace Bhaptics.Tac.Unity
{
    public class BhapticsManager : MonoBehaviour
    {
        [Serializable]
        public class SignalMapping
        {
            public SignalMapping(string key, string path)
            {
                Key = key;
                Path = path;
            }

            public string Key;
            public string Path;
        }

        private VisualFeedabck[] visualFeedbacks;

        [SerializeField]
        public bool visualizeFeedbacks;

        [Tooltip("File Prefix")]
        [SerializeField]
        private string PathPrefix = "Assets/bHapticsManager/Feedbacks/";

        [Tooltip("Use Streaming Path or not")]
        [SerializeField] private bool useStreamingPath;

        [Tooltip("Feedback Mapping Infos")]
        [SerializeField]
        internal List<SignalMapping> FeedbackMappings;

//        public HapticPlayer HapticPlayer;
        private IHapticPlayer _hapticPlayer;

        private WebSocketSender _sender;
        public IHapticPlayer HapticPlayer()
        {
            if (_hapticPlayer == null)
            {
                InitPlayer();
            }

            return _hapticPlayer;
        }

        #region Unity Method

        private readonly List<HapticFeedback> _changedFeedbacks = new List<HapticFeedback>();
        private bool _isInit;
        private bool _isTryLaunchApp;

        void Awake()
        {
            var uninstalledMessage = "bHaptics Player is not installed. Plugin is now disabled. Please download here." +
                                     "\nhttp://bhaptics.com/app.html#download";
            if (!BhapticsUtils.IsPlayerInstalled())
            {
                Debug.LogError(uninstalledMessage);
                return;
            }

            if (!BhapticsUtils.IsPlayerRunning())
            {
                Debug.Log("bHaptics Player is not running, try launching bHaptics Player.");
                BhapticsUtils.LaunchPlayer();
                _isTryLaunchApp = true;
                
                return;
            }
        }

        void OnEnable()
        {
            if (!BhapticsUtils.IsPlayerInstalled())
            {
                gameObject.SetActive(false);
                return;
            }

            InitPlayer();

            _hapticPlayer.FeedbackChanged += OnFeedbackChanged;
            _hapticPlayer.Enable();
        }

        void OnDisable()
        {
            if (!BhapticsUtils.IsPlayerInstalled())
            {
                return;
            }
            _hapticPlayer.TurnOff();
            _hapticPlayer.Disable();
            _hapticPlayer.FeedbackChanged -= OnFeedbackChanged;
        }

        void Update()
        {
            if (!BhapticsUtils.IsPlayerInstalled())
            {
                return;
            }

            if (!visualizeFeedbacks)
            {
                return;
            }

            if (_changedFeedbacks.Count <= 0)
            {
                return;
            }

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

        void OnApplicationPause(bool pauseState)
        {
            try
            {
                if (_sender != null)
                {
                    _sender.PlayFeedback(HapticFeedbackFrame.AsTurnOffFrame(PositionType.All));
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            if (pauseState)
            {
                OnDisable();
            }
            else
            {
                OnEnable();
            }
        }

        #endregion

        void InitializeFeedbacks()
        {
            visualFeedbacks = GetComponentsInChildren<VisualFeedabck>(true);

            foreach (var go in visualFeedbacks)
            {
                go.gameObject.SetActive(visualizeFeedbacks);
            }

            OnFeedbackChanged(new HapticFeedback(PositionType.All, new byte[20], FeedbackMode.DOT_MODE));
        }

        public void InitPlayer()
        {
            if (!BhapticsUtils.IsPlayerInstalled())
            {
                _hapticPlayer = new DumbPlayer();
                return;
            }

            if (_isInit)
            {
                return;
            }

            _isInit = true;
            InitializeFeedbacks();

            // Setup Haptic Player
            _sender = new WebSocketSender(() => _isTryLaunchApp = true
            , () =>
            {
                if (!_isTryLaunchApp)
                {
                    BhapticsUtils.LaunchPlayer();
                    _isTryLaunchApp = true;
                }
            });

            var timer = GetComponent<UnityTimer>();
            _hapticPlayer = new HapticPlayer(_sender, timer);
            LoadFeedbackFile();
        }

        private void LoadFeedbackFile()
        {
            FeedbackMappings.Clear();
            string fileRootPath = PathPrefix;

            if (useStreamingPath)
            {
                fileRootPath = Application.dataPath + "/StreamingAssets/" + PathPrefix;
            }

            try
            {
                string[] allPaths = Directory.GetFiles(fileRootPath, "*.tactosy", SearchOption.AllDirectories);

                foreach (var filePath in allPaths)
                {
                    try
                    {
                        var fileName = Path.GetFileNameWithoutExtension(filePath);

                        string json = LoadStringFromFile(filePath);

                        _hapticPlayer.Register(fileName, new BufferedHapticFeedback(json));
                        FeedbackMappings.Add(new SignalMapping(fileName, fileName + ".tactosy"));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("failed to read feedback file " + filePath + " : " + e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("FeedbackFile Loading failed : " + e.Message);
            }
        }

        private string LoadStringFromFile(string filePath)
        {
            string json;
            if (Application.platform == RuntimePlatform.Android)
            {
                WWW www = new WWW(filePath);

                while (!www.isDone)
                {
                }

                json = www.text;
            }
            else
            {
                json = File.ReadAllText(filePath);
            }
            return json;
        }

        private void OnFeedbackChanged(HapticFeedback feedback)
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
                _changedFeedbacks.Add(feedback);
            }
        }

        public void Play(string key)
        {
            if (_hapticPlayer == null)
            {
                Debug.Log("Haptic player is not initialized.");
                return;
            }
            _hapticPlayer.SubmitRegistered(key);
        }

        public void TurnOff()
        {
            if (_hapticPlayer == null)
            {
                Debug.Log("Haptic player is not initialized.");
                return;
            }

            _hapticPlayer.TurnOff();
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

        public void Register(string key, BufferedHapticFeedback tactosyFile)
        {
            // nothing to do
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

        public void SubmitRegistered(string key, float intensity, float duration)
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

        public event FeedbackEvent.HapticFeedbackChangeEvent FeedbackChanged;
    }
}

