using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using Tactosy.Common;
using Tactosy.Common.Sender;
using UnityEngine.UI;

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
        
        public event EventHandler Elapsed;

        public bool visualizeFeedbacks;

        [Tooltip("Tactosy File Prefix")]
        [SerializeField]
        private string PathPrefix = "Assets/Tactosy/Feedbacks/";

        [Tooltip("Tactosy Feedback Mapping Infos")]
        [SerializeField]
        internal List<SignalMapping> FeedbackMappings;

        //private Dictionary<string, FeedbackSignal> FeedbackSignalMappings;

        public TactosyPlayer TactosyPlayer;
        private ISender sender;
        private ITimer timer;

        void OnEnable()
        {
            InitializeFeedbacks();
            InitPlayer();
        }
        
        void InitializeFeedbacks()
        {
            if (leftHandModel != null && rightHandMoadel != null)
            {
                leftHandModel.gameObject.SetActive(false);
                rightHandMoadel.gameObject.SetActive(false);

                if (visualizeFeedbacks)
                {
                    leftHandModel.gameObject.SetActive(true);
                    rightHandMoadel.gameObject.SetActive(true);

                }
            }
        }

        void UpdateFeedbacks(TactosyFeedback tactosyFeedback)
        {
            if (tactosyFeedback.Position == PositionType.Left)
            {
                UpdateFeedbacks(leftHandModel, tactosyFeedback);
            }
            else if (tactosyFeedback.Position == PositionType.Right)
            {
                UpdateFeedbacks(rightHandMoadel, tactosyFeedback);
            }
            else if (tactosyFeedback.Position == PositionType.All)
            {
                UpdateFeedbacks(leftHandModel, tactosyFeedback);
                UpdateFeedbacks(rightHandMoadel, tactosyFeedback);
            }
        }

        private void UpdateFeedbacks(GameObject handModel, TactosyFeedback tactosyFeedback)
        {
            Debug.Log("test");
            var container = handModel.transform.GetChild(1);

            for (int i = 0; i < container.childCount; i++)
            {
                var scale = tactosyFeedback.Values[i] / 200f;
                if (container.transform.GetChild(i) != null)
                {
                    container.transform.GetChild(i).localScale = Vector3.one * scale;
                }
            }
        }

        private void InitPlayer()
        {
            // Setup Tactosy Player
            sender = new WebSocketSender();
            timer = GetComponent<UnityTimer>();
            
            TactosyPlayer = new TactosyPlayer(sender, timer);

            TactosyPlayer.ValueChanged += TactosyPlayerOnValueChanged;
            //FeedbackSignalMappings = new Dictionary<string, FeedbackSignal>();


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
                    //FeedbackSignalMappings.Add(feedbackMapping.Key, new FeedbackSignal(json));
                    
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    Debug.LogError("failed to read feedback file " + Path.Combine(PathPrefix, feedbackMapping.Path) + " : " + e.Message);
                }
            }

            TactosyPlayer.Start();
        }

        private void TactosyPlayerOnValueChanged(TactosyFeedback feedback)
        {
            Debug.Log("value changed");
            UpdateFeedbacks(feedback);
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

        void OnDisable()
        {
            TactosyPlayer.Stop();
        }

        void OnApplicationPause(bool pauseState)
        {
            if (pauseState)
            {
                OnDisable();
            }
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
                //TactosyPlayer.SendSignal("test", PositionType.All, bytes, 100);

                TactosyPlayer.SendSignal("Fireball", 0.2f);
            }
        }
    }
}

