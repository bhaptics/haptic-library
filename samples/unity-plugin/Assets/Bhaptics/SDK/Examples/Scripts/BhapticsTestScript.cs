using Bhaptics.Tact.Unity;
using UnityEngine;
using UnityEngine.UI;


public class BhapticsTestScript : MonoBehaviour
{
    [SerializeField] private HapticClip[] tactClips;


    public Slider intensitySlider;
    public Slider durationSlider;
    public Slider offsetXSlider;
    public Slider offsetYSlider;

    public Toggle reflectToggle;

    [SerializeField] private Button openBluetoothSettingButton, playButton, toggleEnableAllButton;

    void Awake()
    {
        if (openBluetoothSettingButton != null)
        {
            openBluetoothSettingButton.onClick.AddListener(Open);
        }
        if (playButton != null)
        {
            playButton.onClick.AddListener(Play);
        }
        if (toggleEnableAllButton != null)
        {
            toggleEnableAllButton.onClick.AddListener(ToggleEnable);
        }
    }

    private void ToggleEnable()
    {
        foreach (var hapticDevice in BhapticsAndroidManager.GetDevices())
        {
            BhapticsAndroidManager.ToggleEnableDevice(hapticDevice);
        }
    }

    private void Open()
    {
        BhapticsAndroidManager.ShowBluetoothSetting();
    }

    private void Play()
    {
        foreach (var hapticClip in tactClips)
        {
            var fileHapticClip = hapticClip as ArmsHapticClip;
            if (fileHapticClip != null)
            {
                fileHapticClip.IsReflect = reflectToggle.isOn;
            }

            hapticClip.Play(intensitySlider.value, durationSlider.value, offsetXSlider.value, offsetYSlider.value);
        }
    }
}