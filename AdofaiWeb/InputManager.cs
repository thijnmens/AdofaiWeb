using System.Linq;
using AdofaiWeb.Messages;
using UnityEngine;
using UnityModManagerNet;

namespace AdofaiWeb
{
	public static class InputManager
	{
		public static void OnLateUpdate(UnityModManager.ModEntry modEntry, float dt) {
			if (RDInput.inputs == null || !AdofaiWeb.Enabled) return;

			var downKeys = RDInput.GetStateKeys();
			var upKeys = RDInput.GetStateKeys(ButtonState.WentUp);

			foreach (var anyKeyCode in downKeys) AdofaiWeb.ModEntry.Logger.Log(anyKeyCode.value.GetType().Name);

			if (downKeys.Count > 0)
				AdofaiWeb.WebsocketHelper.SendMessage(
					new KeyDownMessage(downKeys.Select(key => {
						if (key.value.GetType() == typeof(KeyCode)) return (int)(KeyCode)key.value;
						return ((AsyncKeyCode)key.value).key;
					}).ToList())
				);

			if (upKeys.Count > 0)
				AdofaiWeb.WebsocketHelper.SendMessage(
					new KeyUpMessage(upKeys.Select(key => {
						if (key.value.GetType() == typeof(KeyCode)) return (int)(KeyCode)key.value;
						return ((AsyncKeyCode)key.value).key;
					}).ToList())
				);
		}
	}
}