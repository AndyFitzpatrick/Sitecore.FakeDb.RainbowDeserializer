using Sitecore.Data;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.FakeDb.RainbowSerialization
{
    public static class InheritedFields
    {
        public static void Add(List<DbItem> items, List<DbTemplate> templates)
        {
            if (items != null && items.Count > 0 && templates != null && templates.Count > 0)
            {
                foreach (var item in items)
                {
                    var fields = GetBaseTemplateFields(item.TemplateID, templates, items);

                    if (fields != null && fields.Count > 0)
                    {
                        foreach (var field in fields)
                        {
                            if (field != null && field.Value != null && !item.Fields.Any(f => f.ID == field.ID))
                                item.Fields.Add(field.Name, field.Value);
                        }
                    }
                }
            }
        }

        private static List<DbField> GetBaseTemplateFields(ID id, List<DbTemplate> templates, List<DbItem> items)
        {
            List<DbField> fields = new List<DbField>();

            if (!ID.IsNullOrEmpty(id) && templates != null && templates.Count > 0)
            {
                var template = templates.FirstOrDefault(t => t.ID == id);

                if (template != null)
                {
                    var standardValues = items.FirstOrDefault(item => item.ParentID == template.ID && item.Name == "__Standard Values");
                    if (standardValues != null)
                    {
                        foreach (var field in standardValues.Fields)
                        {
                            if (!fields.Any(f => f.Name == field.Name))
                                fields.Add(field);
                        }
                    }

                    if (template.BaseIDs != null && template.BaseIDs.Count() > 0)
                    {
                        foreach (ID bid in template.BaseIDs)
                            fields.AddRange(GetBaseTemplateFields(bid, templates, items));
                    }
                }
            }

            return fields;
        }
    }
}