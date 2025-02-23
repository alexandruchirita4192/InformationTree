using InformationTree.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using InformationTree.Domain.Services.Graphics;
using InformationTree.Extra.Graphics.Computation;
using InformationTree.Extra.Graphics.Services;
using InformationTree.Extra.Sound;
using InformationTree.Forms;
using InformationTree.PgpEncryption;
using InformationTree.Render.WinForms.Services;

namespace InformationTree.Infrastructure;

public static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    public static void Main(string[] _)
    {
        var host = CreateHostBuilder().Build();
        var application = host.Services.GetRequiredService<IApplication>();
        application.Run();
    }

    private static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(MainForm).Assembly));
                services.AddSingleton<IConfigurationReader, ConfigurationReader>();
                services.AddSingleton<ISoundProvider, SoundProvider>();
                services.AddSingleton<IPopUpService, WinFormsPopUpService>();
                services.AddSingleton<IGraphicsFileFactory, GraphicsFileFactory>();
                services.AddSingleton<ICompressionProvider, CompressionProvider>();
                services.AddSingleton<IImportTreeFromXmlService, ImportTreeFromXmlService>();
                services.AddSingleton<IExportTreeToXmlService, ExportTreeToXmlService>();
                services.AddSingleton<ICachingService, CachingService>();
                services.AddSingleton<ITreeNodeDataCachingService, TreeNodeDataCachingService>();
                services.AddSingleton<ITreeNodeToTreeNodeDataAdapter, TreeNodeToTreeNodeDataAdapter>();
                services.AddSingleton<ITreeNodeDataToTreeNodeAdapter, TreeNodeDataToTreeNodeAdapter>();
                services.AddSingleton<IApplication, WinFormsApplication>();
                services.AddSingleton<ICanvasFormFactory, CanvasPopUpFormFactory>();
                services.AddSingleton<IPGPEncryptionProvider, PGPEncryptionProvider>();
                services.AddSingleton<IPGPEncryptionAndSigningProvider, PGPEncryptionAndSigningProvider>();
                services.AddSingleton<IExportNodeToRtfService, ExportNodeToRtfService>();
                services.AddSingleton<IImportExportTreeXmlService, ImportExportTreeXmlService>();
                services.AddSingleton<ITreeNodeSelectionCachingService, TreeNodeSelectionCachingService>();
                services.AddSingleton<IProfilingService, ProfilingService>();
                services.AddSingleton<IGraphicsParser, GraphicsParser>();
            });
    }
}