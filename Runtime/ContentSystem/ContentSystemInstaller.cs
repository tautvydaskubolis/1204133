using MEMCore.ContentSystem.Interfaces;
using System;
using Zenject;

namespace Packages.MEMCoreUnity.Runtime.ContentSystem
{
    public class ContentSystemInstaller : IInstaller
    {
        private DiContainer parentContainer;

        public bool IsEnabled { get { return true; } }

        public ContentSystemInstaller(DiContainer container)
        {
            parentContainer = container;
        }

        public void InstallBindings()
        {
#if UNITY_ADDRESSABLES
            parentContainer.Bind<IAssetLoader>().To<AddressablesLoader>().AsSingle();
#else
            parentContainer.Bind<IAssetLoader>().To<AssetbundleLoader>().AsSingle();
#endif
        }
    }
}
