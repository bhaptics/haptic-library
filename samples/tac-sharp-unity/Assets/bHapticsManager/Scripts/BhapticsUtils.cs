using System;
using System.IO;
using Bhaptics.Tac.Designer;
using UnityEngine;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
using Microsoft.Win32;
#endif

namespace Bhaptics.Tac.Unity
{
    public class BhapticsUtils
    {
        private static bool isInit = false;
        private static string exeFilePath = null;

        public static string GetExePath()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            if (isInit)
            {
                return exeFilePath;
            }
            isInit = true;
            RegistryKey rkey = Registry.ClassesRoot.OpenSubKey(@"bhaptics-app\shell\open\command");
            if (rkey == null)
            {
                return null;
            }

            var val = rkey.GetValue("");

            if (val == null)
            {
                return null;
            }

            var path = val.ToString();
            exeFilePath = path;
            return path;
#else
            return "";
#endif
        }

        public static bool IsPlayerInstalled()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            var path = GetExePath();

            return path != null;
#else
            return true;
#endif
        }

        public static bool IsPlayerRunning()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            var fileName = Path.GetFileNameWithoutExtension(GetExePath());


            if (Is64BitBuild())
            {
                // 64 bit machine
                var processs = System.Diagnostics.Process.GetProcessesByName(fileName);
                if (processs.Length >= 1)
                {
                    return true;
                }
                return false;
            }
            else
            {
                // 32 bit machine does not support GetProcessByName.
                return true;
            }
#else
            return true;
#endif

        }

        private static bool Is64BitBuild()
        {
            return IntPtr.Size == 8;
        }

        public static void LaunchPlayer(bool tryLaunch)
        {
            if (!tryLaunch)
            {
                return;
            }

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            var myProcess = new System.Diagnostics.Process();
            myProcess.StartInfo.FileName = GetExePath();
            myProcess.Start();
#endif
        }


        public static float Angle(Vector3 fwd, Vector3 targetDir)
        {
            var fwd2d = new Vector3(fwd.x, 0, fwd.z);
            var targetDir2d = new Vector3(targetDir.x, 0, targetDir.z);

            float angle = Vector3.Angle(fwd2d, targetDir2d);

            if (AngleDir(fwd, targetDir, Vector3.up) == -1)
            {
                angle = 360.0f - angle;
                if (angle > 359.9999f)
                    angle -= 360.0f;
                return angle;
            }

            return angle;
        }

        private static int AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
        {
            Vector3 perp = Vector3.Cross(fwd, targetDir);
            float dir = Vector3.Dot(perp, up);

            if (dir > 0.0)
            {
                return 1;
            }

            if (dir < 0.0)
            {
                return -1;
            }

            return 0;
        }

        public static Project ReflectLeftRight(string projectStr)
        {
            var feedbackFile = CommonUtils.ConvertJsonStringToTactosyFile(projectStr);
            var project = feedbackFile.Project;

            foreach (var projectTrack in project.Tracks)
            {
                foreach (var projectTrackEffect in projectTrack.Effects)
                {
                    HapticEffectMode right = null, left = null;
                    if (projectTrackEffect.Modes.ContainsKey(TypeRight))
                    {
                        right = projectTrackEffect.Modes[TypeRight];
                    }

                    if (projectTrackEffect.Modes.ContainsKey(TypeLeft))
                    {
                        left = projectTrackEffect.Modes[TypeLeft];
                    }

                    projectTrackEffect.Modes[TypeLeft] = right;
                    projectTrackEffect.Modes[TypeRight] = left;
                }
            }

            return project;
        }

        public static string TypeVest = "Vest";
        public static string TypeTactosy = "Tactosy";
        public static string TypeRight = "Right";
        public static string TypeLeft = "Left";
    }
}

