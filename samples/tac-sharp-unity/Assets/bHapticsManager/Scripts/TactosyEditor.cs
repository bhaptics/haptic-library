#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Bhaptics.Tac.Unity
{
#if UNITY_EDITOR
    [CustomEditor(typeof(BhapticsManager))]
    public class TactosyEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (!Application.isPlaying)
            {
                return;
            }

            BhapticsManager manager = (BhapticsManager) target;

            GUILayout.Space(15);
            GUILayout.Label("File Path (Base)");
            GUILayout.TextArea(manager.RootPath);
            foreach (var mappings in manager.FeedbackFileMapping)
            {
                var innerMappings = mappings.Value;
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Relative Path", GUILayout.Width(100));
                GUILayout.TextArea(mappings.Key);
                GUILayout.EndHorizontal();
                foreach (var mapping in innerMappings)
                {
                    GUILayout.BeginHorizontal();
                    var key = mapping.Key;
                    var project = mapping.Value;
                    var type = project.Layout.Type;

                    GUILayout.Label(type, GUILayout.Width(100));

                    if (GUILayout.Button(key))
                    {
                        manager.Play(key);
                    }
                    GUILayout.EndHorizontal();
                }
            }

            if (GUILayout.Button("Turn Off"))
            {
                manager.TurnOff();
            }
        }
    }
#endif
}
