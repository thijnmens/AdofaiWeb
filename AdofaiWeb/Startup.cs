using System;
using System.IO;
using UnityModManagerNet;

namespace AdofaiWeb
{
	internal static class Startup
	{

		internal static void Load(UnityModManager.ModEntry modEntry) {
			//LoadAssembly("Mods/AdofaiWebOld/websocket-sharp.dll");

			AdofaiWeb.Setup(modEntry);
		}

		private static void LoadAssembly(string path) {
			using (var stream = new FileStream(path, FileMode.Open)) {
				var data = new byte[stream.Length];
				stream.Read(data, 0, data.Length);
				AppDomain.CurrentDomain.Load(data);
			}
		}
	}
}