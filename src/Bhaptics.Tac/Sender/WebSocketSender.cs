using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using CustomWebSocketSharp;

namespace Bhaptics.Tac.Sender
{
    public class WebSocketSender : ISender
    {
        private WebSocket _webSocket;
        private bool _websocketConnected = false;
        private readonly string WebsocketUrl = "ws://127.0.0.1:15881/feedbacks";
        public event FeedbackEvent.HapticFeedbackChangeEvent FeedbackChangeReceived;
        public event FeedbackEvent.ConnectionEvent Connected;
        public event FeedbackEvent.ConnectionEvent Disconnected;
        private int retry = 0;
        private int retryCount = 5;

        public WebSocketSender(FeedbackEvent.ConnectionEvent connected, FeedbackEvent.ConnectionEvent disconnected, bool tryReconnect = true)
        {
            Connected = connected;
            Disconnected = disconnected;
            Initialize(tryReconnect);
        }

        public WebSocketSender(bool tryReconnect = true)
        {
            Initialize(tryReconnect);
        }

        private void Initialize(bool tryReconnect)
        {
            if (tryReconnect)
            {
                Timer timer = new Timer(3 * 1000); // 3 sec
                timer.Elapsed += delegate (object sender, ElapsedEventArgs args)
                {
                    if (!_websocketConnected)
                    {
                        retry++;

                        _webSocket.Connect();
                        if (retry >= retryCount)
                        {
                            timer.Stop();
                        }
                    }
                };
                timer.Start();
            }

            _webSocket = new WebSocket(WebsocketUrl);

            _webSocket.OnMessage += (sender, e) =>
            {
                var deserializeObject = SimpleJson.DeserializeObject<Dictionary<string, string>>(e.Data);

                try
                {
                    var position = deserializeObject["Position"];
                    PositionType t = (PositionType)int.Parse(position);
                    var values = deserializeObject["Values"];

                    var ints = SimpleJson.DeserializeObject<int[]>(values);
                    if (FeedbackChangeReceived != null)
                    {
                        byte[] bytes = new byte[ints.Length];
                        for (int i = 0; i < bytes.Length; i++)
                        {
                            bytes[i] = (byte)ints[i];
                        }
                        var feedback = new HapticFeedback(t, bytes, FeedbackMode.DOT_MODE);
                        FeedbackChangeReceived(feedback);
                    }
                }
                catch (Exception exception)
                {
                    Debug.WriteLine($"{exception.Message}");
                }
            };

            _webSocket.OnOpen += (sender, args) =>
            {
                Debug.WriteLine("Connected");
                if (Connected != null)
                {
                    Connected();
                }
                _websocketConnected = true;
            };

            _webSocket.OnError += (sender, args) =>
            {
                Debug.WriteLine("OnError " + args.Message);
            };

            _webSocket.OnClose += (sender, args) =>
            {
                Debug.WriteLine("Closed");
                if (Disconnected != null)
                {
                    Disconnected();
                }

                _websocketConnected = false;
            };

            _webSocket.Connect();
        }

        public void PlayFeedback(HapticFeedbackFrame feedback)
        {
            if (!_websocketConnected)
            {
                return;
            }

            try
            {
                var serializeObject = SimpleJson.SerializeObject(feedback);
                _webSocket.Send(serializeObject);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void Dispose()
        {
            try
            {
                _webSocket.CloseAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}
