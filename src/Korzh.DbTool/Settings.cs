using System;
using System.IO;

namespace Korzh.DbTool
{
    public static class Settings
    {

        static Settings()
        {
            string homePath = Environment.OSVersion.Platform == PlatformID.Win32NT
                                ? Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%")
                                : Environment.GetEnvironmentVariable("HOME");

            GlobalConfigFilePath = Path.Combine(homePath, ".korzh", "dbtool.config");
        }


        public static readonly string GlobalConfigFilePath;
    }
}
