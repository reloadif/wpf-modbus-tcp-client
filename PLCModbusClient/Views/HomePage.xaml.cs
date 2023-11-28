using PLCModbusClient.ViewModels;
using System.Windows.Controls;

namespace PLCModbusClient.Views
{
    public partial class HomePage : UserControl
    {
        public HomePage(HomePageViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
