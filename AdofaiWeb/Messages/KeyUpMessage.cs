using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace AdofaiWeb.Messages
{
	public class KeyUpMessage : IMessage<List<KeyCode>>
	{

		public KeyUpMessage(List<KeyCode> keys) {
			this.Type = MessageType.KeyUp;
			this.ModVersion = AdofaiWeb.ModEntry.Version.ToString();
			this.GameVersion = AdofaiWeb.ModEntry.GameVersion.ToString();
			this.Data = keys;
		}

		public MessageType Type { get; }
		public string ModVersion { get; }
		public string GameVersion { get; }
		public List<KeyCode> Data { get; }

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

			return JsonConvert.SerializeObject(message);
		}

		internal class JsonData
		{

			[JsonProperty("keys")] public List<KeyCode> Keys;
		}
	}
}