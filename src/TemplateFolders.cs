using System.Collections.Generic;
using System.Linq;

namespace Sitecore.FakeDb.RainbowSerialization
{
    public static class TemplateFolders
    {
        public static IEnumerable<DbItem> Get(List<DbItem> items, bool preventOrphans)
        {
            var model = items.Where(item => item.FullPath.StartsWith("/sitecore/templates/") &&
                item.TemplateID != TemplateIDs.Template && item.TemplateID != TemplateIDs.TemplateSection &&
                item.TemplateID != TemplateIDs.TemplateField && item.Name != "__Standard Values");

            if (preventOrphans && model != null && model.Count() > 0)
            {
                foreach (var item in model)
                {
                    if (item != null && !items.Any(t => t.ID == item.ParentID))
                        item.ParentID = ItemIDs.TemplateRoot;
                }
            }

            return model;
        }
    }
}