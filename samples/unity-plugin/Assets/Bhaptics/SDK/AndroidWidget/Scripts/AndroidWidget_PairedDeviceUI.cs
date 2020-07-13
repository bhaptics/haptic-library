using UnityEngine;
using UnityEngine.UI;

namespace Bhaptics.Tact.Unity
{
    public class AndroidWidget_PairedDeviceUI : MonoBehaviour
    {
        [Header("[UI]")] [SerializeField] private Image deviceImage;
        [SerializeField] private Text deviceName;
        [SerializeField] private Button pingButton;
        [SerializeField] private Button unPairButton;
        [SerializeField] private Button toggleButton;


        [Header("[Sprites]")] [SerializeField] private Sprite leftSide;
        [SerializeField] private Sprite rightSide;

        private HapticDevice device;  
        void Start()
        {
            pingButton.onClick.AddListener(OnPing);
            unPairButton.onClick.AddListener(OnUnpair);
            toggleButton.onClick.AddListener(OnSwap);
        }
        
        public void Refresh(HapticDevice tactDevice, Bhaptics_Widget_Setting setting)
        {
            device = tactDevice;
            deviceName.text = device.DeviceName;
            toggleButton.interactable = tactDevice.IsConnected;

            if (!AndroidUtils.CanChangePosition(tactDevice.Position))
            {
                toggleButton.gameObject.SetActive(false);
            }
            else
            {
                if (tactDevice.IsConnected)
                {
                    if (AndroidUtils.IsLeft(device.Position))
                    {
                        toggleButton.image.sprite = leftSide;
                    }
                    else
                    {
                        toggleButton.image.sprite = rightSide;
                    }
                }

                toggleButton.gameObject.SetActive(true);
            }
            
            if (setting != null)
            {
                deviceImage.sprite = setting.GetPairedDeviceSprite(device);
            }
        }


        private void OnPing()
        {
            if (device.IsConnected)
            {
                BhapticsAndroidManager.Ping(device);
            }
        }

        private void OnUnpair()
        {
            if (device.IsConnected ||
                (AndroidUtils.ConvertConnectionStatus(device.ConnectionStatus) == 2 && device.IsPaired))
            { 
                BhapticsAndroidManager.Unpair(device.Address);
            }
        }

        private void OnSwap()
        {
            if (device.IsConnected)
            {
                BhapticsAndroidManager.TogglePosition(device.Address);
                if (AndroidUtils.IsLeft(device.Position))
                {
                    toggleButton.image.sprite = leftSide;
                }
                else
                {
                    toggleButton.image.sprite = rightSide;
                }
            }
        }
    }
}
