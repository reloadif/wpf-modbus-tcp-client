using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PLCModbusClient.ViewModels.Base
{
    public abstract class BaseViewModel : INotifyPropertyChanged, IDisposable
    {
        private bool _disposed;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected virtual void Dispose(bool Disposing)
        {
            // Освобождение неуправляемых ресурсов

            if (!Disposing || _disposed) return;
            _disposed = true;

            // Освобождение управляемых ресурсов
        }
        public void Dispose()
        {
            Dispose(true);
        }

        //~BaseViewModel()
        //{
        //    Dispose(false);
        //}
    }
}
