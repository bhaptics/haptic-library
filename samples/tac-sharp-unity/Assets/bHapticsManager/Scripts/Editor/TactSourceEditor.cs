using System.Linq;
using UnityEngine;
using UnityEditor;


namespace Bhaptics.Tac.Unity
{
    [CustomEditor(typeof(TactSource))]
    public class TactSourceEditor : Editor
    {
        private const int IntensityMin = 0;
        private const int IntensityMax = 100;
        private const float CoordinateMin = 0f;
        private const float CoordinateMax = 1f;

        private const int TimeMillisMin = 20;
        private const int TimeMillisMax = 10000;

        private const float RatioMin = 0.2f;
        private const float RatioMax = 5f;

        private string[] _feedbackDescOptions;
        private int _selectedFeedbackIndex;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var source = (TactSource)target;

            FeedbackTypeUi();
            
            switch (source.FeedbackType)
            {
                case FeedbackType.PathMode:
                    PositionUi();
                    PathPointUi();
                    TimeMillisUi();
                break;
                case FeedbackType.DotMode:
                    PositionUi();
                    DotPointUi();
                    TimeMillisUi();
                break;
                case FeedbackType.TactFile:
                    TactFileUi();
                    break;
            }

            PlayUi();
        }

        private void TactFileUi()
        {
            var source = (TactSource)target;
            if (_feedbackDescOptions == null)
            {
                var key = source.FeedbackFile.Id;

                var length = TactFileAsset.Instance.FeedbackFiles.Length;

                _feedbackDescOptions = new string[length];
                for (var i = 0; i < length; i++)
                {
                    var file = TactFileAsset.Instance.FeedbackFiles[i];
                    _feedbackDescOptions[i] = file.Type + " - " + file.Key;
                    if (key == file.Id)
                    {
                        _selectedFeedbackIndex = i;
                    }
                }
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Tact File");

            _selectedFeedbackIndex = EditorGUILayout.Popup(_selectedFeedbackIndex, _feedbackDescOptions);
            source.FeedbackFile = TactFileAsset.Instance.FeedbackFiles[_selectedFeedbackIndex];

            GUILayout.EndHorizontal();

            if (source.FeedbackFile.Type == BhapticsUtils.TypeTactosy)
            {
                GUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 120f;
                source.IsReflectTactosy = EditorGUILayout.Toggle("Reflect Left-Right", source.IsReflectTactosy);
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 120f;
            source.Duration = Mathf.Clamp(EditorGUILayout.FloatField("Duration Multiplier", source.Duration),
                RatioMin, RatioMax);

            source.Intensity = Mathf.Clamp(EditorGUILayout.FloatField("Intensity Multiplier", source.Intensity),
                RatioMin, RatioMax);


            GUILayout.EndHorizontal();

            if (source.FeedbackFile.Type == BhapticsUtils.TypeVest)
            {
                GUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 120f;
                source.TactFileOffsetX = Mathf.Clamp(EditorGUILayout.FloatField("Tact File Offset (X)", source.TactFileOffsetX),
                    0, 360);

                source.TactFileOffsetY = Mathf.Clamp(EditorGUILayout.FloatField("Tact File Offset (Y)", source.TactFileOffsetY),
                    -1, 1);
                GUILayout.EndHorizontal();
            }
        }

        private void FeedbackTypeUi()
        {
            var source = (TactSource)target;

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Feedback Type");

            source.FeedbackType = (FeedbackType)EditorGUILayout.EnumPopup(source.FeedbackType);

            GUILayout.EndHorizontal();
        }

        private void PositionUi()
        {
            var source = (TactSource)target;

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Position");
            source.Position = (Pos)EditorGUILayout.EnumPopup(source.Position);

            GUILayout.EndHorizontal();
        }

        private void TimeMillisUi()
        {
            var source = (TactSource)target;

            GUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 100f;
            source.TimeMillis = Mathf.Clamp(EditorGUILayout.IntField("Time (ms)", source.TimeMillis), 
                TimeMillisMin, TimeMillisMax);

            GUILayout.EndHorizontal();
        }

        private void PlayUi()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            var source = (TactSource)target;
            if (GUILayout.Button("Play"))
            {
                source.Play();
            }
        }

        private void DotPointUi()
        {
            var source = (TactSource)target;

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Dot Points", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            for (var index = 0; index < source.DotPoints.Length; index++)
            {

                if (index % 5 == 0)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
                
                EditorGUIUtility.labelWidth = 20f;
                source.DotPoints[index] = (byte) Mathf.Clamp(EditorGUILayout.IntField("" + (index + 1), source.DotPoints[index]), IntensityMin, IntensityMax);
            }
            GUILayout.EndHorizontal();
        }

        private void PathPointUi()
        {
            var source = (TactSource)target;

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Points", EditorStyles.boldLabel);
            if (GUILayout.Button("  +  ", GUILayout.Width(50)))
            {
                source.AddPathPoint();
            }
            GUILayout.EndHorizontal();

            for (var index = 0; index < source.Points.Length; index++)
            {
                var point = source.Points[index];
                GUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 15f;
                point.X = Mathf.Clamp(EditorGUILayout.FloatField("X", point.X), CoordinateMin, CoordinateMax);

                EditorGUIUtility.labelWidth = 15f;
                point.Y = Mathf.Clamp(EditorGUILayout.FloatField("Y", point.Y), CoordinateMin, CoordinateMax);

                EditorGUIUtility.labelWidth = 60f;
                point.Intensity = Mathf.Clamp(EditorGUILayout.IntField("Intensity", point.Intensity), IntensityMin, IntensityMax);

                if (GUILayout.Button(new GUIContent("-", "delete"), GUILayout.Width(50)))
                {
                    source.RemovePathPoint(index);
                }

                GUILayout.EndHorizontal();
            }
        }

    }
}