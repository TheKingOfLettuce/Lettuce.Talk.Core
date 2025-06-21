# Lettuce.Talk.Core
*Let Us Talk!*<br>
A minimalistic messaging protocol to communicate messages internally or a framework to communicate  via inter-process communication (IPC).

## Quick Start
`Lettuce.Talk.Core` is available on [nuget](https://www.nuget.org/packages/Lettuce.Talk.Core)

### Internal Communication
The first thing you need to do is create a message by inheriting from `Message`:

```csharp
public class EchoMessage : Message {
    public readonly string EchoString;

    public EchoMessage(string echoString) {
        EchoString = echoString;
    }
}
```

Now we need to create a handler method and subscribe to our message via `GlobalMessageHandler`:

```csharp
public static class Program {
    public static void Main() {
        GlobalMessageHandler.Subscribe<EchoMessage>(HandleEchoMessage);
    }

    public static void HandleEchoMessage(EchoMessage message) {
        Console.Writeline($"Received echo message with string: {message.EchoString}");
    }
}
```

And then something to publish a message via `GlobalMessageHandler`:

```csharp
public static class Publisher {
    public static void PublishEchoMessage() {
        EchoMessage message = new EchoMessage("Hello World");
        GlobalMessageHandler.Publish(message);
    }
}