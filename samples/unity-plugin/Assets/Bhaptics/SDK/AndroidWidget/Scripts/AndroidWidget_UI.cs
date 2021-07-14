using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bhaptics.Tact.Unity
{
    public class AndroidWidget_UI : MonoBehaviour
    {
        [SerializeField] private GameObject uiContainer;
        [SerializeField] private Button pingAllButton;


        [Header("DeviceImages")] [SerializeField]
        private Bhaptics_Widget_Setting WidgetSetting;

        private AndroidWidget_ObjectPool settingObjectPool;
        private AudioSource buttonClickAudio;
        private Animator animator;

        private bool widgetActive = true;

        private AndroidWidget_ControlButton[] controllButtons;

        public static AndroidWidget_UI Instance;

        void Awake()
        {
            InitializeButtons();
            buttonClickAudio = GetComponent<AudioSource>();
            settingObjectPool = GetComponent<AndroidWidget_ObjectPool>();


            controllButtons = GetComponentsInChildren<AndroidWidget_ControlButton>(true);
            animator = GetComponent<Animator>();
            GetComponent<Canvas>().worldCamera = Camera.main;
            Instance = this;
            BhapticsAndroidManager.AddRefreshAction(Refresh);
        }

        void Start()
        {
            if (WidgetSetting == null)
            {
                BhapticsLogger.LogError("[bhaptics] WidgetSetting is null");
            }

            Refresh();

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
            }
            pingAllButton.onClick.AddListener(BhapticsAndroidManager.PingAll);
        }

        // calling from the UI Button
        public void ToggleWidgetButton()
        {
            widgetActive = !widgetActive;

            if (widgetActive)
            {
                animator.Play("ShowWidget");
                
                ShowWidget();
            }
            else
            {
                animator.Play("HideWidget");
                HideWidget();
            }
        }

        public void ShowWidget()
        {
            uiContainer.SetActive(true);
        }

        public void HideWidget()
        {
            uiContainer.SetActive(false);
        } 

        public void ButtonClickSound()
        {
            buttonClickAudio.Play();
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

        public void Refresh()
        {
            var devices = BhapticsAndroidManager.GetDevices();

            if (settingObjectPool != null)
            {
                settingObjectPool.DisableAll();
            }

            RefreshPairedDevices(devices);

            foreach (var controlButton in controllButtons)
            {
                controlButton.Refresh();
            }
        }
        #endregion
    }
}
