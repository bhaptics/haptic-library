using Bhaptics.Tact.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour {
	// Use this for initialization
	
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
