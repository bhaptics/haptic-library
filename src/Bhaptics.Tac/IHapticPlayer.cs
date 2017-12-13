using System;
using System.Collections.Generic;
using Bhaptics.Tac.Designer;
using Bhaptics.Tac.Sender;

namespace Bhaptics.Tac
{
    public class FeedbackEvent
    {
        public delegate void StatusReceivedEvent(PlayerResponse feedback);
        public delegate void ConnectionEvent(bool isConnected);
    }
    
    public interface IHapticPlayer : IDisposable
    {
        void Enable();
        void Disable();
        
        bool IsActive(PositionType type);

        bool IsPlaying(string key);
        bool IsPlaying();
        
        void Register(string key, string path );
        void Register(string key, Project project );
        
        void Submit(string key, PositionType position, byte[] motorBytes, int durationMillis);
        void Submit(string key, PositionType position, List<DotPoint> points, int durationMillis);
        void Submit(string key, PositionType position, DotPoint point, int durationMillis);
        void Submit(string key, PositionType position, List<PathPoint> points, int durationMillis);
        void Submit(string key, PositionType position, PathPoint point, int durationMillis);

        void SubmitRegistered(string key, float intensity, float duration);
        void SubmitRegistered(string key, TransformOption option);
        void SubmitRegistered(string key);
        void SubmitRegistered(string key, float duration);

        void TurnOff(string key);
        void TurnOff();
        
        event FeedbackEvent.StatusReceivedEvent StatusReceived;
    }
}
