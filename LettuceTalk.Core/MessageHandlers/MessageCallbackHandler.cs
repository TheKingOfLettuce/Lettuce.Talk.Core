namespace LettuceTalk.Core.MessageHandlers;

#pragma warning disable CS8602 // null reference, but we can gurantee the callbacks are not null

/// <summary>
/// A callback handler that can handle a pub/sub system for <see cref="Message"/> types
/// </summary>
public class MessageCallbackHandler {
    private Dictionary<Type, CallbackHandlerBase> _messageCallbacks;

    public MessageCallbackHandler() {
        _messageCallbacks = new Dictionary<Type, CallbackHandlerBase>();
    }

    /// <summary>
    /// Subscribes a method from a given <see cref="Message"/>
    /// </summary>
    /// <param name="func">the method to callback to</param>
    /// <typeparam name="T">the message to fire on</typeparam>
    public void Subscribe<T>(Action<T> func) where T : Message {
        Type messageType = typeof(T);
        if (!_messageCallbacks.ContainsKey(messageType)) {
            _messageCallbacks.Add(messageType, new CallbackHandler<T>());
        }

        (_messageCallbacks[messageType] as CallbackHandler<T>).AddCallback(func);
    }

    /// <summary>
    /// Unsubscribes a method from a given <see cref="Message"/>
    /// </summary>
    /// <param name="func">the method to remove on</param>
    /// <typeparam name="T">the message to fire on</typeparam>
    public void Unsubscribe<T>(Action<T> func) where T : Message {
        Type messageType = typeof(T);
        if (!_messageCallbacks.ContainsKey(messageType)) return;

        (_messageCallbacks[messageType] as CallbackHandler<T>).RemoveCallback(func);
    }

    /// <summary>
    /// Check to see if we have any subscribers to a message type
    /// </summary>
    /// <typeparam name="T">the message type to check</typeparam>
    /// <returns>if we have any subscribers to a message type</returns>
    public bool HasSubscribers<T>() where T : Message {
        Type messageType = typeof(T);
        if (!_messageCallbacks.ContainsKey(messageType)) return false;

        return (_messageCallbacks[messageType] as CallbackHandler<T>).HasCallbacks();
    }

    /// <summary>
    /// Publishes a <see cref="Message"/> and fires any callbacks that are subscribed
    /// </summary>
    /// <param name="message">the message to publish</param>
    public void Publish(Message message) {
        Type messageType = message.GetType();
        if (!_messageCallbacks.ContainsKey(messageType)) return;

        _messageCallbacks[messageType].HandleMessage(message);
    }
}

#pragma warning restore CS8602