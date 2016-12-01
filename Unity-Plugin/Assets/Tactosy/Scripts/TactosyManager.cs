using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using TactosyCommon.Models;
using WebSocketSharp;

namespace TactosyCommon.Unity
{
    /// <summary>
    /// TactosyManager is a class for managing tactosy feedback.</summary>
    /// <remarks>
    /// TactosyManager
    /// </remarks>
    public class TactosyManager : MonoBehaviour
    {
        [Serializable]
        public class SignalMapping
        {
            public string Key;
            public string Path;
        }

        public bool Enable = true;
        public List<SignalMapping> FeedbackMappings = new List<SignalMapping>();

        private Dictionary<string, FeedbackSignal> _registeredSignals;
        private Dictionary<string, FeedbackSignal> _activeSignals;
        private int _currentTime;
        private int _interval = 20;
        private int _motorSize = 20;
        
#if UNITY_STANDALONE || UNITY_EDITOR
        private WebSocket _webSocket;
        private bool _websocketConnected = false;
        private readonly string WebsocketUrl = "ws://127.0.0.1:15881/feedbackBytes";
#endif

#if (UNITY_ANDROID && !UNITY_EDITOR)
        private AndroidJavaObject javaObject;
#endif

        void Awake()
        {
            _registeredSignals = new Dictionary<string, FeedbackSignal>();
            
            foreach (var feedbackMapping in FeedbackMappings)
            {
                string streamingPath = Application.streamingAssetsPath;
                string filePath = "";
                try
                {
                    filePath = Path.Combine(streamingPath, feedbackMapping.Path);
                    _registeredSignals[feedbackMapping.Key] = new FeedbackSignal(filePath);
                }
                catch (Exception)
                {
                    Debug.LogError("failed to read feedback file " + filePath);
                }
                
            }

            _activeSignals = new Dictionary<string, FeedbackSignal>();

            Debug.Log(_registeredSignals.Count + " Tactosy Feedback registred.");
            _currentTime = 0;

#if UNITY_STANDALONE || UNITY_EDITOR
            Debug.Log("STANDALONE MODE");

            _webSocket = new WebSocket(WebsocketUrl);

            _webSocket.OnMessage += (sender, e) =>
            {
                Debug.Log("Message Received : " + e.Data);
            };

            _webSocket.OnOpen += (sender, args) =>
            {
                Debug.Log("Connected");
                _websocketConnected = true;
            };

            _webSocket.OnError += (sender, args) =>
            {
                Debug.Log("OnError " + args.Message);
            };

            _webSocket.OnClose += (sender, args) =>
            {
                Debug.Log("Closed");
                _websocketConnected = false;
            };

            _webSocket.Connect();
#endif

#if (UNITY_ANDROID && !UNITY_EDITOR)
            try
            {
                javaObject = new AndroidJavaObject("com.bhaptics.unity_ble.UnityClient");
                Debug.Log(javaObject + " Unity javaObject init()");
                int result = javaObject.Call<int>("init");
                Log("init result : " + result);
            }
            catch (Exception ex)
            {
                Log("Initilization failed" + ex.Message + "," + ex.StackTrace);
            }
#endif
        }

        void OnDestroy()
        {
            Debug.Log("OnDestroy");
#if UNITY_STANDALONE || UNITY_EDITOR
            _webSocket.Close();
#endif

#if (UNITY_ANDROID && !UNITY_EDITOR)
        if (javaObject != null)
        {
            javaObject.Call("destroy");
        }
#endif
        }

