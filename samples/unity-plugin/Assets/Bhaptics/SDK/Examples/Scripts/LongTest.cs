using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bhaptics.Tact.Unity
{
    public class LongTest : MonoBehaviour
    {
        [SerializeField]
        private HapticClip[] tactClips;


        // Use this for initialization
        void Start()
        {
            InvokeRepeating("TriggerPlay", 1f, 4f);

            Invoke("ReloadScene", 10f);
        }

        void TriggerPlay()
        {
            foreach (var tactClip in tactClips)
            {
                tactClip.Play();
            }
        }

        void ReloadScene()
        {
            BhapticsLogger.LogInfo("Reloading scene.");
            int scene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(scene, LoadSceneMode.Single);
            Time.timeScale = 1;
        }
    }
}