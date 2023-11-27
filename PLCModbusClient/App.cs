using PLCModbusClient.Views;
using System.Windows;

namespace PLCModbusClient
{
    public class App : Application
    {
        private readonly MainPage _mainPage;

        public App(MainPage mainPage)
        {
            _mainPage = mainPage;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _mainPage.Show();
            base.OnStartup(e);
        }
    }
}
