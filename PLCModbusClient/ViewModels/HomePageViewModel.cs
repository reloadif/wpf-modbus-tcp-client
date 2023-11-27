using FluentModbus;
using PLCModbusClient.Infrastructure.AsyncCommands;
using PLCModbusClient.Infrastructure.Commands;
using PLCModbusClient.Infrastructure.Configs;
using PLCModbusClient.ViewModels.Base;
using System.Windows;
using System.Windows.Input;

namespace PLCModbusClient.ViewModels
{
    public class HomePageViewModel : BaseViewModel
    {
        #region Fields
        private ModbusTcpClient _modbusClient;
        private bool _isBusy;
        #endregion

        #region Properties
        public bool IsModbusClientConnected
        {
            get => _modbusClient.IsConnected;
        }
        #endregion

        public HomePageViewModel()
        {
            _modbusClient = new();
            _isBusy = false;

            ModbusTCPClientConnect = new LambdaCommand(ExecuteModbusTCPClientConnect, CanExecuteModbusTCPClientConnect);
            ModbusTCPClientDisconnect = new LambdaCommand(ExecuteModbusTCPClientDisconnect, CanExecuteModbusTCPClientDisconnect);
            TemporarilySetCoilsFirstBit = new AsyncCommand(ExecuteTemporarilySetCoilsFirstBit, CanExecuteTemporarilySetCoilsFirstBit);
        }

        #region Commands
        public ICommand ModbusTCPClientConnect {  get; init; }
        public ICommand ModbusTCPClientDisconnect {  get; init; }
        public ICommand TemporarilySetCoilsFirstBit {  get; init; }
        #endregion

        #region Private methods
        private void ExecuteModbusTCPClientConnect(object p)
        {
            try
            {
                _modbusClient.Connect(string.Join(':', Config.DefaultModbusControllerIp, Config.DefaultModbusControllerPort));
                OnPropertyChanged(nameof(IsModbusClientConnected));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения к серверу Modbus TCP:\n" + ex.Message, "Ошибка подключения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool CanExecuteModbusTCPClientConnect(object p)
        {
            return !_modbusClient.IsConnected;
        }

        private void ExecuteModbusTCPClientDisconnect(object p)
        {
            try
            {
                _modbusClient.Disconnect();
                OnPropertyChanged(nameof(IsModbusClientConnected));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка отключении от сервера Modbus TCP:\n" + ex.Message, "Ошибка отключения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool CanExecuteModbusTCPClientDisconnect(object p)
        {
            return _modbusClient.IsConnected;
        }

        private async Task ExecuteTemporarilySetCoilsFirstBit()
        {
            _isBusy = true;

            await _modbusClient.WriteSingleCoilAsync(0, 1, true);
            await Task.Delay(3000);

            _isBusy = false;
        }
        private bool CanExecuteTemporarilySetCoilsFirstBit(object p)
        {
            return _modbusClient.IsConnected && !_isBusy;
        }
        #endregion
    }
}
