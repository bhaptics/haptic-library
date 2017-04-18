using System;
using System.Diagnostics;
using System.Timers;
using CustomWebSocketSharp;

namespace Tactosy.Common.Sender
{
    public class WebSocketSender : ISender
    {
        private WebSocket _webSocket;
        private bool _websocketConnected = false;
        private readonly string WebsocketUrl = "ws://127.0.0.1:15881/feedbackBytes";

        public WebSocketSender(bool tryReconnect = true)
        {
            if (tryReconnect)
            {
                Timer timer = new Timer(5 * 1000); // 5 sec
                timer.Elapsed += delegate(object sender, ElapsedEventArgs args)
                {
                    if (!_websocketConnected)
                    {
                        _webSocket.Connect();
                    }
                };
                timer.Start();
            }

            _webSocket = new WebSocket(WebsocketUrl);

            _webSocket.OnMessage += (sender, e) =>
            {
                Debug.WriteLine("Message Received : " + e.Data);
            };

            _webSocket.OnOpen += (sender, args) =>
            {
                Debug.WriteLine("Connected");
                _websocketConnected = true;
            };

            _webSocket.OnError += (sender, args) =>
            {
                Debug.WriteLine("OnError " + args.Message);
            };

            _webSocket.OnClose += (sender, args) =>
            {
                Debug.WriteLine("Closed");
                _websocketConnected = false;
            };

            _webSocket.Connect();
        }

        public void PlayFeedback(TactosyFeedback feedback)
        {
            if (!_websocketConnected)
            {
                return;
            }

            try
            {
                _webSocket.Send(TactosyFeedback.ToBytes(feedback));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }
    }
}
