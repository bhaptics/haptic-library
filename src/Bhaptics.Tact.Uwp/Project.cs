using System;
using System.Collections.Generic;
#if NETFX_CORE
using Windows.Data.Json;
#endif

namespace Bhaptics.Tact
{

    public class Project
    {
        public Track[] Tracks { get; set; }
        public Layout Layout { get; set; }

        public override string ToString()
        {
            return "Project { Tracks=" + Tracks +
                   ", Layout=" + Layout + "}";
        }
#if NETFX_CORE
        internal JsonObject JsonObject { get; set; }
        public static Project ToProject(JsonObject jsonObject)
        {

            Project project = new Project();
            var trackList = new List<Track>();
            project.JsonObject = jsonObject;
            var tracks = jsonObject.GetNamedArray("tracks", new JsonArray());

            foreach (var tJObject in tracks)
            {
                var track = Track.ToTrack(tJObject.GetObject());
                trackList.Add(track);
            }
            
            var layoutValue = jsonObject.GetNamedValue("layout", JsonValue.CreateNullValue());
            project.Layout = Layout.ToLayout(layoutValue.GetObject());
            
            project.Tracks = trackList.ToArray();
            return project;
        }

        public JsonObject ToJsonObject()
        {
            var jsonObject = new JsonObject();

            JsonArray tracks = new JsonArray();
            foreach (var track in Tracks)
            {
                tracks.Add(track.ToJsonObject());
            }

            jsonObject.SetNamedValue("tracks", tracks);
            jsonObject.SetNamedValue("layout", Layout.ToJsonObject());

            return jsonObject;
        }

#endif
    }

    public class HapticFeedbackFile
    {
        public int IntervalMillis;
        public int Size;
        public int DurationMillis;
        public Project Project;

#if NETFX_CORE
        public static HapticFeedbackFile ToHapticFeedbackFile(string jsonStr)
        {
            HapticFeedbackFile feedbackFile = new HapticFeedbackFile();
            
            JsonObject jsonObject = JsonObject.Parse(jsonStr);
            var projectObj = jsonObject.GetNamedObject("project");

            feedbackFile.Project = Project.ToProject(projectObj);
            feedbackFile.DurationMillis = (int) jsonObject.GetNamedNumber("durationMillis");
            feedbackFile.IntervalMillis = (int) jsonObject.GetNamedNumber("intervalMillis");
            feedbackFile.Size = (int) jsonObject.GetNamedNumber("size");
            return feedbackFile;
        }
#endif
    }

    public class Track
    {
        public HapticEffect[] Effects { get; set; }

        public override string ToString()
        {
            return "Track {  Effects=" + Effects + "}";
        }

#if NETFX_CORE
        internal static Track ToTrack(JsonObject jsonObj)
        {
            Track track = new Track();
            
            List<HapticEffect> effectList = new List<HapticEffect>();
            var effects = jsonObj.GetNamedArray("effects", new JsonArray());
            foreach (var effect in effects)
            {
                effectList.Add(HapticEffect.ToEffect(effect.GetObject()));

            }
            track.Effects = effectList.ToArray();

            return track;
        }

        internal JsonObject ToJsonObject()
        {
            var jsonObject = new JsonObject();
            
            JsonArray effectArray = new JsonArray();

            foreach (var effect in Effects)
            {
                effectArray.Add(effect.ToJsonObject());
            }
            jsonObject.SetNamedValue("effects", effectArray);
            return jsonObject;
        }
#endif
    }

    public class HapticEffect
    {
        public int StartTime { get; set; }
        public int OffsetTime { get; set; }
        public Dictionary<string, HapticEffectMode> Modes {get; set; }

        public override string ToString()
        {
            return "HapticEffect { StartTime=" + StartTime +
                   ", OffsetTime=" + OffsetTime +
                   ", Modes=" + Modes + "}";
        }

#if NETFX_CORE

