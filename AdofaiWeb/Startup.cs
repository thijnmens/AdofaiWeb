using System;
using System.IO;
using UnityModManagerNet;

namespace AdofaiWeb
{
    internal static class Startup
    {

        internal static void Load(UnityModManager.ModEntry modEntry)
        {
            //LoadAssembly("Mods/AdofaiWeb/websocket-sharp.dll");

            Main.Setup(modEntry);
        }

        private static void LoadAssembly(string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                AppDomain.CurrentDomain.Load(data);
            }
        }
    }
}
