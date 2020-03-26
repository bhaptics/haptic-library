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
        Dark, Light, Dark_Simple, Light_Simple, None
    }
    public class AndroidWidget_UIManager : MonoBehaviour
    {
        [SerializeField] private WidgetType WidgetType;

        [SerializeField] private GameObject darkWidgetObject;
        [SerializeField] private GameObject lightWidgetObject;
        [SerializeField] private GameObject darkSimpleWidgetObject;
        [SerializeField] private GameObject lightSimpleWidgetObject;

        private GameObject widget;
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
                case WidgetType.None:
                default:
                    widget = null;
                    break;
            }

            if (widget != null)
            {
                widget.SetActive(true);
            }
        }
        private void OnDisable()
        {
#if !UNITY_ANDROID
            return;
#endif
            if (widget != null)
            {
                widget.SetActive(false);
                widget = null;
            }
        }

        
    }
}