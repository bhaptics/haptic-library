using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bhaptics.Tact.Unity
{
    [Serializable]
    struct AndroidPlayerResponse
    {
        public string[] RegisteredKeys;
        public string[] ActiveKeys;
        public string[] ConnectedPositions;
        public int ConnectedDeviceCount;
        public AndroidStatus Status;


        public static PlayerResponse ConvertToPlayerResponse(string message)
        {

            var playerResponse2 = JsonUtility.FromJson<AndroidPlayerResponse>(message);

            List<PositionType> type = new List<PositionType>();

            for (var i = 0; i < playerResponse2.ConnectedPositions.Length; i++)
            {
                type.Add(EnumParser.ToPositionType(playerResponse2.ConnectedPositions[i]));
            }
            var response = new PlayerResponse();
            response.ActiveKeys = new List<string>(playerResponse2.ActiveKeys);
            response.RegisteredKeys = new List<string>(playerResponse2.RegisteredKeys);
            response.ConnectedPositions = type;
            response.ConnectedDeviceCount = playerResponse2.ConnectedDeviceCount;
            response.Status = new Dictionary<string, int[]>();

            response.Status["Head"] = playerResponse2.Status.Head;
            response.Status["VestFront"] = playerResponse2.Status.VestFront;
            response.Status["VestBack"] = playerResponse2.Status.VestBack;
            response.Status["ForearmL"] = playerResponse2.Status.ForearmL;
            response.Status["ForearmR"] = playerResponse2.Status.ForearmR;
            response.Status["HandL"] = playerResponse2.Status.HandL;
            response.Status["HandR"] = playerResponse2.Status.HandR;
            response.Status["FootL"] = playerResponse2.Status.FootL;
            response.Status["FootR"] = playerResponse2.Status.FootR;

            return response;
        }
    }

    [Serializable]
    struct AndroidStatus
    {
        public int[] Head;
        public int[] VestFront;
        public int[] VestBack;
        public int[] ForearmL;
        public int[] ForearmR;
        public int[] HandL;
        public int[] HandR;
        public int[] FootL;
        public int[] FootR;
    }




    /// <summary>
    /// Request
    /// </summary>
    [Serializable]
    struct AndroidFrameRequest
    {
        public string key;
        public string type;
        public AndroidFrame frame;
    }

    [Serializable]
    struct AndroidFrame
    {
        public int durationMillis;
        public string position;
        public AndroidPathPoint[] pathPoints;
        public AndroidDotPoint[] dotPoints;
    }

    [Serializable]
    struct AndroidPathPoint
    {
        public float x;
        public float y;
        public int intensity;
        public int motorCount;
    }

    [Serializable]
    struct AndroidDotPoint
    {
        public int index;
        public int intensity;
    }
}
