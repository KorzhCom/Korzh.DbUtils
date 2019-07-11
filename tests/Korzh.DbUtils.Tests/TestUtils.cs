using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Korzh.DbUtils.Tests
{
    internal static class TestUtils
    {
        public static Stream GetResourceStream(this Assembly assembly, string resourceFolder, string resourceFileName)
        {

            string[] nameParts = assembly.FullName.Split(',');

            string resourceName = nameParts[0] + "." + resourceFolder + "." + resourceFileName;

            var resources = new List<string>(assembly.GetManifestResourceNames());
            if (resources.Contains(resourceName))
                return assembly.GetManifestResourceStream(resourceName);
            else
                return null;
        }

        public static Stream GetResourceStream(this Type type, string resourceFolder, string resourceFileName)
        {
            var assembly = type.GetTypeInfo().Assembly;
            return assembly.GetResourceStream(resourceFolder, resourceFileName);
        }

        public static Stream GetResourceStream(string resourceFolder, string resourceFileName)
        {
            return typeof(TestUtils).GetResourceStream(resourceFolder, resourceFileName);
        }
    }
}
