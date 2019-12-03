using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bhaptics.Tact.Unity
{

    public class AndroidWidget_ScannedDeviceUI : MonoBehaviour
    { 
        [Header("[UI]")]
        [SerializeField] private Image deviceImage;
        [SerializeField] private Text deviceName;
        [SerializeField] private Button pairButton;

        private BhapticsDevice device;  
        void Start()
        {
            pairButton.onClick.AddListener(OnPair);
        }

        public void Setup(BhapticsDevice tactDevice, Sprite sprite)
        {
            device = tactDevice;
            deviceName.text = device.DeviceName; 
            if (sprite != null)
            {
                deviceImage.sprite = sprite;
            }
        }


        private void OnPair()
        {
            if (AndroidWidget_CompareDeviceString.convertConnectionStatus(device.ConnectionStatus) == 2)
            {
                AndroidWidget_DeviceManager.Instance.Pair(device.Address);
            }
        }
    }
}