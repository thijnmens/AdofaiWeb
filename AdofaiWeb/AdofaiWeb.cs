using System;
using UnityModManagerNet;

namespace AdofaiWeb
{
	public class AdofaiWeb
	{

		public static bool Enabled { get; private set; }

		public static bool Setup(UnityModManager.ModEntry modEntry) {
			try {
				modEntry.OnToggle = OnToggle;
				return true;
			}
			catch (Exception e) {
				modEntry.Logger.Critical(e.ToString());
				return false;
			}
		}

		private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value) {
			var success = value ? Run(modEntry) : Stop(modEntry);

			if (success) Enabled = value;
			return success;
		}

		private static bool Run(UnityModManager.ModEntry modEntry) {
			return true;
		}

		private static bool Stop(UnityModManager.ModEntry modEntry) {
			return true;
		}
	}
}