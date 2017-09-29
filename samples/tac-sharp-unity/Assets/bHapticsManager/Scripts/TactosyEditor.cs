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
        private bool init;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            BhapticsManager manager = (BhapticsManager) target;

            if (!init)
            {
                init = true;
            }
            
            foreach (var mappings in manager.FeedbackMappings)
            {
                var key = mappings.Key;
                if (GUILayout.Button(key))
                { 
                    manager.Play(key);
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
