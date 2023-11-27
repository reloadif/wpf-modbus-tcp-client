using System.Windows.Input;

namespace PLCModbusClient.Infrastructure.AsyncCommands.Interface
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
