using System.Collections;
using System.Collections.Generic;
using Bhaptics.Tact.Unity;
using UnityEngine;

public class BhapticsWidgetInstiate : MonoBehaviour
{
    [SerializeField]
    private AndroidWidget_UI widgetPrefab;

    void Start()
    {

        Invoke("CreateWidget", 10f);
    }

    void CreateWidget()
    {

        Instantiate(widgetPrefab);
    }
}
