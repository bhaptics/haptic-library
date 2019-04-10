using UnityEngine;
using UnityEditor;

namespace Bhaptics.Tact.Unity
{
    [CustomEditor(typeof(TactSender))]
    public class TactSenderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            PlayUi();
        }

        private void PlayUi()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            var source = (TactSender)target;
            if (GUILayout.Button("Play"))
            {
                source.Play();
            }
        }
    }
}