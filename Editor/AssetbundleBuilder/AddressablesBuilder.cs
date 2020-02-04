using UnityEditor;
#if UNITY_ADDRESSABLES
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Interfaces;
#endif

namespace Packages.MEMCoreUnity.Editor.AssetbundleBuilder
{
    public class AddressablesBuilder
    {
        public static void BuildAssetbundles(BuildTarget buildTarget, BuildTargetGroup buildGroup, string outputPath)
        {
#if UNITY_ADDRESSABLES
            var assetbundleBuilds = ContentBuildInterface.GenerateAssetBundleBuilds();
            var buildContent = new BundleBuildContent(assetbundleBuilds);
            var buildParams = new BundleBuildParameters(buildTarget, buildGroup, outputPath);

            BundleBuildParameters bundleBuildParameters = null;
            IBundleBuildResults results;
            ContentPipeline.BuildAssetBundles(bundleBuildParameters, buildContent, out results);

            foreach (var bi in results.BundleInfos)
            {
                Debug.Log(bi.Key + " - " + bi.Value.FileName);
            }
#endif
        }
    }
}