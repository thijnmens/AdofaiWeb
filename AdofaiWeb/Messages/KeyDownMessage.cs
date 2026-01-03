using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdofaiWeb.Messages
{
    public class KeyDownMessage : IMessage<List<int>>
    {
        public KeyDownMessage(List<int> keys)
        {
            Type = MessageType.KeyDown;
            ModVersion = AdofaiWeb.ModEntry.Version.ToString();
            GameVersion = AdofaiWeb.ModEntry.GameVersion.ToString();
            Data = keys;
        }

        public MessageType Type { get; }
        public string ModVersion { get; }
        public string GameVersion { get; }
        public List<int> Data { get; }

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
                        Keys = Data
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