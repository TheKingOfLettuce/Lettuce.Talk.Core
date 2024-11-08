namespace LettuceTalk.Core.MessageBuilders;

/// <summary>
/// <para>
/// Handles converting a <see cref="Message"/> to and from byte data according to the LettuceTalk spec
/// </para>
/// <para>
/// The LettuceTalk message spec defines that the byte data is composed of three parts:
/// <list type="number">
/// <item>
/// The first four bytes represent the message header length
/// </item>
/// <item>
/// The next N bytes (where N is the length from the first part) represent the message header as an UTF-8 string
/// </item>
/// <item>
/// The rest of the byte data is the serialized message
/// </item>
/// </list>
/// </para>
/// </summary>
public interface IMessageBuilder {
    /// <summary>
    /// Converts a message from the byte data from the LettuceTalk spec
    /// </summary>
    /// <param name="data">the full byte data following the LettuceTalk spec</param>
    /// <returns>the converted message returned as <see cref="Message"/> but underneath is the derived type</returns>
    Message FromData(byte[] data);

    /// <summary>
    /// Converts a message from a message header and the byte data only representing the message data
    /// </summary>
    /// <param name="messageHeader">the message header that represents the message</param>
    /// <param name="data">the byte data representing the message only</param>
    /// <returns>the converted message returned as <see cref="Message"/> but underneath is the derived type</returns>
    Message FromData(string messageHeader, byte[] data);

    /// <summary>
    /// Converts a <see cref="Message"/> to byte data from the LettuceTalk spec
    /// </summary>
    /// <param name="message">the message to serialize to byte data</param>
    /// <returns>byte data representing the message following the LettuceTalk spec</returns>
    byte[] ToData(Message message);
}