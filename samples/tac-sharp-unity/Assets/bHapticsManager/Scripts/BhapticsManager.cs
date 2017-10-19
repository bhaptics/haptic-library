using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Bhaptics.Tac.Designer;
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

        [Tooltip("Show visual feedabck or not")]
        [SerializeField]
        public bool visualizeFeedbacks;

        [Tooltip("File Prefix")]
        [SerializeField]
        private string PathPrefix = "Assets/bHapticsManager/Feedbacks/";

        [Tooltip("Use Streaming Path or not")]
        [SerializeField]
        private bool useStreamingPath;

        [Tooltip("Feedback Mapping Infos")]
        [SerializeField]
        internal List<SignalMapping> FeedbackMappings;

        private readonly List<HapticFeedback> _changedFeedbacks = new List<HapticFeedback>();

        private static IHapticPlayer _hapticPlayer;
        private static bool _isTryLaunchApp;

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
                        _hapticPlayer = new HapticPlayer(connected =>
                        {
                            if (connected)
                            {
                                _isTryLaunchApp = true;
                            }
                            else
                            {
                                if (!_isTryLaunchApp)
                                {
                                    BhapticsUtils.LaunchPlayer();
                                    _isTryLaunchApp = true;
                                }
                            }
                        });
                    }
                }

                return _hapticPlayer;
            }
        }

        #region Unity Method

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

        void InitVisualFeedback()
        {
            visualFeedbacks = GetComponentsInChildren<VisualFeedabck>(true);

            foreach (var go in visualFeedbacks)
            {
                go.gameObject.SetActive(visualizeFeedbacks);
            }
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
                string[] extensions = { "*.tactosy", "*.tact" };

                foreach (var extension in extensions)
                {
                    string[] allPaths = Directory.GetFiles(fileRootPath, extension, SearchOption.AllDirectories);

                    foreach (var filePath in allPaths)
                    {
                        try
                        {
                            var fileName = Path.GetFileNameWithoutExtension(filePath);
                            string json = LoadStringFromFile(filePath);
                            var file = CommonUtils.ConvertJsonStringToTactosyFile(json);

                            _hapticPlayer.Register(fileName, file.Project);
                            FeedbackMappings.Add(new SignalMapping(fileName, fileName + ".tact"));
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("failed to read feedback file " + filePath + " : " + e.Message);
                        }
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

        public void Play(string key)
        {
            HapticPlayer.SubmitRegistered(key);
        }

        public void TurnOff()
        {
            HapticPlayer.TurnOff();
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

        public void Register(string key, Project project)
        {
            // nothing to do
        }

        public event FeedbackEvent.StatusReceivedEvent StatusReceived;
    }
}