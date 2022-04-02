using Castle.Windsor;
using InformationTree.Domain.Services;

namespace InformationTree.Infrastructure
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            var container = new WindsorContainer();
            container.Install(new WindsorInstaller());
            var application = container.Resolve<IApplication>();
            application.Run(args);
        }
    }
}