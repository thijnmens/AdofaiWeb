using System.Collections.Generic;
using ADOFAI;
using Newtonsoft.Json;

namespace AdofaiWeb.Messages
{
	public class StartMapMessage : IMessage<LevelData>
	{

		public StartMapMessage(LevelData data, string levelPath) {
			this.Type = MessageType.StartMap;
			this.ModVersion = AdofaiWeb.ModEntry.Version.ToString();
			this.GameVersion = AdofaiWeb.ModEntry.GameVersion.ToString();
			this.Data = data;
			this.LevelPath = levelPath;
		}

		public string LevelPath { get; }

		public MessageType Type { get; }
		public string ModVersion { get; }
		public string GameVersion { get; }
		public LevelData Data { get; }

		public override string ToString() {
			var message = new Dictionary<string, object>
			{
				{ "type", this.Type.ToString() },
				{ "modVersion", this.ModVersion },
				{ "gameVersion", this.GameVersion },
				{
					"data", new JsonData
					{
						LevelPath = this.LevelPath,
						LevelName = this.Data.song,
						LevelArtist = this.Data.artist,
						LevelAuthor = this.Data.author,
						Bpm = this.Data.bpm,
						Difficulty = this.Data.difficulty,
						BackgroundColor = this.Data.bgImageColor.ToString(),
						BackgroundImage = this.Data.bgImage,
						CountdownTicks = this.Data.countdownTicks,
						LevelDescription = this.Data.levelDesc,
						LevelTags = this.Data.levelTags,
						SongFilename = this.Data.songFilename,
						SeizureWarning = this.Data.seizureWarning,
						PreviewImage = this.Data.previewImage
					}
				}
			};

			return JsonConvert.SerializeObject(message);
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