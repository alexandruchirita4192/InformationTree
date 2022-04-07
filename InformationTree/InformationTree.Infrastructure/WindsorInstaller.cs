using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using InformationTree.Domain.Services;
using InformationTree.Domain.Services.Graphics;
using InformationTree.Extra.Graphics.Computation;
using InformationTree.Extra.Graphics.Services;
using InformationTree.Extra.Sound;
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
            container.Register(Component.For<ITreeRenderer>().ImplementedBy<WinFormsTreeRenderer>().LifeStyle.Singleton);
            container.Register(Component.For<IApplication>().ImplementedBy<WinFormsApplication>().LifeStyle.Singleton);
            container.Register(Component.For<ICommandLineParser>().ImplementedBy<CommandLineParser>().LifeStyle.Singleton);
            container.Register(Component.For<ISoundProvider>().ImplementedBy<SoundProvider>().LifeStyle.Singleton);
            container.Register(Component.For<IGraphicsFileFactory>().ImplementedBy<GraphicsFileFactory>().LifeStyle.Singleton);
            container.Register(Component.For<ICanvasFormFactory>().ImplementedBy<CanvasPopUpFormFactory>().LifeStyle.Singleton);
            container.Register(Component.For<IPopUpService>().ImplementedBy<WinFormsPopUpService>().LifeStyle.Singleton);
        }
    }
}