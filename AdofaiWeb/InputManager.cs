using System;
using System.Linq;
using AdofaiWeb.Messages;
using UnityEngine;
using UnityModManagerNet;

namespace AdofaiWeb
{
	public static class InputManager
	{
		public static void OnLateUpdate(UnityModManager.ModEntry modEntry, float dt) {
			if (!AdofaiWeb.Enabled) return;

			try {
				var downKeys = RDInput.GetStateKeys();
				var upKeys = RDInput.GetStateKeys(ButtonState.WentUp);

				if (downKeys.Count > 0)
					AdofaiWeb.SendMessage(
						new KeyDownMessage(downKeys.Select(key => {
							if (key.value.GetType() == typeof(KeyCode)) return (int)(KeyCode)key.value;
							return ((AsyncKeyCode)key.value).key;
						}).ToList())
					);

				if (upKeys.Count > 0)
					AdofaiWeb.SendMessage(
						new KeyUpMessage(upKeys.Select(key => {
							if (key.value.GetType() == typeof(KeyCode)) return (int)(KeyCode)key.value;
							return ((AsyncKeyCode)key.value).key;
						}).ToList())
					);
			}
			catch (NullReferenceException ignored) {
			}

		}
	}
}