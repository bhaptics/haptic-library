using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bhaptics.Tact.Unity
{ 
    public class AndroidWidget_UI : MonoBehaviour
    {
        private const float autoHideTime = 60f;


        [SerializeField] private GameObject uiContainer;
        [SerializeField] private Button pingAllButton;
        [SerializeField] private Button unpairAllButton;
        public Button scanButton;

        private Coroutine scanCoroutine;
        private AudioSource buttonClickAudio;
        private Animator animator;
        private bool widgetActive;
        private float hideTimer;

        void Start()
        {
            buttonClickAudio = GetComponent<AudioSource>();
            animator = GetComponent<Animator>();
            GetComponent<Canvas>().worldCamera = Camera.main;
            animator.Play("HideWidget", -1, 1);
            ButtonInitialize();
        }

        private void OnEnable()
        {
            if(animator != null)
            {
                animator.Play("HideWidget", -1, 1);
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
            scanButton.onClick.AddListener(AndroidWidget_DeviceManager.Instance.ScanButton);
            pingAllButton.onClick.AddListener(AndroidWidget_DeviceManager.Instance.PingAll);
            unpairAllButton.onClick.AddListener(AndroidWidget_DeviceManager.Instance.UnpairAll);
        }

        public void ToggleWidgetButton()
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
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
                AndroidWidget_DeviceManager.Instance.ForceUpdateDeviceList();
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
                AndroidWidget_DeviceManager.Instance.ScanStop();
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
                if (!AndroidWidget_DeviceManager.Instance.IsScanning)
                {
                    AndroidWidget_DeviceManager.Instance.Scan();
                }

                if(hideTimer < 0f)
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
                yield return new WaitForSeconds(0.5f);
            }
        }

        private void ResetHideTimer()
        {
            hideTimer = autoHideTime;
        }
    } 
}
