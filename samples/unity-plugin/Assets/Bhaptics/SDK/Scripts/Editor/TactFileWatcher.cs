using System.IO;
using UnityEditor;
using UnityEngine;


namespace Bhaptics.Tact.Unity
{
    public class TactFileWatcher : Editor
    {
        public static bool triggerRecompileForTactSource = false;
        private static bool triggerRecompile = false;
        private static bool triggerCheck = false;
        private static FileSystemWatcher tactFileWatcher;
        private static FileSystemWatcher tactClipWatcher;


        [InitializeOnLoadMethod]
        static void UpdateWatchers()
        {
            BhapticsLogger.LogDebug("UpdateWatchers()");
            RegisterWatcher(".tact");
            RegisterTactClipDeleteWatcher();
            EditorApplication.update += OnEditorApplicationUpdate;

            triggerRecompile = CheckChanged();
        }

        private static void RegisterWatcher(string extenstion)
        {
#if UNITY_2017_1_OR_NEWER
            tactFileWatcher = new FileSystemWatcher(Application.dataPath, "*" + extenstion);
            tactFileWatcher.NotifyFilter = NotifyFilters.CreationTime |
                NotifyFilters.Attributes |
                NotifyFilters.DirectoryName |
                NotifyFilters.FileName |
                NotifyFilters.LastAccess |
                NotifyFilters.LastWrite |
                NotifyFilters.Security |
                NotifyFilters.Size;

            tactFileWatcher.IncludeSubdirectories = true;

            //sWatcher.Changed += OnChanged;
            tactFileWatcher.Created += OnCreated;
            //sWatcher.Deleted += OnDeleted;
            //sWatcher.Renamed += OnRenamed;

            tactFileWatcher.EnableRaisingEvents = true;
#else
            BhapticsLogger.LogError("FileSystemWatcher required .Net 4.6 or higher");
#endif
        }

        private static void RegisterTactClipDeleteWatcher()
        {
#if UNITY_2017_1_OR_NEWER
            tactClipWatcher = new FileSystemWatcher(Application.dataPath, "*.asset");
            tactClipWatcher.NotifyFilter = NotifyFilters.CreationTime |
                NotifyFilters.Attributes |
                NotifyFilters.DirectoryName |
                NotifyFilters.FileName |
                NotifyFilters.LastAccess |
                NotifyFilters.LastWrite |
                NotifyFilters.Security |
                NotifyFilters.Size;

            tactClipWatcher.IncludeSubdirectories = true;

            tactClipWatcher.Deleted += OnTactClipDeleted;

            tactClipWatcher.EnableRaisingEvents = true;
#else
            BhapticsLogger.LogError("FileSystemWatcher required .Net 4.6 or higher");
#endif
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            triggerRecompile = true;
        }

        private static void OnCreated(object source, FileSystemEventArgs e)
        {
            triggerRecompile = true;
        }

        private static void OnDeleted(object source, FileSystemEventArgs e)
        {
            triggerRecompile = true;
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            triggerRecompile = true;
        }

        private static void OnTactClipDeleted(object source, FileSystemEventArgs e)
        {
            triggerCheck = true;
        }



        private static bool CheckChanged()
        {
            // TODO
            return Directory.GetFiles("Assets/", "*.tact", SearchOption.AllDirectories).Length != 0;
            // || BhapticsUtils.FindAssetsByType<TactClip>().Count != TactFileAsset.Instance.FeedbackFiles.Length;
        }

        private static void OnEditorApplicationUpdate()
        {
            if (triggerCheck)
            {
                triggerRecompile = CheckChanged();
                triggerCheck = false;
            }
            if (triggerRecompile)
            {
                HapticClipManager.RefreshTactFiles();
                triggerRecompile = false;

                triggerRecompileForTactSource = true;
            }
        }
    }
}