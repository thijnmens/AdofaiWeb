using System;
using System.Reflection;
using HarmonyLib;
using UnityModManagerNet;
using WebSocketSharp.Server;

namespace AdofaiWeb
{
	public static class AdofaiWeb
	{

		public static bool Enabled { get; private set; }
		public static UnityModManager.ModEntry ModEntry { get; private set; }
		public static WebSocketServer WebSocketServer { get; private set; }
		public static WebsocketHelper WebsocketHelper { get; private set; }

		private static Harmony Harmony { get; set; }

		public static bool Setup(UnityModManager.ModEntry modEntry) {
			try {
				ModEntry = modEntry;
				modEntry.OnToggle = OnToggle;
				modEntry.OnUpdate = OnUpdate;
				Harmony = new Harmony(modEntry.Info.Id);
				return true;
			}
			catch (Exception e) {
				modEntry.Logger.Critical(e.ToString());
				return false;
			}
		}

		private static void OnUpdate(UnityModManager.ModEntry modEntry, float dt) {
			InputManager.OnUpdate(modEntry, dt);
		}

		private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value) {
			var success = value ? Run(modEntry) : Stop(modEntry);

			if (success) Enabled = value;
			return success;
		}

		private static bool Run(UnityModManager.ModEntry modEntry) {
			Harmony.PatchAll(Assembly.GetExecutingAssembly());

			WebsocketHelper = new WebsocketHelper();

			WebSocketServer = new WebSocketServer("ws://localhost:4444");
			WebSocketServer.AddWebSocketService("/", () => WebsocketHelper);
			WebSocketServer.Start();
			return true;
		}

		private static bool Stop(UnityModManager.ModEntry modEntry) {
			Harmony.UnpatchAll();
			WebSocketServer.Stop();
			return true;
		}
	}
}