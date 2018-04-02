using System;
using System.Collections.Generic;
using System.Diagnostics;

#if NETFX_CORE
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.Threading.Tasks;
#else 
using Bhaptics.fastJSON;
using CustomWebSocketSharp;
using System.Timers;
#endif

namespace Bhaptics.Tact
{
    public class WebSocketSender : ISender
    {
#if NETFX_CORE
        private MessageWebSocket messageWebSocket;
        private DataWriter messageWriter;

        private async void Connect()
        {
            try
            {
                if (messageWebSocket == null)
                {
                    messageWebSocket = new MessageWebSocket();
                    messageWebSocket.Control.MessageType = SocketMessageType.Utf8;
                    messageWebSocket.MessageReceived += (sender, args) =>
                    {
                        using (DataReader reader = args.GetDataReader())
                        {
                            reader.UnicodeEncoding = UnicodeEncoding.Utf8;

                            try
                            {
                                string read = reader.ReadString(reader.UnconsumedBufferLength);
                                var response = PlayerResponse.ToObject(read);
                                StatusReceived?.Invoke(response);
                            }
                            catch (Exception ex)
                            {
                                LogReceived?.Invoke(ex.Message);
                            }
                        }
                    };
                    messageWebSocket.Closed += (sender, args) =>
                    {
                        _websocketConnected = false;
                        ConnectionChanged?.Invoke(_websocketConnected);
                    };
                }

                await messageWebSocket.ConnectAsync(new Uri(WebsocketUrl));
                messageWriter = new DataWriter(messageWebSocket.OutputStream);

                _websocketConnected = true;
                ConnectionChanged?.Invoke(_websocketConnected);
                AddRegister(_registered);
            }
            catch (Exception)
            {
                _websocketConnected = false;
                ConnectionChanged?.Invoke(_websocketConnected);
                await Task.CompletedTask;
            }
        }

#else 
        private WebSocket _webSocket;
        private Timer _timer;
        private readonly JSONParameters DEFAULT_PARAM = new JSONParameters
        {
            EnableAnonymousTypes = true,
            UsingGlobalTypes = false,
            UseValuesOfEnums = false,
            SerializeToLowerCaseNames = false,
            UseExtensions = true
        };

        private void TimerOnElapsed(object o, ElapsedEventArgs args)
        {
            if (!_websocketConnected)
            {
                if (!_enable)
                {
                    return;
                }
                Console.Write("RetryConnect()\n");
                _retryCount++;
                _webSocket.Connect();
                if (_retryCount >= MaxRetryCount)
                {
                    _timer.Stop();
                }
            }
        }

        private void MessageReceived(object o, MessageEventArgs args)
        {
            if (!_enable)
            {
                return;
            }
            var msg = args.Data;

            var response = JSON.ToObject<PlayerResponse>(msg);
            StatusReceived?.Invoke(response);

        }
#endif


        private bool _websocketConnected = false;
        private readonly string WebsocketUrl = "ws://127.0.0.1:15881/v2/feedbacks";

        public event Action<PlayerResponse> StatusReceived;
        public event Action<bool> ConnectionChanged;
        public event Action<string> LogReceived;
        
        private int _retryCount = 0;
        private const int MaxRetryCount = 5;
        

        // We need copy when conncted
        private readonly List<RegisterRequest> _registered;
        private bool _enable;

        
        private PlayerRequest _activeRequest;

        public WebSocketSender()
        {
            _registered = new List<RegisterRequest>();
        }

        public void Initialize(bool tryReconnect)
        {
#if NETFX_CORE
            Connect();
#else
            if (_webSocket != null)
            {
                Console.Write("Initialized\n");
                return;
            }

            if (tryReconnect)
            {
                _timer = new Timer(3 * 1000); // 3 sec
                _timer.Elapsed += TimerOnElapsed;
                _timer.Start();
            }

            _webSocket = new WebSocket(WebsocketUrl);

            _webSocket.OnMessage += MessageReceived;
            _webSocket.OnOpen += OnConnected;
            _webSocket.OnError += (sender, args) =>
            {
                Console.Write($"OnError {args.Message }\n");
            };

            _webSocket.OnClose += (sender, args) =>
            {
                Console.Write("Closed.\n");
                _websocketConnected = false;
                ConnectionChanged?.Invoke(_websocketConnected);
            };

            _webSocket.Connect();
#endif
            _enable = true;
        }

        public void Enable()
        {
            _enable = true;
#if NETFX_CORE
#else
            _timer.Start();
#endif
        }

        public void Disable()
        {
            _enable = false;
#if NETFX_CORE
#else
            _timer.Stop();
#endif
        }

