using UnityEngine;
using UnityEngine.SceneManagement;
using Bhaptics.Tact.Unity;

public class BhapticsLongTest : MonoBehaviour
{
    [SerializeField] private HapticClip[] tactClips;


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