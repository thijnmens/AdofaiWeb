using System.Linq;
using AdofaiWeb.Messages;
using UnityEngine;
using UnityModManagerNet;

namespace AdofaiWeb
{
	public static class InputManager
	{
		public static void OnUpdate(UnityModManager.ModEntry modEntry, float dt) {
			if (RDInput.inputs == null || !AdofaiWeb.Enabled) return;

			var downKeys = RDInput.GetStateKeys();
			var upKeys = RDInput.GetStateKeys(ButtonState.WentUp);

			if (downKeys.Count > 0)
				AdofaiWeb.WebsocketHelper.SendMessage(
					new KeyDownMessage(downKeys.Select(key => (KeyCode)key.value).ToList())
				);

			if (upKeys.Count > 0)
				AdofaiWeb.WebsocketHelper.SendMessage(
					new KeyUpMessage(upKeys.Select(key => (KeyCode)key.value).ToList())
				);
		}
	}
}