namespace PLCModbusClient.Services.Mediators.ViewModelMediators.Interface
{
    public interface IViewModelMeadiator
    {
        public void Notify(string token, object args);
        public void Subscribe(string token, Action<object?> callback);
        public void Unsubscribe(string token, Action<object?> callback);
    }
}
