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
using System.Threading.Tasks;

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

                SceneManager.activeSceneChanged += SceneChange;

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

        private static async void SceneChange(Scene scene1, Scene scene2)
        {
            if (scene2.name == "scnEditor")
            {
                await Task.Delay(100);

                string data = $"" +
                    $"{{" +
                        $"\"type\": \"sceneChange\"," +
                        $"\"data\": {{" +
                                $"\"previewImage\": \"{Base64Image((Directory.GetParent(scnEditor.instance.levelPath).ToString() + "\\" + levelData.previewImage).Replace('\\', '/'))}\"," +
                                $"\"extension\": \"{levelData.previewImage.Split('.').AsQueryable().Last()}\"" +
                            $"}}" +
                    $"}}";

                webServer.WebSocketServices["/server"].Sessions.BroadcastAsync(data, completed);
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
                    data = $"" +
                    $"{{" +
                        $"\"type\": \"update\"," +
                        $"\"data\": {{" +
                            $"\"paused\": \"{scrController.instance.paused}\"," +
                            $"\"checkpoints\": {scrController.checkpointsUsed}," +
                            $"\"deaths\": {scrController.deaths}," +
                            $"\"attempts\": {Persistence.GetCustomWorldAttempts(levelData.Hash)}," +
                            $"\"speed\": \"{scrController.instance.speed}\"," +
                            $"\"percentComplete\": {scrController.instance.percentComplete}," +
                            $"\"tooEarly\": \"{GetHits(HitMargin.TooEarly)}\"," +
                            $"\"veryEarly\": \"{GetHits(HitMargin.VeryEarly)}\"," +
                            $"\"earlyPerfect\": \"{GetHits(HitMargin.EarlyPerfect)}\"," +
                            $"\"perfect\": \"{GetHits(HitMargin.Perfect)}\"," +
                            $"\"latePerfect\": \"{GetHits(HitMargin.LatePerfect)}\"," +
                            $"\"veryLate\": \"{GetHits(HitMargin.VeryLate)}\"," +
                            $"\"tooLate\": \"{GetHits(HitMargin.TooLate)}\"" +
                        $"}}," +
                        $"\"levelData\": {{" +
                                $"\"artist\": \"{levelData.artist.Replace("\n", "").Replace("\r", "")}\"," +
                                $"\"specialArtistType\": \"{levelData.specialArtistType}\"," +
                                $"\"artistPermission\": \"{levelData.artistPermission.Replace("\n", "").Replace("\r", "")}\"," +
                                $"\"song\": \"{levelData.song.Replace("\n", "").Replace("\r", "")}\"," +
                                $"\"author\": \"{levelData.author.Replace("\n", "").Replace("\r", "")}\"," +
                                $"\"separateCountdownTime\": \"{levelData.separateCountdownTime}\"," +
                                $"\"previewImage\": \"{(Directory.GetParent(scnEditor.instance.levelPath).ToString() + "\\" + levelData.previewImage).Replace('\\', '/')}\"," +
                                $"\"previewIcon\": \"{levelData.previewIcon}\"," +
                                $"\"previesIconColor\": \"{levelData.previewIconColor}\"," +
                                $"\"previewSongStart\": \"{levelData.previewSongStart}\"," +
                                $"\"previewSongDuration\": \"{levelData.previewSongDuration}\"," +
                                $"\"seizureWarning\": \"{levelData.seizureWarning}\"," +
                                $"\"levelDesc\": \"{levelData.levelDesc.Replace("\n", "").Replace("\r", "")}\"," +
                                $"\"levelTags\": \"{levelData.levelTags.Replace("\n", "").Replace("\r", "")}\"," +
                                $"\"artistLinks\": \"{levelData.artistLinks.Replace("\n", "").Replace("\r", "")}\"," +
                                $"\"difficulty\": {levelData.difficulty}," +
                                $"\"speedMultiplier\": \"{scnEditor.instance.speedMultiplier}\"," +
                                $"\"path\": \"{scnEditor.instance.levelPath.Replace('\\', '/').Replace("\r", "")}\"," +
                                $"\"auto\": \"{RDC.auto}\"," +
                                $"\"bpm\": \"{scrConductor.instance.bpm}\"" +
                            $"}}" +
                    $"}}";
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

        [HarmonyPatch(typeof(LevelData), "LoadLevel")]
        public static class LoadLevel
        {

            [HarmonyPostfix]
            static void PostFix(ref LevelData __instance, ref dynamic __result)
            {
                levelData = __instance;
            }

            [HarmonyFinalizer]
            static Exception Finalizer(Exception __exception)
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
