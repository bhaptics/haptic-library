using UnityEngine;
using System.Collections;
using TactosyCommon.Unity;

public class KeyboardHandler : MonoBehaviour
{
    public TactosyManager TactosyManager;
	
    // Update is called once per frame
    void Update () {

        if (Input.GetKeyDown("space"))
        {
            TactosyManager.SendSignal("RifleShoot");
        }
    }
}
