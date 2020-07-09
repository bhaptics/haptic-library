using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Callbacks;


namespace Bhaptics.Tact.Unity
{
    [CustomEditor(typeof(TactClip), true)]
    public class TactClipEditor : Editor
    {
        private bool detailView = false;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DefaultPropertyUi();

            var clipType = (target as TactClip).ClipType;
            if (clipType == TactClipType.Tactot)
            {
                GUILayout.Space(10);
                DetailPropertyUi();
            }
            GUILayout.Space(20);
            PlayUi();

            GUILayout.Space(3);
            SaveAsUi();

            serializedObject.ApplyModifiedProperties();
        }



        [OnOpenAsset(1)]
        public static bool OnOpenTactClip(int instanceID, int line)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceID);
            if (obj is TactClip)
            {
                (obj as TactClip).Play();
            }
            return false;
        }



        private void DefaultPropertyUi()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ClipType", GUILayout.Width(100f));

            var clipTypeSerializedObject = serializedObject.FindProperty("ClipType");
            EditorGUILayout.LabelField(clipTypeSerializedObject.enumNames[clipTypeSerializedObject.enumValueIndex]);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", GUILayout.Width(100f));
            EditorGUILayout.LabelField(serializedObject.FindProperty("Name").stringValue);
            GUILayout.EndHorizontal();

            var originLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 105f;

            var clipType = (target as TactClip).ClipType;
            if (clipType == TactClipType.Tactosy_arms || clipType == TactClipType.Tactosy_feet || clipType == TactClipType.Tactosy_hands)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("IsReflectTactosy"), new GUIContent("IsReflect"), GUILayout.Width(350f));
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Intensity"), GUILayout.Width(350f));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Duration"), GUILayout.Width(350f));
            GUILayout.EndHorizontal();

            EditorGUIUtility.labelWidth = originLabelWidth;
        }

        private void DetailPropertyUi()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(detailView ? "Close Detail" : "Open Detail"))
            {
                detailView = !detailView;
            }
            GUILayout.EndHorizontal();

            if (detailView)
            {
                GUILayout.Space(5);
                var originLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 135f;

                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("VestRotationAngleX"), GUILayout.Width(350f));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("VestRotationOffsetY"), GUILayout.Width(350f));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("TactFileAngleX"), GUILayout.Width(350f));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("TactFileOffsetY"), GUILayout.Width(350f));
                GUILayout.EndHorizontal();

                EditorGUIUtility.labelWidth = originLabelWidth;

                GUILayout.Space(5);
                ResetUi();
            }
        }

        private void PlayUi()
        {
            GUILayout.BeginHorizontal();
            var tactClip = serializedObject.targetObject as TactClip;
            if (tactClip == null)
            {
                BhapticsLogger.LogInfo("tactClip null");
                GUILayout.EndHorizontal();
                return;
            }

            if (GUILayout.Button("Play"))
            {
                tactClip.Play();
            }
            if (GUILayout.Button("Stop"))
            {
                tactClip.Stop();
            }
            GUILayout.EndHorizontal();
        }

        private void ResetUi()
        {
            GUILayout.BeginHorizontal();
            var tactClip = serializedObject.targetObject as TactClip;
            if (GUILayout.Button("Reset Values"))
            {
                tactClip.ResetValues();
            }
            GUILayout.EndHorizontal();
        }

        private void SaveAsUi()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save As *.tact File"))
            {
                SaveAsTactFileFromClip(target);
            }
            GUILayout.EndHorizontal();
        }




        


        private static void SaveAsTactFileFromClip(Object target)
        {
            var tactClip = target as TactClip;
            if (tactClip != null)
            {
                var saveAsPath = EditorUtility.SaveFilePanel("Save as *.tact File", @"\download\", tactClip.Name, "tact");
                if (saveAsPath != "")
                {
                    File.WriteAllText(saveAsPath, tactClip.JsonValue);
                }
            }
        }
    }
}