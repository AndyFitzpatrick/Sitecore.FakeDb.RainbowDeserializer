using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.FakeDb.RainbowDeserializer;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.FakeDb
{
    public static class DbExtension
    {
        /// <summary>
        /// Add multiple DbItems
        /// </summary>
        /// <param name="db"></param>
        /// <param name="items"></param>
        public static void AddRange(this Db db, IEnumerable<DbItem> items)
        {
            if (items != null && items.Count() > 0)
            {
                foreach (var item in items)
                    db.Add(item);
            }
        }

        /// <summary>
        /// Adds multiple DbItems with the option to merge field values into existing DbItems
        /// </summary>
        /// <param name="db"></param>
        /// <param name="items"></param>
        /// <param name="merge"></param>
        public static void AddRange(this Db db, IEnumerable<DbItem> items, bool merge)
        {
            if (items != null && items.Count() > 0)
            {
                if (merge)
                    db.AddRange(items);
                else
                {
                    foreach (DbItem item in items)
                    {
                        var existing = db.GetItem(item.ID);

                        if (existing == null)
                            db.Add(item);
                        else
                        {
                            foreach (DbField field in item.Fields)
                            {
                                if (existing.Fields.Any(f => f.Name == field.Name))
                                {
                                    using (new EditContext(existing))
                                        existing[field.Name] = field.Value;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Add multiple DbTemplates
        /// </summary>
        /// <param name="db"></param>
        /// <param name="templates"></param>
        public static void AddRange(this Db db, IEnumerable<DbTemplate> templates)
        {
            if (templates != null && templates.Count() > 0)
            {
                foreach (var item in templates)
                    db.Add(item);
            }
        }

        /// <summary>
        /// Adds multiple DbTemplates with the option to merge fields into existing DbTemplates
        /// </summary>
        /// <param name="db"></param>
        /// <param name="templates"></param>
        /// <param name="merge"></param>
        public static void AddRange(this Db db, IEnumerable<DbTemplate> templates, bool merge)
        {
            if (templates != null && templates.Count() > 0)
            {
                if (merge)
                    db.AddRange(templates);
                else
                {
                    foreach (DbItem item in templates)
                    {
                        var existing = db.GetItem(item.ID);

                        if (existing == null)
                            db.Add(item);
                        else
                        {
                            //Merge in fields
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Deserializes the provided Rainbow (.yml) files into FakeDb items and templates
        /// </summary>
        /// <param name="db"></param>
        /// <param name="merge">Whether items should be merged with existing FakeDb data</param>
        /// <param name="paths">Paths to Rainbow files/directories</param>
        public static void AddYml(this Db db, bool merge = false, params string[] paths)
        {
            if (paths != null && paths.Length > 0)
            {
                List<DbItem> items = new List<DbItem>();
                List<DbTemplate> templates = new List<DbTemplate>();

                foreach (string path in paths)
                    items.AddRange(YmlFiles.ToDbItems(path));

                templates = AddYmlTemplates(db, items, merge);
                AddYmlItems(db, items, templates, merge);
            }
        }

        private static void AddYmlItems(this Db db, List<DbItem> items, List<DbTemplate> templates, bool merge = false)
        {
            if (items != null && items.Count > 0)
            {
                db.AddRange(LayoutItems.Get(items), merge);
                db.AddRange(SystemItems.Get(items), merge);
                db.AddRange(MediaItems.Get(items), merge);
                db.AddRange(ContentItems.Get(items, templates), merge);
            }
        }

        private static List<DbTemplate> AddYmlTemplates(this Db db, List<DbItem> items, bool merge = false)
        {
            List<DbTemplate> templates = new List<DbTemplate>();

            if (items != null && items.Count > 0)
            {
                templates = Templates.Get(items);
                db.AddRange(TemplateFolders.Get(items), merge);
                db.AddRange(templates, merge);
            }

            return templates;
        }
    }
}