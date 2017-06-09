using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using Tactosy.Common;
using Tactosy.Common.Sender;
using UnityEditorInternal;

namespace Tactosy.Unity
{
    public class Manager_Tactosy : MonoBehaviour
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

        [SerializeField]
        public bool visualizeFeedbacks;

        [Tooltip("Tactosy File Prefix")]
        [SerializeField]
        private string PathPrefix = "Assets/Tactosy/Feedbacks/";

        [Tooltip("Tactosy feedback file path")]
        [SerializeField] private bool isSteamingPath;

        [Tooltip("Tactosy Feedback Mapping Infos")]
        [SerializeField]
        internal List<SignalMapping> FeedbackMappings;

        public HapticPlayer TactosyPlayer;

        private List<HapticFeedback> changedFeedbacks = new List<HapticFeedback>();
        
        #region Unity Method

        private bool initialized;
        void OnEnable()
        {
            InitPlayer();
            
            TactosyPlayer.FeedbackChanged += TactosyPlayerOnValueChanged;
            TactosyPlayer.Enable();
        }

        void OnDisable()
        {
            TactosyPlayer.Disable();
            TactosyPlayer.FeedbackChanged -= TactosyPlayerOnValueChanged;
        }

        void Update()
        {
            if (Input.GetKeyDown("space"))
            {
                byte[] bytes =
                {
                    0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0,
                    0, 0, 100, 100, 0,
                    0, 0, 0, 0, 0
                };

                TactosyPlayer.Submit("test", PositionType.VestFront, bytes, 1000);
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
                        leftHandModel.SendMessage("UpdateFeedbacks", feedback);
                    }

                    else if (feedback.Position == PositionType.Right)
                    {
                        rightHandModel.SendMessage("UpdateFeedbacks", feedback);
                    }

                    else if (feedback.Position == PositionType.VestFront)
                    {
                        vestFrontModel.SendMessage("UpdateFeedbacks", feedback);
                    }

                    else if (feedback.Position == PositionType.VestBack)
                    {
                        vestBackModel.SendMessage("UpdateFeedbacks", feedback);
                    }

                    else if (feedback.Position == PositionType.Head)
                    {
                        headModel.SendMessage("UpdateFeedbacks", feedback);
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

            TactosyPlayerOnValueChanged(new HapticFeedback(PositionType.All, new byte[20], FeedbackMode.DOT_MODE));
        }

        public void InitPlayer()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;
            InitializeFeedbacks();

            // Setup Tactosy Player
            var sender = new WebSocketSender();
            var timer = GetComponent<UnityTimer>();            
            TactosyPlayer = new HapticPlayer(sender, timer);
            LoadTactosyFeedback();
        }

        private void LoadTactosyFeedback()
        {
            FeedbackMappings.Clear();
            string tactosyFileRootPath = PathPrefix;

            if (isSteamingPath)
            {
                tactosyFileRootPath = Application.dataPath + "/StreamingAssets/" + PathPrefix;
            }

            string[] allPaths = Directory.GetFiles(tactosyFileRootPath, "*.tactosy", SearchOption.AllDirectories);

            foreach (var filePath in allPaths)
            {
                try
                {
                    var fileName = Path.GetFileNameWithoutExtension(filePath);

                    string json = LoadStringFromFile(filePath);

                    TactosyPlayer.Register(fileName, new BufferedHapticFeedback(json));
                    FeedbackMappings.Add(new SignalMapping(fileName, fileName + ".tactosy"));
                }
                catch (Exception e)
                {
                    Debug.LogError("failed to read feedback file " + filePath + " : " + e.Message);
                }
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

        private void TactosyPlayerOnValueChanged(HapticFeedback feedback)
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
            if (TactosyPlayer == null)
            {
                Debug.Log("Tactosy player is not initialized.");
                return;
            }
            TactosyPlayer.SubmitRegistered(key);
        }
        

        public void TurnOff()
        {
            if (TactosyPlayer == null)
            {
                Debug.Log("Tactosy player is not initialized.");
                return;
            }

            TactosyPlayer.TurnOff();
        }
    }
}

