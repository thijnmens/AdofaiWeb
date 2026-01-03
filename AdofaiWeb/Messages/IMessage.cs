namespace AdofaiWeb.Messages
{
    public interface IMessage<out T>
    {
        MessageType Type { get; }
        string ModVersion { get; }
        string GameVersion { get; }
        T Data { get; }
    }
}