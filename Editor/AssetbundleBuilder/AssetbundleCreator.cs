using UnityEditor;

namespace Packages.MEMCoreUnity.Editor.AssetbundleBuilder
{
    public class AssetbundleCreator
    {
        [MenuItem("MEM/Build Assetbundles")]
        public static void BuildAssetbundles(MenuCommand command)
        {
            BuildAssetbundles(BuildTarget.StandaloneWindows64, BuildTargetGroup.Standalone, "/Assetbundles/");
        }

        public static void BuildAssetbundles(BuildTarget buildTarget, BuildTargetGroup buildGroup, string outputPath)
        {
#if UNITY_ADDRESSABLES
            AddressablesBuilder.BuildAssetbundles(buildTarget, buildGroup, outputPath);
#else
            AssetbundleBuilder.BuildAssetbundles(buildTarget, buildGroup, outputPath);
#endif
        }
    }
}
