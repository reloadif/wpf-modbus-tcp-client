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
        private bool _isRunning;

        private int _registerNumber;
        private short _registerValue;

        private int _getRegisterNumber;
        private short _getRegisterValue;
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

        public int RegisterNumber
        {
            get => _registerNumber;
            set => Set(ref _registerNumber, value);
        }

        public short RegisterValue
        {
            get => _registerValue;
            set => Set(ref _registerValue, value);
        }

        public int GetRegisterNumber
        {
            get => _getRegisterNumber;
            set => Set(ref _getRegisterNumber, value);
        }

        public short GetRegisterValue
        {
            get => _getRegisterValue;
            set => Set(ref _getRegisterValue, value);
        }
        #endregion

        #region Events
        public event Action? OnClose;
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

            SetHoldingRegisterByte = new LambdaCommand(ExecuteSetHoldingRegisterByte, CanExecuteSetHoldingRegisterByte);
            GetHoldingRegisterByte = new LambdaCommand(ExecuteGetHoldingRegisterByte, CanExecuteGetHoldingRegisterByte);
        }

        #region Commands
        public ICommand ModbusTCPClientConnect { get; init; }
        public ICommand ModbusTCPClientDisconnect { get; init; }
        public ICommand TemporarilySetCoilsFirstBit { get; init; }
        public ICommand SetCoilsSecondBit { get; init; }

        public ICommand SetHoldingRegisterByte { get; init; }
        public ICommand GetHoldingRegisterByte { get; init; }
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
                _isRunning = false;
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
                _isRunning = true;
                while (_isRunning)
                {
                    await Task.Delay(500);
                    if ((await _modbusClient.ReadCoilsAsync(0, 2, 1)).ToArray()[0] == 1)
                    {
                        await _modbusClient.WriteSingleCoilAsync(0, 2, false);
                        _isRunning = false;
                    }
                }

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    _modbusClient.Disconnect();
                    OnClose?.Invoke();
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при чтении третьего бита:\n" + ex.Message, "Ошибка чтения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteSetHoldingRegisterByte(object p)
        {
            try
            {
                _modbusClient.WriteSingleRegister(0, RegisterNumber, RegisterValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при отправке в регистр:\n" + ex.Message, "Ошибка регистра", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool CanExecuteSetHoldingRegisterByte(object p)
        {
            return IsModbusClientConnected;
        }

        private void ExecuteGetHoldingRegisterByte(object p)
        {
            try
            {
                GetRegisterValue = BitConverter.ToInt16(_modbusClient.ReadHoldingRegisters(0, (ushort)GetRegisterNumber, 2).ToArray(), 0) ;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при чтении из регистра:\n" + ex.Message, "Ошибка регистра", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool CanExecuteGetHoldingRegisterByte(object p)
        {
            return IsModbusClientConnected;
        }
        #endregion
    }
}
