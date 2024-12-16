using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdofaiWeb.Messages
{
	public class KeyDownMessage : IMessage<List<int>>
	{

		public KeyDownMessage(List<int> keys) {
			this.Type = MessageType.KeyDown;
			this.ModVersion = AdofaiWeb.ModEntry.Version.ToString();
			this.GameVersion = AdofaiWeb.ModEntry.GameVersion.ToString();
			this.Data = keys;
		}

		public MessageType Type { get; }
		public string ModVersion { get; }
		public string GameVersion { get; }
		public List<int> Data { get; }

		public override string ToString() {
			var message = new Dictionary<string, object>
			{
				{ "type", this.Type.ToString() },
				{ "modVersion", this.ModVersion },
				{ "gameVersion", this.GameVersion },
				{
					"data", new JsonData
					{
						Keys = this.Data
					}
				}
			};

			return JsonConvert.SerializeObject(message, new StringEnumConverter());
		}

		internal class JsonData
		{

			[JsonProperty("keys")] public List<int> Keys;
		}
	}
}