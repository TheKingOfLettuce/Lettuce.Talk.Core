namespace LettuceTalk.Core;

/// <summary>
/// Attribute to assoicate meta data with a <see cref="Message"/>
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class MessageDataAttribute : Attribute {
    /// <summary>
    /// Represents an UTF-8 string for the message header
    /// </summary>
    public readonly string MessageHeader;

    /// <summary>
    /// Constructs our attribute, taking in the header for the message
    /// </summary>
    /// <param name="messageHeader">the header to represent this message, if empty string provided it will use the full message type name</param>
    public MessageDataAttribute(string messageHeader = "") {
        MessageHeader = messageHeader;
    }
}

/// <summary>
/// Base class to inherit from for messages within the LettuceTalk protocol
/// </summary>
/// <remarks>Class provides no functionality but is meant to provide some type security</remarks>
public abstract class Message {}