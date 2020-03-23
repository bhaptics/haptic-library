using Bhaptics.Tact.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testaAA : MonoBehaviour {



	// Use this for initialization
	void Start () {
		InvokeRepeating("CheckScan", 0.1f, 0.2f);
	}
	
	private void CheckScan()
	{
		Debug.Log("SCAN : " + bHapticsAndroidManager.Instance.IsScanning);
	}
}
