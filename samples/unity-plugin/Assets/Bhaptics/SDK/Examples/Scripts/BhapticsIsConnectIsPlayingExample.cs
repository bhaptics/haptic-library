using UnityEngine;
using UnityEngine.UI;
using Bhaptics.Tact;
using Bhaptics.Tact.Unity;


public class BhapticsIsConnectIsPlayingExample : MonoBehaviour
{
    public FileHapticClip[] clips;
    public Text isConnectText;
    public Text clipText;
    public Text isPlayingText;



    void Start()
    {
        if (clipText != null)
        {
            clipText.text = "";
            for (int i = 0; i < clips.Length; ++i)
            {
                clipText.text += clips[i].name;
                if (i < clips.Length - 1)
                {
                    clipText.text += "\n";
                }
            }
        }
        InvokeRepeating("PlayClips", 1f, 2f);
    }

    void Update()
    {
        var hapticPlayer = BhapticsManager.GetHaptic();

        if (hapticPlayer == null)
        {
            return;
        }

        if (isConnectText != null)
        {
            isConnectText.text = hapticPlayer.IsConnect(HapticDeviceType.Tactal) + "\n" +
                                hapticPlayer.IsConnect(HapticDeviceType.TactSuit) + "\n" +
                                hapticPlayer.IsConnect(HapticDeviceType.Tactosy_arms, true) + "/" + hapticPlayer.IsConnect(HapticDeviceType.Tactosy_arms, false) + "\n" +
                                hapticPlayer.IsConnect(HapticDeviceType.Tactosy_hands, true) + "/" + hapticPlayer.IsConnect(HapticDeviceType.Tactosy_hands, false) + "\n" +
                                hapticPlayer.IsConnect(HapticDeviceType.Tactosy_feet, true) + "/" + hapticPlayer.IsConnect(HapticDeviceType.Tactosy_feet, false);
        }

        if (isPlayingText != null)
        {
            isPlayingText.text = "";
            for (int i = 0; i < clips.Length; ++i)
            {
                isPlayingText.text += clips[i].IsPlaying().ToString();
                if (i < clips.Length - 1)
                {
                    isPlayingText.text += "\n";
                }
            }
        }
    }




    private void PlayClips()
    {
        for (int i = 0; i < clips.Length; ++i)
        {
            clips[i].Play();
        }
    }
}