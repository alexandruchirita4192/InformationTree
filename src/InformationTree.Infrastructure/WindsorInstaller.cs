using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using InformationTree.Domain.Services;
using InformationTree.Domain.Services.Graphics;
using InformationTree.Extra.Graphics.Computation;
using InformationTree.Extra.Graphics.Services;
using InformationTree.Extra.Sound;
using InformationTree.Forms;
using InformationTree.Infrastructure.MediatR;
using InformationTree.PgpEncryption;
using InformationTree.Render.WinForms.Services;

namespace InformationTree.Infrastructure
{
    public class WindsorInstaller : IWindsorInstaller
    {
        public WindsorInstaller()
        {
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IConfigurationReader>().ImplementedBy<ConfigurationReader>().LifeStyle.Singleton);
            container.Register(Component.For<IApplication>().ImplementedBy<WinFormsApplication>().LifeStyle.Singleton);
            container.Register(Component.For<ISoundProvider>().ImplementedBy<SoundProvider>().LifeStyle.Singleton);
            container.Register(Component.For<IGraphicsFileFactory>().ImplementedBy<GraphicsFileFactory>().LifeStyle.Singleton);
            container.Register(Component.For<ICanvasFormFactory>().ImplementedBy<CanvasPopUpFormFactory>().LifeStyle.Singleton);
            container.Register(Component.For<IPopUpService>().ImplementedBy<WinFormsPopUpService>().LifeStyle.Singleton);
            container.Register(Component.For<IPGPEncryptionProvider>().ImplementedBy<PGPEncryptionProvider>().LifeStyle.Singleton);
            container.Register(Component.For<IPGPEncryptionAndSigningProvider>().ImplementedBy<PGPEncryptionAndSigningProvider>().LifeStyle.Singleton);
            container.Register(Component.For<ICompressionProvider>().ImplementedBy<CompressionProvider>().LifeStyle.Singleton);
            container.Register(Component.For<IExportNodeToRtfService>().ImplementedBy<ExportNodeToRtfService>().LifeStyle.Singleton);
            container.Register(Component.For<ITreeNodeDataCachingService>().ImplementedBy<TreeNodeDataCachingService>().LifeStyle.Singleton);
            container.Register(Component.For<IExportTreeToXmlService>().ImplementedBy<ExportTreeToXmlService>().LifeStyle.Singleton);
            container.Register(Component.For<IImportTreeFromXmlService>().ImplementedBy<ImportTreeFromXmlService>().LifeStyle.Singleton);
            container.Register(Component.For<IImportExportTreeXmlService>().ImplementedBy<ImportExportTreeXmlService>().LifeStyle.Singleton);
            container.Register(Component.For<ITreeNodeSelectionCachingService>().ImplementedBy<TreeNodeSelectionCachingService>().LifeStyle.Singleton);
            container.Register(Component.For<ICachingService>().ImplementedBy<CachingService>().LifeStyle.Singleton);
            container.Register(Component.For<ITreeNodeToTreeNodeDataAdapter>().ImplementedBy<TreeNodeToTreeNodeDataAdapter>().LifeStyle.Singleton);
            container.Register(Component.For<ITreeNodeDataToTreeNodeAdapter>().ImplementedBy<TreeNodeDataToTreeNodeAdapter>().LifeStyle.Singleton);
            container.Register(Component.For<IProfilingService>().ImplementedBy<ProfilingService>().LifeStyle.Singleton);
            
            container.RegisterMediatorForUsageFrom(typeof(MainForm).Assembly);
        }
    }
}