        void OnTimeUpdate()
        {
            if (_activeSignals.Count == 0)
            {
                if (_currentTime > 0)
                {
                    _currentTime = 0;

                    PlayFeedback(new TactosyFeedback(PositionType.Right, new byte[20], FeedbackMode.DOT_MODE));
                    PlayFeedback(new TactosyFeedback(PositionType.Left, new byte[20], FeedbackMode.DOT_MODE));
                }
                
                return;
            }
            
            int[] dotModeSignalLeft = new int[_motorSize];
            int[] dotModeSignalRight = new int[_motorSize];
            int[] pathModeSignalLeft = new int[_motorSize];
            int[] pathModeSignalRight = new int[_motorSize];

            List<string> expiredSignals = new List<string>();


            bool dotModeLeftActive = false;
            bool dotModeRightActive = false;
            bool pathModeActiveLeft = false;
            bool pathModeActiveRight = false;


            foreach (KeyValuePair<string, FeedbackSignal> keyPair in _activeSignals)
            {
                FeedbackSignal signalData = keyPair.Value;
                if (signalData.StartTime > _currentTime || signalData.StartTime < 0)
                {
                    signalData.StartTime = _currentTime;
                }
                int timePast = _currentTime - signalData.StartTime;

                if (timePast > signalData.EndTime)
                {
                    expiredSignals.Add(keyPair.Key);
                }
                else
                {
                    if (signalData.HapticFeedback.ContainsKey(timePast))
                    {
                        var hapticFeedbackData = signalData.HapticFeedback[timePast];
                        foreach (var feedback in hapticFeedbackData)
                        {
                            if (feedback.Mode == FeedbackMode.PATH_MODE && feedback.Position == PositionType.Left)
                            {
                                int prevSize = pathModeSignalLeft[0];

                                byte[] data = feedback.Values;
                                int size = data[0];
                                if (prevSize + size > 6)
                                {
                                    continue;
                                }

                                pathModeSignalLeft[0] = prevSize + size;

                                for (int i = prevSize; i < prevSize + size; i++)
                                {
                                    pathModeSignalLeft[3 * i + 1] = data[3 * (i - prevSize) + 1];
                                    pathModeSignalLeft[3 * i + 2] = data[3 * (i - prevSize) + 2];
                                    pathModeSignalLeft[3 * i + 3] = data[3 * (i - prevSize) + 3];
                                }

                                pathModeActiveLeft = true;
                            } else if (feedback.Mode == FeedbackMode.PATH_MODE && feedback.Position == PositionType.Right)
                            {
                                int prevSize = pathModeSignalRight[0];

                                byte[] data = feedback.Values;
                                int size = data[0];
                                if (prevSize + size > 6)
                                {
                                    continue;
                                }

                                pathModeSignalRight[0] = prevSize + size;

                                for (int i = prevSize; i < prevSize + size; i++)
                                {
                                    pathModeSignalRight[3 * i + 1] = data[3 * (i - prevSize) + 1];
                                    pathModeSignalRight[3 * i + 2] = data[3 * (i - prevSize) + 2];
                                    pathModeSignalRight[3 * i + 3] = data[3 * (i - prevSize) + 3];
                                }

                                pathModeActiveRight = true;
                            }
                            else if (feedback.Mode == FeedbackMode.DOT_MODE && feedback.Position == PositionType.Left)
                            {
                                for (int i = 0; i < _motorSize; i++)
                                {
                                    dotModeSignalLeft[i] += feedback.Values[i];
                                }
                                
                                dotModeLeftActive = true;
                            } else if (feedback.Mode == FeedbackMode.DOT_MODE && feedback.Position == PositionType.Right)
                            {
                                for (int i = 0; i < _motorSize; i++)
                                {
                                    dotModeSignalRight[i] += feedback.Values[i];
                                }
                                dotModeRightActive = true;
                            }
                        }
                    }
                }
            }
            
            foreach (string key in expiredSignals)
            {
                _activeSignals.Remove(key);
            }

            if (dotModeLeftActive)
            {
                byte[] data = new byte[_motorSize];
                for (int i = 0; i < _motorSize; i++)
                {
                    data[i] = (byte)dotModeSignalLeft[i];
                }

                TactosyFeedback feedback = new TactosyFeedback(PositionType.Left, data, FeedbackMode.DOT_MODE);
                PlayFeedback(feedback);
            } else if (pathModeActiveLeft)
            {
                byte[] data = new byte[_motorSize];
                for (int i = 0; i < _motorSize; i++)
                {
                    data[i] = (byte) pathModeSignalLeft[i];
                }
                TactosyFeedback feedback = new TactosyFeedback(PositionType.Left, data, FeedbackMode.PATH_MODE);
                PlayFeedback(feedback);
            }
            else
            {
                TactosyFeedback feedback = new TactosyFeedback(PositionType.Left, new byte[20], FeedbackMode.DOT_MODE);
                PlayFeedback(feedback);
            }

            if (dotModeRightActive)
            {
                byte[] data = new byte[_motorSize];
                for (int i = 0; i < _motorSize; i++)
                {
                    data[i] = (byte)dotModeSignalRight[i];
                }

                TactosyFeedback feedback = new TactosyFeedback(PositionType.Right, data, FeedbackMode.DOT_MODE);
                PlayFeedback(feedback);
            }
            else if (pathModeActiveRight)
            {
                byte[] data = new byte[_motorSize];
                for (int i = 0; i < _motorSize; i++)
                {
                    data[i] = (byte) pathModeSignalRight[i];
                }
                TactosyFeedback feedback = new TactosyFeedback(PositionType.Right, data, FeedbackMode.PATH_MODE);
                PlayFeedback(feedback);
            }
            else
            {
                TactosyFeedback feedback = new TactosyFeedback(PositionType.Right, new byte[20], FeedbackMode.DOT_MODE);
                PlayFeedback(feedback);
            }

            _currentTime += _interval;
        }

        void Start()
        {
            float intervalSec = _interval * 0.001f;
            InvokeRepeating("OnTimeUpdate", intervalSec, intervalSec);
        }

        /// <summary>
        /// Check if specific feedback is playing
        /// </summary>
        /// <param name="key"> feedback key</param>
        public bool IsPlaying(string key)
        {
            return _activeSignals.ContainsKey(key);
        }

        /// <summary>
        /// Check if feedback is playing
        /// </summary>
        public bool IsPlaying()
        {
            return _activeSignals.Count > 0;
        }

