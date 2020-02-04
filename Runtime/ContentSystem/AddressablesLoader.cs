using MEMCore.ContentSystem.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ADDRESSABLES
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
#endif

namespace Packages.MEMCoreUnity.Runtime.ContentSystem
{
    public class AddressablesLoader : IAssetLoader
    {
#if UNITY_ADDRESSABLES
        public AsyncOperationHandle<IResourceLocator> InitializeAsync(string resourceLocation)
        {
            var waitHandle = Addressables.InitializeAsync();
            waitHandle.Completed += (loader) => { Debug.Log("Addressables initialized for location: " + resourceLocation); };

            return waitHandle;
        }

        public AsyncOperationHandle LoadAssetDependeciesAsync(string identifier)
        {
            var waitHandle = Addressables.DownloadDependenciesAsync(identifier);
            waitHandle.Completed += (loadedObject) => { Debug.Log(identifier + " loaded dependencies"); };

            return waitHandle;
        }

        public AsyncOperationHandle<T> LoadAssetAsync<T>(string identifier, Action<AsyncOperationHandle<T>> callback = null)
        {
            var waitHandle = Addressables.LoadAssetAsync<T>(identifier);
            if (callback != null)
            {
                waitHandle.Completed += callback;
            }
            else
            {
                waitHandle.Completed += (loadedObject) => { Debug.Log(identifier + " loaded"); };
            }

            return waitHandle;
        }
#endif

        public void LoadAsset<T>(string identifier) where T : class
        {
        }

        public IEnumerator LoadAssetAsync<T>(string identifier, Action<T> callback) where T : class
        {
#if UNITY_ADDRESSABLES
            var waitHandle = Addressables.LoadAssetAsync<T>(identifier);
            //if (callback != null)
            //{
            //    waitHandle.Completed += callback;
            //}
            //else
            //{
            //    waitHandle.Completed += (loadedObject) => { Debug.Log(identifier + " loaded"); };
            //}

            return waitHandle;
            //return Addressables.LoadAssetAsync<T>(identifer);
#endif
            yield return null;
        }

        public void ReleaseAsset<T>(T gameObject)
        {
#if UNITY_ADDRESSABLES
            Addressables.Release<T>(gameObject);
#endif
            //Addressables.ReleaseInstance(null); // NOTE: there seems to be a problem with this package, as it tries to reference the UnityEngine.CoreModule which shouldn't be referenced by external dlls. Opening a thread about it in the Unity forum (PH)
        }

#if UNITY_ADDRESSABLES
        public AsyncOperationHandle<SceneInstance> LoadSceneAsync(string identifier)
        {
            return new AsyncOperationHandle<SceneInstance>();
            //var waitHandle = Addressables.LoadSceneAsync(identifier);
            //waitHandle.Completed += (loadedObject) => { Debug.Log(identifier + " scene loaded"); };

            //return waitHandle;
        }

        public AsyncOperationHandle<IResourceLocator> LoadAssetCatalogueAsync(string identifier)
        {
            var waitHandle = Addressables.LoadContentCatalogAsync(identifier);
            waitHandle.Completed += (loadedObject) => { Debug.Log("catalogue: " + identifier + " loaded"); };

            return waitHandle;
        }

        public AsyncOperationHandle<IList<IResourceLocation>> LoadResourceLocationAsync(string identifier)
        {
            var waitHandle = Addressables.LoadResourceLocationsAsync(identifier);
            waitHandle.Completed += (loadedObject) => { Debug.Log("resource location: " + identifier + " loaded"); };

            return waitHandle;
        }

        public AsyncOperationHandle<GameObject> InstantiateAsset(string identifier)
        {
            return new AsyncOperationHandle<GameObject>();
            //var waitHandle = Addressables.InstantiateAsync(identifier);
            //waitHandle.Completed += (loadedObject) => { Debug.Log(identifier + " instantiated"); };

            //return waitHandle;
        }

        public bool DestroyAssetInstance(GameObject gameObject)
        {
            return false;
            //return Addressables.ReleaseInstance(gameObject);
        }

        public AsyncOperationHandle<SceneInstance> UnloadScene(SceneInstance scene)
        {
            var waitHandle = Addressables.UnloadSceneAsync(scene);
            //waitHandle.Completed += (loadedObject) => { Debug.Log("Scene: " + scene.Scene.name + " is unloaded"); }; // maybe this may result in a null ref, as the scene is unloaded on complete? gotta test it out as i'm curious about it (PH)

            return waitHandle;
        }
#endif
    }
}
