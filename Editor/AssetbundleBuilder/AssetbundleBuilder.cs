using Packages.MEMCoreUnity.Runtime.ContentSystem;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Packages.MEMCoreUnity.Editor.AssetbundleBuilder
{
    public class AssetbundleBuilder
    {
        public static void BuildAssetbundles(BuildTarget buildTarget, BuildTargetGroup buildGroup, string outputPath)
        {
            var assetbundleBuilds = new List<AssetBundleBuild>();

            var directories = Directory.GetDirectories(Application.dataPath + "/Data/");
            foreach (string directory in directories)
            {
                var assetbundleBuildsInDirectory = GetAssetbundleFiles(directory);
                assetbundleBuilds.AddRange(assetbundleBuildsInDirectory);
            }

            var assetbundleBuildOptions = BuildAssetBundleOptions.ForceRebuildAssetBundle; // | BuildAssetBundleOptions.AppendHashToAssetBundleName;

            BuildAssetbundles("ModuleMenu", outputPath, assetbundleBuilds.ToArray(), assetbundleBuildOptions, buildTarget);
        }

        private static List<AssetBundleBuild> GetAssetbundleFiles(string path)
        {
            var filePaths = Directory.GetFiles(path);
            var assetbundleBuilds = new List<AssetBundleBuild>();

            for (int i = 0; i < filePaths.Length; i++)
            {
                string filePath = filePaths[i];

                if (filePath.Contains(".meta"))
                    continue;

                var index = filePath.LastIndexOf("\\");
                if (filePath.LastIndexOf("/") > index)
                    index = filePath.LastIndexOf("/");

                var fileName = filePath.Substring(index + 1);
                var cleanedAssetName = fileName.Remove(fileName.LastIndexOf("."));

                index = filePath.IndexOf("Assets");
                var cleanedFilePath = filePath.Substring(index).Replace("\\", "/");

                var atlasAsset = AssetImporter.GetAtPath(cleanedFilePath);

                if (atlasAsset != null)
                {
                    atlasAsset.assetBundleName = cleanedAssetName.ToLower() + ".assetbundle";

                    var assetbundleBuild = new AssetBundleBuild();
                    assetbundleBuild.assetBundleName = cleanedAssetName.ToLower() + ".assetbundle";
                    assetbundleBuild.assetNames = new[] { atlasAsset.assetPath };

                    assetbundleBuilds.Add(assetbundleBuild);

                    Debug.Log("prepare AssetbundleBuild with asset:" + assetbundleBuild.assetBundleName + ". included asset: " + cleanedFilePath);
                }
                else
                    Debug.LogWarning("no asset found at " + cleanedFilePath);
            }

            return assetbundleBuilds;
        }

        public static void BuildAssetbundles(string moduleName, string outputPath, AssetBundleBuild[] assetbundleBuildArray, BuildAssetBundleOptions buildOptions, BuildTarget buildTarget)
        {
            //Debug.Log("building " + aAssetbundleCategory);
            var buildStepTime = DateTime.Now;

            uint manifestCRC = 0;

            Debug.Log("Start building assetbundle category: " + moduleName + " (for platform: " + buildTarget + " and options: " + buildOptions + ")");
            Debug.Log("Output path: " + outputPath);
            Debug.Log("Assetbundle count: " + assetbundleBuildArray.Length);

            //Debug.Log("output path: " + anAssetbundleOutputPath + " building " + aAssetbundleBuildArray.Length + " assetbundles for platform " + aBuildTarget + " with options " + aBuildOptions);

            if (!Directory.Exists(Application.dataPath + "/" + outputPath))
            {
                //Debug.Log("output path: " + Application.dataPath + "/" + aOutputPath + " not present. creating new.");
                Directory.CreateDirectory(Application.dataPath + "/" + outputPath);
            }

            var changedAssetbundleInfoList = new List<AssetbundleCreationInfo>();
            var manifest = BuildPipeline.BuildAssetBundles("Assets" + outputPath, assetbundleBuildArray, buildOptions, buildTarget);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            if (manifest != null)
            {
                //Debug.Log(aManifest.name + " includes: " + aManifest.GetAllAssetBundles().Length + " assetbundles.");
                Debug.Log(moduleName + " Manifest includes: " + manifest.GetAllAssetBundles().Length + " assetbundles.");

                uint crc;
                for (int i = 0; i < assetbundleBuildArray.Length; i++)
                {
                    var assetbundleBuild = assetbundleBuildArray[i];
                    var assetbundlePath = "Assets" + outputPath + assetbundleBuild.assetBundleName; //+ ".meta";

                    var hashCode = manifest.GetAssetBundleHash(assetbundleBuild.assetBundleName);
                    var hashCodeString = hashCode.ToString();

                    BuildPipeline.GetCRCForAssetBundle(assetbundlePath, out crc);
                    manifestCRC += crc;

                    var fileInfo = new FileInfo(assetbundlePath);
                    var fileLength = (int)fileInfo?.Length / 1024;

                    Debug.Log(assetbundleBuild.assetBundleName + " Hash: " + hashCodeString + " CRC: " + crc + " filesize: " + fileLength);

                    bool assetbundleHasChangedOrIsNew;
                    var aAssetbundleInfo = new AssetbundleInfo(hashCodeString, crc, fileLength);
                    if (assetbundleBuild.assetBundleName.Contains("assetbundle_material"))
                        assetbundleHasChangedOrIsNew = AssetBundleVersionizer.UpdateAssetbundleCategoryInfo("assetbundleMaterials", assetbundleBuild.assetBundleName, aAssetbundleInfo);
                    else if (assetbundleBuild.assetBundleName.Contains("assetbundle_mesh"))
                        assetbundleHasChangedOrIsNew = AssetBundleVersionizer.UpdateAssetbundleCategoryInfo("assetbundleMeshes", assetbundleBuild.assetBundleName, aAssetbundleInfo);
                    else
                        assetbundleHasChangedOrIsNew = AssetBundleVersionizer.UpdateAssetbundleCategoryInfo(moduleName, assetbundleBuild.assetBundleName, aAssetbundleInfo);

                    if (assetbundleHasChangedOrIsNew)
                    {
                        var aAssetbundleCreationInfo = new AssetbundleCreationInfo(assetbundleBuild.assetBundleName, aAssetbundleInfo);
                        changedAssetbundleInfoList.Add(aAssetbundleCreationInfo);
                    }
                }

                var aManifestCrcString = manifestCRC.ToString();
                BuildPipeline.GetCRCForAssetBundle(outputPath + moduleName, out crc);
                Debug.Log("Assetbundle Category Manifest: " + outputPath + moduleName + " Hash: " + aManifestCrcString + " CRC: " + crc);
                AssetBundleVersionizer.UpdateAssetbundleCategoryInfo("assetbundleManifests", moduleName, new AssetbundleInfo(aManifestCrcString, crc, 10));

                var aGuids = AssetDatabase.FindAssets(manifest.name, null);
                Debug.Log(manifest.name + " present in following GUIDs (" + aGuids.Length + "):"); // TODO: not sure about the purpose of this search! (PH)
                for (int i = 0; i < aGuids.Length; i++)
                {
                    var aGuid = aGuids[i];
                    Debug.Log(AssetDatabase.GUIDToAssetPath(aGuid));
                }
            }
            else
                Debug.LogError("no assetbundle manifest was created!");

            Debug.Log("Finished Assetbundle creation for category: " + moduleName + " (Time elapased: " + (DateTime.Now - buildStepTime).TotalMinutes + " min).");

            Debug.Log("Assetbundles that have changed for this category:\n\n");
            for (int i = 0; i < changedAssetbundleInfoList.Count; i++)
            {
                var aAssetbundleCreationInfo = changedAssetbundleInfoList[i];
                Debug.Log(aAssetbundleCreationInfo.ToString());
            }


            //Debug.Log("building " + aAssetbundleCategory + " took: " + (DateTime.Now - aBuildStepTime).TotalMinutes + " minutes.");
        }

        //public static void CreateAssetBundlesFromObject<T>(string aSearchPattern, string aLocalPathForAssetbundles, string aPathToLoadScriptableObjectsFrom, string aAssetbundleCategory, string aManifestName, BuildTarget aBuildTarget) where T : UnityEngine.Object
        //{
        //    var aAssetList = loadObjectsFromAssetPath<T>(aPathToLoadScriptableObjectsFrom, aSearchPattern);
        //    var aAssetBundleBuildList = createAssetBundleBuildList(aAssetList, aBuildTarget);

        //    if (aAssetBundleBuildList != null)
        //        BuildAssetbundles(aAssetbundleCategory, aLocalPathForAssetbundles, aAssetBundleBuildList.ToArray(), aManifestName, BuildAssetBundleOptions.None, aBuildTarget);
        //}

        //public static List<AssetBundleBuild> CreateAssetBundleBuildList<T>(List<T> aAssetList, BuildTarget aBuildTarget) where T : UnityEngine.Object
        //{
        //    Debug.Log("created Asset List:");
        //    if (aAssetList != null && aAssetList.Count > 0)
        //    {
        //        var aAssetBundleBuildList = new List<AssetBundleBuild>();
        //        var aAssetBundleNames = String.Empty;
        //        var aComma = "; ";

        //        foreach (T aAsset in aAssetList)
        //        {
        //            var aAssetBundleBuild = new AssetBundleBuild();
        //            aAssetBundleBuild.assetBundleName = aAsset.name + ".assetbundle";
        //            aAssetBundleBuild.assetNames = new[] { AssetDatabase.GetAssetPath(aAsset) };
        //            aAssetBundleBuildList.Add(aAssetBundleBuild);
        //            aAssetBundleNames += aAsset.name + aComma;
        //        }

        //        Debug.Log("AssetBundleBuild created with: " + aAssetBundleNames);

        //        return aAssetBundleBuildList;
        //    }
        //    return null;
        //}

        //public static List<T> LoadObjectsFromAssetPath<T>(string aPath, string aSearchPattern) where T : UnityEngine.Object
        //{
        //    if (Directory.Exists(aPath))
        //    {
        //        var aObjectPathList = Directory.GetFiles(aPath, aSearchPattern, SearchOption.AllDirectories);

        //        var aScriptableObjectList = new List<T>();

        //        if (aObjectPathList != null && aObjectPathList.Length > 0)
        //        {
        //            for (int i = 0; i < aObjectPathList.Length; i++)
        //            {
        //                var aAssetFilePath = aObjectPathList[i];
        //                var aStrippedAssetPath = aAssetFilePath.Replace(Application.dataPath, String.Empty);
        //                var aAsset = AssetDatabase.LoadAssetAtPath<T>(aStrippedAssetPath);
        //                if (aAsset != null)
        //                    aScriptableObjectList.Add(aAsset);
        //            }
        //            if (Debug.isDebugBuild)
        //                Debug.Log("Filtered " + aScriptableObjectList.Count + " Objects at: " + aPath);
        //        }
        //        else if (Debug.isDebugBuild)
        //            Debug.Log("Found no Objects at: " + aPath);

        //        return aScriptableObjectList;
        //    }

        //    Debug.LogError("Directory does not exists " + aPath);
        //    return null;
        //}
    }
}
