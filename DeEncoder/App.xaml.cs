using System;
using System.Windows;
using DeEncoder.ViewModels;
using DevExpress.Xpf.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DeEncoder;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IHost host;

    private IConfiguration Configuration { get; set; }

    private IHost BuildHost() =>
        new HostBuilder().ConfigureAppConfiguration(ConfigureConfiguration)
                         .ConfigureServices(ConfigureServices)
                         .UseSerilog(
                                     (context, loggerConfiguration) =>
                                         loggerConfiguration.ReadFrom.Configuration(context.Configuration)
                                    )
                         .Build();

    private static void ConfigureConfiguration(HostBuilderContext context, IConfigurationBuilder configurationBuilder)
    {
        var optionalJsonFileName = $"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json";

        var basePath = context.HostingEnvironment.ContentRootPath;
        configurationBuilder.SetBasePath(basePath);
        configurationBuilder.AddJsonFile("appsettings.json", false);
        configurationBuilder.AddJsonFile(optionalJsonFileName, true);
        configurationBuilder.AddEnvironmentVariables("DEENCODER");
    }

    private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        Configuration = context.Configuration;
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        DispatcherUnhandledException += (sender, args) =>
                                        {
                                            var logger = host?.Services.GetService<ILogger>();
                                            logger?.Error(args.Exception, "Unhanled exception in WorkMeasure");

                                            MessageBox.Show(
                                                            args.Exception.Message,
                                                            "Anwendung wird beendet",
                                                            MessageBoxButton.OK,
                                                            MessageBoxImage.Error
                                                           );

                                            Shutdown();
                                            args.Handled = true;
                                        };

        try
        {
            host = BuildHost();
            ApplicationThemeHelper.ApplicationThemeName = Configuration.GetValue<string>("ApplicationThemeName");
            await host.StartAsync().ConfigureAwait(false);

            var mainWindow = host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                            ex.Message,
                            "Anwendung wird beendet",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                           );
            Shutdown();
        }
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        try
        {
            if (host is { })
            {
                using (host)
                {
                    await host.StopAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(true);
                }
            }

            base.OnExit(e);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                            ex.Message,
                            "Fehler in OnExit",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                           );
        }
    }
}
