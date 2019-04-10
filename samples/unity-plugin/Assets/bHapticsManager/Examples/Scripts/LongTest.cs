using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bhaptics.Tact.Unity
{
    public class LongTest : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            InvokeRepeating("TriggerPlay", 1f, 4f);

            Invoke("ReloadScene", 10f);
        }

        void TriggerPlay()
        {
            var tactClips = GetComponents<TactSource>();
            foreach (var tactClip in tactClips)
            {
                tactClip.Play();
            }
        }

        void ReloadScene()
        {
            int scene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(scene, LoadSceneMode.Single);
            Time.timeScale = 1;
        }
    }
}