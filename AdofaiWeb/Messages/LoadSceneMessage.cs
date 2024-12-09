using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine.SceneManagement;

namespace AdofaiWeb.Messages
{
	public class LoadSceneMessage : IMessage<SceneManager>
	{

		public LoadSceneMessage(string scene) {
			this.Type = MessageType.LoadScene;
			this.ModVersion = AdofaiWeb.ModEntry.Version.ToString();
			this.GameVersion = AdofaiWeb.ModEntry.GameVersion.ToString();
			this.Data = null;
			this.Scene = scene;
		}

		public string Scene { get; }

		public MessageType Type { get; }
		public string ModVersion { get; }
		public string GameVersion { get; }
		public SceneManager Data { get; }

		public override string ToString() {
			var message = new Dictionary<string, object>
			{
				{ "type", this.Type.ToString() },
				{ "modVersion", this.ModVersion },
				{ "gameVersion", this.GameVersion },
				{
					"data", new JsonData
					{
						Scene = this.Scene
					}
				}
			};

			return JsonConvert.SerializeObject(message, new StringEnumConverter());
		}

		internal class JsonData
		{

			[JsonProperty("scene")] public string Scene;
		}
	}
}