        internal static HapticEffect ToEffect(JsonObject jsonObj)
        {
            var effect = new HapticEffect();

            // TODO
            effect.StartTime = (int)jsonObj.GetNamedNumber("startTime", -1);
            effect.OffsetTime = (int)jsonObj.GetNamedNumber("offsetTime", -1);
            effect.Modes = new Dictionary<string, HapticEffectMode>();

            var modeJson = jsonObj.GetNamedObject("modes", new JsonObject());
            foreach (var mode in modeJson)
            {
                effect.Modes[mode.Key] = HapticEffectMode.ToMode(mode.Value.GetObject());
            }

            return effect;
        }

        internal JsonObject ToJsonObject()
        {
            JsonObject jsonObject = new JsonObject();
            jsonObject.SetNamedValue("startTime", JsonValue.CreateNumberValue(StartTime));
            jsonObject.SetNamedValue("offsetTime", JsonValue.CreateNumberValue(OffsetTime));

            var modeObject = new JsonObject();
            jsonObject.SetNamedValue("modes", modeObject);

            foreach (var hapticEffectMode in Modes)
            {
                modeObject.SetNamedValue(hapticEffectMode.Key, hapticEffectMode.Value.ToJsonObject());
            }

            return jsonObject;
        }
#endif
    }

    public class Layout
    {
        public string Type { get; set; }
        public Dictionary<string, LayoutObject[]> Layouts { get; set; }

#if NETFX_CORE

        internal static Layout ToLayout(JsonObject jsonObj)
        {
            var layout = new Layout();
            var type = jsonObj.GetNamedString("type");
            layout.Type = type;
            layout.Layouts = new Dictionary<string, LayoutObject[]>();

            var layouts = jsonObj.GetNamedObject("layouts");
            foreach (var key in layouts.Keys)
            {
                var arr = layouts.GetNamedArray(key, new JsonArray());
                var layoutObjList = new List<LayoutObject>();
                foreach (var layoutObj in arr)
                {
                    layoutObjList.Add(LayoutObject.ToLayoutObject(layoutObj.GetObject()));
                }
                layout.Layouts[key] = layoutObjList.ToArray();
            }

            return layout;
        }

        internal JsonObject ToJsonObject()
        {
            var jsonObject = new JsonObject();
            jsonObject.SetNamedValue("type", JsonValue.CreateStringValue(Type));
            var layoutsObject = new JsonObject();
            
            foreach (var layout in Layouts)
            {
                JsonArray objArray = new JsonArray();
                foreach (var val in layout.Value)
                {
                    objArray.Add(val.ToJsonObject());
                }
                layoutsObject.SetNamedValue(layout.Key, objArray);
            }

            jsonObject.SetNamedValue("layouts", layoutsObject);

            return jsonObject;
        }
#endif
    }

    public class LayoutObject
    {
        public int Index { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

#if NETFX_CORE

        internal static LayoutObject ToLayoutObject(JsonObject jsonObj)
        {
            LayoutObject layoutObject = new LayoutObject();
            layoutObject.Index = (int) jsonObj.GetNamedNumber("index");
            layoutObject.X = float.Parse(jsonObj.GetNamedString("x"));
            layoutObject.Y = float.Parse(jsonObj.GetNamedString("y"));

            return layoutObject;
        }

        internal JsonObject ToJsonObject()
        {
            var jsonObject = new JsonObject();

            jsonObject.SetNamedValue("index", JsonValue.CreateNumberValue(Index));
            jsonObject.SetNamedValue("x", JsonValue.CreateNumberValue(X));
            jsonObject.SetNamedValue("y", JsonValue.CreateNumberValue(Y));
            return jsonObject;
        }
#endif
    }

    public enum PlaybackType
    {
        NONE, FADE_IN, FADE_OUT, FADE_IN_OUT
    }


    public class HapticEffectMode
    {
        public FeedbackMode Mode { get; set; }
        public DotMode DotMode { get; set; }
        public PathMode PathMode { get; set; }
#if NETFX_CORE

        internal static HapticEffectMode ToMode(JsonObject jsonObj)
        {
            var mode = new HapticEffectMode();

            mode.Mode = EnumParser.ToMode(jsonObj.GetNamedString("mode"));

            mode.DotMode = DotMode.ToDotMode(jsonObj.GetNamedObject("dotMode", new JsonObject()).GetObject());

            mode.PathMode = PathMode.ToPathMode(jsonObj.GetNamedObject("pathMode", new JsonObject()).GetObject());

            return mode;
        }

