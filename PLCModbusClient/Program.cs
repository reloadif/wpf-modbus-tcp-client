using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PLCModbusClient.Services.Mediators.ViewModelMediators;
using PLCModbusClient.Services.Mediators.ViewModelMediators.Interface;
using PLCModbusClient.ViewModels;
using PLCModbusClient.Views;

namespace PLCModbusClient
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            IHost host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<App>();
                    services.AddSingleton<MainPage>();

                    services.AddSingleton<IViewModelMeadiator, ViewModelMeadiator>();  

                    services.AddSingleton<HomePageViewModel>();
                    services.AddSingleton<HomePage>();

                    services.AddSingleton<SettingPageViewModel>();
                    services.AddSingleton<SettingPage>();
                })
                .Build();

            App? app = host.Services.GetService<App>();
            app?.Run();
        }
    }
}
