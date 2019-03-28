using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.FakeDb.RainbowSerialization
{
    public static class Templates
    {
        public static List<DbTemplate> Get(List<DbItem> items, bool preventOrphans = false)
        {
            var templateItems = items.Where(i => i.TemplateID == TemplateIDs.Template);
            var templates = new List<DbTemplate>();

            foreach (var templateItem in templateItems)
            {
                var sections = items.Where(item => item.ParentID == templateItem.ID && item.Name != "__Standard Values").ToList();
                var standardValues = items.FirstOrDefault(item => item.ParentID == templateItem.ID && item.Name == "__Standard Values");

                var template = new DbTemplate(templateItem.Name, templateItem.ID);
                template.ParentID = templateItem.ParentID;
                template.FullPath = templateItem.FullPath;

                var ids = BaseTemplateIds(templateItem, items);
                if (ids != null)
                    template.BaseIDs = ids;

                GetFields(sections, items, template, standardValues);

                if (preventOrphans && !items.Any(t => t.ID == template.ParentID))
                    template.ParentID = ItemIDs.TemplateRoot;

                templates.Add(template);
            }

            return templates;
        }

        public static List<DbTemplate> GetMissing(List<DbItem> items, List<DbTemplate> templates, Db db, bool preventOrphans = false)
        {
            List<DbTemplate> missing = new List<DbTemplate>();

            foreach (var item in items)
            {
                if (!templates.Any(t => t.ID == item.TemplateID) && db.GetItem(item.TemplateID) == null &&
                    (missing.Count == 0 || !missing.Any(t => t.ID == item.TemplateID)))
                {
                    DbTemplate template = new DbTemplate(item.TemplateID);

                    foreach (var field in item.Fields)
                        template.Fields.Add(new DbField(field.Name));

                    missing.Add(template);
                }
            }

            return missing;
        }

        private static ID[] BaseTemplateIds(DbItem item, List<DbItem> items)
        {
            if (item != null && item.Fields.Any(t => t.ID == FieldIDs.BaseTemplate))
            {
                string[] ids = item.Fields[FieldIDs.BaseTemplate].Value.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                return ids.Select(i => new ID(i)).Where(id => items.Any(i => i.ID == id)).ToArray();
            }
            else
                return null;
        }

        private static void GetFields(IEnumerable<DbItem> sections, IEnumerable<DbItem> items, DbTemplate template, DbItem standardValues)
        {
            var fieldItems = items.Where(field => sections.Select(i => i.ID).Contains(field.ParentID)).ToList();

            foreach (var fieldItem in fieldItems)
            {
                if (fieldItem != null && fieldItem.TemplateID == TemplateIDs.TemplateField)
                {
                    if (standardValues != null && standardValues.Fields.Any(t => t.ID == fieldItem.ID))
                    {
                        DbField field = new DbField(fieldItem.Name, fieldItem.ID);
                        field.Value = standardValues.Fields[fieldItem.ID].Value;
                        template.Fields.Add(field);
                    }
                    else
                        template.Fields.Add(new DbField(fieldItem.Name, fieldItem.ID));
                }
            }
        }
    }
}