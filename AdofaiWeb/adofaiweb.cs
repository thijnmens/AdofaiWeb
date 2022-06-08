using HarmonyLib;
using UnityModManagerNet;
using System;
using System.Reflection;
using System.Drawing;
using System.Linq;
using WebSocketSharp;
using WebSocketSharp.Server;
using ADOFAI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

namespace AdofaiWeb
{
    public class Main
    {

        private static void completed() { }

        public static bool enabled;
        public static readonly Dictionary<HitMargin, int> lenientCounts = new Dictionary<HitMargin, int>();
        public static readonly Dictionary<HitMargin, int> normalCounts = new Dictionary<HitMargin, int>();
        public static readonly Dictionary<HitMargin, int> strictCounts = new Dictionary<HitMargin, int>();
        public static UnityModManager.ModEntry mod;
        private static WebSocketServer webServer;
        private static LevelData levelData;
        private static int i = 0;
        private static bool didSendImage = false;


        private static WebSocketServer InitServer(string IPAdress, int Port)
        {
            var wssv = new WebSocketServer($"ws://{IPAdress}:{Port}");
            wssv.AddWebSocketService<Server>("/server");
            return wssv;
        }

        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {

                webServer = InitServer("127.0.0.1", 420);

                var harmony = new Harmony(modEntry.Info.Id);
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                mod = modEntry;
                modEntry.OnToggle = OnToggle;
                modEntry.OnUpdate = OnUpdate;

                return true; // Successfully Loaded
            }
            catch (Exception e)
            {
                modEntry.Logger.Error(e.ToString());
                return false; // Failed to load
            }
        }

