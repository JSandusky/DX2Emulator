using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DX2Emulator
{
    public static class AppUtil
    {
        public static readonly string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        static string exePath_;

        static AppUtil()
        {
            exePath_ = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }

        public static string GetLocalFilePath(string name)
        {
            return System.IO.Path.Combine(exePath_, name);
        }

        public static string GetImageFilePath(string name)
        {
            return GetLocalFilePath("Images/" + name);
        }
    }
}
