using HarmonyLib;
using UnityModManagerNet;
using System;
using System.Reflection;
using WebSocketSharp;
using WebSocketSharp.Server;
using ADOFAI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

namespace AdofaiWeb
{
    public class Main
    {

        public static bool enabled;
        public static UnityModManager.ModEntry mod;
        private static WebSocketServer webServer;
        private static string JsonLevelData = "{}";
        
        private static WebSocketServer InitServer(string IPAdress, int Port)
        {
            var wssv = new WebSocketServer($"ws://{IPAdress}:{Port}");
            wssv.AddWebSocketService<Server>("/server");
            return wssv;
        }

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            try {

                webServer = InitServer("127.0.0.1", 420);

                var harmony = new Harmony(modEntry.Info.Id);
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                mod = modEntry;
                modEntry.OnToggle = OnToggle;
                modEntry.OnUpdate = OnUpdate;

                SceneManager.sceneLoaded += OnSceneLoad;

                return true; // Successfully Loaded
            }
            catch (Exception e) {
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
            } else
            {
                webServer.Stop();
            }

            return true;
        }

        static async void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
        {

            if (scene.name == "scnEditor")
            {
                void completed() { };

                await Task.Delay(250);

                webServer.WebSocketServices["/server"].Sessions.BroadcastAsync(JsonLevelData, completed);
            }
        }

        static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {

            mod.Logger.Log(SceneManager.GetActiveScene().name);

            int i = 0;
            float max = dt*100000;

            if (i < max)
            {
                i++;
                //webServer.WebSocketServices["/server"].Sessions.BroadcastAsync(JsonLevelData, completed);
            } else
            {
                i = 0;
            }
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
                JsonLevelData = $"{{\"artist\": \"{__instance.artist}\"" +
                    $"\"specialArtistType\": \"{__instance.specialArtistType}\"" +
                    $"\"artistPermission\": \"{__instance.artistPermission}\"" +
                    $"\"song\": \"{__instance.song}\"" +
                    $"\"author\": \"{__instance.author}\"" +
                    $"\"separateCountdownTime\": \"{__instance.separateCountdownTime}\"" +
                    $"\"previewImage\": \"{__instance.previewImage}\"" +
                    $"\"previewIcon\": \"{__instance.previewIcon}\"" +
                    $"\"previesIconColor\": \"{__instance.previewIconColor}\"" +
                    $"\"previewSongStart\": \"{__instance.previewSongStart}\"" +
                    $"\"previewSongDuration\": \"{__instance.previewSongDuration}\"" +
                    $"\"seizureWarning\": \"{__instance.seizureWarning}\"" +
                    $"\"levelDesc\": \"{__instance.levelDesc}\"" +
                    $"\"levelTags\": \"{__instance.levelTags}\"" +
                    $"\"artistLinks\": \"{__instance.artistLinks}\"" +
                    $"\"difficulty\": \"{__instance.difficulty}\"}}";
            }

            [HarmonyFinalizer]
            static Exception Finalizer(Exception __exception)
            {
                if (__exception != null)
                {
                    Console.WriteLine(__exception.Message);
                    Console.WriteLine(__exception.StackTrace);
                    Console.WriteLine("\n---Full Exception---\n");
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
            Send("bruh");
        }
    }
}
