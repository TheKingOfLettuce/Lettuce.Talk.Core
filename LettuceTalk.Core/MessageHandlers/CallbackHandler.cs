namespace LettuceTalk.Core.MessageHandlers;

internal interface CallbackHandlerBase {}

internal class CallbackHandler<T> : CallbackHandlerBase where T : Message  {
    private event Action<T>? _callbacks;

    public void AddCallback(Action<T> func) {
        _callbacks += func;
    }

    public void RemoveCallback(Action<T> func) {
        _callbacks -= func;
    }

    public void HandleMessage(T message) {
        _callbacks?.Invoke(message);
    }

    public bool HasCallbacks() {
        return _callbacks != null;
    }
}