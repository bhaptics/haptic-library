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

        private HapticDevice device;  
        void Start()
        {
            pairButton.onClick.AddListener(OnPairSelected);
        }

        public void Refresh(HapticDevice tactDevice, Bhaptics_Widget_Setting setting)
        {
            device = tactDevice;
            deviceName.text = device.DeviceName; 
            if (setting != null)
            {
                deviceImage.sprite = setting.GetScannedDeviceSprite(device.DeviceName);
            }
        }


        private void OnPairSelected()
        {
            if (AndroidUtils.ConvertConnectionStatus(device.ConnectionStatus) == 2)
            {
                BhapticsAndroidManager.Pair(device.Address);
            }
        }
    }
}