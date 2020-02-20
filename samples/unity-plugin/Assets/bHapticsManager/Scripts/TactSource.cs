using System;
using UnityEngine;
using System.Collections.Generic;

namespace Bhaptics.Tact.Unity
{
    [Serializable]
    public class TactSource : MonoBehaviour
    {
        private IHapticPlayer player;
        
        private string _key;

        [HideInInspector]
        public FeedbackType FeedbackType = FeedbackType.TactFile;
        
        [HideInInspector]
        [SerializeField]
        public Pos Position = Pos.VestFront;

        // PathMode
        //[HideInInspector]
        [SerializeField]
        public Point[] Points; // = {new Point(0.5f, 0.5f, 100) };
        
        // DotMode
        [HideInInspector]
        [SerializeField]
        public byte[] DotPoints = new byte[20];

        [HideInInspector]
        [Range(20, 10000)]
        public int TimeMillis = 1000;

        #region TactFile

        [HideInInspector]
        [Range(0.2f, 5f)]
        public float Intensity = 1;
        [HideInInspector]
        [Range(0.2f, 5f)]
        public float Duration = 1;

        [HideInInspector] [SerializeField] public bool IsReflectTactosy;

        [HideInInspector]
        public float VestRotationAngleX;

        [HideInInspector]
        public float VestRotationOffsetY;

        [HideInInspector]
        [Range(0, 360)]
        public float TactFileOffsetX;

        [HideInInspector]
        [Range(-1, 1)]
        public float TactFileOffsetY;

        [HideInInspector]
        [SerializeField]
        public FeedbackFile FeedbackFile;

        #endregion

        private string currentFeedbackFileKey = "";
        private string currentFeedbackFileKeyReflect = "";


        void Awake()
        {
            _key = GetInstanceID() + "";

            if (DotPoints == null || DotPoints.Length != 20)
            {
                DotPoints = new byte[20];
            }

            if (Points == null)
            {
                Points = new Point[] { new Point(0.5f, 0.5f, 100) };
            }
        }

        void OnEnable()
        {
            player = BhapticsManager.HapticPlayer;
            VestRotationAngleX = 0;
            VestRotationOffsetY = 0;
        }

        public bool IsPlaying()
        {
            return player.IsPlaying(_key);
        }

        internal void SetKey(string key)
        {
            _key = key;
        }

        public void Play(ScaleOption option = null)
        {
            if (option == null)
            {
                option = new ScaleOption(Intensity, Duration);
            }
            switch (FeedbackType)
            {
                case FeedbackType.DotMode:
                    if (DotPoints == null)
                    {
                        Debug.LogError("DotPoints not defined");
                        return;
                    }
                    player.Submit(_key, ToPositionType(Position), DotPoints, TimeMillis);
                    break;
                case FeedbackType.PathMode:
                        if (Points == null)
                        {
                            Debug.LogError("Points not defined");
                            return;
                        }
                        player.Submit(_key, ToPositionType(Position), new List<PathPoint>(Convert(Points)), TimeMillis);
                    break;
                case FeedbackType.TactFile:
                    if (currentFeedbackFileKey != FeedbackFile.Id)
                    {
                        currentFeedbackFileKey = FeedbackFile.Id;
                        player.RegisterTactFileStr(currentFeedbackFileKey, FeedbackFile.Value);
                    }

                    if (FeedbackFile.Type == BhapticsUtils.TypeVest || FeedbackFile.Type == BhapticsUtils.TypeTactot)
                    {

                        player.SubmitRegisteredVestRotation(currentFeedbackFileKey, _key, 
                            new RotationOption(VestRotationAngleX + TactFileOffsetX, VestRotationOffsetY + TactFileOffsetY),
                            option);

                    } else if (IsReflectTactosy && (FeedbackFile.Type == BhapticsUtils.TypeTactosy ||
                                                    FeedbackFile.Type == BhapticsUtils.TypeTactosy2))
                    {
                        var reflectKey = FeedbackFile.Id + "Reflect";
                        if (currentFeedbackFileKeyReflect != reflectKey)
                        {
                            player.RegisterTactFileStrReflected(reflectKey,
                                FeedbackFile.Value);
                            currentFeedbackFileKeyReflect = reflectKey;
                        }
                        player.SubmitRegistered(reflectKey, _key, option);
                    }
                    else
                    {
                        player.SubmitRegistered(currentFeedbackFileKey, _key, option);
                    }

                    break;
            }
        }


        // For Windows, it will be working with bHaptics Player 1.5.6 onwards. 
        public void PlayWithStartMillis(int mediaMillis)
        {
            if (currentFeedbackFileKey != FeedbackFile.Id)
            {
                currentFeedbackFileKey = FeedbackFile.Id;
                player.RegisterTactFileStr(currentFeedbackFileKey, FeedbackFile.Value);
            }

            player.SubmitRegistered(currentFeedbackFileKey, mediaMillis);
        }

        public void Play(RotationOption rotation)
        {
            if (currentFeedbackFileKey != FeedbackFile.Id)
            {
                currentFeedbackFileKey = FeedbackFile.Id;
                player.RegisterTactFileStr(currentFeedbackFileKey, FeedbackFile.Value);
            }

            player.SubmitRegisteredVestRotation(currentFeedbackFileKey, _key,
                rotation,
                new ScaleOption(Intensity, Duration));
        }

        public void Stop()
        {
            player.TurnOff(_key);
        }

        private static List<PathPoint> Convert(Point[] points)
        {
            var result = new List<PathPoint>();

            foreach (var point in points)
            {
                result.Add(new PathPoint(point.X, point.Y, point.Intensity));
            }

            return result;
        }

        static PositionType ToPositionType(Pos pos)
        {
            switch (pos)
            {
                case Pos.RightArm:
                    return PositionType.Right;
                case Pos.LeftArm:
                    return PositionType.Left;
                case Pos.Head:
                    return PositionType.Head;
                case Pos.VestFront:
                    return PositionType.VestFront;
                 case Pos.VestBack:
                    return PositionType.VestBack;
                case Pos.LeftHand:
                    return PositionType.HandL;
                case Pos.RightHand:
                    return PositionType.HandR;
                case Pos.LeftFoot:
                    return PositionType.FootL;
                case Pos.RightFoot:
                    return PositionType.FootR;
                case Pos.RightForearm:
                    return PositionType.ForearmR;
                case Pos.LeftForearm:
                    return PositionType.ForearmL;
            }

            return PositionType.ForearmR;
        }
    }

    [Serializable]
    public class Point
    {
        [Range(0, 1f)]
        public float X;
        [Range(0, 1f)]
        public float Y;

        [Range(0, 100)]
        public int Intensity;

        public Point(float x, float y, int intensity)
        {
            X = x;
            Y = y;
            Intensity = intensity;
        }
    }

    [Serializable]
    public enum FeedbackType
    {
        TactFile = 0, DotMode = 1, PathMode = 2
    }

    [Serializable]
    public enum Pos
    {
        Head, VestFront, VestBack, RightArm, LeftArm, LeftHand, RightHand, LeftFoot, RightFoot, RightForearm, LeftForearm
    }
}