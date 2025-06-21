namespace Lettuce.Talk.Core.MessageHandlers;

/// <summary>
/// A statically wrapped <see cref="MessageCallbackHandler"/> to allow for easier internal communication
/// </summary>
public static class GlobalMessageHandler {
    private static Lazy<MessageCallbackHandler> _handler = new(() => new MessageCallbackHandler());

    /// <inheritdoc cref="MessageCallbackHandler.Subscribe{T}(Action{T})"/>
    public static void Subscribe<T>(Action<T> func) where T : Message => _handler.Value.Subscribe<T>(func);

    /// <inheritdoc cref="MessageCallbackHandler.Unsubscribe{T}(Action{T})"/>
    public static void Unsubscribe<T>(Action<T> func) where T : Message => _handler.Value.Unsubscribe<T>(func);

    /// <inheritdoc cref="MessageCallbackHandler.HasSubscribers{T}"/>
    public static bool HasSubscribers<T>() where T : Message => _handler.Value.HasSubscribers<T>();

    /// <inheritdoc cref="MessageCallbackHandler.Publish(Message)"/>
    public static void Publish(Message message) => _handler.Value.Publish(message);
}