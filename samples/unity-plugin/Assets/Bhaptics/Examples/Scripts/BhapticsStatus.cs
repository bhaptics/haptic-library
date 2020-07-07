using System.Collections;
using System.Collections.Generic;
using Bhaptics.Tact;
using Bhaptics.Tact.Unity;
using UnityEngine;
using UnityEngine.UI;

public class BhapticsStatus : MonoBehaviour
{
    [SerializeField] private Text text;

    void Awake()
    {
        InvokeRepeating("SendPing", 4f, 4f);
    }

    void Update()
    {
        if (text != null)
        {
            var hapticPlayer = BhapticsManager.GetHaptic();
            text.text = "Tactal isActive: " + hapticPlayer.IsActive(PositionType.Head) + "\n" +
                        "Tactot isActive: " + hapticPlayer.IsActive(PositionType.Vest) + "\n" +
                        "Tactosy(L) isActive: " + hapticPlayer.IsActive(PositionType.ForearmL) + "\n" +
                        "Tactosy(R) isActive: " + hapticPlayer.IsActive(PositionType.ForearmR) + "\n" ;
        }

    }

    private void SendPing()
    {
        var hapticPlayer = BhapticsManager.GetHaptic();
        hapticPlayer.Submit("___ping____", PositionType.FootL, new List<DotPoint>(), 40);
    }
}
