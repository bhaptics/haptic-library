using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Bhaptics.Tact.Unity
{
    public class BhapticsUtils
    {
        private static bool isInit = false;
        private static string exeFilePath = null;

        private static T[] SubArray<T>(T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static string GetExePath()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            
            if (isInit)
            {
                return exeFilePath;
            }
            isInit = true;
            try
            {
                byte[] buf = new byte[500];
                int size = 0;


                if (HapticApi.TryGetExePath(buf, ref size))
                {
                    byte[] cleanedArray = SubArray(buf, 0, size);

                    exeFilePath = System.Text.Encoding.ASCII.GetString(cleanedArray).Trim();
                    return exeFilePath;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            exeFilePath = "";

            return exeFilePath;
            
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
            try
            {
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
            }
            catch (Exception e)
            {
                Debug.Log("IsPlayerRunning() " + e.Message);
            }

            return true;


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
            try
            {
                var myProcess = new System.Diagnostics.Process();
                myProcess.StartInfo.FileName = GetExePath();
                myProcess.Start();
            }
            catch (Exception e)
            {
                Debug.Log("LaunchPlayer() " + e.Message);
            }
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
            
            List<string> keys = new List<string>();

            bool isFinished = false;

            foreach (var projectTrack in project.Tracks)
            {
                foreach (var projectTrackEffect in projectTrack.Effects)
                {
                    if (isFinished)
                    {
                        continue;
                    }

                    foreach (var modesKey in projectTrackEffect.Modes.Keys)
                    {
                        keys.Add(modesKey);
                        
                    }

                    isFinished = true;
                    break;
                }
            }

            if (keys.Count != 2)
            {
                return feedbackFile.Project;
            }

            string rightType = keys[0];
            string reftType = keys[1];
            foreach (var projectTrack in project.Tracks)
            {
                foreach (var projectTrackEffect in projectTrack.Effects)
                {
                    HapticEffectMode right = null, left = null;
                    if (projectTrackEffect.Modes.ContainsKey(rightType))
                    {
                        right = projectTrackEffect.Modes[rightType];
                    }

                    if (projectTrackEffect.Modes.ContainsKey(reftType))
                    {
                        left = projectTrackEffect.Modes[reftType];
                    }

                    projectTrackEffect.Modes[reftType] = right;
                    projectTrackEffect.Modes[rightType] = left;
                }
            }

            return project;
        }

        public static string TypeVest = "Vest";
        public static string TypeTactot = "Tactot";
        public static string TypeTactosy = "Tactosy";
        public static string TypeTactosy2 = "Tactosy2";
    }
}

