using Sitecore.FakeDb;
using System;
using System.IO;
using System.Linq;

namespace RainbowSerialization.Tests
{
    public static class Dbs
    {
        private static Db _default { get; set; }
        public static Db Default()
        {
            if (_default == null)
            {
                _default = new Db();
                _default.AddYml(true,
                    GetPathTo("Content"),
                    GetPathTo("Templates"));
            }

            return _default;
        }

        public static string GetPathTo(string folder)
        {
            string startupPath = AppDomain.CurrentDomain.BaseDirectory;
            var pathItems = startupPath.Split(Path.DirectorySeparatorChar);
            var pos = pathItems.Reverse().ToList().FindIndex(x => string.Equals("bin", x));
            string projectPath = String.Join(Path.DirectorySeparatorChar.ToString(), pathItems.Take(pathItems.Length - pos - 1));
            return Path.Combine(projectPath, "serialization", folder);
        }
    }
}