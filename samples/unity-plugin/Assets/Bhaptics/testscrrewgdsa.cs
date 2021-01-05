using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bhaptics.Tact.Unity;


public class testscrrewgdsa : MonoBehaviour
{
	public FileHapticClip clip;
	public GameObject target;

	void Start()
	{

	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			clip.Play();
			target.SetActive(!target.activeInHierarchy);
		}
	}
}
