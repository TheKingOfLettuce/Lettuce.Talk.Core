namespace LettuceTalk.Core.MessageBuilders;

public interface IMessageBuilder {
    Message FromData(byte[] data);
    Message FromData(int messageCode, byte[] data);
    byte[] ToData(Message message);
}