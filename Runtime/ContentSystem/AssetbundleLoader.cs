using MEMCore.ContentSystem.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Packages.MEMCoreUnity.Runtime.ContentSystem
{
    public class AssetbundleLoader : IAssetLoader
    {
        private List<AssetBundle> loadedAssetbundles = new List<AssetBundle>();

        public void LoadAsset<T>(string identifier) where T : class
        {
        }

        public IEnumerator LoadAssetAsync<T>(string identifier, Action<T> callback) where T : class
        {
            yield return null;
        }

        public void LoadSceneAssetAsync(string assetName, Action<string[]> callback)
        {
            var assetbundleRequest = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + assetName);  //new UnityWebRequestAssetBundle(Application.streamingAssetsPath + anAssetName);

            if (assetbundleRequest != null)
            {
                assetbundleRequest.completed += (assetbundleOperation) =>
                {
                    if (assetbundleRequest.assetBundle.isStreamedSceneAssetBundle)
                    {
                        var scenePaths = assetbundleRequest?.assetBundle?.GetAllScenePaths();
                        callback?.Invoke(scenePaths);
                    }
                    else
                    {
                        callback?.Invoke(null);
                    }
                };
            }
            else
            {
                callback?.Invoke(null);
            }
        }

        public void LoadAssetAsyncFromDisk<T>(string assetName, Action<T> callback) where T : class
        {
            var assetbundleRequest = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + assetName);  //new UnityWebRequestAssetBundle(Application.streamingAssetsPath + anAssetName);

            if (assetbundleRequest != null)
            {
                assetbundleRequest.completed += (assetbundleOperation) => 
                {
                    if (!assetbundleRequest.assetBundle.isStreamedSceneAssetBundle)
                    {
                        var assetRequest = assetbundleRequest?.assetBundle?.LoadAssetAsync<T>(assetName);

                        if (assetRequest != null)
                        {
                            loadedAssetbundles.Add(assetbundleRequest.assetBundle);
                            assetRequest.completed += (assetOperation) =>
                            {
                                var loadedAsset = assetRequest.asset as T;
                                callback?.Invoke(loadedAsset);
                            };
                        }
                        else
                        {
                            assetbundleRequest.assetBundle.Unload(false);
                            loadedAssetbundles.Remove(assetbundleRequest.assetBundle);

                            callback?.Invoke(null);
                        }
                    }
                    else
                    {
                        callback?.Invoke(null);
                    }
                };
            }
            else
            {
                callback?.Invoke(null);
            }
        }

        public void ReleaseAsset<T>(T gameObject)
        {
        }
    }
}
