using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Packages.MEMCoreUnity.Editor.BuildScript
{
    public class BuildScript : UnityEditor.Editor
    {
        public static void BuildPlayer()
        {
            var buildScenes = GetBuildScenes();
            var applicationName = Environment.GetEnvironmentVariable("APPLICATION_NAME") + ".exe";
            var outputPath = Environment.GetEnvironmentVariable("OUTPUT_PATH") + applicationName;
            var buildTarget = BuildTarget.StandaloneWindows64;
            var buildOptions = BuildOptions.None;

            var developmentBuildString = Environment.GetEnvironmentVariable("DEVELOPMENT_VERSION");
            bool devevelopmentBuild;
            if (bool.TryParse(developmentBuildString, out devevelopmentBuild))
            {
                buildOptions |= BuildOptions.Development;
            }

            var attachProfilerString = Environment.GetEnvironmentVariable("ATTACH_PROFILER");
            bool attachProfiler;
            if (bool.TryParse(attachProfilerString, out attachProfiler))
            {
                buildOptions |= BuildOptions.ConnectWithProfiler;
            }

            Debug.Log("Build application: " + applicationName + " for buildTarget: " + buildTarget + " with options: " + buildOptions + " to: " + outputPath);
            var buildScenesString = string.Empty;
            for (int i = 0; i < buildScenes.Length; i++)
            {
                var scene = buildScenes[i];
                buildScenesString += scene.path + "\n";
            }
            Debug.Log("Build includes scenes:\n" + buildScenesString);

            var buildReport = BuildPipeline.BuildPlayer(buildScenes, outputPath, buildTarget, buildOptions);

            if (buildReport != null)
            {
                Debug.Log("Build report created.");
            }
            else
            {
                Debug.LogError("No build report has been created! Maybe the build has been aborted! See console.");
            }
        }

        private static EditorBuildSettingsScene[] GetBuildScenes()
        {
            var buildScenesString = Environment.GetEnvironmentVariable("SCENES_TO_BUILD");
            var buildScenesSplit = buildScenesString.Split(',');
            var output = string.Empty;
            var checkedBuildScenes = new List<string>();
            for (int i = 0; i < buildScenesSplit.Length; i++)
            {
                var buildScene = buildScenesSplit[i];
                output += buildScene + "\n";
                checkedBuildScenes.Add(buildScene);
            }
            Debug.Log("build scenes activated: \n" + output);

            var activeScenes = new List<EditorBuildSettingsScene>();
            for (int i = 0; i < checkedBuildScenes.Count; i++)
            {
                var checkedBuildScene = checkedBuildScenes[i];

                for (int j = 0; j < EditorBuildSettings.scenes.Length; j++)
                {
                    var scene = EditorBuildSettings.scenes[j];
                    if (scene.path.Contains(checkedBuildScene))
                    {
                        activeScenes.Add(scene);
                    }
                }
            }

            if (checkedBuildScenes.Count != activeScenes.Count)
            {
                Debug.LogError("Number of checked scenes: " + checkedBuildScenes.Count + " is not equal to active scenes: " + activeScenes.Count);
            }
            return activeScenes.ToArray();
        }
    }
}
