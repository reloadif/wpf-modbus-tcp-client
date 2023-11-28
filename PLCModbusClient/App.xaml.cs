using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PLCModbusClient.ViewModels;
using PLCModbusClient.Views;
using System.Windows;

namespace PLCModbusClient
{
    public partial class App : Application
    {
        public static IHost? AppHost { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices ( (hostContext, services) =>
                {
                    services.AddSingleton<HomePageViewModel>();
                    services.AddSingleton<HomePage>();

                    services.AddSingleton<SettingPageViewModel>();
                    services.AddSingleton<SettingPage>();

                    services.AddSingleton<MainPageViewModel>();
                    services.AddSingleton<MainPage>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs eventArgs)
        {
            await AppHost!.StartAsync();

            MainPage startupPage = AppHost.Services.GetRequiredService<MainPage>();
            startupPage.Show();

            MainPageViewModel startupPageViewModel = AppHost.Services.GetRequiredService<MainPageViewModel>();
            startupPageViewModel.OnClose += () =>
            {
                startupPage.Close();
            };

            base.OnStartup(eventArgs);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();
            base.OnExit(e);
        }
    }
}
