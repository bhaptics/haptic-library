using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Tactosy.Common;

public class Manager_HandModel : MonoBehaviour
{
    [SerializeField] private GameObject[] Dots;

	// Use this for initialization
	void Start () {
        for (int i = 0; i< 20; i++)
        {
            Dots[i].GetComponent<MeshRenderer>().material.color = new Color(0f, 0f, 0f, 0.2f);
            Dots[i].GetComponent<Transform>().localScale = new Vector3(4.3f, 0.14f, 4.3f);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void UpdateFeedbacks(TactosyFeedback tactosyFeedback)
    {
        for (int i = 0; i < 20; i++)
        {
            var scale = tactosyFeedback.Values[i] * (8f / 10f);
            Dots[i].GetComponent<MeshRenderer>().material.color = new Color(0.2f + scale, 0.2f + scale, 0.2f + scale, 0.2f + scale);
            Dots[i].GetComponent<Transform>().localScale = new Vector3(4.3f + 3.0f * (scale / 100f), 0.14f, 4.3f + 3.0f * (scale / 100f));
        }
    }
}
