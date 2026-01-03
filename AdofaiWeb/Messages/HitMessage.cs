using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdofaiWeb.Messages
{
    public class HitMessage : IMessage<object>
    {
        public HitMessage(HitMargin hit)
        {
            Type = MessageType.Hit;
            ModVersion = AdofaiWeb.ModEntry.Version.ToString();
            GameVersion = AdofaiWeb.ModEntry.GameVersion.ToString();
            Data = hit;
        }

        public MessageType Type { get; }
        public string ModVersion { get; }
        public string GameVersion { get; }
        public object Data { get; }

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
                        Hit = (HitMargin)Data
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