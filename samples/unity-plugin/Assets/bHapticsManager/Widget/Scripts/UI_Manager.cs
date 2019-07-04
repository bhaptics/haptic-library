using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Bhaptics.Tact.Unity
{
    [Serializable]
    public class DeviceIcon
    {
        public Sprite pairImage;
        public Sprite unpairImage;
        public Sprite scanImage;
    }
    public enum WidgetType
    {
        Dark, Light
    }
    public class UI_Manager : MonoBehaviour
    {
        [SerializeField] private bool IsActivateWidget = true;
        [SerializeField] private WidgetType WidgetType;

        [SerializeField] private GameObject darkWidgetPrefab;
        [SerializeField] private GameObject lightWidgetPrefab;

        private Transform scanButton; 
        private ScrollRect pairedDeviceScrollrect; 
        private ScrollRect ScannedDeviceScrollrect;

        [Header("DeviceImages")]
        [SerializeField] private DeviceIcon Tactosy;
        [SerializeField] private DeviceIcon Tactot;
        [SerializeField] private DeviceIcon TactosyH;
        [SerializeField] private DeviceIcon TactosyF;
        [SerializeField] private DeviceIcon Tactal;

        private IEnumerator ScanAnimationCor;
        private SettingObjectPool settingObjectPool;

        private void Awake()
        {
            #if !UNITY_ANDROID
                    return;
            #endif

            if (!IsActivateWidget)
            {
                return;
            }
            GameObject widget;
            if (WidgetType == WidgetType.Dark)
            {
                widget = Instantiate(darkWidgetPrefab, transform);
            }
            else
            {
                widget = Instantiate(lightWidgetPrefab, transform);
            }

            scanButton = widget.GetComponent<UI_Initialize>().scanButton.transform;
            settingObjectPool = widget.GetComponent<SettingObjectPool>();
        }

        public void Refresh(List<BhapticsDevice> devices, bool isScanning)
        {
            if (!IsActivateWidget)
            {
                return;
            }

            settingObjectPool.AllDeviceUIDisable();
            PairedUiRefresh(devices);
            RefreshScanButtonUi(isScanning);
            if (isScanning)
            {
                ScannedUiRefresh(devices);
            }  
        }
        
        private void PairedUiRefresh(List<BhapticsDevice> devices)
        {
            if (!IsActivateWidget)
            {
                return;
            }
            foreach (var device in devices)
            {
                if (device.IsPaired)
                {
                    bool isConnect = (CompareDeviceString.convertConnectionStatus(device.ConnectionStatus) == 0);

                    PairedDeviceUI deviceUI = settingObjectPool.GetPairedDeviceUI();
                    if (deviceUI != null)
                    {
                        deviceUI.Setup(device, isConnect, GetPairedDeviceSprite(device.DeviceName, isConnect));
                        deviceUI.gameObject.SetActive(true);
                    }
                }
            }
        }

        private void ScannedUiRefresh(List<BhapticsDevice> devices)
        {
            if (!IsActivateWidget)
            {
                return;
            }
            foreach (var device in devices)
            {
                if (!device.IsPaired)
                {
                    ScannedDeviceUI deviceUI = settingObjectPool.GetScannedDeviceUI();
                    if (deviceUI != null)
                    {
                        deviceUI.Setup(device, GetScannedDeviceSprite(device.DeviceName));
                        deviceUI.gameObject.SetActive(true);
                    }
                }
            }
        } 
        private void RefreshScanButtonUi(bool isScanning)
        {
            if (!IsActivateWidget)
            {
                return;
            }
            if (isScanning && ScanAnimationCor == null)
            {
                ScanAnimationCor = ScanAnimation(isScanning);
                StartCoroutine(ScanAnimationCor);
            }
            else if (!isScanning && ScanAnimationCor != null)
            {
                StopCoroutine(ScanAnimationCor);
                ScanAnimationCor = null;
            }
        }
        private IEnumerator ScanAnimation(bool isScanning)
        { 
            while (isScanning)
            {
                scanButton.Rotate(0f, 0f, -5f);
                yield return null;
            }

            ScanAnimationCor = null;
        }

        #region GetSprites

        public Sprite GetPairedDeviceSprite(string deviceType, bool isConnect)
        {
            if (deviceType.StartsWith("TactosyH"))
            {
                return isConnect ? TactosyH.pairImage : TactosyH.unpairImage; 
            }

            if (deviceType.StartsWith("TactosyF"))
            {
                return isConnect ? TactosyF.pairImage : TactosyF.unpairImage; 
            }

            if (deviceType.StartsWith("Tactosy"))
            {
                return isConnect ? Tactosy.pairImage : Tactosy.unpairImage;  
            }

            if (deviceType.StartsWith("Tactal"))
            {
                return isConnect ? Tactal.pairImage : Tactal.unpairImage; 
            }

            if (deviceType.StartsWith("Tactot"))
            {
                return isConnect ? Tactot.pairImage : Tactot.unpairImage; 
            }

            return null;
        }

        public Sprite GetScannedDeviceSprite(string deviceType)
        {
            if (deviceType.StartsWith("TactosyH"))
            {
                return TactosyH.scanImage;
            }

            if (deviceType.StartsWith("TactosyF"))
            {
                return TactosyF.scanImage;
            }

            if (deviceType.StartsWith("Tactosy"))
            {
                return Tactosy.scanImage;
            }

            if (deviceType.StartsWith("Tactal"))
            {
                return Tactal.scanImage;
            }

            if (deviceType.StartsWith("Tactot"))
            {
                return Tactot.scanImage;
            }

            return null;
        }

        #endregion          
    }
}