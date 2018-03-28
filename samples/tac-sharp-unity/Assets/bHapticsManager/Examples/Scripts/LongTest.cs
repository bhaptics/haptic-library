using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bhaptics.Tact.Unity
{
    public class LongTest : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            InvokeRepeating("TriggerBowShoot", .1f, 2f);

            Invoke("ReloadScene", 5f);
        }

        void TriggerBowShoot()
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