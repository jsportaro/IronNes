using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace IronNes.Tests
{
    public static class GamePakLibrary
    {
        public static byte[] Read(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream($"IronNes.Tests.GamePaks.{name}"))
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);

                return ms.ToArray();
            }
        }
    }
}
