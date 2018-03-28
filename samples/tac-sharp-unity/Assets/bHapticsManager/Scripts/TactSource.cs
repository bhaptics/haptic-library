using System;
using UnityEngine;
using System.Collections.Generic;

namespace Bhaptics.Tact.Unity
{
    public class TactSource : MonoBehaviour
    {
        private IHapticPlayer player;
        
        private string _key;

        [HideInInspector]
        public FeedbackType FeedbackType = FeedbackType.TactFile;
        
        [HideInInspector]
        public Pos Position = Pos.RightArm;

        // PathMode
        [HideInInspector]
        public Point[] Points = {new Point(0.5f, 0.5f, 100) };

        // DotMode
        [HideInInspector]
        public byte[] DotPoints = new byte[20];

        [HideInInspector]
        public int TimeMillis = 1000;

        #region TactFile

        [HideInInspector]
        public float Intensity = 1;
        [HideInInspector]
        public float Duration = 1;

        [HideInInspector] public bool IsReflectTactosy;

        private bool isRegistered;

        [HideInInspector]
        public float VestRotationAngleX;

        [HideInInspector]
        public float VestRotationOffsetY;

        [HideInInspector]
        public float TactFileOffsetX;

        [HideInInspector]
        public float TactFileOffsetY;

        [HideInInspector]
        [SerializeField]
        public FeedbackFile FeedbackFile;

        #endregion

        void Awake()
        {
            _key = GetInstanceID() + "";
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
                    if (FeedbackFile.Type == BhapticsUtils.TypeVest)
                    {
                        player.SubmitRegisteredVestRotation(FeedbackFile.Id, _key, 
                            new RotationOption(VestRotationAngleX + TactFileOffsetX, VestRotationOffsetY + TactFileOffsetY),
                            option);

                    } else if (FeedbackFile.Type == BhapticsUtils.TypeTactosy && IsReflectTactosy)
                    {
                        if (!isRegistered)
                        {
                            isRegistered = true;
                            var project = BhapticsUtils.ReflectLeftRight(FeedbackFile.Value);

                            player.Register(FeedbackFile.Id + "Reflect", project);
                        }
                        player.SubmitRegistered(FeedbackFile.Id + "Reflect", _key, option);
                    }
                    else
                    {
                        player.SubmitRegistered(FeedbackFile.Id, _key, option);
                    }

                    break;
            }
        }

        public void Stop()
        {
            player.TurnOff(_key);
        }

        public void AddPathPoint()
        {
            var list = new List<Point>(Points);
            list.Add(new Point(0.5f, 0.5f, 100));
            Points = list.ToArray();
        }

        public void RemovePathPoint(int index)
        {
            var list = new List<Point>(Points);
            list.RemoveAt(index);
            Points = list.ToArray();
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
                case Pos.HandL:
                    return PositionType.HandL;
                case Pos.HandR:
                    return PositionType.HandR;
                case Pos.FootL:
                    return PositionType.FootL;
                case Pos.FootR:
                    return PositionType.FootR;
                case Pos.Racket:
                    return PositionType.Racket;
            }

            return PositionType.Right;
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
        RightArm, LeftArm, VestFront, VestBack, Head, HandL, HandR, FootL, FootR, Racket
    }
}