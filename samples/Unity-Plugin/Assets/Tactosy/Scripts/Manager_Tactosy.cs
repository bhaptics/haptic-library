using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Tactosy.Common;
using Tactosy.Common.Sender;

namespace Tactosy.Unity
{
    public class Manager_Tactosy : MonoBehaviour
    {
        [Serializable]
        public class SignalMapping
        {
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

        [Tooltip("Tactosy Feedback Mapping Infos")]
        [SerializeField]
        internal List<SignalMapping> FeedbackMappings;

        public TactosyPlayer TactosyPlayer;
        private ITimer timer;


        #region Unity Method

        void OnEnable()
        {
            InitializeFeedbacks();
            InitPlayer();
        }

        void OnDisable()
        {
            TactosyPlayer.Stop();
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
                TactosyPlayer.SendSignal("Fireball", 0.2f);
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
            TactosyPlayerOnValueChanged(new TactosyFeedback(PositionType.All, new byte[20], FeedbackMode.DOT_MODE));
        }

        private void InitPlayer()
        {
            // Setup Tactosy Player
            var sender = new WebSocketSender();
            timer = GetComponent<UnityTimer>();            
            TactosyPlayer = new TactosyPlayer(sender, timer);
            TactosyPlayer.ValueChanged += TactosyPlayerOnValueChanged;

            foreach (var feedbackMapping in FeedbackMappings)
            {
                try
                {
                    string filePath = Path.Combine(PathPrefix, feedbackMapping.Path);

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
                    TactosyPlayer.RegisterFeedback(feedbackMapping.Key, new FeedbackSignal(json));
                }
                catch (Exception e)
                {
                    Debug.LogError("failed to read feedback file " + Path.Combine(PathPrefix, feedbackMapping.Path) + " : " + e.Message);
                }
            }

            TactosyPlayer.Start();
        }

        private void TactosyPlayerOnValueChanged(TactosyFeedback feedback)
        {
            if (visualizeFeedbacks == false)
            {
                return;
            }

            if (leftHandModel == null)
            {
                Debug.LogError("failed to find the left hand model for feedback visualization");
                return;
            }

            if (rightHandModel == null)
            {
                Debug.LogError("failed to find the right hand model for feedback visualization");
                return;
            }

            if (vestFrontModel == null)
            {
                Debug.LogError("failed to find the vestFront model for feedback visualization");
                return;
            }

            if (vestBackModel == null)
            {
                Debug.LogError("failed to find the vestBack model for feedback visualization");
                return;
            }

            if (headModel == null)
            {
                Debug.LogError("failed to find the head model for feedback visualization");
                return;
            }

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
            else if (feedback.Position == PositionType.All)
            {
                leftHandModel.SendMessage("UpdateFeedbacks", feedback);
                rightHandModel.SendMessage("UpdateFeedbacks", feedback);
            }
        }

        public void Play(string key)
        {
            if (TactosyPlayer == null)
            {
                Debug.LogError("Tactosy player is not initialized.");
                return;
            }
            TactosyPlayer.SendSignal(key);
        }
        

        public void TurnOff()
        {
            if (TactosyPlayer == null)
            {
                Debug.LogError("Tactosy player is not initialized.");
                return;
            }

            TactosyPlayer.TurnOff();
        }
    }
}