        private static string Base64Image(string path)
        {
            using (Image image = Image.FromFile(path))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            }
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool active)
        {
            enabled = active;

            if (active)
            {
                webServer.Start();
            }
            else
            {
                webServer.Stop();
            }

            return true;
        }

        private static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {

            if (i < 10)
            {
                i++;
            }
            else
            {
                i = 0;

                string data = "{\"type\": \"default\"}";

                if (SceneManager.GetActiveScene().name == "scnEditor")
                {
                    string path = scrController.instance.levelPath;

                    if (!didSendImage)
                    {
                        didSendImage = true;

                        data = $"" +
                        $"{{" +
                            $"\"type\": \"levelImage\"," +
                            $"\"data\": {{" +
                                $"\"previewImage\": \"{Base64Image((Directory.GetParent(path).ToString() + "\\" + levelData.previewImage).Replace('\\', '/'))}\"," +
                                $"\"previewImageExtension\": \"{levelData.previewImage.Split('.').AsQueryable().Last()}\"" +
                            $"}}" +
                        $"}}";
                    } else
                    {
                        data = $"" +
                        $"{{" +
                            $"\"type\": \"update\"," +
                            $"\"data\": {{" +
                                $"\"paused\": {scrController.instance.paused},".ToLower() +
                                $"\"checkpoints\": {scrController.checkpointsUsed}," +
                                $"\"deaths\": {scrController.deaths}," +
                                $"\"attempts\": {Persistence.GetCustomWorldAttempts(levelData.Hash)}," +
                                $"\"speed\": \"{scrController.instance.speed}\"," +
                                $"\"percentComplete\": {scrController.instance.percentComplete}," +
                                $"\"tooEarly\": {GetHits(HitMargin.TooEarly)}," +
                                $"\"veryEarly\": {GetHits(HitMargin.VeryEarly)}," +
                                $"\"earlyPerfect\": {GetHits(HitMargin.EarlyPerfect)}," +
                                $"\"perfect\": {GetHits(HitMargin.Perfect)}," +
                                $"\"latePerfect\": {GetHits(HitMargin.LatePerfect)}," +
                                $"\"veryLate\": {GetHits(HitMargin.VeryLate)}," +
                                $"\"tooLate\": {GetHits(HitMargin.TooLate)}" +
                            $"}}" +
                        $"}}";
                    }

                }

                webServer.WebSocketServices["/server"].Sessions.BroadcastAsync(data, completed);
            }
        }

        private static int GetHits(HitMargin hit)
        {
            return scrMistakesManager.hitMarginsCount[(int)hit];
        }

        private static bool OnUnload(UnityModManager.ModEntry modEntry, bool active)
        {
            webServer.Stop();
            return true;
        }

        public static void WaitUntil()
        {

        }

        [HarmonyPatch(typeof(LevelData), "LoadLevel")]
        public static class LoadLevel
        {

            [HarmonyPostfix]
            public static void PostFix(ref LevelData __instance, ref dynamic __result)
            {
                levelData = __instance;
               

                if (SceneManager.GetActiveScene().name == "scnEditor")
                {
                    didSendImage = false;
                    string data = $"" +
                        $"{{" +
                            $"\"type\": \"loadLevel\"," +
                            $"\"data\": {{" +
                                $"\"calibration_i\": {scrConductor.calibration_i}," +
                                $"\"calibration_v\": {scrConductor.calibration_v}," +
                                $"\"beatNumber\": {scrConductor.instance.beatNumber}," +
                                $"\"angleData\": [{string.Join(",", levelData.angleData)}]," +
                                $"\"artist\": \"{levelData.artist.Replace("\n", "").Replace("\r", "")}\"," +
                                $"\"artistLinks\": \"{levelData.artistLinks.Replace("\n", "").Replace("\r", "")}\"," +
                                $"\"artistPermission\": \"{levelData.artistPermission.Replace("\n", "").Replace("\r", "")}\"," +
                                $"\"author\": \"{levelData.author.Replace("\n", "").Replace("\r", "")}\"," +
                                $"\"backgroundColor\": \"{levelData.backgroundColor}\"," +
                                $"\"bgFitScreen\": " + $"{levelData.bgFitScreen},".ToLower() +
                                $"\"bgImage\": \"{levelData.bgImage.Replace("\n", "").Replace("\r", "")}\"," +
                                $"\"bgImageColor\": \"{levelData.bgImageColor}\"," +
                                $"\"bgLockRot\": " + $"{levelData.bgLockRot},".ToLower() +
                                $"\"bgLooping\": " + $"{levelData.bgLooping},".ToLower() +
                                $"\"bgParallax\": \"{levelData.bgParallax}\"," +
                                $"\"bgShowDefaultBGIfNoImage\": "+$"{levelData.bgShowDefaultBGIfNoImage},".ToLower() +
                                $"\"bgTiling\": " + $"{levelData.bgTiling},".ToLower() +
                                $"\"bgVideo\": \"{levelData.bgVideo.Replace("\n", "").Replace("\r", "")}\"," +
                                $"\"bpm\": {levelData.bpm}," +
                                $"\"camPosition\": \"{levelData.camPosition}\"," +
                                $"\"camRelativeTo\": \"{levelData.camRelativeTo}\"," +
                                $"\"camRotation\": {levelData.camRotation}," +
                                $"\"camZoom\": {levelData.camZoom}," +
                                $"\"countdownTicks\": {levelData.countdownTicks}," +
                                $"\"difficulty\": {levelData.difficulty}," +
                                $"\"floorIconOutlines\": " + $"{levelData.floorIconOutlines},".ToLower() +
                                $"\"fullCaption\": \"{levelData.fullCaption.Replace("\n", "").Replace("\r", "")}\"," +
                                $"\"fullCaptionTagged\": \"{levelData.fullCaptionTagged.Replace("\n", "").Replace("\r", "")}\"," +
                                $"\"Hash\": \"{levelData.Hash.Replace("\n", "").Replace("\r", "")}\"," +
                                $"\"hitsound\": \"{levelData.hitsound}\"," +
                                $"\"hitsoundVolume\": {levelData.hitsoundVolume}," +
                                $"\"isOldLevel\": " + $"{levelData.isOldLevel},".ToLower() +
                                $"\"legacyFlash\": " + $"{levelData.legacyFlash},".ToLower() +
                                $"\"levelDesc\": \"{levelData.levelDesc.Replace("\n", "").Replace("\r", "")}\"," +
                                $"\"levelTags\": [\"{string.Join("\", \"", levelData.levelTags.Replace("\n", "").Replace("\r", "").Split(','))}\"]," +
                                $"\"offset\": {levelData.offset}," +
                                $"\"oldCameraFollowStyle\": " + $"{levelData.oldCameraFollowStyle},".ToLower() +
                                $"\"pathData\": \"{levelData.pathData.Replace("\n", "").Replace("\r", "")}\"," +
                                $"\"pitch\": {levelData.pitch}," +
                                $"\"planetEase\": \"{levelData.planetEase}\"," +
                                $"\"planetEaseParts\": {levelData.planetEaseParts}," +
                                $"\"previewIcon\": \"{levelData.previewIcon.Replace("\n", "").Replace("\r", "")}\"," +
                                $"\"previewIconColor\": \"{levelData.previewIconColor}\"," +
                                $"\"previewSongDuration\": {levelData.previewSongDuration}," +
                                $"\"previewSongStart\": {levelData.previewSongStart}," +
                                $"\"requiredDLC\": \"{levelData.requiredDLC}\"," +
                                $"\"secondaryTrackColor\": \"{levelData.secondaryTrackColor}\"," +
                                $"\"seizureWarning\": " + $"{levelData.seizureWarning},".ToLower() +
                                $"\"separateCountdownTime\": " + $"{levelData.separateCountdownTime},".ToLower() +
                                $"\"song\": \"{levelData.song.Replace("\n", "").Replace("\r", "")}\"," +
                                $"\"songFilename\": \"{levelData.songFilename.Replace("\n", "").Replace("\r", "")}\"," +
                                $"\"specialArtistType\": \"{levelData.specialArtistType}\"," +
                                $"\"stickToFloors\": " + $"{levelData.stickToFloors},".ToLower() +
                                $"\"trackAnimation\": \"{levelData.trackAnimation}\"," +
                                $"\"trackBeatsAhead\": {levelData.trackBeatsAhead}," +
                                $"\"trackBeatsBehind\": {levelData.trackBeatsBehind}," +
                                $"\"trackColor\": \"{levelData.trackColor}\"," +
                                $"\"trackColorAnimDuration\": {levelData.trackColorAnimDuration}," +
                                $"\"trackColorPulse\": \"{levelData.trackColorPulse}\"," +
                                $"\"trackColorType\": \"{levelData.trackColorType}\"," +
                                $"\"trackDisappearAnimation\": \"{levelData.trackDisappearAnimation}\"," +
                                $"\"trackPulseLength\": {levelData.trackPulseLength}," +
                                $"\"trackStyle\": \"{levelData.trackStyle}\"," +
                                $"\"unscaledSize\": {levelData.unscaledSize}," +
                                $"\"version\": {levelData.version}," +
                                $"\"volume\": {levelData.volume}" +
                            $"}}" +
                        $"}}";



                    webServer.WebSocketServices["/server"].Sessions.BroadcastAsync(data, completed);
                }
            }

            [HarmonyFinalizer]
            public static Exception Finalizer(Exception __exception)
            {
                if (__exception != null)
                {
                    Console.WriteLine(__exception.ToString());
                }

                return null;
            }
        }
    }

    public class Server: WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            switch(e.Data.ToLower())
            {
                case "connect":
                    Send("{\"status\": \"Connected Successfully!\"}");
                    break;

                case "ping":
                    Send("{\"status\": \"pong\"}");
                    break;

                default:
                    Send("{\"status\": \"I dont know that command\"}");
                    break;
            }
        }
    }
}
