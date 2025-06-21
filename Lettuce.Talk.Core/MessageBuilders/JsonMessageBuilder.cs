namespace Lettuce.Talk.Core.MessageBuilders;

/// <summary>
/// A <see cref="IMessageBuilder"/> that serializes and deserializes <see cref="Message"/>
/// by converting to JSON string and then to byte data following the LettuceTalk protocol
/// </summary>
public class JsonMessageBuilder : IMessageBuilder {

    /// <inheritdoc/>
    public Message FromData(byte[] data) {
        int headerLength = BitConverter.ToInt32(data, 0);
        if (headerLength == 0) {
            throw new Exception("Received 0 header length in message data");
        }
        string messageHeader = System.Text.Encoding.UTF8.GetString(data, 4, headerLength);
        if (messageHeader == string.Empty) {
            throw new Exception("Received empty header string in message data");
        }
        int jsonDataLength = data.Length-headerLength-4;
        byte[] jsonData = new byte[jsonDataLength];
        Buffer.BlockCopy(data, 4+headerLength, jsonData, 0, jsonDataLength);
        
        return FromData(messageHeader, jsonData);
    }

    /// <inheritdoc/>
    public Message FromData(string messageHeader, byte[] data) {
        Type messageType = MessageFactory.GetMessageType(messageHeader);
        string jsonString = System.Text.Encoding.UTF8.GetString(data);
        Message? message = (Message?)System.Text.Json.JsonSerializer.Deserialize(jsonString, messageType);
        if (message == null) {
            throw new Exception("Message failed to deserialize from JSON");
        }

        return message;
    }

    /// <inheritdoc/>
    public byte[] ToData(Message message) {
        string jsonString = System.Text.Json.JsonSerializer.Serialize(message, message.GetType());
        byte[] jsonData = System.Text.Encoding.UTF8.GetBytes(jsonString);
        byte[] messageHeader = System.Text.Encoding.UTF8.GetBytes(MessageFactory.GetMessageHeader(message));
        byte[] messageHeaderLength = BitConverter.GetBytes(messageHeader.Length);

        byte[] messageData = new byte[jsonData.Length+4+messageHeader.Length];
        // follow 3 part data spec
        Buffer.BlockCopy(messageHeaderLength, 0, messageData, 0, 4);
        Buffer.BlockCopy(messageHeader, 0, messageData, 4, messageHeader.Length);
        Buffer.BlockCopy(jsonData, 0, messageData, 4+messageHeader.Length, jsonData.Length);

        return messageData;
    }
}