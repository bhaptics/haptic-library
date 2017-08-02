using System;
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

        [SerializeField]
        private GameObject leftHandModel, rightHandModel, headModel, vestFrontModel, vestBackModel;

        [SerializeField] private GameObject[] uis;

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

        public HapticPlayer HapticPlayer;

        private List<HapticFeedback> changedFeedbacks = new List<HapticFeedback>();
        
        #region Unity Method

        private bool initialized;

        private bool tryLaunchApp = false;

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
                tryLaunchApp = true;
                
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
            
            HapticPlayer.FeedbackChanged += OnFeedbackChanged;
            HapticPlayer.Enable();
        }

        void OnDisable()
        {
            if (!BhapticsUtils.IsPlayerInstalled())
            {
                return;
            }

            HapticPlayer.Disable();
            HapticPlayer.FeedbackChanged -= OnFeedbackChanged;
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

            if (changedFeedbacks.Count <= 0)
            {
                return;
            }

            if (!Monitor.TryEnter(changedFeedbacks))
            {
                Debug.Log("failed to enter");
                return;
            }
            try
            {
                foreach (var feedback in changedFeedbacks)
                {
                    if (feedback.Position == PositionType.Left)
                    {
                        leftHandModel.SendMessage("UpdateFeedbacks", feedback, SendMessageOptions.DontRequireReceiver);
                    }

                    else if (feedback.Position == PositionType.Right)
                    {
                        rightHandModel.SendMessage("UpdateFeedbacks", feedback, SendMessageOptions.DontRequireReceiver);
                    }

                    else if (feedback.Position == PositionType.VestFront)
                    {
                        vestFrontModel.SendMessage("UpdateFeedbacks", feedback, SendMessageOptions.DontRequireReceiver);
                    }

                    else if (feedback.Position == PositionType.VestBack)
                    {
                        vestBackModel.SendMessage("UpdateFeedbacks", feedback, SendMessageOptions.DontRequireReceiver);
                    }

                    else if (feedback.Position == PositionType.Head)
                    {
                        headModel.SendMessage("UpdateFeedbacks", feedback, SendMessageOptions.DontRequireReceiver);
                    }
                }
                changedFeedbacks.Clear();
            }
            finally
            {
                Monitor.Exit(changedFeedbacks);
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

        void InitializeFeedbacks()
        {
            foreach (var ui in uis)
            {
                ui.SetActive(visualizeFeedbacks);
            }

            if (leftHandModel != null)
            {
                leftHandModel.gameObject.SetActive(visualizeFeedbacks);
            }

            if (rightHandModel != null)
            {
                rightHandModel.gameObject.SetActive(visualizeFeedbacks);
            }

            if (vestFrontModel != null)
            {
                vestFrontModel.gameObject.SetActive(visualizeFeedbacks);
            }

            if (vestBackModel != null)
            {
                vestBackModel.gameObject.SetActive(visualizeFeedbacks);
            }

            if (headModel != null)
            {
                headModel.gameObject.SetActive(visualizeFeedbacks);
            }

            OnFeedbackChanged(new HapticFeedback(PositionType.All, new byte[20], FeedbackMode.DOT_MODE));
        }

        public void InitPlayer()
        {
            if (!BhapticsUtils.IsPlayerInstalled())
            {
                return;
            }

            if (initialized)
            {
                return;
            }

            initialized = true;
            InitializeFeedbacks();

            // Setup Haptic Player
            var sender = new WebSocketSender(() => tryLaunchApp = true
            , () =>
            {
                if (!tryLaunchApp)
                {
                    BhapticsUtils.LaunchPlayer();
                    tryLaunchApp = true;
                }
            });

            var timer = GetComponent<UnityTimer>();
            HapticPlayer = new HapticPlayer(sender, timer);
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

                        HapticPlayer.Register(fileName, new BufferedHapticFeedback(json));
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

            if (leftHandModel == null)
            {
                Debug.Log("failed to find the left hand model for feedback visualization");
                return;
            }

            if (rightHandModel == null)
            {
                Debug.Log("failed to find the right hand model for feedback visualization");
                return;
            }

            if (vestFrontModel == null)
            {
                Debug.Log("failed to find the vestFront model for feedback visualization");
                return;
            }

            if (vestBackModel == null)
            {
                Debug.Log("failed to find the vestBack model for feedback visualization");
                return;
            }

            if (headModel == null)
            {
                Debug.Log("failed to find the head model for feedback visualization");
                return;
            }

            if (changedFeedbacks == null)
            {
                return;
            }

            lock (changedFeedbacks)
            {
                changedFeedbacks.Add(feedback);
            }
        }

        public void Play(string key)
        {
            if (HapticPlayer == null)
            {
                Debug.Log("Haptic player is not initialized.");
                return;
            }
            HapticPlayer.SubmitRegistered(key);
        }
        

        public void TurnOff()
        {
            if (HapticPlayer == null)
            {
                Debug.Log("Haptic player is not initialized.");
                return;
            }

            HapticPlayer.TurnOff();
        }
    }
}

