namespace Packages.MEMCoreUnity.Runtime.ContentSystem
{
    public class AssetbundleInfo
    {
        public readonly string HashCodeString;
        public readonly uint CrcValue;
        public readonly int FileLength;

        public AssetbundleInfo(AssetbundleInfo assetbundleInfo)
        {
            HashCodeString = assetbundleInfo.HashCodeString;
            CrcValue = assetbundleInfo.CrcValue;
            FileLength = assetbundleInfo.FileLength;
        }

        public AssetbundleInfo(string hashCode, uint crc, int length)
        {
            HashCodeString = hashCode;
            CrcValue = crc;
            FileLength = length;
        }
    }
}