        /// <summary>
        /// Play Feedback
        /// </summary>
        /// <param name="feedback"> TactosyFeedback</param>
        private void PlayFeedback(TactosyFeedback feedback)
        {
            if (!Enable)
            {
                feedback.Mode = FeedbackMode.DOT_MODE;
                feedback.Values = new byte[20];
            }


#if UNITY_STANDALONE || UNITY_EDITOR
            if (_websocketConnected)
            {
                _webSocket.Send(TactosyFeedback.ToBytes(feedback));
            }
            else
            {
                Debug.Log("Tactosy Connection closed.");    
            }

#endif
#if (UNITY_ANDROID && !UNITY_EDITOR)

            if (javaObject != null)
            {
                byte[] bytes = feedback.Values;
                int position = feedback.Position == PositionType.Left ? 0 : 1;
                bool mode = feedback.Mode == FeedbackMode.PATH_MODE;

                javaObject.Call("setMotor", bytes, position, mode);
            }
#endif
        }

        /// <summary>
        /// SendSignal with individual motor values and feedback duration
        /// </summary>
        /// <param name="key"> key for TactosyFeedback</param>
        /// <param name="position"> position of tactosy</param>
        /// <param name="motorBytes"> motorBytes</param>
        /// <param name="durationMillis">TactosyFeedback duration millis</param>
        public void SendSignal(string key, PositionType position, byte[] motorBytes, int durationMillis)
        {
            TactosyFeedback feedback = new TactosyFeedback(position, motorBytes, FeedbackMode.DOT_MODE);
            _activeSignals[key] = new FeedbackSignal(feedback, durationMillis, _interval);
        }

        /// <summary>
        /// SendSignal wtih points and duration
        /// </summary>
        /// <param name="key"> key for feedback</param>
        /// <param name="position"> position of tactosy</param>
        /// <param name="points"> points array with x = [0, 1], y = [0, 1], intensity = [0, 1]</param>
        /// <param name="durationMillis"> duration millis</param>
        public void SendSignal(string key, PositionType position, List<PathPoint> points, int durationMillis)
        {
            if (points.Count > 6 || points.Count <= 0)
            {
                Debug.Log("number of points should be [1~6]");
                return;
            }

            byte[] bytes = new byte[20];
            bytes[0] = (byte)points.Count;

            for (var i = 0; i < points.Count; i++)
            {
                bytes[3 * i + 1] = (byte)Mathf.Min(40f, Mathf.Max(0f, (float)points[i].X * 40f)); // x
                bytes[3 * i + 2] = (byte)Mathf.Min(30f, Mathf.Max(0f, (float)points[i].Y * 40f)); // y
                bytes[3 * i + 3] = (byte)Mathf.Min(100f, Mathf.Max(0f, (float)points[i].Intensity * 100f)); // z

                Debug.Log(bytes[3 * i + 1] + " " + bytes[3 * i + 2] + " " + bytes[3 * i + 3]);
            }

            TactosyFeedback feedback = new TactosyFeedback(position, bytes, FeedbackMode.PATH_MODE);
            _activeSignals[key] = new FeedbackSignal(feedback, durationMillis, _interval);
        }

        /// <summary>
        /// SendSignal for tactosy file with duration and intensity
        /// </summary>
        /// /// <param name="key"> key for TactosyFeedback</param>
        /// <param name="intensity"> intensity ratio (0.01~100) </param>
        /// <param name="duration">duration ratio (0.01~100)</param>
        public void SendSignal(string key, float intensity, float duration)
        {
            if (!_registeredSignals.ContainsKey(key))
            {
                Debug.Log("Key : " + key + " is not registered.");

                return;
            }

            if (duration < 0.01f || duration > 100f)
            {
                Debug.Log("not allowed duration " + duration);
                return;
            }

            if (intensity < 0.01f || intensity > 100f)
            {
                Debug.Log("not allowed intensity " + duration);
                return;
            }


            FeedbackSignal signal = _registeredSignals[key];

            FeedbackSignal copiedFeedbackSignal = FeedbackSignal.Copy(signal, _interval, intensity, duration);

            _activeSignals[key] = copiedFeedbackSignal;
        }

        /// <summary>
        /// SendSignal tactosy file with key
        /// </summary>
        /// /// <param name="key"> key for TactosyFeedback</param>
        public void SendSignal(string key)
        {
            if (!_registeredSignals.ContainsKey(key))
            {
                Debug.Log("Key : " + key + " is not registered.");

                return;
            }

            var signal = _registeredSignals[key];

            signal.StartTime = -1;
            if (!_activeSignals.ContainsKey(key))
            {
                _activeSignals[key] = signal;
            }
        }

        /// <summary>
        /// TurnOff designated feedback
        /// </summary>
        /// /// <param name="key"> key for TactosyFeedback</param>
        public void TurnOff(string key)
        {
            if (!_activeSignals.ContainsKey(key))
            {
                Debug.Log("feedback with key( " + key + " ) is not playing.");
                return;
            }

            _activeSignals.Remove(key);
        }

        /// <summary>
        /// TurnOff All
        /// </summary>
        public void TurnOff()
        { 
            _activeSignals.Clear();
        }

        /// <summary>
        /// Toggle tactosy feedback Enable
        /// </summary>
        public void ToggleEnable()
        {
            TurnOff();
            Enable = !Enable;
        }

        public void Log(string msg)
        {
            Debug.Log(msg);
        }
    }
}

