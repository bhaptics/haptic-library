using UnityEngine;
using UnityEditor;


namespace Bhaptics.Tact.Unity
{
    [CustomEditor(typeof(TactSource))]
    public class TactSourceEditor : Editor
    {
        private string[] _feedbackDescOptions;
        private int _selectedFeedbackIndex;

        public override void OnInspectorGUI()
        {
            //            DrawDefaultInspector();
            serializedObject.Update();
            
            FeedbackTypeUi();

            var feedbackTypeProperty = serializedObject.FindProperty("FeedbackType");

            switch (feedbackTypeProperty.enumNames[feedbackTypeProperty.enumValueIndex])
            {
                case "PathMode":
                    PositionUi();
                    PathPointUi();
                    TimeMillisUi();
                break;
                case "DotMode":
                    PositionUi();
                    DotPointUi();
                    TimeMillisUi();
                break;
                case "TactFile":
                    TactFileUi();
                    break;
            }

            PlayUi();
            serializedObject.ApplyModifiedProperties();
        }

        private void TactFileUi()
        {
            if (_feedbackDescOptions == null)
            {
                var key = serializedObject.FindProperty("FeedbackFile.Id").stringValue;

                var length = TactFileAsset.Instance.FeedbackFiles.Length;
                _selectedFeedbackIndex = -1;
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


            GUILayout.EndHorizontal();
            if (_selectedFeedbackIndex >= 0)
            {
                serializedObject.FindProperty("FeedbackFile.Id").stringValue =
                    TactFileAsset.Instance.FeedbackFiles[_selectedFeedbackIndex].Id;

                serializedObject.FindProperty("FeedbackFile.Value").stringValue =
                    TactFileAsset.Instance.FeedbackFiles[_selectedFeedbackIndex].Value;
                serializedObject.FindProperty("FeedbackFile.Key").stringValue =
                    TactFileAsset.Instance.FeedbackFiles[_selectedFeedbackIndex].Key;

                serializedObject.FindProperty("FeedbackFile.Type").stringValue =
                    TactFileAsset.Instance.FeedbackFiles[_selectedFeedbackIndex].Type;
            }

            if (_selectedFeedbackIndex >= 0 && (serializedObject.FindProperty("FeedbackFile.Type").stringValue == BhapticsUtils.TypeTactosy ||
                                                serializedObject.FindProperty("FeedbackFile.Type").stringValue == BhapticsUtils.TypeTactosy2))
            {
                GUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 120f;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("IsReflectTactosy"), new GUIContent("Reflect Left-Right"));
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 120f;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Duration"), new GUIContent("Duration Multiplier"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Intensity"), new GUIContent("Intensity Multiplier"));


            GUILayout.EndHorizontal();

            if (_selectedFeedbackIndex >= 0 && 
                (serializedObject.FindProperty("FeedbackFile.Type").stringValue == BhapticsUtils.TypeVest || 
                 serializedObject.FindProperty("FeedbackFile.Type").stringValue == BhapticsUtils.TypeTactot))
            {
                GUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 120f;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("TactFileOffsetX"), 
                    new GUIContent("Tact File Angle (X)"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("TactFileOffsetY"), 
                    new GUIContent("Tact File Offset (Y)"));
                GUILayout.EndHorizontal();
            }
        }

        private void FeedbackTypeUi()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("FeedbackType"));

            GUILayout.EndHorizontal();
        }

        private void PositionUi()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Position"));

            GUILayout.EndHorizontal();
        }

        private void TimeMillisUi()
        {
            GUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 100f;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TimeMillis"), new GUIContent("Time (ms)"));

            GUILayout.EndHorizontal();
        }

        private void PlayUi()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            GUILayout.BeginHorizontal();
            var source = (TactSource) serializedObject.targetObject;
            if (GUILayout.Button("Play"))
            {
                source.Play();
            }

            if (GUILayout.Button("Stop"))
            {
                source.Stop();
            }
            GUILayout.EndHorizontal();
        }

        private void DotPointUi()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Dot Points", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            var dotPoints = serializedObject.FindProperty("DotPoints");

            for (var index = 0; index < dotPoints.arraySize; index++)
            {

                if (index % 5 == 0)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
                
                EditorGUIUtility.labelWidth = 20f;
                SerializedProperty property = dotPoints.GetArrayElementAtIndex(index);
                
                property.intValue = Mathf.Min(100, Mathf.Max(0, property.intValue));
                EditorGUILayout.PropertyField(property, new GUIContent("" + (index + 1)));
            }
            GUILayout.EndHorizontal();
        }

        private void PathPointUi()
        {
            GUILayout.BeginHorizontal();
            var points = serializedObject.FindProperty("Points");
            EditorGUILayout.LabelField(points.name, EditorStyles.boldLabel);
            if (GUILayout.Button("  +  ", GUILayout.Width(50)))
            {
                int inserted = points.arraySize;
                points.InsertArrayElementAtIndex(inserted);
                points.GetArrayElementAtIndex(inserted).FindPropertyRelative("X").floatValue = 0.5f;
                points.GetArrayElementAtIndex(inserted).FindPropertyRelative("Y").floatValue = 0.5f;
                points.GetArrayElementAtIndex(inserted).FindPropertyRelative("Intensity").intValue = 100;
            }
            GUILayout.EndHorizontal();
            

            for (var index = 0; index < points.arraySize; index++)
            {
                GUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 15f;

                SerializedProperty property = points.GetArrayElementAtIndex(index);

                SerializedProperty x = property.FindPropertyRelative("X");
                SerializedProperty y = property.FindPropertyRelative("Y");
                SerializedProperty intensity = property.FindPropertyRelative("Intensity");

                x.floatValue = Mathf.Min(1, Mathf.Max(0, x.floatValue));
                y.floatValue = Mathf.Min(1, Mathf.Max(0, y.floatValue));
                intensity.intValue = Mathf.Min(100, Mathf.Max(0, intensity.intValue));

                EditorGUILayout.PropertyField(x, new GUIContent(x.name));
                EditorGUILayout.PropertyField(y, new GUIContent(y.name));
                GUILayout.Label(intensity.name, GUILayout.Width(55));
                EditorGUILayout.PropertyField(intensity, GUIContent.none);

                if (GUILayout.Button(new GUIContent("-", "delete"), GUILayout.Width(50)))
                {
                    points.DeleteArrayElementAtIndex(index);
                }

                GUILayout.EndHorizontal();
            }
        }

    }
}