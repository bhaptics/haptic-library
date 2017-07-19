﻿using System;
using System.Collections.Generic;

namespace Bhaptics.Tac
{
    public class FeedbackEvent
    {
        public delegate void HapticFeedbackChangeEvent(HapticFeedback feedback);
        public delegate void ConnectionEvent();
    }
    
    public interface IHapticPlayer
    {
        void Enable();
        void Disable();
        
        bool IsPlaying(string key);
        bool IsPlaying();
        
        void Register(string key, string path );
        void Register(string key, BufferedHapticFeedback tactosyFile);
        
        void Submit(string key, PositionType position, byte[] motorBytes, int durationMillis);
        void Submit(string key, PositionType position, List<DotPoint> points, int durationMillis);
        void Submit(string key, PositionType position, DotPoint point, int durationMillis);
        void Submit(string key, PositionType position, List<PathPoint> points, int durationMillis);
        void Submit(string key, PositionType position, PathPoint point, int durationMillis);

        void SubmitRegistered(string key, float intensity, float duration);
        void SubmitRegistered(string key);
        void SubmitRegistered(string key, float duration);

        void TurnOff(string key);
        void TurnOff();

        event FeedbackEvent.HapticFeedbackChangeEvent FeedbackChanged;
    }
}
