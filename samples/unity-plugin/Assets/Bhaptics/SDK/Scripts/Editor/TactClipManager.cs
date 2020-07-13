﻿using System;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine.WSA;
using Application = UnityEngine.Application;


namespace Bhaptics.Tact.Unity
{
    [ExecuteInEditMode]
    public class TactClipManager : ScriptableObject
    {

        private static HapticClipType GetMappedDeviceType(string clipType)
        {
            if (clipType == "")
            {
                return HapticClipType.None;
            }
            switch (clipType)
            {
                case BhapticsUtils.TypeHead:
                case BhapticsUtils.TypeTactal:
                    return HapticClipType.Tactal;

                case BhapticsUtils.TypeVest:
                case BhapticsUtils.TypeTactot:
                    return HapticClipType.Tactot;

                case BhapticsUtils.TypeTactosy:
                case BhapticsUtils.TypeTactosy2:
                    return HapticClipType.Tactosy_arms;

                case BhapticsUtils.TypeHand:
                    return HapticClipType.Tactosy_hands;

                case BhapticsUtils.TypeFoot:
                    return HapticClipType.Tactosy_feet;

                default:
                    return HapticClipType.None;
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

                HapticClipType type = GetMappedDeviceType(file.Project.Layout.Type);

                FileHapticClip tactClip;
                if (type == HapticClipType.Tactot)
                {
                    tactClip = CreateInstance<VestHapticClip>();
                } else if (type == HapticClipType.Tactal)
                {
                    tactClip = CreateInstance<HeadHapticClip>();
                } else if (type == HapticClipType.Tactosy_arms)
                {
                    tactClip = CreateInstance<ArmsHapticClip>();
                } else if (type == HapticClipType.Tactosy_hands)
                {
                    tactClip = CreateInstance<HandsHapticClip>();
                }else if (type == HapticClipType.Tactosy_feet)
                {
                    tactClip = CreateInstance<FeetHapticClip>();
                }
                else
                {
                    tactClip = CreateInstance<FileHapticClip>();
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

        [MenuItem("Bhaptics/tact files ->  HapticClips")]
        private static void OnClickRefreshAssetFiles()
        {
            RefreshTactFiles();
        }

        [MenuItem("Bhaptics/HapticClips -> tact files")]
        private static void OnClickRefreshTactFiles()
        {
            var saveAsPath = EditorUtility.SaveFolderPanel("Save as *.tact File", @"\download\", "");

            if (!string.IsNullOrEmpty(saveAsPath))
            {
                var allInstances = GetAllInstances<FileHapticClip>();


                foreach (var allInstance in allInstances)
                {
                    var path = saveAsPath + @"\" + allInstance.ClipType;
                    if (!Directory.Exists(saveAsPath + @"\" + allInstance.ClipType))
                    {
                        //if it doesn't, create it
                        Directory.CreateDirectory(path);

                    }


                    File.WriteAllText(path + @"\" + allInstance.name + ".tact", allInstance.JsonValue);
                }

                BhapticsLogger.LogInfo("tact files saved to {0}", saveAsPath);
            }
            else
            {
                BhapticsLogger.LogError("Folder not selected.");
            }

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
