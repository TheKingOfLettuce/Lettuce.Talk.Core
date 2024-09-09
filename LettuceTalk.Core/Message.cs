namespace LettuceTalk.Core;

/// <summary>
/// Attribute to assoicate meta data with a <see cref="Message"/>
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class MessageDataAttribute : Attribute {
    public readonly int MessageCode;

    public MessageDataAttribute(int messageCode) {
        MessageCode = messageCode;
    }
}

/// <summary>
/// Base class to inherit from for messages within the LettuceTalk protocol
/// </summary>
/// <remarks>Class provides no functionality but is meant to serve as type restrictions</remarks>
public abstract class Message {}