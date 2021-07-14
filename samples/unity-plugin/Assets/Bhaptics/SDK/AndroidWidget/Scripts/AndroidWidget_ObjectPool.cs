using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bhaptics.Tact.Unity
{
    public class AndroidWidget_ObjectPool : MonoBehaviour
    {
        [SerializeField] private ScrollRect pariedDeviceScrollrect;
        [SerializeField] private AndroidWidget_PairedDeviceUI pairedDeviceUIGameObject;
        [SerializeField] private int objectCount;


        private List<AndroidWidget_PairedDeviceUI> pairedUIList;


        void Awake()
        {
            pairedUIList = new List<AndroidWidget_PairedDeviceUI>();

            for (int i = 0; i < objectCount; i++)
            {
                pairedUIList.Add(Instantiate(pairedDeviceUIGameObject, pariedDeviceScrollrect.content) as AndroidWidget_PairedDeviceUI);

                pairedUIList[i].gameObject.SetActive(false);
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

        public void DisableAll()
        {
            for (int i = 0; i < pairedUIList.Count; i++)
            {
                pairedUIList[i].gameObject.SetActive(false);
            }
        }
    }
}