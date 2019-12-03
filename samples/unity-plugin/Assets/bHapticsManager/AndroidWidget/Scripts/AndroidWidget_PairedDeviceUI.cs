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

        private BhapticsDevice device;  
        void Start()
        {
            pingButton.onClick.AddListener(OnPing);
            unPairButton.onClick.AddListener(OnUnpair);
            toggleButton.onClick.AddListener(OnSwap);
        }
        
        public void Setup(BhapticsDevice tactDevice, bool isConnect, Sprite sprite)
        {
            device = tactDevice;
            deviceName.text = device.DeviceName;
            toggleButton.interactable = isConnect;

            if (tactDevice.DeviceName.StartsWith("Tactal") || tactDevice.DeviceName.StartsWith("Tactot"))
            {
                toggleButton.gameObject.SetActive(false);
            }
            else
            {
                if (isConnect)
                {
                    if (device.IsLeft())
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
            if (sprite != null)
            {
                deviceImage.sprite = sprite;
            }
        }


        private void OnPing()
        {
            if (AndroidWidget_CompareDeviceString.convertConnectionStatus(device.ConnectionStatus) == 0)
            {
                AndroidWidget_DeviceManager.Instance.Ping(device.Address);
            }
        }

        private void OnUnpair()
        {
            if (AndroidWidget_CompareDeviceString.convertConnectionStatus(device.ConnectionStatus) == 0 ||
                (AndroidWidget_CompareDeviceString.convertConnectionStatus(device.ConnectionStatus) == 2 && device.IsPaired))
            { 
                AndroidWidget_DeviceManager.Instance.Unpair(device.Address);
            }
        }

        private void OnSwap()
        {
            if (AndroidWidget_CompareDeviceString.convertConnectionStatus(device.ConnectionStatus) == 0)
            {
                AndroidWidget_DeviceManager.Instance.TogglePosition(device.Address);
                if (device.IsLeft())
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
