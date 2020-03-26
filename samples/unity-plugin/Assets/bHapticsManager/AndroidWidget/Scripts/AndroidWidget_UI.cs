using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bhaptics.Tact.Unity
{ 
    public class AndroidWidget_UI : MonoBehaviour
    {
        private const float autoHideTime = 60f;
        [SerializeField] private bool alwaysActive;
        [SerializeField] private GameObject uiContainer;
        [SerializeField] private Button pingAllButton;
        [SerializeField] private Button unpairAllButton;


        [Header("DeviceImages")]
        [SerializeField] private DeviceIcon Tactosy;
        [SerializeField] private DeviceIcon Tactot;
        [SerializeField] private DeviceIcon TactosyH;
        [SerializeField] private DeviceIcon TactosyF;
        [SerializeField] private DeviceIcon Tactal;

        private AndroidWidget_ObjectPool settingObjectPool;
        private Coroutine scanCoroutine;
        private AudioSource buttonClickAudio;
        private Animator animator;
        private bool widgetActive;
        private float hideTimer;

        void Start()
        {
            ButtonInitialize();
            buttonClickAudio = GetComponent<AudioSource>();
            settingObjectPool = GetComponent<AndroidWidget_ObjectPool>();
            animator = GetComponent<Animator>();
            GetComponent<Canvas>().worldCamera = Camera.main;

            if (!alwaysActive)
            {
                animator.Play("HideWidget", -1, 1);
            }

            if (BhapticsAndroidManager.Instance != null)
            {
                BhapticsAndroidManager.Instance.RefreshUIAddListener(Refresh);
            }
        }

        private void OnEnable()
        {
            if (alwaysActive)
            {
                if (!AndroidPermissionsManager.CheckBluetoothPermissions())
                {
                    AndroidPermissionsManager.RequestPermission();
                }

                scanCoroutine = StartCoroutine(LoopScan());
            }
            else
            {
                if (animator != null)
                {
                    animator.Play("HideWidget", -1, 1);
                }

            }

            if (BhapticsAndroidManager.Instance != null)
            {
                BhapticsAndroidManager.Instance.RefreshUIAddListener(Refresh);
            }
        }
        private void OnDisable()
        {

            if (alwaysActive)
            { 
                if(scanCoroutine != null)
                {
                    StopCoroutine(scanCoroutine);
                    scanCoroutine = null;
                }
            }


                if (BhapticsAndroidManager.Instance != null)
            {
                BhapticsAndroidManager.Instance.RefreshUIRemoveListener(Refresh);
            }
        }
        

        private void ButtonInitialize()
        {
            var buttons = GetComponentsInChildren<Button>(true);
            foreach (var btn in buttons)
            {
                if (btn.GetComponent<Collider>() == null)
                {
                    BoxCollider col = btn.gameObject.AddComponent<BoxCollider>();
                    RectTransform rect = btn.GetComponent<RectTransform>();
                    col.size = new Vector3(rect.sizeDelta.x, rect.sizeDelta.y, 0f);
                }
                btn.onClick.AddListener(ButtonClickSound);
                btn.onClick.AddListener(ResetHideTimer);
            }
            if (BhapticsAndroidManager.Instance != null)
            {
                pingAllButton.onClick.AddListener(BhapticsAndroidManager.Instance.PingAll);
                unpairAllButton.onClick.AddListener(BhapticsAndroidManager.Instance.UnpairAll);
            }
        }

        public void ToggleWidgetButton()
        {
            if (!AndroidPermissionsManager.CheckBluetoothPermissions())
            {
                AndroidPermissionsManager.RequestPermission();
            }

            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || alwaysActive)
            {
                return;
            }

            if (!widgetActive)
            {
                if (AndroidPermissionsManager.CheckBluetoothPermissions())
                {
                    animator.Play("ShowWidget");
                }
                else
                {
                    AndroidPermissionsManager.RequestPermission();
                    return;
                }
                if (BhapticsAndroidManager.Instance != null)
                {
                    BhapticsAndroidManager.Instance.ForceUpdateDeviceList();
                }
            }
            else
            {
                animator.Play("HideWidget");
            }

            widgetActive = !widgetActive;
        }

        public void ShowWidget()
        {
            uiContainer.SetActive(true);
            hideTimer = autoHideTime;
            scanCoroutine = StartCoroutine(LoopScan());
        }
        public void HideWidget()
        {
            uiContainer.SetActive(false);
            if(scanCoroutine != null)
            {
                if (BhapticsAndroidManager.Instance != null)
                {
                    BhapticsAndroidManager.Instance.ScanStop();
                }
                StopCoroutine(scanCoroutine);
                scanCoroutine = null;
            }
        } 
        public void ButtonClickSound()
        {
            buttonClickAudio.Play();
        }

        private IEnumerator LoopScan()
        {
            while (true)
            {
                if (BhapticsAndroidManager.Instance != null && !BhapticsAndroidManager.Instance.IsScanning)
                {
                    BhapticsAndroidManager.Instance.Scan();
                }

                if (!alwaysActive)
                {
                    if (hideTimer < 0f)
                    {
                        scanCoroutine = null;
                        animator.Play("HideWidget");
                        widgetActive = !widgetActive;
                        break;
                    }
                    else
                    {
                        hideTimer -= 0.5f;
                    }
                }
                yield return new WaitForSeconds(0.5f);
            }
        }

        private void ResetHideTimer()
        {
            hideTimer = autoHideTime;
        }


        #region RefreshUI Function
        private void PairedUiRefresh(List<BhapticsDevice> devices)
        {
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

        public void Refresh(List<BhapticsDevice> devices, bool isScanning)
        {
            settingObjectPool.AllDeviceUIDisable();
            PairedUiRefresh(devices);
            if (isScanning)
            {
                ScannedUiRefresh(devices);
            }
        }
        #endregion
        #region GetUISprites Function
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
