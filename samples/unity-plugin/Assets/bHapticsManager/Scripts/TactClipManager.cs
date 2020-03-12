using System;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Bhaptics.Tact.Unity
{
    public class TactClipManager : ScriptableObject
    {








        public static void RefreshTactFiles()
        {
            string extension = "*.tact";
            string[] tactFiles = Directory.GetFiles("Assets/", extension, SearchOption.AllDirectories);
            for (int i = 0; i < tactFiles.Length; ++i)
            {
                var fileName = Path.GetFileNameWithoutExtension(tactFiles[i]);
                EditorUtility.DisplayProgressBar("Hold On", "Converting " + fileName + ".tact -> "
                                                + fileName + ".asset (" + i + " / " + tactFiles.Length + ")", i / (float)tactFiles.Length);
                CreateTactClip(tactFiles[i]);
            }
            EditorUtility.ClearProgressBar();
        }








        private static void CreateTactClip(string tactFilePath)
        {
#if UNITY_EDITOR
            try
            {
                var fileName = Path.GetFileNameWithoutExtension(tactFilePath);
                if (fileName == null)
                {
                    Debug.LogError("File name is null. Path: " + tactFilePath);
                    return;
                }
                string json = LoadJsonStringFromFile(tactFilePath);
                var file = CommonUtils.ConvertJsonStringToTactosyFile(json);
                var fileHash = GetHash(tactFilePath);
                var clipPath = tactFilePath.Replace(".tact", ".asset");

                if (File.Exists(clipPath))
                {
                    clipPath = GetUsableFileName(clipPath);
                    if (clipPath == null)
                    {
                        Debug.LogError("File duplicated. Path: " + tactFilePath);
                        return;
                    }
                }
                clipPath = ConvertToAssetPathFromAbsolutePath(clipPath);
                var tactClip = CreateInstance<TactClip>();
                tactClip.JsonValue = json;
                tactClip.Name = fileName;
                tactClip.ClipType = file.Project.Layout.Type;
                tactClip.Identifier = fileHash;
                tactClip.Path = clipPath;

                File.Delete(tactFilePath);
                AssetDatabase.CreateAsset(tactClip, clipPath);
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to read tact file. Path: " + tactFilePath + "\n" + e.Message);
            }
#endif
        }

        private static string GetUsableFileName(string path)
        {
            if (!File.Exists(path))
            {
                return path;
            }
            var name = Path.GetFileNameWithoutExtension(path);
            var extension = Path.GetExtension(path);
            if (name == null || extension == null)
            {
                return null;
            }
            for (int i = 1; i < 1000; ++i)
            {
                var res = path.Replace(name + extension, name + " " + i + extension);
                if (!File.Exists(res))
                {
                    return res;
                }
            }
            return null;
        }

        private static string ConvertToAssetPathFromAbsolutePath(string absolutePath)
        {
            absolutePath = absolutePath.Replace("\\", "/");
            if (absolutePath.StartsWith(Application.dataPath))
            {
                return "Assets" + absolutePath.Substring(Application.dataPath.Length);
            }
            else if (absolutePath.StartsWith("Assets/"))
            {
                return absolutePath;
            }
            else
            {
                Debug.LogError("Path is not absolutePath");
                return null;
            }
        }

        private static string LoadJsonStringFromFile(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return json;
        }

        private static string GetSavePath()
        {
            var scriptPaths = Directory.GetFiles(Application.dataPath, "TactClipManager.cs", SearchOption.AllDirectories);
            var savePath = scriptPaths[0];
            savePath = "Assets" + savePath.Substring(Application.dataPath.Length);
            savePath = savePath.Replace(Path.GetFileName(savePath), "");
            return savePath.Replace(@"\Scripts\", @"\Resources\");
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








#if UNITY_EDITOR
        [MenuItem("Assets/Refresh Tact Files")]
        private static void OnClickRefreshTactFiles()
        {
            RefreshTactFiles();
            TactFileAsset.Initialized = false;
            var temp = TactFileAsset.Instance;
        }

        //[MenuItem("Assets/Refresh Tact Files", true)]
        //private static bool CanRefresh()
        //{
        //    string _path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
        //    bool _isTactFile = _path.ToLower().EndsWith(".tact");
        //    bool _isTactClip = Selection.activeObject is TactClip;
        //    bool _isTactClipManager = Selection.activeObject is TactClipManager;
        //    bool _isTactFileAsset = Selection.activeObject is TactFileAsset;
        //    return _isTactFile || _isTactClip || _isTactClipManager || _isTactFileAsset;
        //}
#endif
    }
}
