using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class BhapticsDevice
{
    public string DeviceName;
    public string Address;
    public int Battery;
    public bool IsPaired;
    public string ConnectionStatus;
    public string Position;
    public string Rssi;


    public const string TACTAL_POS = "Head";
    public const string TACTOT_POS = "Vest";
    public const string TACTOSY_LPOS = "ForearmL";
    public const string TACTOSY_RPOS = "ForearmR";
    public const string TACTOSY_LFPOS = "FootL";
    public const string TACTOSY_RFPOS = "FootR";
    public const string TACTOSY_LHPOS = "HandL";
    public const string TACTOSY_RHPOS = "HandR";


    public bool IsLeft()
    {
        if (Position.StartsWith(TACTOSY_LPOS) ||
            Position.StartsWith(TACTOSY_LFPOS)||
            Position.StartsWith(TACTOSY_LHPOS))
        {
            return true;
        } 
        return false;
    }
}