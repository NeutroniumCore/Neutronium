namespace Neutronium.JavascriptFramework.Vue.Communication
{
    public class MessageEvent
    {
        public string Message { get; }
        public string Channel { get; }

        public MessageEvent(string channel, string message)
        {
            Message = message;
            Channel = channel;
        }
    }
}
