using System;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;


namespace Bhaptics.Tact.Unity
{
    [ExecuteInEditMode]
    public class TactClipManager : ScriptableObject
    {

        private static TactClipType GetMappedDeviceType(string clipType)
        {
            if (clipType == "")
            {
                return TactClipType.None;
            }
            switch (clipType)
            {
                case BhapticsUtils.TypeHead:
                case BhapticsUtils.TypeTactal:
                    return TactClipType.Tactal;

                case BhapticsUtils.TypeVest:
                case BhapticsUtils.TypeTactot:
                    return TactClipType.Tactot;

                case BhapticsUtils.TypeTactosy:
                case BhapticsUtils.TypeTactosy2:
                    return TactClipType.Tactosy_arms;

                case BhapticsUtils.TypeHand:
                    return TactClipType.Tactosy_hands;

                case BhapticsUtils.TypeFoot:
                    return TactClipType.Tactosy_feet;

                default:
                    return TactClipType.None;
            }
        }


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
            try
            {
                var fileName = Path.GetFileNameWithoutExtension(tactFilePath);
                if (fileName == null)
                {
                    BhapticsLogger.LogError("File name is null. Path: ");
                    return;
                }
                string json = LoadJsonStringFromFile(tactFilePath);
                var file = CommonUtils.ConvertJsonStringToTactosyFile(json);
                // var fileHash = GetHash(tactFilePath);
                var clipPath = tactFilePath.Replace(".tact", ".asset");

                if (File.Exists(clipPath))
                {
                    clipPath = GetUsableFileName(clipPath);
                    if (clipPath == null)
                    {
                        BhapticsLogger.LogError("File duplicated. Path: " + tactFilePath);
                        return;
                    }
                }
                clipPath = ConvertToAssetPathFromAbsolutePath(clipPath);

                TactClipType type = GetMappedDeviceType(file.Project.Layout.Type);

                TactFileClip tactClip;
                if (type == TactClipType.Tactot)
                {
                    tactClip = CreateInstance<TactotTactClip>();
                } else if (type == TactClipType.Tactal)
                {
                    tactClip = CreateInstance<TactalTactClip>();
                } else if (type == TactClipType.Tactosy_arms)
                {
                    tactClip = CreateInstance<TactosyTactClip>();
                } else if (type == TactClipType.Tactosy_hands)
                {
                    tactClip = CreateInstance<HandTactClip>();
                }else if (type == TactClipType.Tactosy_feet)
                {
                    tactClip = CreateInstance<FootTactClip>();
                }
                else
                {
                    tactClip = CreateInstance<TactFileClip>();
                }


                tactClip.JsonValue = json;
                tactClip.Name = fileName;


                tactClip.ClipType = type;

                File.Delete(tactFilePath);
                AssetDatabase.CreateAsset(tactClip, clipPath);
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
            }
            catch (Exception e)
            {
                BhapticsLogger.LogError("Failed to read tact file. Path: " + tactFilePath + "\n" + e.Message);
            }
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
                BhapticsLogger.LogError("Path is not absolutePath");
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


        [MenuItem("Bhapitcs/Refresh TactClip Asset Files")]
        private static void OnClickRefreshAssetFiles()
        {
            var allInstances = GetAllInstances<TactFileClip>();
            foreach (var allInstance in allInstances)
            {

            }
        }

        // [MenuItem("Bhapitcs/Refresh Tact Files")]
        private static void OnClickRefreshTactFiles()
        {
            RefreshTactFiles();
        }


        public static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);  //FindAssets uses tags check documentation for more info
            T[] a = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++)         //probably could get optimized 
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }

            return a;

        }
    }
}
