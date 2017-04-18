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
        private GameObject leftHandModel, rightHandMoadel;

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
            if (leftHandModel != null && rightHandMoadel != null)
            {
                leftHandModel.gameObject.SetActive(visualizeFeedbacks);
                rightHandMoadel.gameObject.SetActive(visualizeFeedbacks);

                TactosyPlayerOnValueChanged(new TactosyFeedback(PositionType.All, new byte[20], FeedbackMode.DOT_MODE));
            }
        }

        private void UpdateFeedbacks(GameObject handModel, TactosyFeedback tactosyFeedback)
        {
            if (handModel == null)
            {
                return;
            }

            var container = handModel.transform.GetChild(1);

            for (int i = 0; i < container.childCount; i++)
            {
                var scale = tactosyFeedback.Values[i] / 200f;
                if (container.transform.GetChild(i) != null)
                {
                    container.transform.GetChild(i).localScale = new Vector3(scale, .02f, scale);
                }
            }
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
            if (feedback.Position == PositionType.Left)
            {
                UpdateFeedbacks(leftHandModel, feedback);
            }
            else if (feedback.Position == PositionType.Right)
            {
                UpdateFeedbacks(rightHandMoadel, feedback);
            }
            else if (feedback.Position == PositionType.All)
            {
                UpdateFeedbacks(leftHandModel, feedback);
                UpdateFeedbacks(rightHandMoadel, feedback);
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

