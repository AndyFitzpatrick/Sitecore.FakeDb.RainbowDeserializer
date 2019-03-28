using Sitecore.Diagnostics;
using Sitecore.FakeDb.RainbowSerialization;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.FakeDb
{
    public static class DbExtension
    {
        public static void AddYml(this Db db, string filePath, bool preventOrphans = true)
        {
            Assert.IsNotNullOrEmpty(filePath, "Filepath is null or empty");

            List<DbTemplate> templates = AddYmlTemplates(db, filePath, preventOrphans);
            AddYmlItems(db, templates, filePath, preventOrphans);
        }

        public static void AddYml(this Db db, bool preventOrphans = true, params string[] paths)
        {
            if (paths != null && paths.Length > 0)
            {
                List<DbTemplate> templates = new List<DbTemplate>();

                foreach (string path in paths)
                    templates.AddRange(AddYmlTemplates(db, path, preventOrphans));

                foreach (string path in paths)
                    AddYmlItems(db, templates, path, preventOrphans);
            }
        }

        private static void AddYmlItems(this Db db, List<DbTemplate> templates, string filePath, bool preventOrphans = true)
        {
            Assert.IsNotNullOrEmpty(filePath, "Filepath is null or empty");

            List<DbItem> items = YmlFiles.ToDbItems(filePath);

            if (items != null && items.Count > 0)
            {
                IEnumerable<DbItem> layouts = LayoutItems.Get(items, preventOrphans);
                IEnumerable<DbItem> systems = SystemItems.Get(items, preventOrphans);
                IEnumerable<DbItem> medias = MediaItems.Get(items, preventOrphans);
                IEnumerable<DbItem> contents = ContentItems.Get(items, templates, preventOrphans);
                List<DbTemplate> missing = Templates.GetMissing(items, templates, db);
                
                if (missing.Count > 0)
                    db.AddRange(missing);
                if (layouts != null && layouts.Count() > 0)
                    db.AddRange(layouts);
                if (systems != null && systems.Count() > 0)
                    db.AddRange(systems);
                if (medias != null && medias.Count() > 0)
                    db.AddRange(medias);
                if (contents != null && contents.Count() > 0)
                    db.AddRange(contents);
            }
        }

        private static List<DbTemplate> AddYmlTemplates(this Db db, string filePath, bool preventOrphans = true)
        {
            Assert.IsNotNullOrEmpty(filePath, "Filepath is null or empty");
            List<DbTemplate> templates = new List<DbTemplate>();
            List<DbItem> items = YmlFiles.ToDbItems(filePath);

            if (items != null && items.Count > 0)
            {
                templates = Templates.Get(items, preventOrphans);
                db.AddRange(TemplateFolders.Get(items, preventOrphans));
                db.AddRange(templates);
            }

            return templates;
        }

        public static void AddRange(this Db db, IEnumerable<DbItem> items)
        {
            if (items != null && items.Count() > 0)
            {
                foreach (var item in items)
                    db.Add(item);
            }
        }

        public static void AddRange(this Db db, IEnumerable<DbTemplate> templates)
        {
            if (templates != null && templates.Count() > 0)
            {
                foreach (var item in templates)
                    db.Add(item);
            }
        }
    }
}