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
        
        [SerializeField]
        private Transform feedbackModelPrefab, feedbackModelParent;

        private int numOfFeedbackRow = 4;
        private int numOfFeedbackColumn = 5;
        private GameObject[] feedbacks;
        
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
            InitializeFeedbacks();
            InitPlayer();
        }

        
        void InitializeFeedbacks()
        {
            if (feedbacks == null)
            {
                feedbacks = new GameObject[numOfFeedbackColumn * numOfFeedbackRow];
            }

            for (int i = 0; i < numOfFeedbackRow; i++)          //4
            {
                for (int j = 0; j < numOfFeedbackColumn; j++)   //5
                {
                    int index = i * numOfFeedbackColumn + j;
                    feedbacks[index] = Instantiate(feedbackModelPrefab, feedbackModelParent).gameObject;
                    var x = j * feedbacks[index].transform.localScale.magnitude; // + distanceBetweenMotors;
                    var y = i * feedbacks[index].transform.localScale.magnitude; // + distanceBetweenMotors;
                    var z = feedbacks[index].transform.localScale.z;

                    feedbacks[index].transform.position = new Vector3(x, y, z);
                    feedbacks[index].transform.parent = transform;
                }
            }

            Debug.Log(feedbacks);
        }

        void UpdateFeedbacks(TactosyFeedback tactosyFeedback)
        {
            for (int i = 0; i < feedbacks.Length; i++)
            {
                var scale = tactosyFeedback.Values[i] / 200f;
                if (feedbacks[i] != null)
                {
                    feedbacks[i].transform.localScale = Vector3.one * scale;
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
            if (feedbacks != null)
            {
                feedbacks = null;
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

