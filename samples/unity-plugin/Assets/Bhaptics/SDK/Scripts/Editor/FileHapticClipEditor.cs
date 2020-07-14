using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Callbacks;


namespace Bhaptics.Tact.Unity
{
    [CustomEditor(typeof(FileHapticClip), true)]
    public class FileHapticClipEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DefaultPropertyUi();

            GUILayout.Space(3);
            ResetUi();

            GUILayout.Space(20);
            PlayUi();
            
            GUILayout.Space(3);
            SaveAsUi();

            serializedObject.ApplyModifiedProperties();
        }

        protected void ReflectUi()
        {
            var clipType = (target as FileHapticClip).ClipType;
            if (clipType == HapticClipType.Tactosy_arms || clipType == HapticClipType.Tactosy_feet || clipType == HapticClipType.Tactosy_hands)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("IsReflect"), new GUIContent("IsReflect"), GUILayout.Width(350f));
                GUILayout.EndHorizontal();
            }
        }

        protected void DefaultPropertyUi()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ClipType", GUILayout.Width(100f));

            var clipTypeSerializedObject = serializedObject.FindProperty("ClipType");
            EditorGUILayout.LabelField(clipTypeSerializedObject.enumNames[clipTypeSerializedObject.enumValueIndex]);
            GUILayout.EndHorizontal();

            var originLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 105f;

            ReflectUi();


            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Intensity"), GUILayout.Width(350f));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Duration"), GUILayout.Width(350f));
            GUILayout.EndHorizontal();

            EditorGUIUtility.labelWidth = originLabelWidth;
        }

        protected void PlayUi()
        {
            GUILayout.BeginHorizontal();
            var tactClip = serializedObject.targetObject as HapticClip;
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

        protected void ResetUi()
        {
            GUILayout.BeginHorizontal();
            var tactClip = serializedObject.targetObject as HapticClip;
            if (GUILayout.Button("Reset Values"))
            {
                tactClip.ResetValues();
            }
            GUILayout.EndHorizontal();
        }

        protected void SaveAsUi()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save As *.tact File"))
            {
                SaveAsTactFileFromClip(target);
            }
            GUILayout.EndHorizontal();
        }

        private void SaveAsTactFileFromClip(Object target)
        {
            var tactClip = target as FileHapticClip;
            if (tactClip != null)
            {
                var saveAsPath = EditorUtility.SaveFilePanel("Save as *.tact File", @"\download\", tactClip.name, "tact");
                if (saveAsPath != "")
                {
                    File.WriteAllText(saveAsPath, tactClip.JsonValue);
                }
            }
        }

        [OnOpenAsset(1)]
        public static bool OnOpenTactClip(int instanceID, int line)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceID);
            if (obj is HapticClip)
            {
                (obj as HapticClip).Play();
                return true;
            }
            return false;
        }
    }
}