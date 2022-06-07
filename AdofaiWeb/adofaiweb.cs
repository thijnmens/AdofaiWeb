using HarmonyLib;
using UnityModManagerNet;
using System;
using System.Reflection;
using WebSocketSharp;
using WebSocketSharp.Server;
using ADOFAI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AdofaiWeb
{
    public class Main
    {

        static void completed() { }

        public static bool enabled;
        public static readonly Dictionary<HitMargin, int> lenientCounts = new Dictionary<HitMargin, int>();
        public static readonly Dictionary<HitMargin, int> normalCounts = new Dictionary<HitMargin, int>();
        public static readonly Dictionary<HitMargin, int> strictCounts = new Dictionary<HitMargin, int>();
        public static UnityModManager.ModEntry mod;
        private static WebSocketServer webServer;
        private static LevelData levelData;
        private static System.Timers.Timer timer = new System.Timers.Timer(100);


        private static WebSocketServer InitServer(string IPAdress, int Port)
        {
            var wssv = new WebSocketServer($"ws://{IPAdress}:{Port}");
            wssv.AddWebSocketService<Server>("/server");
            return wssv;
        }

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {

                webServer = InitServer("127.0.0.1", 420);

                var harmony = new Harmony(modEntry.Info.Id);
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                mod = modEntry;
                modEntry.OnToggle = OnToggle;
                modEntry.OnUpdate = OnUpdate;

                SceneManager.sceneLoaded += OnSceneLoad;

                return true; // Successfully Loaded
            }
            catch (Exception e)
            {
                modEntry.Logger.Error(e.ToString());
                return false; // Failed to load
            }
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool active)
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

        static async void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
        {

            string data = "{\"type\": \"none\"}";

            switch (scene.name)
            {
                case "scnEditor":
                    await Task.Delay(250);

                    data = $"" +
                        $"{{" +
                            $"\"type\": \"loadScene\"," +
                            $"\"scene\": \"scnEditor\"," +
                            $"\"data\": {{" +
                                $"\"artist\": \"{levelData.artist}\"," +
                                $"\"specialArtistType\": \"{levelData.specialArtistType}\"," +
                                $"\"artistPermission\": \"{levelData.artistPermission}\"," +
                                $"\"song\": \"{levelData.song}\"," +
                                $"\"author\": \"{levelData.author}\"," +
                                $"\"separateCountdownTime\": \"{levelData.separateCountdownTime}\"," +
                                $"\"previewImage\": \"{levelData.previewImage}\"," +
                                $"\"previewIcon\": \"{levelData.previewIcon}\"," +
                                $"\"previesIconColor\": \"{levelData.previewIconColor}\"," +
                                $"\"previewSongStart\": \"{levelData.previewSongStart}\"," +
                                $"\"previewSongDuration\": \"{levelData.previewSongDuration}\"," +
                                $"\"seizureWarning\": \"{levelData.seizureWarning}\"," +
                                $"\"levelDesc\": \"{levelData.levelDesc}\"," +
                                $"\"levelTags\": \"{levelData.levelTags}\"," +
                                $"\"artistLinks\": \"{levelData.artistLinks}\"," +
                                $"\"difficulty\": \"{levelData.difficulty}\"," +
                                $"\"speedMultiplier\": \"{scnEditor.instance.speedMultiplier}\"" +
                                $"\"path\": \"{scnEditor.instance.levelPath}\"," +
                                $"\"auto\": \"{RDC.auto}\"," +
                                $"\"bpm\": \"{scrConductor.instance.bpm}\"," +
                            $"}}" +
                        $"}}";
                    timer.AutoReset = true;
                    timer.Elapsed += SendUpdate;
                    timer.Start();
                    break;

                case "scnCLS":
                    data = $"" +
                        $"{{" +
                            $"\"type\": \"loadScene\"," +
                            $"\"scene\": \"scnCLS\"," +
                            $"\"data\": {{}}" +
                        $"}}";
                    timer.Stop();
                    break;

                case "scnSplash":
                    data = $"" +
                        $"{{" +
                            $"\"type\": \"loadScene\"," +
                            $"\"scene\": \"scnSplash\"," +
                            $"\"data\": {{}}" +
                        $"}}";
                    timer.Stop();
                    break;
                default:
                    timer.Stop();
                    break;
            }

            webServer.WebSocketServices["/server"].Sessions.BroadcastAsync(data, completed);
        }

        static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {

        }

        static dynamic GetHits(HitMargin hit)
        {
            return scrMistakesManager.hitMarginsCount[(int)hit];
        }

        static void SendUpdate(object sender, EventArgs e)
        {
            string data = $"" +
                    $"{{" +
                        $"\"type\": \"update\"," +
                        $"\"data\": {{" +
                            $"\"paused\": \"{scrController.instance.paused}\"," +
                            $"\"checkpoints\": \"{scrController.checkpointsUsed}\"," +
                            $"\"deaths\": \"{scrController.deaths}\"," +
                            $"\"attempts\": \"{Persistence.GetCustomWorldAttempts(levelData.Hash)}\"," +
                            $"\"speed\": \"{scrController.instance.speed}\"," +
                            $"\"missesOnCurrFloor\": \"{scrController.instance.missesOnCurrFloor}\"," +
                            $"\"percentComplete\": \"{scrController.instance.percentComplete}\"," +
                            $"\"tooEarly\": \"{GetHits(HitMargin.TooEarly)}\"," +
                            $"\"veryEarly\": \"{GetHits(HitMargin.VeryEarly)}\"," +
                            $"\"earlyPerfect\": \"{GetHits(HitMargin.EarlyPerfect)}\"," +
                            $"\"perfect\": \"{GetHits(HitMargin.Perfect)}\"," +
                            $"\"latePerfect\": \"{GetHits(HitMargin.LatePerfect)}\"," +
                            $"\"veryLate\": \"{GetHits(HitMargin.VeryLate)}\"," +
                            $"\"tooLate\": \"{GetHits(HitMargin.TooLate)}\"," +
                        $"}}" +
                    $"}}";

            webServer.WebSocketServices["/server"].Sessions.BroadcastAsync(data, completed);
        }

        static bool OnUnload(UnityModManager.ModEntry modEntry, bool active)
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
                case "connected":
                    Send("Connected Successfully!");
                    break;

                case "ping":
                    Send("Pong");
                    break;

                default:
                    Send("I don't know that command");
                    break;
            }
        }
    }
}
