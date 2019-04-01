using System.Collections.Generic;
using System.Linq;

namespace Sitecore.FakeDb.RainbowDeserializer
{
    public static class LayoutItems
    {
        public static IEnumerable<DbItem> Get(List<DbItem> items)
        {
            var model = items.Where(item => item.FullPath.StartsWith("/sitecore/layout/"));

            if (model != null && model.Count() > 0)
            {
                foreach (var item in model)
                {
                    if (item != null && !items.Any(t => t.ID == item.ParentID))
                        item.ParentID = ItemIDs.LayoutRoot;
                }
            }

            return model;
        }
    }
}