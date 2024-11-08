using System.Reflection;
using LettuceTalk.Core.MessageBuilders;

namespace LettuceTalk.Core;

internal class BiDirectionalDictionary<Key, Value> where Key : notnull where Value : notnull {
    private Dictionary<Key, Value> _keyValuePairs;
    private Dictionary<Value, Key> _valueKeyPairs;

    public BiDirectionalDictionary() {
        _keyValuePairs = new Dictionary<Key, Value>();
        _valueKeyPairs = new Dictionary<Value, Key>();
    }

    public Value Get(Key key) {
        if (!Contains(key)) {
            throw new ArgumentException($"Key {key} does not exist in the map", nameof(key));
        }

        return _keyValuePairs[key];
    }

    public Key Get(Value value) {
        if (!Contains(value)) {
            throw new ArgumentException($"Value {value} does not exist in the map", nameof(value));
        }

        return _valueKeyPairs[value];
    }

    public void Add(Key key, Value value) {
        if (_keyValuePairs.ContainsKey(key)) {
            throw new ArgumentException("Key already exists in pair", nameof(key));
        }
        if (_valueKeyPairs.ContainsKey(value)) {
            throw new ArgumentException("Value already exists in pair", nameof(value));
        }

        _keyValuePairs.Add(key, value);
        _valueKeyPairs.Add(value, key);
    }

    public Value Remove(Key key) {
        if (!_keyValuePairs.ContainsKey(key)) {
            throw new ArgumentException("Key does not exist in pair", nameof(key));
        }

        Value toRemove = _keyValuePairs[key];
        _ = _keyValuePairs.Remove(key);
        _ = _valueKeyPairs.Remove(toRemove);
        
        return toRemove;
    }

    public Key Remove(Value value) {
        if (!_valueKeyPairs.ContainsKey(value)) {
            throw new ArgumentException("Value does not exist in pair", nameof(value));
        }

        Key toRemove = _valueKeyPairs[value];
        _ = _valueKeyPairs.Remove(value);
        _ = _keyValuePairs.Remove(toRemove);
        
        return toRemove;
    }

    public bool Contains(Key key) {
        return _keyValuePairs.ContainsKey(key);
    }

    public bool Contains(Value value) {
        return _valueKeyPairs.ContainsKey(value);
    }
}

/// <summary>
/// The MessageFactory to use in the LettuceTalk Protocol
/// Associates message headers to message types and handles the serialization/deserialization via <see cref="IMessageBuilder"/>
/// </summary>
public static class MessageFactory {
    private static BiDirectionalDictionary<string, Type> _messageMap = new BiDirectionalDictionary<string, Type>();
    private static IMessageBuilder _messageBuilder = new JsonMessageBuilder();

    /// <summary>
    /// Takes an assembly and reads all the derives types of <see cref="Message"/> and associates them with the data in <see cref="MessageDataAttribute"/>
    /// </summary>
    /// <param name="assembly">the assembly to search for <see cref="Message"/></param>
    public static void AssociateAssembly(Assembly assembly) {
        foreach(Type messageType in assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Message)))) {
            MessageDataAttribute? messageData = messageType.GetCustomAttribute<MessageDataAttribute>();
            if (messageData == null) {
                continue;
            }

            if (messageData.MessageHeader != string.Empty)
                _ = AssociateMessage(messageData.MessageHeader, messageType);
            else if (messageType.FullName != null)
                _ = AssociateMessage(messageType.FullName, messageType);
            else
                throw new Exception($"Failed to add message type {messageType}, no header provided & could not fetch full name");
        }
    }

    /// <summary>
    /// Manually associates a given message type with a message header
    /// </summary>
    /// <param name="messageHeader">the message header to associate the type with</param>
    /// <param name="messageType">the type of message to be associated</param>
    /// <returns>if it was able to associate the message type without collision</returns>
    /// <exception cref="ArgumentException">throws if the given type is not derived from <see cref="Message"/></exception>
    public static bool AssociateMessage(string messageHeader, Type messageType) {
        if (!messageType.IsSubclassOf(typeof(Message))) {
            throw new ArgumentException($"Provided message type is not of type {typeof(Message)}", nameof(messageType));
        }

        if (_messageMap.Contains(messageHeader)) {
            return false;
        }

        _messageMap.Add(messageHeader, messageType);
        return true;
    }

    /// <summary>
    /// Gets a message type from a message header
    /// </summary>
    /// <param name="messageHeader">the message header to find a type with</param>
    /// <returns>the associated message type</returns>
    /// <exception cref="ArgumentException">if the given message header is not associated</exception>
    public static Type GetMessageType(string messageHeader) {
        if (!_messageMap.Contains(messageHeader)) {
            throw new ArgumentException($"Provided message header {messageHeader} does not have an associated type.", nameof(messageHeader));
        }

        return _messageMap.Get(messageHeader);
    }

    /// <summary>
    /// Gets a message header from a message instance
    /// </summary>
    /// <param name="message">the message to find the header with</param>
    /// <returns>the associated message header</returns>
    /// <seealso cref="GetMessageHeader(Type)"/>
    public static string GetMessageHeader(Message message) => GetMessageHeader(message.GetType());

    /// <summary>
    /// Gets a message header from a message type
    /// </summary>
    /// <param name="messageType">the message type to find a header with</param>
    /// <returns>the associated message header</returns>
    /// <exception cref="ArgumentException">if the given message type is not associated</exception>
    public static string GetMessageHeader(Type messageType) {
        if (!_messageMap.Contains(messageType)) {
            throw new ArgumentException($"Provided message type {messageType} does not have an associated message header.", nameof(messageType));
        }

        return _messageMap.Get(messageType);
    }

    /// <summary>
    /// Assigns a new message builder to for how to serialize/deserialize <see cref="Message"/>
    /// </summary>
    /// <param name="newBuilder">the new builder to assign</param>
    /// <exception cref="ArgumentNullException">if the provided message builder is null</exception>
    public static void AssignMessageBuilder(IMessageBuilder newBuilder) {
        _messageBuilder = newBuilder ?? throw new ArgumentNullException(nameof(newBuilder), $"Provided message builder is null");
    }

    /// <summary>
    /// Serializes the <see cref="Message"/> into byte data via the LettuceTalk protocol
    /// </summary>
    /// <param name="message">the message to serialize</param>
    /// <returns>the byte data of the message including the message header</returns>
    public static byte[] GetMessageData(Message message) => _messageBuilder.ToData(message);

    /// <summary>
    /// De-Serializes byte data via the LettuceTalk protocol into the derived <see cref="Message"/>
    /// </summary>
    /// <param name="protocolData">the full message protocol data</param>
    /// <returns>the derived <see cref="Message"/></returns>
    public static Message GetMessage(byte[] protocolData) => _messageBuilder.FromData(protocolData);

    /// <summary>
    /// De-Serializes byte data with a given message header into the derived <see cref="Message"/>
    /// </summary>
    /// <param name="messageHeader">the known message header to de-serialize with</param>
    /// <param name="messageData">the message data, not including the message header</param>
    /// <returns>the derived <see cref="Message"/></returns>
    public static Message GetMessage(string messageHeader, byte[] messageData) => _messageBuilder.FromData(messageHeader, messageData);
}