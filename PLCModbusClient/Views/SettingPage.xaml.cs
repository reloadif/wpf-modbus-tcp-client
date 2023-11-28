using PLCModbusClient.ViewModels;
using System.Windows.Controls;

namespace PLCModbusClient.Views
{
    public partial class SettingPage : UserControl
    {
        public SettingPage(SettingPageViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