        internal JsonObject ToJsonObject()
        {
            var jsonObject= new JsonObject();

            jsonObject.SetNamedValue("mode", JsonValue.CreateStringValue(Mode.ToString()));
            jsonObject.SetNamedValue("dotMode", DotMode.ToJsonObject());
            jsonObject.SetNamedValue("pathMode", PathMode.ToJsonObject());
            return jsonObject;
        }

#endif
    }

    public class DotMode
    {
        public bool DotConnected { get; set; }
        public DotModeObjectCollection[] Feedback { get; set; }

#if NETFX_CORE

        internal static DotMode ToDotMode(JsonObject jsonObj)
        {
            var dotMode = new DotMode();
            dotMode.DotConnected = jsonObj.GetNamedBoolean("dotConnected");
            var feedbackList = new List<DotModeObjectCollection>();
            var arr = jsonObj.GetNamedArray("feedback", new JsonArray());
            foreach (var val in arr)
            {
                feedbackList.Add(DotModeObjectCollection.ToObject(val.GetObject()));
            }

            dotMode.Feedback = feedbackList.ToArray();
            return dotMode;
        }

        internal IJsonValue ToJsonObject()
        {
            var jsonObject = new JsonObject();

            jsonObject.SetNamedValue("dotConnected", JsonValue.CreateBooleanValue(DotConnected));
            var feedbackArray = new JsonArray();
            jsonObject.SetNamedValue("feedback", feedbackArray);

            foreach (var dotModeObjectCollection in Feedback)
            {
                feedbackArray.Add(dotModeObjectCollection.ToJsonObject());
            }

            return jsonObject;
        }
#endif
    }

    public class DotModeObjectCollection
    {
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public PlaybackType PlaybackType = PlaybackType.NONE;
        public DotModeObject[] PointList { get; set; }
#if NETFX_CORE

        internal static DotModeObjectCollection ToObject(JsonObject val)
        {
            var obj = new DotModeObjectCollection();
            obj.StartTime = (int) val.GetNamedNumber("startTime");
            obj.EndTime = (int)val.GetNamedNumber("endTime");

            obj.PlaybackType = EnumParser.ToPlaybackType(val.GetNamedString("playbackType", "NONE"));
            var list = new List<DotModeObject>();

            foreach (var jsonValue in val.GetNamedArray("pointList", new JsonArray()))
            {
                list.Add(DotModeObject.ToObject(jsonValue.GetObject()));
            }

            obj.PointList = list.ToArray();

            return obj;
        }

        internal IJsonValue ToJsonObject()
        {
            var jsonObject = new JsonObject();

            jsonObject.SetNamedValue("startTime", JsonValue.CreateNumberValue(StartTime));
            jsonObject.SetNamedValue("endTime", JsonValue.CreateNumberValue(EndTime));
            jsonObject.SetNamedValue("playbackType", JsonValue.CreateStringValue(PlaybackType.ToString()));

            var pointList = new JsonArray();

            jsonObject.SetNamedValue("pointList", pointList);
            foreach (var dotModeObject in PointList)
            {
                pointList.Add(dotModeObject.ToJsonObject());
            }

            return jsonObject;
        }
#endif
    }

    public class DotModeObject
    {
        public int Index { get; set; }
        public float Intensity { get; set; }

#if NETFX_CORE
        internal static DotModeObject ToObject(JsonObject jsonObject)
        {
            var obj = new DotModeObject();

            obj.Index = (int) jsonObject.GetNamedNumber("index");
            obj.Intensity = ParseUtil.GetFloat(jsonObject, "intensity");

            return obj;
        }

        internal IJsonValue ToJsonObject()
        {
            var jsonObject = new JsonObject();

            jsonObject.SetNamedValue("index", JsonValue.CreateNumberValue(Index));
            jsonObject.SetNamedValue("intensity", JsonValue.CreateNumberValue(Intensity));

            return jsonObject;
        }
#endif
    }




    public enum PathMovingPattern
    {
        CONST_SPEED, CONST_TDM
    }

