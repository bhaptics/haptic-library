using System.Collections;
using System.Collections.Generic;
using Bhaptics.Tact;
using Bhaptics.Tact.Unity;
using UnityEngine;
using UnityEngine.UI;

public class BhapticsStatus : MonoBehaviour
{
    [SerializeField] private Text text;

    [SerializeField] private TactSource tactSource;

    void Update()
    {
        if (text != null)
        {
            var hapticPlayer = BhapticsManager.HapticPlayer;
            text.text = "Tactal isActive: " + hapticPlayer.IsActive(PositionType.Head) + "\n" +
                        "Tactot isActive: " + hapticPlayer.IsActive(PositionType.Vest) + "\n" +
                        "Tactosy(L) isActive: " + hapticPlayer.IsActive(PositionType.ForearmL) + "\n" +
                        "Tactosy(R) isActive: " + hapticPlayer.IsActive(PositionType.ForearmR) + "\n" +
                        "Headshot isPlaying: " + tactSource.IsPlaying() + "\n" ;
        }

    }
}
