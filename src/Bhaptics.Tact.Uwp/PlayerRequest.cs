using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bhaptics.Tact
{
#if NETFX_CORE
    using Windows.Data.Json;

    public interface IParsable
    {
        JsonObject ToJsonObject();
    }

#endif
    public class PlayerRequest
#if NETFX_CORE
        : IParsable
#endif
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

#if NETFX_CORE

        public JsonObject ToJsonObject()
        {
            var requestArray = new JsonArray();

            foreach (var registerRequest in Register)
            {
                JsonObject obj = new JsonObject();
                obj.SetNamedValue("Key", JsonValue.CreateStringValue(registerRequest.Key));
                obj.SetNamedValue("Project", registerRequest.Project.ToJsonObject());
                requestArray.Add(obj);
            }
            var array = new JsonArray();
            foreach (var submitRequest in Submit)
            {
                array.Add(submitRequest.ToJsonObject());
            }

            JsonObject jsonObject = new JsonObject();
            jsonObject.SetNamedValue("Register", requestArray);
            jsonObject.SetNamedValue("Submit", array);

            return jsonObject;
        }

#endif
    }

    public class RegisterRequest
    {
        public string Key { get; set; }
        public Project Project { get; set; }
    }

    public class SubmitRequest
#if NETFX_CORE
        : IParsable
#endif
    {
        public string Type { get; set; }
        public string Key { get; set; }
        public Dictionary<string, object> Parameters { get; set; } // durationRatio
        public Frame Frame { get; set; }

#if NETFX_CORE
        public JsonObject ToJsonObject()
        {
            JsonObject jsonObject = new JsonObject();
            jsonObject.SetNamedValue("Type", JsonValue.CreateStringValue(Type));
            jsonObject.SetNamedValue("Key", JsonValue.CreateStringValue(Key));
            if (Parameters != null)
            {
                JsonObject paramsValue = new JsonObject();
                foreach (var parameter in Parameters)
                {
                    try
                    {
                        var parameterKey = parameter.Key;
                        var value = parameter.Value;
                        var par = value as IParsable;

                        if (par != null)
                        {
                            paramsValue[parameterKey] = par.ToJsonObject();
                        }
                        else
                        {
                            try
                            {
                                var str = (string) value;
                                paramsValue[parameterKey] = JsonValue.CreateStringValue(str);
                            }
                            catch (Exception e)
                            {
                                var floatVal = (float) value;
                                paramsValue[parameterKey] = JsonValue.CreateNumberValue(floatVal);
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Params" + e.Message);
                    }

                }
                jsonObject.SetNamedValue("Parameters", paramsValue);
            }


            if (Frame != null)
            {
                jsonObject.SetNamedValue("Frame", Frame.ToJsonObject());
            }
            

            return jsonObject;
        }

#endif
    }

    public class RotationOption
#if NETFX_CORE
        : IParsable
#endif
    {
        public float OffsetAngleX { get; set; }
        public float OffsetY { get; set; }

        public RotationOption(float offsetAngleX, float offsetY)
        {
            OffsetAngleX = offsetAngleX;
            OffsetY = offsetY;
        }

#if NETFX_CORE
        public JsonObject ToJsonObject()
        {
            JsonObject jsonObject = new JsonObject();
            jsonObject.SetNamedValue("OffsetAngleX", JsonValue.CreateNumberValue(OffsetAngleX));
            jsonObject.SetNamedValue("OffsetY", JsonValue.CreateNumberValue(OffsetY));

            return jsonObject;
        }
#endif
    }

    public class ScaleOption
#if NETFX_CORE
        : IParsable
#endif
    {
        public float Intensity { get; set; }
        public float Duration { get; set; }

        public ScaleOption(float intensity, float duration)
        {
            Intensity = intensity;
            Duration = duration;
        }


#if NETFX_CORE
        public JsonObject ToJsonObject()
        {
            JsonObject jsonObject = new JsonObject();
            jsonObject.SetNamedValue("Intensity", JsonValue.CreateNumberValue(Intensity));
            jsonObject.SetNamedValue("Duration", JsonValue.CreateNumberValue(Duration));

            return jsonObject;
        }
#endif
    }

    public class PlayerResponse
    {
        public List<string> RegisteredKeys { get; set; }
        public List<string> ActiveKeys { get; set; }
        public int ConnectedDeviceCount { get; set; }
        public List<PositionType> ConnectedPositions { get; set; }
        public Dictionary<string, int[]> Status { get; set; }


#if NETFX_CORE

        internal static PlayerResponse ToObject(string jsonStr)
        {
            JsonObject jsonObject = JsonObject.Parse(jsonStr);
            var obj = new PlayerResponse();
            
            obj.ConnectedDeviceCount = (int)jsonObject.GetNamedNumber("ConnectedDeviceCount");

            obj.RegisteredKeys = new List<string>();
            foreach (var jsonValue in jsonObject.GetNamedArray("RegisteredKeys", new JsonArray()))
            {
                obj.RegisteredKeys.Add(jsonValue.GetString());
            }

            obj.ActiveKeys = new List<string>();
            foreach (var jsonValue in jsonObject.GetNamedArray("ActiveKeys", new JsonArray()))
            {
                obj.ActiveKeys.Add(jsonValue.GetString());
            }

            obj.ConnectedPositions = new List<PositionType>();
            foreach (var jsonValue in jsonObject.GetNamedArray("ConnectedPositions", new JsonArray()))
            {
                obj.ConnectedPositions.Add(EnumParser.ToPositionType(jsonValue.GetString()));
            }

            obj.Status = new Dictionary<string, int[]>();

            var status = jsonObject.GetNamedObject("Status");
            foreach (var statusKey in status.Keys)
            {
                var arr =  status.GetNamedArray(statusKey, new JsonArray());
                var item = new int[arr.Count];
                var i = 0;
                foreach (var jsonValue in arr)
                {
                    item[i] = (int)jsonValue.GetNumber();
                    i++;
                }
                obj.Status[statusKey] = item;
            }

            return obj;
        }
#endif
    }

    public class Frame
    {
        public int DurationMillis { get; set; }
        public PositionType Position { get; set; }
        public List<PathPoint> PathPoints { get; set; }
        public List<DotPoint> DotPoints { get; set; }

#if NETFX_CORE

        public JsonObject ToJsonObject()
        {
            JsonArray pathPointList = new JsonArray();
            foreach (PathPoint point in PathPoints)
            {
                pathPointList.Add(point.ToJsonObject());
            }

            JsonArray dotPointList = new JsonArray();
            foreach (DotPoint point in DotPoints)
            {
                dotPointList.Add(point.ToJsonObject());
            }

            JsonObject jsonObject = new JsonObject();
            jsonObject.SetNamedValue("DurationMillis", JsonValue.CreateNumberValue(DurationMillis));
            jsonObject.SetNamedValue("Position", JsonValue.CreateStringValue(Position.ToString()));
            jsonObject.SetNamedValue("PathPoints", pathPointList);
            jsonObject.SetNamedValue("DotPoints", dotPointList);

            return jsonObject;
        }

#endif
    }
}
