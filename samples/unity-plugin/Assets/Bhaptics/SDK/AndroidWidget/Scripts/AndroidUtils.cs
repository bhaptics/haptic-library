using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bhaptics.Tact.Unity
{
    [Serializable]
    public class HapticDevice
    {
        public bool IsPaired;
        public bool IsConnected;
        public string DeviceName;
        public int Battery;
        public PositionType Position;
        public string ConnectionStatus;
        public int Rssi;
        public string Address;
        public PositionType[] Candidates;
    }

    public static class AndroidUtils
    {
        [Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }


        [Serializable]
        private class Device
        {
            public bool IsPaired;
            public string DeviceName;
            public int Battery;
            public string Position;
            public string ConnectionStatus;
            public int Rssi;
            public string Address;
        }

        private static PositionType ToDeviceType(string type)
        {
            switch (type)
            {
                case "Head":
                    return PositionType.Head;
                case "Vest":
                    return PositionType.Vest;
                case "ForearmL":
                    return PositionType.ForearmL;
                case "ForearmR":
                    return PositionType.ForearmR;
                case "HandL":
                    return PositionType.HandL;
                case "HandR":
                    return PositionType.HandR;
                case "FootL":
                    return PositionType.FootL;
                case "FootR":
                    return PositionType.FootR;

            }

            return PositionType.Vest;
        }

        private static PositionType[] ToCandidates(string type)
        {
            switch (type)
            {
                case "Head":
                    return new PositionType[] { PositionType.Head };
                case "Vest":
                    return new PositionType[] { PositionType.Vest };
                case "ForearmL":
                    return new PositionType[] { PositionType.ForearmL, PositionType.ForearmR };
                case "ForearmR":
                    return new PositionType[] { PositionType.ForearmL, PositionType.ForearmR };
                case "HandL":
                    return new PositionType[] { PositionType.HandL, PositionType.HandR };
                case "HandR":
                    return new PositionType[] { PositionType.HandL, PositionType.HandR };
                case "FootL":
                    return new PositionType[] { PositionType.FootR, PositionType.FootL };
                case "FootR":
                    return new PositionType[] { PositionType.FootR, PositionType.FootL };

            }

            return new PositionType[0];
        }

        public static bool IsLeft(PositionType pos)
        {
            return pos == PositionType.ForearmL
                   || pos == PositionType.FootL
                   || pos == PositionType.HandL;
        }

        public static bool CanChangePosition(PositionType pos)
        {
            return !(pos == PositionType.Head || pos == PositionType.Vest);
        }



        // https://answers.unity.com/questions/1123326/jsonutility-array-not-supported.html
        public static T[] GetJsonArray<T>(string json)
        {
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }


        private static HapticDevice Convert(Device d)
        {
            var isConnected = ConvertConnectionStatus(d.ConnectionStatus) == 0;
            return new HapticDevice()
            {
                IsPaired = d.IsPaired,
                IsConnected = isConnected,
                Address = d.Address,
                Battery = d.Battery,
                Position = ToDeviceType(d.Position),
                DeviceName = d.DeviceName,
                Rssi = d.Rssi,
                ConnectionStatus = d.ConnectionStatus,
                Candidates = ToCandidates(d.Position),

            };
        }

        public static bool CanPair(HapticDevice device, PositionType deviceType)
        {
            var containsInCandidates = false;
            for (var i = 0; i < device.Candidates.Length; i++)
            {
                var candi = device.Candidates[i];
                if (candi == deviceType)
                {
                    containsInCandidates = true;
                    break;
                }
            }

            return (containsInCandidates || device.Position == deviceType) &&
                   !device.IsPaired &&
                   ConvertConnectionStatus(device.ConnectionStatus) == 2;
        }

        public static List<HapticDevice> ConvertToBhapticsDevices(string deviceJson)
        {
            var res = new List<HapticDevice>();

            var devices = GetJsonArray<Device>(deviceJson);

            foreach (var d in devices)
            {
                res.Add(Convert(d));
            }

            return res;
        }

        public static int ConvertConnectionStatus(string status)
        {
            if (status == "Connected")
            {
                return 0;
            }
            else if (status == "Connecting")
            {
                return 1;
            }
            else if (status == "Disconnected")
            {
                return 2;
            }
            return 3;
        }



        public static void CallNativeVoidMethod(IntPtr androidObjPtr, IntPtr methodPtr, object[] param)
        {
            jvalue[] args = AndroidJNIHelper.CreateJNIArgArray(param);
            try
            {
                AndroidJNI.CallVoidMethod(androidObjPtr, methodPtr, args);
            }
            catch (Exception e)
            {
                BhapticsLogger.LogError("CallNativeVoidMethod() : {0}", e.Message);
            }
            finally
            {
                AndroidJNIHelper.DeleteJNIArgArray(param, args);
            }
        }


        public static bool CallNativeBoolMethod(IntPtr androidObjPtr, IntPtr methodPtr, object[] param)
        {
            jvalue[] args = AndroidJNIHelper.CreateJNIArgArray(param);
            bool res = false;
            try
            {
                res = AndroidJNI.CallBooleanMethod(androidObjPtr, methodPtr, args);
            }
            catch (Exception e)
            {
                BhapticsLogger.LogError("CallNativeBoolMethod() : {0}", e.Message);
            }
            finally
            {
                AndroidJNIHelper.DeleteJNIArgArray(param, args);
            }

            return res;
        }
    }

}