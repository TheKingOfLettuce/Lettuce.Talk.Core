namespace Lettuce.Talk.Core.MessageHandlers;

internal abstract class CallbackHandlerBase {
    public abstract void HandleMessage(Message message);
}

internal class CallbackHandler<T> : CallbackHandlerBase where T : Message  {
    private event Action<T>? _callbacks;

    public void AddCallback(Action<T> func) {
        _callbacks += func;
    }

    public void RemoveCallback(Action<T> func) {
        _callbacks -= func;
    }

    public override void HandleMessage(Message message) => HandleMessage((T)message);

    public void HandleMessage(T message) {
        _callbacks?.Invoke(message);
    }

    public bool HasCallbacks() {
        return _callbacks != null;
    }
}