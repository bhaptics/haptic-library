using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bhaptics.Tact.Unity;
using UnityEngine.UI;

public class SettingObjectPool : MonoBehaviour {
    [SerializeField] private ScrollRect pariedDeviceScrollrect;
    [SerializeField] private ScrollRect scannedDeviceScrollrect;
    [SerializeField] private PairedDeviceUI pairedDeviceUIGameObject;
    [SerializeField] private ScannedDeviceUI scannedDeviceUIGameObject;
    [SerializeField] private int objectCount;

    
    private List<PairedDeviceUI> pairedUIList;
    private List<ScannedDeviceUI> scannedUIList;

	void Start () {
        pairedUIList = new List<PairedDeviceUI>();
        scannedUIList = new List<ScannedDeviceUI>();

        for(int i = 0; i < objectCount; i++)
        {
            pairedUIList.Add(Instantiate(pairedDeviceUIGameObject, pariedDeviceScrollrect.content) as PairedDeviceUI);
            scannedUIList.Add(Instantiate(scannedDeviceUIGameObject, scannedDeviceScrollrect.content)as ScannedDeviceUI);

            pairedUIList[i].gameObject.SetActive(false);
            scannedUIList[i].gameObject.SetActive(false);
        }
	}

    public PairedDeviceUI GetPairedDeviceUI()
    {
        for(int i = 0; i < pairedUIList.Count; i++)
        {
            if (pairedUIList[i].gameObject.activeSelf)
            {
                continue;
            }
            return pairedUIList[i];
        }
        return null;
    }
    public ScannedDeviceUI GetScannedDeviceUI()
    {
        for(int i = 0; i < scannedUIList.Count; i++)
        {
            if (scannedUIList[i].gameObject.activeSelf)
            {
                continue;
            }
            return scannedUIList[i];
        }
        return null;
    }

    public void AllDeviceUIDisable()
    {
        for(int i = 0; i < pairedUIList.Count; i++)
        { 
            pairedUIList[i].gameObject.SetActive(false);
        }
        for(int i = 0; i < scannedUIList.Count; i++)
        { 
            scannedUIList[i].gameObject.SetActive(false);
        }
    }
}
