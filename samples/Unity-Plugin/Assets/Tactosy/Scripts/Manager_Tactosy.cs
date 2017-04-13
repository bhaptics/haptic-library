using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

        public class Feedback
        {
            public Transform transform;
            public int power;
            public Color color;

            public Feedback(Color color)
            {
                this.color = color;
            }
        }

        [SerializeField]
        private Transform feedbackModelPrefab, feedbackModelParent;

        private int numOfFeedbackRow = 4;
        private int numOfFeedbackColumn = 5;
        private Feedback[] feedbacks;
        
        public event EventHandler Elapsed;
        
        [SerializeField]
        public bool visualizeFeedbacks;

        [Tooltip("Tactosy File Prefix")]
        [SerializeField]
        private string PathPrefix = "Assets/Tactosy/Feedbacks/";

        [Tooltip("Tactosy Feedback Mapping Infos")]
        [SerializeField]
        internal List<SignalMapping> FeedbackMappings;

        private Dictionary<string, FeedbackSignal> FeedbackSignalMappings;

        //public delegate void SendFeedbackSignal(FeedbackSignal fbSignal);
        //public static event SendFeedbackSignal sendFeedbackSignal;

        public TactosyPlayer TactosyPlayer;
        private ISender sender;
        private ITimer timer;

        void OnEnable()
        {
            feedbacks = new Feedback[numOfFeedbackColumn * numOfFeedbackRow];
            InitializeFeedbacks(feedbacks);
            InitPlayer();
        }

        void InitializeFeedbacks(Feedback[] feedbacks)
        {
            for (int i = 0; i < numOfFeedbackRow; i++)
            {
                for (int j = 0; j < numOfFeedbackColumn; j++)
                {
                    feedbacks[i + j] = new Feedback(Color.black);
                    feedbacks[i + j].transform = Instantiate(feedbackModelPrefab, feedbackModelParent);
                    var x = j * feedbacks[i + j].transform.localScale.magnitude; // + distanceBetweenMotors;
                    var y = i * feedbacks[i + j].transform.localScale.magnitude; // + distanceBetweenMotors;
                    var z = feedbacks[i + j].transform.localScale.z;

                    feedbacks[i + j].transform.position = new Vector3(x, y, z);
                    feedbacks[i + j].transform.parent = feedbackModelParent;
                }
            }
        }

        void UpdateFeedbacks(TactosyFeedback tactosyFeedback)
        {
            for (int i = 0; i < tactosyFeedback.Values.Length; i++)
            {
                feedbacks[i].transform.localScale = new Vector3(tactosyFeedback.Values[i], tactosyFeedback.Values[i], tactosyFeedback.Values[i]);
            }
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
            if (visualizeFeedbacks)
            {

            }
            else if (feedbacks != null)
            {
                
            }
        }

        private void TactosyPlayerOnValueChanged(TactosyFeedback feedback)
        {
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
                //TactosyPlayer.SendSignal("test", PositionType.All, bytes, 100);

                TactosyPlayer.SendSignal("Fireball", 0.2f);
            }
        }
    }
}

