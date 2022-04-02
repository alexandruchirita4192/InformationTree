using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using InformationTree.Domain.Services;
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
            container.Register(Component.For<IPopUpConfirmation>().ImplementedBy<WinFormsPopUpConfirmation>().LifeStyle.Singleton);
            container.Register(Component.For<ICommandLineParser>().ImplementedBy<CommandLineParser>().LifeStyle.Singleton);
            container.Register(Component.For<ISoundProvider>().ImplementedBy<SoundProvider>().LifeStyle.Singleton);
        }
    }
}