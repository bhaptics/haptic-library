using Bhaptics.Tact.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testaAA : MonoBehaviour {
	public TactSource tactSource;



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton(0))
		{
			tactSource.Play();
			if(!AndroidWidget_DeviceManager.Instance.IsScanning)
				AndroidWidget_DeviceManager.Instance.Scan();
		}
	}
}
