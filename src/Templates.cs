using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.FakeDb.RainbowSerialization
{
    public static class Templates
    {
        public static List<DbTemplate> Get(List<DbItem> items)
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

                if (!items.Any(t => t.ID == template.ParentID))
                    template.ParentID = ItemIDs.TemplateRoot;

                templates.Add(template);
            }

            AddMissing(items, templates);

            return templates;
        }

        private static void AddMissing(List<DbItem> items, List<DbTemplate> templates)
        {
            DbTemplate missingFields = new DbTemplate("Missing Fields", new ID());

            for (int i = 0; i < items.Count; i++)
            {
                if (!items[i].FullPath.StartsWith("/sitecore/templates/"))
                {
                    var existing = templates.FirstOrDefault(t => t.ID == items[i].TemplateID);
                    if (existing == null)
                    {
                        //Add missing templates
                        DbTemplate template = new DbTemplate(items[i].TemplateID);
                        template.BaseIDs = new ID[] { missingFields.ID };

                        foreach (var field in items[0].Fields.Where(field => !field.Name.StartsWith("__")))
                        {
                            if (!missingFields.Fields.Any(f => f.Name == field.Name))
                                missingFields.Fields.Add(new DbField(field.Name, field.ID));
                        }

                        templates.Add(template);
                    }
                    else
                    {
                        // Add missing fields
                        var baseFields = BaseTemplateFields(existing, templates);
                        foreach (var field in items[0].Fields.Where(field => !field.Name.StartsWith("__") && 
                            !existing.Fields.Any(tfield => tfield.Name == field.Name) &&
                            !baseFields.Any(bField => bField.Name == field.Name)))
                        {
                            if (!missingFields.Fields.Any(f => f.Name == field.Name))
                                missingFields.Fields.Add(new DbField(field.Name, field.ID));

                            if (!existing.BaseIDs.Contains(missingFields.ID))
                                existing.BaseIDs = existing.BaseIDs.Concat(new ID[] { missingFields.ID }).ToArray();
                        }
                    }
                }
            }

            if (missingFields.Fields.Count() > 0)
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

        private static IEnumerable<DbField> BaseTemplateFields(DbTemplate template, List<DbTemplate> templates)
        {
            List<DbField> fields = new List<DbField>();

            if (template != null && template.BaseIDs != null && template.BaseIDs.Length > 0)
            {
                var baseTemplates = templates.Where(t => template.BaseIDs.Contains(t.ID));

                if (baseTemplates != null && baseTemplates.Count() > 0)
                {
                    foreach (var bt in baseTemplates)
                        fields.AddRange(bt.Fields);
                }
            }

            return fields;
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