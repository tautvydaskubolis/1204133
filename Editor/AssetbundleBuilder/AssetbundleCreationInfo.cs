using Packages.MEMCoreUnity.Runtime.ContentSystem;

namespace Packages.MEMCoreUnity.Editor.AssetbundleBuilder
{
    public struct AssetbundleCreationInfo
    {
        public readonly string assetbundleName;
        public readonly AssetbundleInfo assetbundleInfo;

        public AssetbundleCreationInfo(string name, AssetbundleInfo info)
        {
            assetbundleName = name;
            assetbundleInfo = info;
        }

        public override string ToString()
        {
            return assetbundleName + " (Hash:" + assetbundleInfo.HashCodeString + " - CRC:" + assetbundleInfo.CrcValue + " - Size:" + assetbundleInfo.FileLength + ")";
        }
    }
}
