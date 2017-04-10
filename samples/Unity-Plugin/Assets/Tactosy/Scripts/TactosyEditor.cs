using UnityEditor;
using UnityEngine;

namespace Tactosy.Unity
{ 
    [CustomEditor(typeof(Manager_Tactosy))]
    public class TactosyEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Manager_Tactosy tactosyManager = (Manager_Tactosy) target;

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
}
