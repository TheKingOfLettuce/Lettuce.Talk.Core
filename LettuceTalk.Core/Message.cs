namespace LettuceTalk.Core;

public class MessageDataAttribute : System.Attribute {
    public readonly int MessageCode;

    public MessageDataAttribute(int messageCode) {
        MessageCode = messageCode;
    }
}

public abstract class Message {}