using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bhaptics.Tact;
using Bhaptics.Tact.Unity;


public class BhapticsDotPointControllerExample : MonoBehaviour
{
    public HapticClipPositionType clipPositionType;

    [HideInInspector] public List<DotPoint> dotPointList;

    private string key = System.Guid.NewGuid().ToString();

    private int duration = 100;





    void Awake()
    {
        dotPointList = new List<DotPoint>();
    }

    void Update()
    {
        var haptic = BhapticsManager.GetHaptic();

        if (haptic == null)
        {
            return;
        }

        haptic.Submit(key, BhapticsUtils.ToPositionType(clipPositionType), dotPointList, duration);
    }




    public void Toggle(DotPoint dot)
    {
        if (dotPointList.Contains(dot))
        {
            RemoveAtList(dot);
        }
        else
        {
            AddToList(dot);
        }
    }






    private bool AddToList(DotPoint dot)
    {
        if (dotPointList.Contains(dot))
        {
            return false;
        }
        dotPointList.Add(dot);
        return true;
    }

    private bool RemoveAtList(DotPoint dot)
    {
        if (dotPointList.Contains(dot))
        {
            dotPointList.Remove(dot);
            return true;
        }
        return false;
    }
}
