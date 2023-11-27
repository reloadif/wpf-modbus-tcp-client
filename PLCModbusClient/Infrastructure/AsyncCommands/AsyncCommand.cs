using PLCModbusClient.Infrastructure.AsyncCommands.Base;

namespace PLCModbusClient.Infrastructure.AsyncCommands
{
    public class AsyncCommand : AsyncCommandBase
    {
        private readonly Func<Task> _command;
        private readonly Func<object, bool>? _canExecute;

        public AsyncCommand(Func<Task> command, Func<object, bool>? canExecute = null)
        {
            _command = command;
            _canExecute = canExecute;
        }
        public override bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }
        public override Task ExecuteAsync(object parameter)
        {
            return _command();
        }
    }
}
