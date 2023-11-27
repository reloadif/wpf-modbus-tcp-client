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

        private bool _coilsSecondBit;
        #endregion

        #region Properties
        public bool IsModbusClientConnected
        {
            get => _modbusClient.IsConnected;
        }

        public bool IsCoilsSecondBit
        {
            get => _coilsSecondBit;
            set => Set(ref _coilsSecondBit, value);
        }
        #endregion

        public HomePageViewModel()
        {
            _modbusClient = new();
            _isBusy = false;

            _coilsSecondBit = false;

            ModbusTCPClientConnect = new LambdaCommand(ExecuteModbusTCPClientConnect, CanExecuteModbusTCPClientConnect);
            ModbusTCPClientDisconnect = new LambdaCommand(ExecuteModbusTCPClientDisconnect, CanExecuteModbusTCPClientDisconnect);
            TemporarilySetCoilsFirstBit = new AsyncCommand(ExecuteTemporarilySetCoilsFirstBit, CanExecuteTemporarilySetCoilsFirstBit);
            SetCoilsSecondBit = new AsyncCommand(ExecuteSetCoilsSecondBit, CanExecuteSetCoilsSecondBit);
        }

        #region Commands
        public ICommand ModbusTCPClientConnect {  get; init; }
        public ICommand ModbusTCPClientDisconnect {  get; init; }
        public ICommand TemporarilySetCoilsFirstBit {  get; init; }
        public ICommand SetCoilsSecondBit {  get; init; }
        #endregion

        #region Private methods
        private void ExecuteModbusTCPClientConnect(object p)
        {
            try
            {
                _modbusClient.Connect(string.Join(':', Config.DefaultModbusControllerIp, Config.DefaultModbusControllerPort));
                OnPropertyChanged(nameof(IsModbusClientConnected));
                Task.Factory.StartNew(async () =>
                {
                    await CheckThirdCoils();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения к серверу Modbus TCP:\n" + ex.Message, "Ошибка подключения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool CanExecuteModbusTCPClientConnect(object p)
        {
            return !IsModbusClientConnected;
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
            return IsModbusClientConnected;
        }

        private async Task ExecuteTemporarilySetCoilsFirstBit()
        {
            try
            {
                _isBusy = true;

                await _modbusClient.WriteSingleCoilAsync(0, 0, true);
                await Task.Delay(3000);
                await _modbusClient.WriteSingleCoilAsync(0, 0, false);

                _isBusy = false;
                TemporarilySetCoilsFirstBit.CanExecute(_isBusy);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при установке первого бита:\n" + ex.Message, "Ошибка установки", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool CanExecuteTemporarilySetCoilsFirstBit(object p)
        {
            return IsModbusClientConnected && !_isBusy;
        }

        private async Task ExecuteSetCoilsSecondBit()
        {
            try
            {
                await _modbusClient.WriteSingleCoilAsync(0, 1, !IsCoilsSecondBit);
                IsCoilsSecondBit = !IsCoilsSecondBit;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при установке второго бита:\n" + ex.Message, "Ошибка установки", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool CanExecuteSetCoilsSecondBit(object p)
        {
            return IsModbusClientConnected;
        }

        private async Task CheckThirdCoils()
        {
            try
            {
                bool isRunning = true;
                while (isRunning)
                {
                    if (_modbusClient.ReadCoils(0, 2, 1).ToArray()[0] == 1)
                    {
                        isRunning = false;
                    }

                    await Task.Delay(500);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при чтении третьего бита:\n" + ex.Message, "Ошибка чтения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
