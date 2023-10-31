using System.Runtime.InteropServices;

namespace Bhaptics.Tact.Unity
{
    public class HapticApi
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct point
        {
            public float x, y;
            public int intensity, motorCount;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct status
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public int[] values;
        };

        [DllImport("haptic_library")]
        [return:MarshalAs(UnmanagedType.I1)]
        public static extern bool TryGetExePath(byte[] buf, ref int size);

        [DllImport("haptic_library")]
        public static extern void Initialise(string appId, string appName);

        [DllImport("haptic_library")]
        public static extern void Destroy();
        
        [DllImport("haptic_library", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RegisterFeedback(string str, string projectJson);

        [DllImport("haptic_library", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RegisterFeedbackFromTactFile(string str, string tactFileStr);

        [DllImport("haptic_library", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RegisterFeedbackFromTactFileReflected(string str, string tactFileStr);

        [DllImport("haptic_library", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SubmitRegistered(string key);
        
        // Works with bHaptics Player 1.5.6 onwards
        [DllImport("haptic_library", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SubmitRegisteredStartMillis(string key, int startTimeMillis);

        [DllImport("haptic_library", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SubmitRegisteredWithOption(string key, string altKey, float intensity, float duration, float offsetX, float offsetY);

        [DllImport("haptic_library", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SubmitByteArray(string key, PositionType pos, byte[] charPtr, int length, int durationMillis);
        
        [DllImport("haptic_library", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SubmitPathArray(string key, PositionType pos, point[] charPtr, int length, int durationMillis);
        
        [DllImport("haptic_library", CallingConvention = CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.I1)]
        public static extern bool IsFeedbackRegistered(string key);
        
        [DllImport("haptic_library", CallingConvention = CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.I1)]
        public static extern bool IsPlaying();

        [DllImport("haptic_library", CallingConvention = CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.I1)]
        public static extern bool IsPlayingKey(string key);

        [DllImport("haptic_library", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TurnOff();
        
        [DllImport("haptic_library", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TurnOffKey(string key);

        [DllImport("haptic_library", CallingConvention = CallingConvention.Cdecl)]
        public static extern void EnableFeedback();

        [DllImport("haptic_library", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DisableFeedback();

        [DllImport("haptic_library", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ToggleFeedback();
        
        [DllImport("haptic_library", CallingConvention = CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.I1)]
        public static extern bool IsDevicePlaying(PositionType pos);

        [DllImport("haptic_library", CallingConvention = CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.I1)]
        public static extern bool TryGetResponseForPosition(PositionType pos, out status status);
    }
}
