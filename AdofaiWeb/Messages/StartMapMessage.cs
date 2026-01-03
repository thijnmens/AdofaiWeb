using System.Collections.Generic;
using ADOFAI;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdofaiWeb.Messages
{
    public class StartMapMessage : IMessage<LevelData>
    {
        public StartMapMessage(LevelData data, string levelPath)
        {
            Type = MessageType.StartMap;
            ModVersion = AdofaiWeb.ModEntry.Version.ToString();
            GameVersion = AdofaiWeb.ModEntry.GameVersion.ToString();
            Data = data;
            LevelPath = levelPath;
        }

        public string LevelPath { get; }

        public MessageType Type { get; }
        public string ModVersion { get; }
        public string GameVersion { get; }
        public LevelData Data { get; }

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
                        LevelPath = LevelPath,
                        LevelName = Data.song,
                        LevelArtist = Data.artist,
                        LevelAuthor = Data.author,
                        Bpm = Data.bpm,
                        Difficulty = Data.difficulty,
                        BackgroundColor = Data.bgImageColor.ToString(),
                        BackgroundImage = Data.bgImage,
                        CountdownTicks = Data.countdownTicks,
                        LevelDescription = Data.levelDesc,
                        LevelTags = Data.levelTags,
                        SongFilename = Data.songFilename,
                        SeizureWarning = Data.seizureWarning,
                        PreviewImage = Data.previewImage
                    }
                }
            };

            return JsonConvert.SerializeObject(message, new StringEnumConverter());
        }

        internal class JsonData
        {
            [JsonProperty("backgroundColor")] public string BackgroundColor;

            [JsonProperty("backgroundImage")] public string BackgroundImage;

            [JsonProperty("bpm")] public float Bpm;

            [JsonProperty("countdownTicks")] public int CountdownTicks;

            [JsonProperty("difficulty")] public int Difficulty;

            [JsonProperty("levelArtist")] public string LevelArtist;

            [JsonProperty("levelAuthor")] public string LevelAuthor;

            [JsonProperty("levelDescription")] public string LevelDescription;

            [JsonProperty("levelName")] public string LevelName;

            [JsonProperty("levelPath")] public string LevelPath;

            [JsonProperty("levelTags")] public string LevelTags;

            [JsonProperty("previewImage")] public string PreviewImage;

            [JsonProperty("seizureWarning")] public bool SeizureWarning;

            [JsonProperty("songFilename")] public string SongFilename;
        }
    }
}