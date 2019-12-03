namespace Bhaptics.Tact.Unity
{
    public enum TactDeviceType
    {
        Tactal,
        Tactot,
        TactosyLeft,
        TactosyRight,
        TactosyLeftHand,
        TactosyRightHand,
        TactosyLeftFoot,
        TactosyRightFoot
    } 

    public class AndroidWidget_CompareDeviceString
    { 
        public static string GetDeviceNameString(TactDeviceType deviceType)
        {
            string pairDevice = string.Empty;

            switch (deviceType)
            {
                case TactDeviceType.Tactal:
                    pairDevice = "Tactal";
                    break;
                case TactDeviceType.TactosyLeft:
                case TactDeviceType.TactosyRight:
                    pairDevice = "Tactosy";
                    break;
                case TactDeviceType.TactosyLeftFoot:
                case TactDeviceType.TactosyRightFoot:
                    pairDevice = "TactosyF";
                    break;
                case TactDeviceType.TactosyLeftHand:
                case TactDeviceType.TactosyRightHand:
                    pairDevice = "TactosyH"; 
                    break;
                case TactDeviceType.Tactot:
                    pairDevice = "Tactot";
                    break;
            }

            return pairDevice;
        }

        public static string GetPositionString(TactDeviceType deviceType)
        {
            string position = string.Empty;
            switch (deviceType)
            {
                case TactDeviceType.Tactal:
                    position = BhapticsDevice.TACTAL_POS;
                    break;
                case TactDeviceType.TactosyLeft:
                    position = BhapticsDevice.TACTOSY_LPOS;
                    break;
                case TactDeviceType.TactosyRight:
                    position = BhapticsDevice.TACTOSY_RPOS;
                    break;
                case TactDeviceType.TactosyLeftFoot:
                    position = BhapticsDevice.TACTOSY_LFPOS;
                    break;
                case TactDeviceType.TactosyRightFoot:
                    position = BhapticsDevice.TACTOSY_RFPOS;
                    break;
                case TactDeviceType.TactosyLeftHand:
                    position = BhapticsDevice.TACTOSY_LHPOS;
                    break;
                case TactDeviceType.TactosyRightHand:
                    position = BhapticsDevice.TACTOSY_RHPOS;
                    break;
                case TactDeviceType.Tactot:
                    position = BhapticsDevice.TACTOT_POS;
                    break;
            }

            return position;
        }


        public static int convertConnectionStatus(string status)
        {
            if (status == "Connected")
            {
                return 0;
            }
            else if (status == "Connecting")
            {
                return 1;
            }
            else if (status == "Disconnected")
            {
                return 2;
            }
            return 3;
        }
    }
}
