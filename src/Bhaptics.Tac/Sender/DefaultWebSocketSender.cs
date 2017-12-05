using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using Bhaptics.fastJSON;
using Bhaptics.Tac.Designer;
using CustomWebSocketSharp;

namespace Bhaptics.Tac.Sender
{
    public class DefaultWebSocketSender
    {
        private WebSocket _webSocket;
        private bool _websocketConnected = false;
        private readonly string WebsocketUrl = "ws://127.0.0.1:15881/v2/feedbacks";

        public event FeedbackEvent.StatusReceivedEvent StatusReceived;
        public event FeedbackEvent.ConnectionEvent ConnectionChanged;

        #region Initilization
        private int _retryCount = 0;
        private const int MaxRetryCount = 5;
        private Timer _timer;

        // We need copy when conncted
        private readonly List<RegisterRequest> _registered;
        private bool _enable;

        private readonly JSONParameters DEFAULT_PARAM = new JSONParameters
        {
            EnableAnonymousTypes = true,
            UsingGlobalTypes = false,
            UseValuesOfEnums = false,
            SerializeToLowerCaseNames = false,
            UseExtensions = true
        };
        private PlayerRequest _activeRequest;

        public DefaultWebSocketSender()
        {
            _registered = new List<RegisterRequest>();
        }

        public void Initialize(bool tryReconnect)
        {
            if (_webSocket != null)
            {
                Debug.WriteLine("Initialized");
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
                Debug.WriteLine("OnError " + args.Message);
            };

            _webSocket.OnClose += (sender, args) =>
            {
                Debug.WriteLine("Closed.");
                _websocketConnected = false;
                ConnectionChanged?.Invoke(_websocketConnected);
            };

            _webSocket.Connect();
            _enable = true;
        }

        private void TimerOnElapsed(object o, ElapsedEventArgs args)
        {
            if (!_websocketConnected)
            {
                Debug.WriteLine("RetryConnect()");
                _retryCount++;
                _webSocket.Connect();
                if (_retryCount >= MaxRetryCount)
                {
                    _timer.Stop();
                }
            }
        }

        public void Enable()
        {
            _enable = true;
        }

        public void Disable()
        {
            _enable = false;
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
        #endregion

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
            Debug.WriteLine("Connected");
            _websocketConnected = true;

            AddRegister(_registered);
            ConnectionChanged?.Invoke(_websocketConnected);
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

        public void SubmitRegistered(string key, float intensityRatio, float durationRatio)
        {
            var submitRequest = new SubmitRequest
            {
                Key = key,
                Type = "key",
                Parameters = new Dictionary<string, object>
                {
                    { "intensityRatio", intensityRatio},
                    { "durationRatio", durationRatio}
                }
            };

            AddSubmit(submitRequest);
        }


        public void SubmitRegistered(string key, TransformOption option)
        {
            var submitRequest = new SubmitRequest
            {
                Key = key,
                Type = "key",
                Parameters = new Dictionary<string, object>
                {
                    { "transformOption", option},
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
                Debug.WriteLine($"Getting request failed.");
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
                Debug.WriteLine($"Getting request failed.");
                return;
            }

            request.Register.AddRange(req);

            Send();
        }

        private void Send()
        {
            try
            {
                if (!_websocketConnected)
                {
                    Debug.WriteLine("not connected.");
                    return;
                }

                var msg = JSON.ToJSON(GetActiveRequest(), DEFAULT_PARAM);
                _webSocket.SendAsync(msg, b =>
                {
                    _activeRequest = null;
                });
            }
            catch (Exception e)
            {
               Debug.WriteLine($"{e.Message}");
            }
        }
    }

    public class PlayerRequest
    {
        public List<RegisterRequest> Register;
        public List<SubmitRequest> Submit;

        public static PlayerRequest Create()
        {
            return new PlayerRequest
            {
                Register = new List<RegisterRequest>(),
                Submit = new List<SubmitRequest>()
            };
        }
    }

    public class RegisterRequest
    {
        public string Key { get; set; }
        public Project Project { get; set; }
    }

    public class SubmitRequest
    {
        public string Type { get; set; }
        public string Key { get; set; }
        public Dictionary<string, object> Parameters { get; set; } // durationRatio
        public Frame Frame { get; set; }
    }

    public class TransformOption
    {
        public double DeltaX { get; set; }
        public double DeltaY { get; set; }
        public bool IsValueRotate { get; set; }

        public TransformOption(double deltaX, double deltaY, bool isValueRotate = true)
        {
            DeltaX = deltaX;
            DeltaY = deltaY;
            IsValueRotate = isValueRotate;
        }
    }

    public class PlayerResponse
    {
        public List<string> RegisteredKeys { get; set; }
        public List<string> ActiveKeys { get; set; }
        public int ConnectedDeviceCount { get; set; }
        public List<PositionType> ConnectedPositions { get; set; }
        public Dictionary<string, int[]> Status { get; set; }
    }

    public class Frame
    {
        public int DurationMillis { get; set; }
        public PositionType Position { get; set; }
        public List<PathPoint> PathPoints { get; set; }
        public List<DotPoint> DotPoints { get; set; }
        public int Texture { get; set; }

        public static Frame AsPathPointFrame(List<PathPoint> points, 
            PositionType position, int durationMillis, int texture = 0)
        {
            var frame = new Frame
            {
                Position =  position,
                DurationMillis = durationMillis,
                PathPoints =  points,
                DotPoints = new List<DotPoint>(),
                Texture = texture
            };

            return frame;
        }

        public static Frame AsDotPointFrame(List<DotPoint> points,
            PositionType position, int durationMillis, int texture = 0)
        {
            var frame = new Frame
            {
                Position = position,
                DurationMillis = durationMillis,
                PathPoints = new List<PathPoint>(),
                DotPoints = points,
                Texture = texture
            };

            return frame;
        }
    }
}
