using System.Collections;
using System.Collections.Generic;
using Bhaptics.Tact.Unity;
using UnityEngine;
using UnityEngine.UI;

public class BhapticsStreamingTest : MonoBehaviour
{
	[SerializeField]
    private Button button;

    private Text text;

    [SerializeField]
    private HapticClip clip;

	// Use this for initialization
	void Start () {
		InvokeRepeating("Check", 1f, 1f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void Check()
    {
        if (button == null)
        {
            return;
        }

        if (text == null)
        {
            text = button.GetComponentInChildren<Text>();
        }

        if (text == null)
        {
            return;
        }

        var hosts = BhapticsAndroidManager.GetStreamingHosts();
        string s = "";

        foreach (var streamHost in hosts)
        {
            s += " {" + streamHost.ip + ", " + streamHost.connected + "}\n ";
        }
        if (BhapticsAndroidManager.IsStreaming())
        {

            text.text = "Stream: On \n" + s;
        }
        else
        {
            text.text = "Stream: Off \n" + s;
        }

        if (clip != null)
        {
            clip.Play();
        }
    }

    public void Toggle()
    {
		BhapticsAndroidManager.ToggleStreaming();
    }
}
