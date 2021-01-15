using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bhaptics.Tact;
using Bhaptics.Tact.Unity;


public class BhapticsDotPointExample : MonoBehaviour
{
    public int motorIndex = 0;
    public int motorIntensity = 100;


    private BhapticsDotPointControllerExample controller;
    private DotPoint dotPoint;



    void Awake()
    {
        dotPoint = new DotPoint(motorIndex, motorIntensity);
        controller = GetComponentInParent<BhapticsDotPointControllerExample>();
    }





    public void Toggle()
    {
        if (controller == null)
        {
            return;
        }
        controller.Toggle(dotPoint);
    }
}