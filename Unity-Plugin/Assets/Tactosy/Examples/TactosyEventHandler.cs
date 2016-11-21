using System.Collections.Generic;
using TactosyCommon.Models;
using UnityEngine;
using TactosyCommon.Unity;

public class TactosyEventHandler : MonoBehaviour
{
    public float Duration = 1f;
    public float Intensity = 1f;

    public TactosyManager TactosyManager;

    public void OnButtonClick(string operation)
    {
        switch (operation)
        {
            case "turnoff":
                TactosyManager.TurnOff();
                
                break;
            case "type1":
                TactosyManager.SendSignal("RifleShoot", Intensity, Duration);
                break;
            case "type2":
                if (!TactosyManager.IsPlaying("reload"))
                {
                    TactosyManager.SendSignal("reload", Intensity, Duration);
                }

                break;
            case "type3":
                byte[] bytes = new byte[20];
                bytes[0] = 100;
                bytes[1] = 100;
                bytes[2] = 100;
                TactosyManager.SendSignal("dot", PositionType.Right, bytes, 500);
                break;
            case "type4":
                TactosyManager.SendSignal("Electricgun");
                break;
            case "type5":
                TactosyManager.SendSignal("Shotgun", Intensity, Duration);
                break;
            case "undefined":
                TactosyManager.SendSignal("undefined", Intensity, Duration);
                TactosyManager.TurnOff("undefined");
                break;
            case "path":
                TactosyManager.SendSignal("PathFeedback", PositionType.All, new List<PathPoint>
                {
                    new PathPoint(0.5, 0.2, 0.5),
                    new PathPoint(1, 1, 1)
                }, 2000);
                break;
            default:
                break;
        }
    }


}
