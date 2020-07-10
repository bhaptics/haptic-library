using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Callbacks;


namespace Bhaptics.Tact.Unity
{
    [CustomEditor(typeof(TactFileClip), true)]
    public class TactFileClipEditor : Editor
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
            var clipType = (target as TactFileClip).ClipType;
            if (clipType == TactClipType.Tactosy_arms || clipType == TactClipType.Tactosy_feet || clipType == TactClipType.Tactosy_hands)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("IsReflectTactosy"), new GUIContent("IsReflect"), GUILayout.Width(350f));
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

        protected void ResetUi()
        {
            GUILayout.BeginHorizontal();
            var tactClip = serializedObject.targetObject as TactClip;
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
            var tactClip = target as TactFileClip;
            if (tactClip != null)
            {
                var saveAsPath = EditorUtility.SaveFilePanel("Save as *.tact File", @"\download\", tactClip.Name, "tact");
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
            if (obj is TactClip)
            {
                (obj as TactClip).Play();
                return true;
            }
            return false;
        }
    }
}