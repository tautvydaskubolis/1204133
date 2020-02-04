﻿using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Packages.MEMCoreUnity.Editor.ReferenceFinder
{
    public class ReferenceFinder : EditorWindow
    {
        private string guidToFind = string.Empty;
        private string replacementGuid = string.Empty;
        private Object searchedObject;
        private Dictionary<Object, int> referenceObjects = new Dictionary<Object, int>();
        private Vector2 scrollPosition;
        private Stopwatch searchTimer = new Stopwatch();

        [MenuItem("Window/Reference Finder")]
        public static void Init()
        {
            GetWindow(typeof(ReferenceFinder), false, "Reference Finder");
        }

        public void OnGUI()
        {
            if (EditorSettings.serializationMode == SerializationMode.ForceText)
            {
                DisplayMainMenu();

                if (GUILayout.Button("Search"))
                {
                    searchTimer.Reset();
                    searchTimer.Start();

                    referenceObjects.Clear();
                    var pathToAsset = AssetDatabase.GUIDToAssetPath(guidToFind);
                    if (!string.IsNullOrEmpty(pathToAsset))
                    {
                        searchedObject = AssetDatabase.LoadAssetAtPath<Object>(pathToAsset);

                        var allPathToAssetsList = new List<string>();
                        var allPrefabs = Directory.GetFiles(Application.dataPath, "*.prefab", SearchOption.AllDirectories);
                        allPathToAssetsList.AddRange(allPrefabs);
                        var allScenes = Directory.GetFiles(Application.dataPath, "*.unity", SearchOption.AllDirectories);
                        allPathToAssetsList.AddRange(allScenes);

                        string assetPath;
                        for (int i = 0; i < allPathToAssetsList.Count; i++)
                        {
                            assetPath = allPathToAssetsList[i];
                            var text = File.ReadAllText(assetPath);
                            var lines = text.Split('\n');
                            for (int j = 0; j < lines.Length; j++)
                            {
                                var line = lines[j];
                                if (line.Contains("guid:"))
                                {
                                    if (line.Contains(guidToFind))
                                    {
                                        var pathToReferenceAsset = assetPath.Replace(Application.dataPath, string.Empty);
                                        pathToReferenceAsset = pathToReferenceAsset.Replace(".meta", string.Empty);
                                        var path = "Assets" + pathToReferenceAsset;
                                        path = path.Replace(@"\", "/"); // fix OSX/Windows path
                                        var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                                        if (asset != null)
                                        {
                                            if (!referenceObjects.ContainsKey(asset))
                                            {
                                                referenceObjects.Add(asset, 0);
                                            }
                                            referenceObjects[asset]++;
                                        }
                                        else
                                        {
                                            Debug.LogError(path + " could not be loaded");
                                        }
                                    }
                                }
                            }
                        }

                        searchTimer.Stop();
                        //Debug.Log("Search took " + searchTimer.Elapsed);
                    }
                    else
                    {
                        Debug.LogError("no asset found for GUID: " + guidToFind);
                    }
                }

                if (referenceObjects.Count > 0 && GUILayout.Button("Replace"))
                {
                    ReplaceGuids(referenceObjects, guidToFind, replacementGuid);
                }

                DisplayReferenceObjectList(referenceObjects);
            }
            else
            {
                DisplaySerializationWarning();
            }
        }

        private void ReplaceGuids(Dictionary<Object, int> referenceObjects, string guidToFind, string replacementGuid)
        {
            foreach (var referenceObject in referenceObjects.Keys)
            {
                var assetPath = AssetDatabase.GetAssetPath(referenceObject);
                var text = File.ReadAllText(assetPath);
                var newText = text.Replace(guidToFind, replacementGuid);
                Debug.Log("Overwriting file data of: " + referenceObject.name + "\n\nOld:\n" + text + "\n\nNew:\n" + newText);
                File.WriteAllText(assetPath, newText);
            }
            AssetDatabase.Refresh(ImportAssetOptions.Default);
        }

        private void DisplaySerializationWarning()
        {
            GUILayout.Label("The Reference Finder relies on readable meta files (visible text serialization).\nPlease change your serialization mode in \"Edit/Project Settings/Editor/Version Control\"\n to \"Visisble Meta Files\" and \"Asset Serialization\" to \"Force Text\".");
        }

        private void DisplayReferenceObjectList(Dictionary<Object, int> referenceObjectsDictionary)
        {
            GUILayout.Label("Reference by: " + referenceObjectsDictionary.Count + " assets. (Last search duration: " + searchTimer.Elapsed + ")");
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            foreach (var referenceObject in referenceObjectsDictionary)
            {
                var referencingObject = referenceObject.Key;
                var referenceCount = referenceObject.Value;
                EditorGUILayout.ObjectField(referencingObject.name + " (" + referenceCount + ")", referencingObject, typeof(Object), false);
            }
            EditorGUILayout.EndScrollView();
        }

        private void DisplayMainMenu()
        {
            EditorGUILayout.BeginHorizontal();
            searchedObject = EditorGUILayout.ObjectField(searchedObject != null ? searchedObject.name : "Drag & Drop Asset", searchedObject, typeof(Object), false);
            if (GUILayout.Button("Get GUID") && searchedObject != null)
            {
                var pathToAsset = AssetDatabase.GetAssetPath(searchedObject);
                guidToFind = AssetDatabase.AssetPathToGUID(pathToAsset);
            }
            EditorGUILayout.EndHorizontal();

            var newGuidToFind = EditorGUILayout.TextField("GUID", guidToFind);
            if (!guidToFind.Equals(newGuidToFind))
                guidToFind = newGuidToFind;

            if (referenceObjects != null && referenceObjects.Count > 0)
            {
                var newReplacementGuid = EditorGUILayout.TextField("Replacement", replacementGuid);
                if (!replacementGuid.Equals(newReplacementGuid))
                    replacementGuid = newReplacementGuid;
            }
        }
    }
}