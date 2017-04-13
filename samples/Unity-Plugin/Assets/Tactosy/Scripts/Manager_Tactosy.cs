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
        public bool visualizeMotors;

        [Tooltip("Tactosy File Prefix")]
        [SerializeField]
        private string PathPrefix = "Assets/Tactosy/Feedbacks/";

        [Tooltip("Tactosy Feedback Mapping Infos")]
        [SerializeField]
        internal List<SignalMapping> FeedbackMappings;

        private Dictionary<string, FeedbackSignal> FeedbackSignalMappings;

        public delegate void SendFeedbackSignal(FeedbackSignal fbSignal);
        public static event SendFeedbackSignal sendFeedbackSignal;

        public TactosyPlayer TactosyPlayer;
        private ISender sender;
        private ITimer timer;

        void OnEnable()
        {
            InitPlayer();
        }

        private void InitPlayer()
        {
            // Setup Tactosy Player
            sender = new WebSocketSender();
            timer = GetComponent<UnityTimer>();
            
            TactosyPlayer = new TactosyPlayer(sender, timer);

            TactosyPlayer.ValueChanged += TactosyPlayerOnValueChanged;
            FeedbackSignalMappings = new Dictionary<string, FeedbackSignal>();


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
                    FeedbackSignalMappings.Add(feedbackMapping.Key, new FeedbackSignal(json));
                    
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
            
            Debug.Log(TactosyUtils.ConvertByteArrayToString(feedback.Values));
        }

        public void Play(string key)
        {
            if (TactosyPlayer == null)
            {
                Debug.LogError("Tactosy player is not initialized.");
                return;
            }
            TactosyPlayer.SendSignal(key);
            //sendFeedbackSignal(FeedbackSignalMappings[key]);

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
            else
            {
                OnEnable();
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
//                TactosyPlayer.SendSignal("test", PositionType.All, bytes, 100);

                TactosyPlayer.SendSignal("Fireball", 0.2f);
            }
        }
    }
}

