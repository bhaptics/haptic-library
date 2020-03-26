using System.IO;
using UnityEditor;
using UnityEngine;


namespace Bhaptics.Tact.Unity
{
    public class TactFileWatcher : Editor
    {
        public static bool triggerRecompileForTactSource = false;
        private static bool triggerRecompile = false;
        private static FileSystemWatcher sWatcher;



        [InitializeOnLoadMethod]
        static void UpdateWatchers()
        {
            RegisterWatcher(".tact");
            EditorApplication.update += OnEditorApplicationUpdate;
        }

        private static void RegisterWatcher(string extenstion)
        {
#if UNITY_2017_1_OR_NEWER
            //if (PlayerSettings.scriptingRuntimeVersion != ScriptingRuntimeVersion.Latest)
            //{
            //    Debug.LogError("FileSystemWatcher required .Net 4.6 or higher.");
            //    return;
            //}

            sWatcher = new FileSystemWatcher(Application.dataPath, "*" + extenstion);

            sWatcher.NotifyFilter = NotifyFilters.CreationTime |
                NotifyFilters.Attributes |
                NotifyFilters.DirectoryName |
                NotifyFilters.FileName |
                NotifyFilters.LastAccess |
                NotifyFilters.LastWrite |
                NotifyFilters.Security |
                NotifyFilters.Size;

            sWatcher.IncludeSubdirectories = true;

            sWatcher.Changed += OnChanged;
            sWatcher.Created += OnCreated;
            sWatcher.Deleted += OnDeleted;
            sWatcher.Renamed += OnRenamed;

            sWatcher.EnableRaisingEvents = true;
#else
        Debug.LogError("FileSystemWatcher required .Net 4.6 or higher");
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

        private static void OnEditorApplicationUpdate()
        {
            if (triggerRecompile)
            {
                TactFileAsset.Initialized = false;
                var temp = TactFileAsset.Instance.FeedbackFiles;
                triggerRecompileForTactSource = true;

                //TactClipManager.Changed = true;
                //TactClipManager.Refresh();

                triggerRecompile = false;
            }
        }
    }
}