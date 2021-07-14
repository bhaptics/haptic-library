using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bhaptics.Tact.Unity
{
    public class AndroidWidget_ControlButton : MonoBehaviour
    {

        [SerializeField] private PositionType DeviceType;

        [Header("Images")] [SerializeField] private Sprite defaultImage;
        [SerializeField] private Sprite pairImage;
        [SerializeField] private Sprite defaultHoverImage;
        [SerializeField] private Sprite pairHoverImage;


        [SerializeField] private Transform pairDeviceCount;

        private Button button;
        
        void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClickDevice);
        }

        public void Refresh()
        {
            if (button == null)
            {
                // TODO
                Debug.LogFormat("not ready.");
                return;
            }

            BhapticsLogger.LogDebug("Refresh()");

            var connectedDevices = BhapticsAndroidManager.GetConnectedDevices(DeviceType);
            if (connectedDevices.Count > 0)
            {
                button.image.sprite = pairImage;
                var spriteState = button.spriteState;
                spriteState.highlightedSprite = pairHoverImage;
                button.spriteState = spriteState;

                for (int i = 0; i < pairDeviceCount.childCount; i++)
                {
                    if (!pairDeviceCount.GetChild(i).gameObject.activeSelf)
                    {
                        break;
                    }

                    pairDeviceCount.GetChild(i).gameObject.SetActive(false);
                }

                for (int i = 0; i < connectedDevices.Count; i++)
                {
                    if (pairDeviceCount.GetChild(i) != null)
                    {
                        pairDeviceCount.GetChild(i).gameObject.SetActive(true);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                BhapticsLogger.LogDebug("button + ", " + defaultImage " + DeviceType + ", " + button);
                button.image.sprite = defaultImage;
                var spriteState = button.spriteState;
                spriteState.highlightedSprite = defaultHoverImage;
                button.spriteState = spriteState;
                

                for (int i = 0; i < pairDeviceCount.childCount; i++)
                {
                    pairDeviceCount.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

        public void OnPingDevice()
        {
            BhapticsAndroidManager.Ping(DeviceType);
        }

        public void OnClickDevice()
        {
            var connectedDevices = BhapticsAndroidManager.GetConnectedDevices(DeviceType);

            if (connectedDevices.Count > 0)
            {
                OnPingDevice();
            }
        }
    }
}
