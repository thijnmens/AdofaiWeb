using AdofaiWeb.Messages;
using HarmonyLib;

namespace AdofaiWeb.Patches
{
	[HarmonyPatch(typeof(scrMistakesManager), "AddHit")]
	public class scrMistakesManagerAddHitPatch
	{
		[HarmonyPostfix]
		public static void Postfix(HitMargin hit) {
			if (!AdofaiWeb.Enabled) return;

			AdofaiWeb.WebsocketHelper.SendMessage(new HitMessage(hit));
		}
	}
}