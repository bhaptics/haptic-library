using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bhaptics.Tact.Unity
{
    [Serializable]
    class AndroidPlayerResponse
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
    class AndroidStatus
    {
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
    class AndroidSubmitRequest
    {
        public string key;
        public string type;
    }

    [Serializable]
    class AndroidParamSubmitRequest
    {
        public string key;
        public string type;
        public AndroidParameters parameters;
    }
    [Serializable]
    class AndroidParamStartTimeSubmitRequest
    {
        public string key;
        public string type;
        public AndroidStartTimeParameters parameters;
    }

    [Serializable]
    class AndroidParameters
    {
        public AndroidRotationOption rotationOption;
        public AndroidScaleOption scaleOption;
        public string altKey;
    }
    [Serializable]

    class AndroidStartTimeParameters
    {
        public string startTimeMillis;
    }

    [Serializable]
    class AndroidRotationOption
    {
        public float offsetAngleX = 0;
        public float offsetY = 0;
    }

    [Serializable]
    class AndroidScaleOption
    {
        public float intensity = 0;
        public float duration = 0;
    }

    [Serializable]
    class AndroidFrameRequest
    {
        public string key;
        public string type = "frame";
        public AndroidFrame frame;
    }

    [Serializable]
    class AndroidFrame
    {
        public int durationMillis;
        public string position;
        public AndroidPathPoint[] pathPoints;
        public AndroidDotPoint[] dotPoints;
    }

    [Serializable]
    class AndroidPathPoint
    {
        public float x;
        public float y;
        public int intensity;
        public int motorCount = 3;
    }

    [Serializable]
    class AndroidDotPoint
    {
        public int index;
        public int intensity;
    }
}