        public void Dispose()
        {
#if NETFX_CORE
            if (messageWriter != null)
            {
                // In order to reuse the socket with another DataWriter, the socket's output stream needs to be detached.
                // Otherwise, the DataWriter's destructor will automatically close the stream and all subsequent I/O operations
                // invoked on the socket's output stream will fail with ObjectDisposedException.
                //
                // This is only added for completeness, as this sample closes the socket in the very next code block.
                messageWriter.DetachStream();
                messageWriter.Dispose();
                messageWriter = null;
            }

            if (messageWebSocket != null)
            {
                try
                {
                    messageWebSocket.Close(1000, "Closed due to user request.");
                }
                catch (Exception ex)
                {
                    LogReceived?.Invoke(ex.Message);
                }
            }
#else
            try
            {
                _webSocket.CloseAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
#endif

        }

        private PlayerRequest GetActiveRequest()
        {
            if (_activeRequest == null)
            {
                _activeRequest = PlayerRequest.Create();
            }

            return _activeRequest;
        }

        private void OnConnected(object o, EventArgs args)
        {
            _websocketConnected = true;

            AddRegister(_registered);
            ConnectionChanged?.Invoke(_websocketConnected);
        }

        public void TurnOff(string key)
        {
            if (!_enable)
            {
                return;
            }

            var req = new SubmitRequest
            {
                Key = key,
                Type = "turnOff"
            };
            AddSubmit(req);
        }

        public void TurnOff()
        {
            if (!_enable)
            {
                return;
            }

            var req = new SubmitRequest
            {
                Type = "turnOffAll"
            };
            AddSubmit(req);
        }

        public void Register(string key, Project project)
        {
            var req = new RegisterRequest
            {
                Key = key,
                Project = project
            };
            _registered.Add(req);

            AddRegister(req);
        }

        public void SubmitRegistered(string key)
        {
            if (!_enable)
            {
                return;
            }

            var submitRequest = new SubmitRequest
            {
                Key = key,
                Type = "key"
            };
            AddSubmit(submitRequest);
        }

        public void SubmitRegistered(string key, float ratio)
        {
            var submitRequest = new SubmitRequest
            {
                Key = key,
                Type = "key",
                Parameters = new Dictionary<string, object>
                {
                    { "ratio", ratio}
                }
            };

            AddSubmit(submitRequest);
        }

        public void SubmitRegistered(string key, string altKey, ScaleOption option)
        {
            var submitRequest = new SubmitRequest
            {
                Key = key,
                Type = "key",
                Parameters = new Dictionary<string, object>
                {
                    { "scaleOption", option},
                    { "altKey", altKey}
                }
            };

            AddSubmit(submitRequest);
        }


        public void SubmitRegistered(string key, string altKey, RotationOption option, ScaleOption sOption)
        {
            var submitRequest = new SubmitRequest
            {
                Key = key,
                Type = "key",
                Parameters = new Dictionary<string, object>
                {
                    { "rotationOption", option},
                    { "scaleOption", sOption},
                    { "altKey", altKey}
                }
            };

            AddSubmit(submitRequest);
        }

        public void Submit(string key, Frame req)
        {
            if (!_enable)
            {
                return;
            }

            var submitRequest = new SubmitRequest
            {
                Frame = req,
                Key = key,
                Type = "frame"
            };
            AddSubmit(submitRequest);
        }

        private void AddSubmit(SubmitRequest submitRequest)
        {
            var request = GetActiveRequest();

            if (request == null)
            {
                return;
            }

            request.Submit.Add(submitRequest);

            Send();
        }

        private void AddRegister(RegisterRequest req)
        {
            AddRegister(new List<RegisterRequest> { req });
        }

        private void AddRegister(List<RegisterRequest> req)
        {
            var request = GetActiveRequest();

            if (request == null)
            {
                return;
            }

            request.Register.AddRange(req);

            Send();
        }

        private void Send()
        {
#if NETFX_CORE
            try
            {
                if (!_websocketConnected)
                {
                    return;
                }

                var req = GetActiveRequest();
                
                var jsonStr = req.ToJsonObject().Stringify();
                LogReceived?.Invoke("Send() " + jsonStr);
                messageWriter.WriteString(jsonStr);
                messageWriter.StoreAsync();
                _activeRequest = null;

            }
            catch (Exception e)
            {
                LogReceived?.Invoke("Send() " + e.Message);
            }
#else

            try
            {
                if (!_websocketConnected)
                {
                    Console.Write("not connected.\n");
                    return;
                }

                var msg = JSON.ToJSON(GetActiveRequest(), DEFAULT_PARAM);
                Debug.WriteLine("Send() String " + msg);
                _webSocket.SendAsync(msg, b =>
                {
                    _activeRequest = null;
                });
            }
            catch (Exception e)
            {
                Console.Write($"{e.Message} {e}\n");
            }
#endif
        }
    }
}
