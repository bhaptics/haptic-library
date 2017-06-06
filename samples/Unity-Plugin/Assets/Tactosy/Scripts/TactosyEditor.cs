#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Tactosy.Unity
{
#if UNITY_EDITOR
    [CustomEditor(typeof(Manager_Tactosy))]
    public class TactosyEditor : Editor
    {
        private bool init;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Manager_Tactosy tactosyManager = (Manager_Tactosy) target;

            if (!init)
            {
                init = true;
                tactosyManager.InitPlayer();
            }
            
            foreach (var mappings in tactosyManager.FeedbackMappings)
            {
                var key = mappings.Key;
                if (GUILayout.Button(key))
                { 
                    tactosyManager.Play(key);
                }
            }

            if (GUILayout.Button("Turn Off"))
            {
                tactosyManager.TurnOff();
            }
        }
    }
#endif
}
