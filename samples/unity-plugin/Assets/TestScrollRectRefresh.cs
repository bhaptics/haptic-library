using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestScrollRectRefresh : MonoBehaviour {
    [SerializeField] private ScrollRect scrollRect;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AAA()
    {
        scrollRect.viewport.gameObject.SetActive(!scrollRect.viewport.gameObject.activeSelf);
    }
}
