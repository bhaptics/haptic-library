using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bhaptics.Tact.Unity
{
    public class Android_UIController : MonoBehaviour
    {
        [SerializeField] private RectTransform MainMenu;
        [SerializeField] private Transform devicesContainer;
        [SerializeField] private Android_DeviceController devicePrefab;

        [Header("No Paired Device UI")]
        [SerializeField] private GameObject noPairedDeviceUi;
        [SerializeField] private Button helpButton;
        [SerializeField] private Button bHpaticsLinkButton;


        [SerializeField] private GameObject helpUi;
        [SerializeField] private Button helpCloseButton;

        private List<Android_DeviceController> controllers = new List<Android_DeviceController>();
        private int deviceListSize = 10;

        [SerializeField]
        private int containerMaxHeight = 360;

        private int containerDefaultHeight = 185; 

        void Awake()
        {
            for (int i = 0; i < deviceListSize; i++)
            {
                var go = Instantiate(devicePrefab, devicesContainer.transform);
                go.gameObject.SetActive(false);
                controllers.Add(go);
            }

            BhapticsAndroidManager.AddRefreshAction(Refresh);
            if (helpButton != null)
            {
                helpButton.onClick.AddListener(OnHelp);
            }

            if (helpCloseButton != null)
            {
                helpCloseButton.onClick.AddListener(CloseHelpNotification);
            }
            if (bHpaticsLinkButton != null)
            {
                bHpaticsLinkButton.onClick.AddListener(OpenLink);
            }
        }


        private void Refresh()
        {
            var devices = BhapticsAndroidManager.GetDevices();

            if (devices.Count == 0)
            {
                noPairedDeviceUi.SetActive(true);
            }
            else
            {
                if (devices.Count >= 4)
                {
                    var rectTransform = devicesContainer.GetComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, containerMaxHeight);
                    MainMenu.sizeDelta = new Vector2(MainMenu.rect.width, containerMaxHeight + 119);
                }
                else
                {
                    var rectTransform = devicesContainer.GetComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, containerDefaultHeight);
                    MainMenu.sizeDelta = new Vector2(MainMenu.rect.width, containerDefaultHeight + 119);
                }

                noPairedDeviceUi.SetActive(false);
                helpUi.gameObject.SetActive(false);
            }

            for (int i = 0; i < deviceListSize; i++)
            {
                if (i <= devices.Count - 1)
                {
                    controllers[i].RefreshDevice(devices[i]);
                }
                else
                {
                    controllers[i].gameObject.SetActive(false);
                }
            }
        }

        private void OnHelp()
        {
            if (helpUi != null)
            {
                helpUi.SetActive(true);
            }
        }

        private void CloseHelpNotification()
        {
            if (helpUi != null)
            {
                helpUi.SetActive(false);
            }
        }

        private void OpenLink()
        {
            Application.OpenURL("https://www.bhaptics.com/support/download");
        }
    }
}
