using ADOFAI;
using AdofaiWeb.Messages;
using HarmonyLib;

namespace AdofaiWeb.Patches
{
    [HarmonyPatch(typeof(LevelData), "LoadLevel")]
    public class LevelDataLoadLevelPatch
    {
        [HarmonyPostfix]
        public static void Postfix(string levelPath, LevelData __instance)
        {
            if (!AdofaiWeb.Enabled) return;

            AdofaiWeb.SendMessage(new StartMapMessage(__instance, levelPath));
        }
    }
}