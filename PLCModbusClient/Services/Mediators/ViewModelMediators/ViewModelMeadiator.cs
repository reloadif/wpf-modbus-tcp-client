using PLCModbusClient.Services.Mediators.ViewModelMediators.Interface;

namespace PLCModbusClient.Services.Mediators.ViewModelMediators
{
    public class ViewModelMeadiator : IViewModelMeadiator
    {
        private readonly IDictionary<string, Action<object?>> _mainDictionary;

        public ViewModelMeadiator()
        {
            _mainDictionary = new Dictionary<string, Action<object?>>();
        }

        public void Notify(string token, object? args = null)
        {
            if (_mainDictionary.ContainsKey(token))
            {
                _mainDictionary[token].Invoke(args);
            }
            else
            {
                throw new Exception("This token doesn't exist!");
            }
        }

        public void Subscribe(string token, Action<object?> callback)
        {
            if (!_mainDictionary.ContainsKey(token))
            {
                _mainDictionary.Add(token, callback);
            }
            else
            {
                throw new Exception("This token already exist!");
            }
        }

        public void Unsubscribe(string token, Action<object?> callback)
        {
            if (_mainDictionary.ContainsKey(token))
            {
                _ = _mainDictionary.Remove(token);
            }
        }
    }
}