    public class PathMode
    {
        public PathModeObjectCollection[] Feedback { get; set; }
#if NETFX_CORE

        internal static PathMode ToPathMode(JsonObject jsonObject)
        {
            var pathMode = new PathMode();

            var list = new List<PathModeObjectCollection>();
            foreach (var jsonValue in jsonObject.GetNamedArray("feedback", new JsonArray()))
            {
                list.Add(PathModeObjectCollection.ToObject(jsonValue.GetObject())); 
            }

            pathMode.Feedback = list.ToArray();
            return pathMode;
        }

        internal IJsonValue ToJsonObject()
        {
            var jsonObject = new JsonObject();

            var feedbackArray = new JsonArray();
            jsonObject.SetNamedValue("feedback", feedbackArray);
            foreach (var pathModeObjectCollection in Feedback)
            {
                feedbackArray.Add(pathModeObjectCollection.ToJsonObject());
            }

            return jsonObject;
        }
#endif
    }

    public class PathModeObjectCollection
    {
        public PlaybackType PlaybackType = PlaybackType.NONE;
        public PathMovingPattern MovingPattern = PathMovingPattern.CONST_TDM;
        public PathModeObject[] PointList { get; set; }
#if NETFX_CORE
        internal static PathModeObjectCollection ToObject(JsonObject jsonObject)
        {
           var collection = new PathModeObjectCollection();

            collection.PlaybackType = EnumParser.ToPlaybackType(jsonObject.GetNamedString("playbackType", "NONE"));
            collection.MovingPattern = EnumParser.ToMovingPattern(jsonObject.GetNamedString("movingPattern"));
            
            List<PathModeObject> list = new List<PathModeObject>();

            foreach (var jsonValue in jsonObject.GetNamedArray("pointList", new JsonArray()))
            {
                list.Add(PathModeObject.ToObject(jsonValue.GetObject()));
            }

            collection.PointList = list.ToArray();

            return collection;
        }

        internal IJsonValue ToJsonObject()
        {
            var jsonObject = new JsonObject();

            jsonObject.SetNamedValue("playbackType", JsonValue.CreateStringValue(PlaybackType.ToString()));
            jsonObject.SetNamedValue("movingPattern", JsonValue.CreateStringValue(MovingPattern.ToString()));

            var pointListArray = new JsonArray();
            jsonObject.SetNamedValue("pointList", pointListArray);
            foreach (var pathModeObject in PointList)
            {
                pointListArray.Add(pathModeObject.ToJsonObject());
            }

            return jsonObject;
        }
#endif
    }

    public class PathModeObject
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Intensity { get; set; }
        public int Time { get; set; }
#if NETFX_CORE
        internal static PathModeObject ToObject(JsonObject jsonObject)
        {
            var obj = new PathModeObject();
            
            obj.Intensity = ParseUtil.GetFloat(jsonObject, "intensity");
            obj.X = ParseUtil.GetFloat(jsonObject, "x");
            obj.Y = ParseUtil.GetFloat(jsonObject, "y");
            obj.Time = (int) jsonObject.GetNamedNumber("time");

            return obj;
        }

        internal IJsonValue ToJsonObject()
        {
            var jsonObject = new JsonObject();

            jsonObject.SetNamedValue("x", JsonValue.CreateNumberValue(X));
            jsonObject.SetNamedValue("y", JsonValue.CreateNumberValue(Y));
            jsonObject.SetNamedValue("intensity", JsonValue.CreateNumberValue(Intensity));
            jsonObject.SetNamedValue("time", JsonValue.CreateNumberValue(Time));

            return jsonObject;
        }
#endif
    }

#if NETFX_CORE
    internal class ParseUtil
    {
        internal static float GetFloat(JsonObject obj, string key)
        {
            var type = obj.GetNamedValue(key).ValueType;
            if (type == JsonValueType.Number)
            {
                return (float) obj.GetNamedNumber(key);
            }

            if (type == JsonValueType.String)
            {
                return float.Parse(obj.GetNamedString(key));
            }
            // wrong
            return -1f;
        }
    }
#endif
}
