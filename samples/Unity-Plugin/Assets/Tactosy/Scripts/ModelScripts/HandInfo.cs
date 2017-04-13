using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInfo : MonoBehaviour
{
    [SerializeField] private GameObject FeedbackModel;

    public delegate void OnStatusChanged();
    public static event OnStatusChanged onStatusChanged; 

}
