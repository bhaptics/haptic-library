using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bhaptics.Tact.Unity
{
    public class AndroidWidget_ObjectPool : MonoBehaviour
    {
        [SerializeField] private ScrollRect pariedDeviceScrollrect;
        [SerializeField] private ScrollRect scannedDeviceScrollrect;
        [SerializeField] private AndroidWidget_PairedDeviceUI pairedDeviceUIGameObject;
        [SerializeField] private AndroidWidget_ScannedDeviceUI scannedDeviceUIGameObject;
        [SerializeField] private int objectCount;


        private List<AndroidWidget_PairedDeviceUI> pairedUIList;
        private List<AndroidWidget_ScannedDeviceUI> scannedUIList;


        void Start()
        {
            pairedUIList = new List<AndroidWidget_PairedDeviceUI>();
            scannedUIList = new List<AndroidWidget_ScannedDeviceUI>();

            for (int i = 0; i < objectCount; i++)
            {
                pairedUIList.Add(Instantiate(pairedDeviceUIGameObject, pariedDeviceScrollrect.content) as AndroidWidget_PairedDeviceUI);
                scannedUIList.Add(Instantiate(scannedDeviceUIGameObject, scannedDeviceScrollrect.content) as AndroidWidget_ScannedDeviceUI);

                pairedUIList[i].gameObject.SetActive(false);
                scannedUIList[i].gameObject.SetActive(false);
            }
        }

        public AndroidWidget_PairedDeviceUI GetPairedDeviceUI()
        {
            for (int i = 0; i < pairedUIList.Count; i++)
            {
                if (pairedUIList[i].gameObject.activeSelf)
                {
                    continue;
                }
                return pairedUIList[i];
            }
            return null;
        }
        public AndroidWidget_ScannedDeviceUI GetScannedDeviceUI()
        {
            for (int i = 0; i < scannedUIList.Count; i++)
            {
                if (scannedUIList[i].gameObject.activeSelf)
                {
                    continue;
                }
                return scannedUIList[i];
            }
            return null;
        }

        public void DisableAll()
        {
            for (int i = 0; i < pairedUIList.Count; i++)
            {
                pairedUIList[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < scannedUIList.Count; i++)
            {
                scannedUIList[i].gameObject.SetActive(false);
            }
        }
    }
}