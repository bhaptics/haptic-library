using Bhaptics.Tact.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour {
	// Use this for initialization
	void Start ()
	{
		if (AndroidWidget_DeviceManager.Instance != null)
		{
			AndroidWidget_DeviceManager.Instance.AddListener(AAA);
		}
	}
	bool toggle;
	// Update is called once per frame
	void Update () {
			if (toggle)
		{
			transform.Translate(Vector3.right * Time.deltaTime * 2);
		}
		else
		{
			transform.Translate(Vector3.left * Time.deltaTime * 2);
		}

		if (transform.position.x > 1)
		{
			toggle = false;
		}
		else if (transform.position.x < -1) 
		{
			toggle = true;
		}
	}
	
}
