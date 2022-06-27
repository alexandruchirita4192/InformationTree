using Castle.Windsor;
using InformationTree.Domain.Services;
using InformationTree.Infrastructure.MediatR;
using InformationTree.Infrastructure.MediatR.SelfTest;

namespace InformationTree.Infrastructure;

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

        var mediatorSelfTest = container.Resolve<IConfigurationReader>()
            ?.GetConfiguration()
            ?.ApplicationFeatures
            ?.MediatorSelfTest ?? false;

        if (mediatorSelfTest)
        {
            var writer = new StringWriter();
            try
            {
                writer.WriteLine($"Starting MediatR with Castle Windsor self-test at {DateTime.Now}");
                var mediator = container.BuildMediatorForSelfTest(writer);

                var mediatorSelfTestFunc = () => TestRunner.Run(mediator, writer, "MediatoR.CastleWindsor.SelfTest", true);
                Task.Run(() => mediatorSelfTestFunc())
                    .Wait();
            }
            catch (Exception ex)
            {
                writer.WriteLine($"MediatR self-test failed with exception: {ex}.");
            }
            finally
            {
                writer.WriteLine($"MediatR self-test finished at {DateTime.Now}. Press enter to exit.");
            }
            File.WriteAllText("MediatR.SelfTest.txt", writer.ToString());
        }
        else
        {
            var application = container.Resolve<IApplication>();
            application.Run();
        }
    }
}