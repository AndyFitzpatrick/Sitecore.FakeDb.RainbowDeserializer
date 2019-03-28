using Sitecore.FakeDb;
using System;
using System.IO;
using System.Linq;

namespace RainbowSerialization.Tests
{
    public static class Dbs
    {
        public static Db Default()
        {
            var db = new Db();
            db.AddYml(true,
                GetPathTo("Content"),
                GetPathTo("Templates"));

            return db;
        }

        public static Db ContentOnly()
        {
            var db = new Db();
            db.AddYml(true,
                GetPathTo("Content"));

            return db;
        }

        public static Db TemplatesOnly()
        {
            var db = new Db();
            db.AddYml(true,
                GetPathTo(@"Templates\Habitat\Page Types"));

            return db;
        }

        public static Db ContentFile()
        {
            var db = new Db();
            db.AddYml(true,
                GetPathTo(@"Content\Habitat\Home\About Habitat.yml"));

            return db;
        }

        public static Db ContentAndTemplateFiles()
        {
            var db = new Db();
            db.AddYml(true,
                GetPathTo(@"Templates\Habitat\Page Types\Section.yml"),
                GetPathTo(@"Content\Habitat\Home\About Habitat.yml"));

            return db;
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