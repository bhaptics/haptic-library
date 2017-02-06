using UnityEditor;
using UnityEngine;

namespace TactosyCommon.Unity
{ 
    [CustomEditor(typeof(TactosyManager))]
    public class TactosyEditor : Editor
    {
        private bool showParam = true;
        private float intensity = 0f;
        private float duration = 0f;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TactosyManager tactosyManager = (TactosyManager) target;
            

            if (GUILayout.Button("Turn Off Feedback"))
            {
                tactosyManager.TurnOff();
            }

            GUILayout.Label("Registered Feedbacks");
            foreach (var mappings in tactosyManager._registeredSignals)
            {
                var key = mappings.Key;
                if (GUILayout.Button(key))
                {
                    if (showParam)
                    {
                        var inten = Mathf.Pow(10, intensity);
                        var du = Mathf.Pow(10, duration);
                        tactosyManager.SendSignal(key, inten, du);
                    }
                    else
                    {
                        tactosyManager.SendSignal(key);
                    }
                }
            }

            showParam = GUILayout.Toggle(showParam, "Feeback With Parameter");

            if (showParam)
            {
                GUILayout.Label("Intensity(0.1 ~ 10) with Logarithmic scale");
                intensity = GUILayout.HorizontalSlider(intensity, -1f, 1f);

                GUILayout.Label("Duration(0.1 ~ 10) with Logarithmic scale");
                duration = GUILayout.HorizontalSlider(duration, -1f, 1f);
            }
        }
    }
}

