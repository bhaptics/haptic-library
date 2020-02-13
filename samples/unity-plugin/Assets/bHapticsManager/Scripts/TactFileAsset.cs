using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Bhaptics.Tact.Unity
{
    public class TactFileAsset : ScriptableObject
    {
        public static bool Initialized = false;
        private static TactFileAsset _instance;

        public FeedbackFile[] FeedbackFiles;
        public string[] Types;

        public Dictionary<string, FeedbackFile> FilesMap;

        public static TactFileAsset Instance
        {
            get
            {
                Initialize();
                return _instance;
            }
        }

        private static void Initialize()
        {
            if (Initialized)
            {
                return;
            }

            try
            {
                SaveAssetFile();
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Exception handling {0}", e.Message);
            }


            var @object = Resources.Load("TactFileAsset", typeof(TactFileAsset));

            if (@object != null)
            {
                _instance = @object as TactFileAsset;
            }
            else
            {
                _instance = CreateInstance<TactFileAsset>();
            }

            Initialized = true;
        }

        public void OnDestroy()
        {
            Initialized = false;
            _instance = (TactFileAsset)null;
        }

        public static void SaveAssetFile()
        {
#if UNITY_EDITOR
            MonoScript ms = MonoScript.FromScriptableObject(CreateInstance<TactFileAsset>());
            string scriptPath = AssetDatabase.GetAssetPath(ms);
            scriptPath = scriptPath.Replace(Path.GetFileName(scriptPath), "");
            scriptPath = scriptPath.Replace("/Scripts/", "/Resources/");

            var item = CreateInstance<TactFileAsset>();
            item.FeedbackFiles = LoadFeedbackFile();
            item.FilesMap = new Dictionary<string, FeedbackFile>();
            foreach (var itemFeedbackFile in item.FeedbackFiles)
            {
                item.FilesMap[itemFeedbackFile.Key] = itemFeedbackFile;
            }
            item.Types = LoadTypes(item.FeedbackFiles);
            // TODO
            string assetPath = scriptPath + "TactFileAsset" + ".asset";
            var existingAsset = AssetDatabase.LoadAssetAtPath<TactFileAsset>(assetPath);


            if (!AssetDatabase.IsValidFolder(scriptPath))
            {
                Directory.CreateDirectory(scriptPath);
            }

            if (existingAsset == null)
            {
                AssetDatabase.CreateAsset(item, assetPath);
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
            }
            else
            {

                existingAsset.FilesMap = new Dictionary<string, FeedbackFile>();
                foreach (var itemFeedbackFile in existingAsset.FeedbackFiles)
                {
                    existingAsset.FilesMap[itemFeedbackFile.Key] = itemFeedbackFile;
                }
                existingAsset.Types = LoadTypes(item.FeedbackFiles);

                if (!EqualsAsset(item, existingAsset))
                {
                    EditorUtility.CopySerialized(item, existingAsset);
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
                }
            }
#endif
        }

        private static bool EqualsAsset(TactFileAsset a, TactFileAsset b)
        {
            try
            {
                if (a == null || b == null)
                {
                    return false;
                }

                if (a.FeedbackFiles.Length != b.FeedbackFiles.Length)
                {
                    return false;
                }

                foreach (var keyValuePair in a.FilesMap)
                {
                    var key = keyValuePair.Key;
                    if (!b.FilesMap.ContainsKey(key))
                    {
                        return false;
                    }

                    if (keyValuePair.Value.Id != b.FilesMap[key].Id)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogFormat("EqualsAsset: {0}, {1}", e.Message, e.StackTrace);
                return false;
            }

        }

        private static string[] LoadTypes(FeedbackFile[] feedbackFiles)
        {
            var types = new Dictionary<string, string>();
            foreach (var file in feedbackFiles)
            {
                types[file.Type] = file.Type;
            }

            return types.Keys.ToArray();
        }

        private static FeedbackFile[] LoadFeedbackFile()
        {
            List<FeedbackFile> files = new List<FeedbackFile>();

            try
            {
                string[] extensions = { "*.tactosy", "*.tact" };

                foreach (var extension in extensions)
                {
                    string[] allPaths = Directory.GetFiles("Assets/", extension, SearchOption.AllDirectories);

                    foreach (var filePath in allPaths)
                    {
                        try
                        {
                            var fileName = Path.GetFileNameWithoutExtension(filePath);
                            string json = LoadStringFromFile(filePath);
                            var file = CommonUtils.ConvertJsonStringToTactosyFile(json);

                            if (fileName == null)
                            {
                                Debug.LogError("file name is null " + filePath);
                                continue;
                            }
                            FeedbackFile feedbackFile = new FeedbackFile();
                            feedbackFile.Value = json;
                            feedbackFile.Key = fileName;
                            feedbackFile.Type = file.Project.Layout.Type;

                            // File Path is unique
                            feedbackFile.Id = GetHash(filePath);
                            files.Add(feedbackFile);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("failed to read feedback file " + filePath + " : " + e.Message);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("FeedbackFile Loading failed : " + e.Message);
            }
            return files.ToArray();

        }

        private static string secretKey = "seed";
        private static string GetHash(string value)
        {
            var md5 = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(value + secretKey);
            byte[] hash = md5.ComputeHash(bytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }

        private static string LoadStringFromFile(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return json;
        }
    }

    [Serializable]
    public class FeedbackFile
    {
        public string Id;
        public string Key;
        public string Value;
        public string Type;
    }
}
