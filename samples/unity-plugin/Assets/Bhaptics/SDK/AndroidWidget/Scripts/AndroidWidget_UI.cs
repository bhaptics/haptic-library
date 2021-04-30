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


        [Header("DeviceImages")] [SerializeField]
        private Bhaptics_Widget_Setting WidgetSetting;

        private AndroidWidget_ObjectPool settingObjectPool;
        private Coroutine scanCoroutine;
        private AudioSource buttonClickAudio;
        private Animator animator;

        private bool widgetActive;
        private float hideTimer;

        private AndroidWidget_ControlButton[] controllButtons;

        void Awake()
        {
            InitializeButtons();
            buttonClickAudio = GetComponent<AudioSource>();
            settingObjectPool = GetComponent<AndroidWidget_ObjectPool>();


            controllButtons = GetComponentsInChildren<AndroidWidget_ControlButton>(true);
            animator = GetComponent<Animator>();
            GetComponent<Canvas>().worldCamera = Camera.main;
        }

        void Start()
        {
            if (WidgetSetting == null)
            {
                BhapticsLogger.LogError("[bhaptics] WidgetSetting is null");
            }

            if (!alwaysActive)
            {
                animator.Play("HideWidget", -1, 1);
            }
            else
            {
                BhapticsAndroidManager.AddRefreshAction(Refresh);
            }

            Refresh();

        }

        private void OnEnable()
        {
            if (alwaysActive)
            {
                ShowWidget();
                scanCoroutine = StartCoroutine(LoopScan());
            }
            else
            {
                if (animator != null)
                {
                    animator.Play("HideWidget", -1, 1);
                }
            }
        }

        private void OnDisable()
        {
            if(scanCoroutine != null)
            {
                StopCoroutine(scanCoroutine);
                scanCoroutine = null;
            }
        }
        

        private void InitializeButtons()
        {
            var buttons = GetComponentsInChildren<Button>(true);
            foreach (var btn in buttons)
            {
                if (btn.GetComponent<Collider>() == null)
                {
                    var col = btn.gameObject.AddComponent<BoxCollider>();
                    var rect = btn.GetComponent<RectTransform>();
                    col.size = new Vector3(rect.sizeDelta.x, rect.sizeDelta.y, 0f);
                }
                btn.onClick.AddListener(ButtonClickSound);
                btn.onClick.AddListener(ResetHideTimer);
            }
            pingAllButton.onClick.AddListener(BhapticsAndroidManager.PingAll);
            unpairAllButton.onClick.AddListener(BhapticsAndroidManager.UnpairAll);
        }

        // calling from the UI Button
        public void ToggleWidgetButton()
        {
            if (!BhapticsAndroidManager.CheckPermission())
            {
                BhapticsAndroidManager.RequestPermission();
            
                return;
            }

            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || alwaysActive)
            {
                return;
            }

            widgetActive = !widgetActive;

            if (widgetActive)
            {
                animator.Play("ShowWidget");
                
                ShowWidget();
                BhapticsAndroidManager.AddRefreshAction(Refresh);
            }
            else
            {
                animator.Play("HideWidget");
                HideWidget();

                BhapticsAndroidManager.RemoveRefreshAction();
            }
        }

        public void ShowWidget()
        {
            uiContainer.SetActive(true);
            hideTimer = autoHideTime;

            BhapticsAndroidManager.Scan();
            scanCoroutine = StartCoroutine(LoopScan());
        }

        public void HideWidget()
        {
            uiContainer.SetActive(false);

            BhapticsAndroidManager.ScanStop();

            if (scanCoroutine != null)
            {
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
                if (!alwaysActive)
                {
                    if (hideTimer < 0f)
                    {
                        scanCoroutine = null;
                        animator.Play("HideWidget");
                        HideWidget();
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
        private void RefreshPairedDevices(List<HapticDevice> devices)
        {
            foreach (var device in devices)
            {
                if (device.IsPaired)
                {
                    var ui = settingObjectPool.GetPairedDeviceUI();
                    if (ui == null)
                    {
                        continue;
                    }

                    ui.Refresh(device, WidgetSetting);
                    ui.gameObject.SetActive(true);
                }
            }
        }

        private void RefreshScannedDevices(List<HapticDevice> devices)
        {
            foreach (var device in devices)
            {
                if (!device.IsPaired)
                {
                    var ui = settingObjectPool.GetScannedDeviceUI();
                    if (ui == null)
                    {
                        continue;
                    }
                    ui.Refresh(device, WidgetSetting);
                    ui.gameObject.SetActive(true);
                }
            }
        }

        public void Refresh()
        {
            var devices = BhapticsAndroidManager.GetDevices();

            if (settingObjectPool != null)
            {
                settingObjectPool.DisableAll();
            }

            RefreshPairedDevices(devices);
            RefreshScannedDevices(devices);

            foreach (var controlButton in controllButtons)
            {
                controlButton.Refresh();
            }
        }
        #endregion
    }
}
