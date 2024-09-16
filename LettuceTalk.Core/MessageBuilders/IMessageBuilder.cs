namespace LettuceTalk.Core.MessageBuilders;

/// <summary>
/// Handles converting a <see cref="Message"/> to and from byte data according to the LettuceTalk spec
/// The LettuceTalk message spec defines that the byte data is composed of two parts:
/// the first four bytes represent the integer message code and the rest of the data represents the message data
/// </summary>
public interface IMessageBuilder {
    /// <summary>
    /// Converts a message from the byte data from the LettuceTalk spec
    /// </summary>
    /// <param name="data">the full byte data following the LettuceTalk spec</param>
    /// <returns>the converted message returned as <see cref="Message"/> but underneath is the derived type</returns>
    Message FromData(byte[] data);

    /// <summary>
    /// Converts a message from a message code and the byte data only representing the message data
    /// </summary>
    /// <param name="messageCode">the message code that represents the message</param>
    /// <param name="data">the byte data representing the message only</param>
    /// <returns>the converted message returned as <see cref="Message"/> but underneath is the derived type</returns>
    Message FromData(int messageCode, byte[] data);

    /// <summary>
    /// Converts a <see cref="Message"> to byte data from the LettuceTalk spec
    /// </summary>
    /// <param name="message">the message to serialize to byte data</param>
    /// <returns>byte data representing the message following the LettuceTalk spec</returns>
    byte[] ToData(Message message);
}