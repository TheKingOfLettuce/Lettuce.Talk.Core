namespace Lettuce.Talk.Core.MessageHandlers;

/// <summary>
/// Arguments for sending a message
/// </summary>
/// <remarks>Useful in case a specific communication implementation requires specific arguments</remarks>
public class SendMessageArgs {
    public readonly Message Message;

    public SendMessageArgs(Message message) {
        Message = message;
    }
}

/// <summary>
/// Represents something that can send and receive messages
/// </summary>
public abstract class TalkingPoint : MessageCallbackHandler {
    public TalkingPoint() : base() {}

    /// <summary>
    /// Handles sending a message with <see cref="SendMessageArgs"/>
    /// </summary>
    /// <param name="args">the message and arguments to send with</param>
    /// <returns>if it sent the message successfully or not</returns>
    public abstract bool SendMessage(SendMessageArgs args);
}