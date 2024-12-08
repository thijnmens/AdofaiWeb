using AdofaiWeb.Messages;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace AdofaiWeb
{
	public class WebsocketHelper : WebSocketBehavior
	{
		protected override void OnMessage(MessageEventArgs e) {
			var msg = e.Data == "BALUS"
				? "I've been balused already..."
				: "I'm not available now.";

			Send(msg);
		}

		public void SendMessage(IMessage<object> msg) {
			Send(msg.ToString());
		}
	}
}