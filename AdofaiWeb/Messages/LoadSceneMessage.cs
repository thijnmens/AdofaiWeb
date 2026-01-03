using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine.SceneManagement;

namespace AdofaiWeb.Messages
{
    public class LoadSceneMessage : IMessage<SceneManager>
    {
        public LoadSceneMessage(string scene)
        {
            Type = MessageType.LoadScene;
            ModVersion = AdofaiWeb.ModEntry.Version.ToString();
            GameVersion = AdofaiWeb.ModEntry.GameVersion.ToString();
            Data = null;
            Scene = scene;
        }

        public string Scene { get; }

        public MessageType Type { get; }
        public string ModVersion { get; }
        public string GameVersion { get; }
        public SceneManager Data { get; }

        public override string ToString()
        {
            var message = new Dictionary<string, object>
            {
                { "type", Type.ToString() },
                { "modVersion", ModVersion },
                { "gameVersion", GameVersion },
                {
                    "data", new JsonData
                    {
                        Scene = Scene
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