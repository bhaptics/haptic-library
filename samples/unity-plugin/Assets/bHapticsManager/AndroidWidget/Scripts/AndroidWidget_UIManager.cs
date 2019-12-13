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
        Dark, Light, Dark_Simple, Light_Simple
    }
    public class AndroidWidget_UIManager : MonoBehaviour
    {
        [SerializeField] private WidgetType WidgetType;
        [SerializeField] private bool isActivateWidget = true;

        [SerializeField] private GameObject darkWidgetObject;
        [SerializeField] private GameObject lightWidgetObject;
        [SerializeField] private GameObject darkSimpleWidgetObject;
        [SerializeField] private GameObject lightSimpleWidgetObject;

        private GameObject widget;


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
        private AndroidWidget_ObjectPool settingObjectPool;

        private void Awake()
        {
#if !UNITY_ANDROID
            darkWidgetObject.SetActive(false);
            lightWidgetObject.SetActive(false);
            darkSimpleWidgetObject.SetActive(false);
            lightSimpleWidgetObject.SetActive(false);

            enabled = false;
            return;
#endif
            if (!isActivateWidget)
            {
                return;
            }
        }

        private void OnEnable()
        {
#if !UNITY_ANDROID
            return;
#endif
            darkWidgetObject.SetActive(false);
            lightWidgetObject.SetActive(false);
            darkSimpleWidgetObject.SetActive(false);
            lightSimpleWidgetObject.SetActive(false);

            switch (WidgetType)
            {
                case WidgetType.Dark:
                    widget = darkWidgetObject;
                    break;
                case WidgetType.Light:
                    widget = lightWidgetObject;
                    break;
                case WidgetType.Dark_Simple:
                    widget = darkSimpleWidgetObject;
                    break;
                case WidgetType.Light_Simple:
                    widget = lightSimpleWidgetObject;
                    break;
                default:
                    widget = null;
                    break;
            }

            if (widget != null)
            {
                widget.SetActive(true);
                scanButton = widget.GetComponent<AndroidWidget_UI>().scanButton.transform;
                settingObjectPool = widget.GetComponent<AndroidWidget_ObjectPool>();
            }
            else
            {
                Debug.LogError("Widget Object is null");
            }
        }
        private void OnDisable()
        {
#if !UNITY_ANDROID
            return;
#endif
            widget.SetActive(false);
            widget = null;
        }

        public void Refresh(List<BhapticsDevice> devices, bool isScanning)
        {
            if (!isActivateWidget)
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
            if (!isActivateWidget)
            {
                return;
            }
            foreach (var device in devices)
            {
                if (device.IsPaired)
                {
                    bool isConnect = (AndroidWidget_CompareDeviceString.convertConnectionStatus(device.ConnectionStatus) == 0);

                    AndroidWidget_PairedDeviceUI deviceUI = settingObjectPool.GetPairedDeviceUI();
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
            if (!isActivateWidget)
            {
                return;
            }
            foreach (var device in devices)
            {
                if (!device.IsPaired)
                {
                    AndroidWidget_ScannedDeviceUI deviceUI = settingObjectPool.GetScannedDeviceUI();
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
            if (!isActivateWidget)
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