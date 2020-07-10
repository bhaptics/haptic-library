using Bhaptics.Tact.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo_bHapticsScanning : MonoBehaviour {

	// Use this for initialization
	void Start() {
	}

	// Update is called once per frame
	void Update() {

		if (Input.anyKeyDown)
		{
			if (!AndroidPermissionsManager.CheckBluetoothPermissions())
			{
				AndroidPermissionsManager.RequestPermission();
			}
		}

        BhapticsAndroidManager.Scan();
	}

}
