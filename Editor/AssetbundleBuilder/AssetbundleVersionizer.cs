using Packages.MEMCoreUnity.Runtime.ContentSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Packages.MEMCoreUnity.Editor.AssetbundleBuilder
{
    public class AssetBundleVersionizer
    {
        public static int version;
        public static Dictionary<string, Dictionary<string, AssetbundleInfo>> assetBundleCategoryDictionary = new Dictionary<string, Dictionary<string, AssetbundleInfo>>();

        private static bool versioningAlreadyChanged = false;

        //public static void InitLocalVersions(LocalAssetbundles aLocalAssetbundles)
        //{
        //    Dictionary<string, Dictionary<string, AssetbundleInfo>> categoryDictionary = assetBundleCategoryDictionary;

        //    foreach (var aLocalAssetbundlesForCategoryDictionary in aLocalAssetbundles.categoryDictionary)
        //    {
        //        if (!categoryDictionary.ContainsKey(aLocalAssetbundlesForCategoryDictionary.Key))
        //        {
        //            categoryDictionary.Add(aLocalAssetbundlesForCategoryDictionary.Key, new Dictionary<string, AssetbundleInfo>());
        //        }
        //        updateCategory(aLocalAssetbundlesForCategoryDictionary.Value, categoryDictionary[aLocalAssetbundlesForCategoryDictionary.Key]);
        //    }
        //}

        private static void UpdateCategory(Dictionary<string, AssetbundleInfo> localAssetbundlesForCategoryDictionary, Dictionary<string, AssetbundleInfo> categoryItemsDictionary)
        {
            foreach (var categoryItem in localAssetbundlesForCategoryDictionary)
            {
                var categoryKey = categoryItem.Key;
                var originAssetbundleInfo = categoryItem.Value;

                if (!categoryItemsDictionary.ContainsKey(categoryKey))
                {
                    categoryItemsDictionary.Add(categoryKey, new AssetbundleInfo(originAssetbundleInfo.HashCodeString, originAssetbundleInfo.CrcValue, originAssetbundleInfo.FileLength));
                }
                else
                {
                    var assetbundleInfo = new AssetbundleInfo(categoryItemsDictionary[categoryKey]);
                    categoryItemsDictionary[categoryKey] = assetbundleInfo;
                }
            }
        }

        public static void InitAssetbundleVersions(Dictionary<string, object> versionDictionary)
        {
            versioningAlreadyChanged = false;

            foreach (var versionDictionaryEntry in versionDictionary)
            {
                if (versionDictionaryEntry.Key == "VersioningList")
                {
                    if (versionDictionaryEntry.Value is string)
                    {
                        int.TryParse(versionDictionaryEntry.Value as string, out version);
                    }
                    else if (versionDictionaryEntry.Value is int)
                    {
                        version = (int)versionDictionaryEntry.Value;
                    }
                    else
                    {
                        version = 0;
                        Debug.LogError("VersioningList version could not be readed (type is not string and not int)");
                    }
                }
                if (versionDictionaryEntry.Value is Dictionary<string, object>)
                {
                    if (!assetBundleCategoryDictionary.ContainsKey(versionDictionaryEntry.Key))
                    {
                        assetBundleCategoryDictionary.Add(versionDictionaryEntry.Key, new Dictionary<string, AssetbundleInfo>());
                    }
                    UpdateCategory(versionDictionaryEntry.Value as Dictionary<string, object>, assetBundleCategoryDictionary[versionDictionaryEntry.Key]);
                }
            }
        }

        private static void UpdateCategory(Dictionary<string, object> categoryObjectsDictionary, Dictionary<string, AssetbundleInfo> categoryItemsDictionary)
        {
            foreach (var categoryItem in categoryObjectsDictionary)
            {
                if (!categoryItemsDictionary.ContainsKey(categoryItem.Key))
                {
                    categoryItemsDictionary.Add(categoryItem.Key, new AssetbundleInfo(string.Empty, 0, 0));
                }
                else
                {
                    var cachedAssetbundleInfo = new AssetbundleInfo(categoryItemsDictionary[categoryItem.Key]);
                    categoryItemsDictionary[categoryItem.Key] = cachedAssetbundleInfo;
                }
            }
        }

        public static AssetbundleInfo GetAssetbundleInfo(string assetbundleKey, string category)
        {
            if (!string.IsNullOrEmpty(assetbundleKey))
            {
                var lowerCaseKey = assetbundleKey.ToLower();

                if (assetBundleCategoryDictionary.TryGetValue(category, out var categoryDictionary))
                {
                    AssetbundleInfo assetbundleInfo;
                    if (categoryDictionary.TryGetValue(lowerCaseKey, out assetbundleInfo))
                    {
                        return assetbundleInfo;
                    }
                    if (categoryDictionary.TryGetValue(lowerCaseKey + ".assetbundle", out assetbundleInfo))
                    {
                        return assetbundleInfo;
                    }
                }
                else
                {
                    Debug.LogError("Assetbundle: " + lowerCaseKey + " not found for category: " + category + " in VersioningList.plist!");
                }
            }
            else
            {
                Debug.LogError("Try to parse assetbundleInfo --> assetbundleKey is NULL for category: " + category);
            }
            return new AssetbundleInfo(string.Empty, 0, 0);
        }

        public static bool ContainsKey(string assetbundleKey, string category)
        {
            var lowerCaseKey = assetbundleKey.ToLower();
            if (assetBundleCategoryDictionary.TryGetValue(category, out var categoryDictionary))
            {
                if (categoryDictionary.ContainsKey(lowerCaseKey))
                    return true;
                if (categoryDictionary.ContainsKey(lowerCaseKey + ".assetbundle"))
                    return true;
            }
            return false;
        }

        public static bool UpdateAssetbundleCategoryInfo(string categoryIdentifier, string assetbundleIdentifier, AssetbundleInfo bundleInfo)
        {
            var assetbundleHashIsNewOrHasChanged = false;

            if (assetBundleCategoryDictionary.TryGetValue(categoryIdentifier, out var categoryDictionary))
            {
                if (categoryDictionary.ContainsKey(assetbundleIdentifier))
                {
                    var currentCategoryDictionary = categoryDictionary[assetbundleIdentifier];
                    var currentHashCode = currentCategoryDictionary.HashCodeString;
                    var newHashCode = bundleInfo.HashCodeString;
                    if (!currentHashCode.Equals(newHashCode))
                    {
                        if (!versioningAlreadyChanged)
                        {
                            versioningAlreadyChanged = true;
                            version += 1;
                        }

                        Debug.Log("HashCode has changed for: " + assetbundleIdentifier + " previous hash: " + currentCategoryDictionary.HashCodeString + " new hash: " + bundleInfo.HashCodeString + " current versioningList-Version: " + version);
                        assetbundleHashIsNewOrHasChanged = true;
                    }
                    categoryDictionary[assetbundleIdentifier] = bundleInfo;
                }
                else
                {
                    assetbundleHashIsNewOrHasChanged = true;
                    categoryDictionary.Add(assetbundleIdentifier, bundleInfo);
                }
            }
            else
            {
                assetbundleHashIsNewOrHasChanged = true;
                assetBundleCategoryDictionary.Add(categoryIdentifier, new Dictionary<string, AssetbundleInfo>());
                assetBundleCategoryDictionary[categoryIdentifier].Add(assetbundleIdentifier, bundleInfo);
            }
            return assetbundleHashIsNewOrHasChanged;
        }

        public static List<string> GetAllAssetbundleIdentifiers(string category)
        {
            var assetbundleIdentifiers = new List<string>();
            if (assetBundleCategoryDictionary.TryGetValue(category, out var categoryDictionary))
            {
                foreach (var anAssetbundleInfo in categoryDictionary)
                {
                    if (!string.IsNullOrEmpty(anAssetbundleInfo.Key) && !assetbundleIdentifiers.Contains(anAssetbundleInfo.Key))
                        assetbundleIdentifiers.Add(anAssetbundleInfo.Key);
                }
            }
            return assetbundleIdentifiers;
        }

        public static List<string> GetAllAssetbundlesToDownload(string category, string pathPrefix)
        {
            var assetbundleIdentifiers = new List<string>();
            if (assetBundleCategoryDictionary.TryGetValue(category, out var categoryDictionary))
            {
                foreach (var assetbundleInfo in categoryDictionary)
                {
                    if (Caching.IsVersionCached(assetbundleInfo.Key, Hash128.Parse(assetbundleInfo.Value.HashCodeString)))
                    {
                        continue;
                    }
                    //if (assetbundleInfo.Value.HasAlreadyBeenDownloaded())
                    //{
                    //    continue;
                    //}
                    if (!string.IsNullOrEmpty(assetbundleInfo.Key))
                    {
                        string aStrippedAssetbundleName = assetbundleInfo.Key.Substring(0, assetbundleInfo.Key.IndexOf(".assetbundle"));
                        if (!assetbundleIdentifiers.Contains(aStrippedAssetbundleName))
                            assetbundleIdentifiers.Add(aStrippedAssetbundleName);
                    }
                }
            }
            return assetbundleIdentifiers;
        }
    }
}