using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bhaptics.Tact.Unity
{
    public class Android_UIController : MonoBehaviour
    {
        [SerializeField] private Transform devicesContainer;
        [SerializeField] private Android_DeviceController devicePrefab;

        [Header("No Paired Device UI")]
        [SerializeField] private GameObject noPairedDeviceUi;
        [SerializeField] private Button helpButton;


        [SerializeField] private GameObject helpUi;
        [SerializeField] private Button helpCloseButton;

        private List<Android_DeviceController> controllers = new List<Android_DeviceController>();
        private int deviceListSize = 10;

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
    }
}
