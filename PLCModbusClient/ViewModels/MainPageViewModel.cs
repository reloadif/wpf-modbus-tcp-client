using PLCModbusClient.ViewModels.Base;
using PLCModbusClient.Views;
using System.Windows.Controls;

namespace PLCModbusClient.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        #region Properties
        public ContentControl HomePageContentControl { get; init; }
        public ContentControl SettingPageContentControl { get; init; }
        #endregion

        #region Events
        public event Action? OnClose;
        #endregion

        public MainPageViewModel(HomePage homePage, HomePageViewModel homePageViewModel, SettingPage settingPage)
        {
            HomePageContentControl = homePage;
            SettingPageContentControl = settingPage;

            homePageViewModel.OnClose += OnHomePageClose;
        }

        #region PrivateMethods
        private void OnHomePageClose()
        {
            OnClose?.Invoke();
        }
        #endregion
    }
}
