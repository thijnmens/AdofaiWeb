using AdofaiWeb.Messages;
using HarmonyLib;
using UnityEngine.SceneManagement;

namespace AdofaiWeb.Patches
{
    [HarmonyPatch(typeof(SceneManager), "LoadSceneAsync", typeof(string), typeof(LoadSceneParameters))]
    public class SceneManagerLoadSceneAsyncPatch
    {
        [HarmonyPostfix]
        public static void Postfix(string sceneName, LoadSceneParameters parameters)
        {
            if (!AdofaiWeb.Enabled) return;

            AdofaiWeb.SendMessage(new LoadSceneMessage(sceneName));
        }
    }
}