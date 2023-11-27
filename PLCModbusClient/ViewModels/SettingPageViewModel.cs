using FluentModbus;
using PLCModbusClient.Infrastructure.Commands;
using PLCModbusClient.Infrastructure.Configs;
using PLCModbusClient.ViewModels.Base;
using System.Windows;
using System.Windows.Input;

namespace PLCModbusClient.ViewModels
{
    public class SettingPageViewModel : BaseViewModel
    {
        #region Fields
        private string _controllerIp;
        private string _controllerPort;
        #endregion

        #region Properties
        public string ControllerIp
        {
            get => _controllerIp;
            set => Set(ref _controllerIp, value);
        }

        public string СontrollerPort
        {
            get => _controllerPort;
            set => Set(ref _controllerPort, value);
        }
        #endregion

        public SettingPageViewModel()
        {
            _controllerIp = Config.DefaultModbusControllerIp;
            _controllerPort = Config.DefaultModbusControllerPort;

            ClickTestConnectionButton = new LambdaCommand(ExecuteTestConnectionButton, CanExecuteTestConnectionButton);
        }

        #region Commands
        public ICommand ClickTestConnectionButton { get; init; }
        #endregion

        #region Private methods
        private void ExecuteTestConnectionButton(object p)
        {
            try
            {
                ModbusTcpClient modbusClient = new();
                modbusClient.Connect(string.Join(':', _controllerIp, _controllerPort));
                if (modbusClient.IsConnected)
                {
                    MessageBox.Show("Успешное тестовое подключения к серверу Modbus TCP!");
                    modbusClient.Disconnect();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка тестового подключения к серверу Modbus TCP:\n" + ex.Message, "Ошибка подключения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool CanExecuteTestConnectionButton(object p)
        {
            return !(string.IsNullOrEmpty(_controllerIp) && string.IsNullOrEmpty(_controllerPort));
        }
        #endregion
    }
}
