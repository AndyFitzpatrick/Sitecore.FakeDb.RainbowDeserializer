using System.Collections.Generic;
using System.Linq;

namespace Sitecore.FakeDb.RainbowDeserializer
{
    public static class ContentItems
    {
        public static IEnumerable<DbItem> Get(List<DbItem> items, List<DbTemplate> templates)
        {
            List<DbItem> model = items.Where(i => i.FullPath.StartsWith("/sitecore/content/")).ToList();
            InheritedFields.Add(model, items, templates);

            if (model != null && model.Count() > 0)
            {
                foreach (var item in model)
                {
                    if (item != null && !items.Any(t => t.ID == item.ParentID))
                        item.ParentID = ItemIDs.ContentRoot;
                }
            }

            return model;
        }
    }
}