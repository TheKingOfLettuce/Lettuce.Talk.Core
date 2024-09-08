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

public static class MessageFactory {
    private static BiDirectionalDictionary<int, Type> _messageMap = new BiDirectionalDictionary<int, Type>();
    private static IMessageBuilder _messageBuilder = new JsonMessageBuilder();

    public static void AssociateAssembly(Assembly assembly) {
        foreach(Type messageType in assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Message)))) {
            MessageDataAttribute? messageData = messageType.GetCustomAttribute<MessageDataAttribute>();
            if (messageData == null) {
                continue;
            }

            AssociateMessage(messageData.MessageCode, messageType);
        }
    }

    public static bool AssociateMessage(int messageCode, Type messageType) {
        if (!messageType.IsSubclassOf(typeof(Message))) {
            throw new ArgumentException($"Provided message type is not of type {typeof(Message)}", nameof(messageType));
        }

        if (_messageMap.Contains(messageCode)) {
            return false;
        }

        _messageMap.Add(messageCode, messageType);
        return true;
    }

    public static Type GetMessageType(int messageCode) {
        if (!_messageMap.Contains(messageCode)) {
            throw new ArgumentException($"Provided message code {messageCode} does not have an associated type.", nameof(messageCode));
        }

        return _messageMap.Get(messageCode);
    }

    public static int GetMessageCode(Message message) {
        Type messageType = message.GetType();
        if (!_messageMap.Contains(messageType)) {
            throw new ArgumentException($"Provided message type {messageType} does not have an associated message code.", nameof(message));
        }

        return _messageMap.Get(messageType);
    }

    public static void AssignMessageBuilder(IMessageBuilder newBuilder) {
        _messageBuilder = newBuilder;
    }

    public static byte[] GetMessageData(Message message) => _messageBuilder.ToData(message);
    public static Message GetMessage(byte[] protocolData) => _messageBuilder.FromData(protocolData);
    public static Message GetMessage(int messageCode, byte[] messageData) => _messageBuilder.FromData(messageCode, messageData);
}