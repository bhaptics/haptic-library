using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Tactosy.Common;

/* Class helps to visualize feedbacks for the Vest Product */
public class Manager_VestModel : MonoBehaviour
{
    /* All dots should be registered */
    [SerializeField] private GameObject[] Dots;

    /* Initialization,default colors and scales of dots can be modified by user */
    void Start()
    {
        for (int i = 0; i < 20; i++)
        {
            Dots[i].GetComponent<MeshRenderer>().material.color = new Color(0f, 0f, 0f, 0.2f);
            Dots[i].GetComponent<Transform>().localScale = new Vector3(4.3f, 0.14f, 4.3f);
        }
    }

    /* Change the color and the scale of the dot according to haptic feedback */
    void UpdateFeedbacks(TactosyFeedback tactosyFeedback)
    {
        for (int i = 0; i < 20; i++)
        {
            /* Delta values of colors and scales can be modified */
            var scale = tactosyFeedback.Values[i] * (8f / 10f);
            Dots[i].GetComponent<MeshRenderer>().material.color = new Color(0.2f + scale, 0.2f + scale, 0.2f + scale, 0.2f + scale);
            Dots[i].GetComponent<Transform>().localScale = new Vector3(4.3f + 3.0f * (scale / 100f), 0.14f, 4.3f + 3.0f * (scale / 100f));
        }
    }
}
