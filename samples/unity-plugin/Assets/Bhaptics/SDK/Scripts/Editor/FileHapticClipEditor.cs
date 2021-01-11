using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Callbacks;


namespace Bhaptics.Tact.Unity
{
    [CustomEditor(typeof(FileHapticClip), true)]
    public class FileHapticClipEditor : Editor
    {
        FileHapticClip targetScript;


        void OnEnable()
        {
            targetScript = target as FileHapticClip;
        }

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
            var clipType = targetScript.ClipType;
            if (clipType == HapticDeviceType.Tactosy_arms || clipType == HapticDeviceType.Tactosy_feet || clipType == HapticDeviceType.Tactosy_hands)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("IsReflect"), new GUIContent("IsReflect"), GUILayout.Width(350f));
                GUILayout.EndHorizontal();
            }
        }

        protected void DefaultPropertyUi()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Clip Type", GUILayout.Width(100f));

            var clipTypeSerializedObject = serializedObject.FindProperty("ClipType");
            EditorGUILayout.LabelField(clipTypeSerializedObject.enumNames[clipTypeSerializedObject.enumValueIndex]);
            GUILayout.EndHorizontal();

            var originLabelWidth = EditorGUIUtility.labelWidth;

            EditorGUIUtility.labelWidth = 130f;
            GUI.enabled = false;
            GUILayout.BeginHorizontal();
            var temp = targetScript.ClipDurationTime;
            GUIContent customLabel = new GUIContent("└ Duration Time(ms)");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_clipDurationTime"), customLabel, GUILayout.Width(350f));
            GUILayout.EndHorizontal();
            GUI.enabled = true;

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
            if (targetScript == null)
            {
                BhapticsLogger.LogInfo("hapticClip null");
                GUILayout.EndHorizontal();
                return;
            }

            if (GUILayout.Button("Play"))
            {
                targetScript.Play();
            }
            if (GUILayout.Button("Stop"))
            {
                targetScript.Stop();
            }
            GUILayout.EndHorizontal();
        }

        protected void ResetUi()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset Values"))
            {
                targetScript.ResetValues();
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
            if (targetScript != null)
            {
                var saveAsPath = EditorUtility.SaveFilePanel("Save as *.tact File", @"\download\", targetScript.name, "tact");
                if (saveAsPath != "")
                {
                    File.WriteAllText(saveAsPath, targetScript.JsonValue);
                }
            }
        }

        [OnOpenAsset(1)]
        public static bool OnOpenTactClip(int instanceID, int line)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceID);
            if (obj is FileHapticClip)
            {
                (obj as FileHapticClip).Play();
                return true;
            }
            return false;
        }
    }
}