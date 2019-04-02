using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.FakeDb.RainbowDeserializer
{
    public static class Templates
    {
        public static List<DbTemplate> Get(List<DbItem> items)
        {
            var templateItems = items.Where(i => i.TemplateID == TemplateIDs.Template);
            var templates = new List<DbTemplate>();

            foreach (var templateItem in templateItems)
            {
                var template = new DbTemplate(templateItem.Name, templateItem.ID);
                template.ParentID = templateItem.ParentID;
                template.FullPath = templateItem.FullPath;

                var ids = BaseTemplateIds(templateItem, items);
                if (ids != null)
                    template.BaseIDs = ids;

                GetFields(items, template);

                if (!items.Any(t => t.ID == template.ParentID))
                    template.ParentID = ItemIDs.TemplateRoot;

                templates.Add(template);
            }

            AddMissing(items, templates, GetAllFields(templates));

            return templates;
        }

        private static void AddMissing(List<DbItem> items, List<DbTemplate> templates, List<DbField> fields)
        {
            DbTemplate missingFields = new DbTemplate("Missing Fields", new ID(Guid.NewGuid()));

            for (int i = 0; i < items.Count; i++)
            {
                if (!items[i].FullPath.StartsWith("/sitecore/templates/") &&
                    items[i].TemplateID != Sitecore.TemplateIDs.Folder &&
                    items[i].TemplateID != Sitecore.TemplateIDs.MediaFolder)
                {
                    var missing = items[i].Fields.Where(f => !f.Name.StartsWith("__") && !fields.Any(tfield => tfield.ID == f.ID));

                    if (missing != null && missing.Count() > 0)
                    {
                        var existing = templates.FirstOrDefault(t => t.ID == items[i].TemplateID);

                        if (existing == null)
                        {
                            //Add missing templates
                            DbTemplate template = new DbTemplate(items[i].TemplateID);
                            template.BaseIDs = new ID[] { missingFields.ID };

                            foreach (var field in missing)
                            {
                                if (!missingFields.Fields.Any(f => f.ID == field.ID))
                                    missingFields.Fields.Add(new DbField(field.Name, field.ID));
                            }

                            templates.Add(template);
                        }
                        else
                        {
                            // Add missing fields
                            foreach (var field in missing)
                            {
                                if (!missingFields.Fields.Any(f => f.ID == field.ID))
                                    missingFields.Fields.Add(new DbField(field.Name, field.ID));

                                if (!existing.BaseIDs.Contains(missingFields.ID))
                                    existing.BaseIDs = existing.BaseIDs.Concat(new ID[] { missingFields.ID }).ToArray();
                            }
                        }
                    }
                }
            }

            templates.Add(missingFields);
        }

        private static ID[] BaseTemplateIds(DbItem item)
        {
            if (item != null && item.Fields.Any(t => t.ID == FieldIDs.BaseTemplate))
                return item.Fields[FieldIDs.BaseTemplate].Value
                    .Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(i => new ID(i))
                    .ToArray();
            else
                return new ID[0];
        }

        private static ID[] BaseTemplateIds(DbItem item, List<DbItem> items)
        {
            var ids = BaseTemplateIds(item);

            if (ids != null)
                return ids.Where(id => id == TemplateIDs.Template || items.Any(i => i.ID == id)).ToArray();
            else
                return new ID[0];
        }

        private static List<DbField> GetAllFields(List<DbTemplate> templates)
        {
            List<DbField> fields = new List<DbField>();

            if (templates != null && templates.Count > 0)
            {
                foreach (DbTemplate template in templates)
                    fields.AddRange(template.Fields);
            }

            return fields;
        }

        private static void GetFields(IEnumerable<DbItem> items, DbTemplate template)
        {
            var sections = items.Where(item => item.ParentID == template.ID && item.Name != "__Standard Values").ToList();
            var fieldItems = items.Where(field => sections != null && sections.Select(i => i.ID).Contains(field.ParentID)).ToList();

            foreach (var fieldItem in fieldItems)
            {
                if (fieldItem != null && fieldItem.TemplateID == TemplateIDs.TemplateField)
                    template.Fields.Add(new DbField(fieldItem.Name, fieldItem.ID));
            }
        }
    }
}