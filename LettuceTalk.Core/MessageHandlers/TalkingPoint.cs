namespace LettuceTalk.Core.MessageHandlers;

public class SendMessageArgs {
    public readonly Message Message;

    public SendMessageArgs(Message message) {
        Message = message;
    }
}

public abstract class TalkingPoint : MessageCallbackHandler {
    public abstract bool SendMessage(SendMessageArgs args);
}