using System;
using System.IO;
using System.Reflection;
using UnityModManagerNet;

namespace AdofaiWeb
{
    internal static class Startup
    {

        internal static void Load(UnityModManager.ModEntry modEntry)
        {
            LoadAssembly("C:/Program Files (x86)/Steam/steamapps/common/A Dance of Fire and Ice/A Dance of Fire and Ice_Data/Managed/websocket-sharp.dll");

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
