using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdofaiWeb.Messages
{
	public class HitMessage : IMessage<object>
	{

		public HitMessage(HitMargin hit) {
			this.Type = MessageType.Hit;
			this.ModVersion = AdofaiWeb.ModEntry.Version.ToString();
			this.GameVersion = AdofaiWeb.ModEntry.GameVersion.ToString();
			this.Data = hit;
		}

		public MessageType Type { get; }
		public string ModVersion { get; }
		public string GameVersion { get; }
		public object Data { get; }

		public override string ToString() {
			var message = new Dictionary<string, object>
			{
				{ "type", this.Type.ToString() },
				{ "modVersion", this.ModVersion },
				{ "gameVersion", this.GameVersion },
				{
					"data", new JsonData
					{
						Hit = (HitMargin)this.Data
					}
				}
			};

			return JsonConvert.SerializeObject(message, new StringEnumConverter());
		}

		internal class JsonData
		{

			[JsonProperty("hit")] public HitMargin Hit;
		}
	}
}