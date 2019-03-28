using System.Collections.Generic;
using System.Linq;

namespace Sitecore.FakeDb.RainbowSerialization
{
    public static class LayoutItems
    {
        public static IEnumerable<DbItem> Get(List<DbItem> items, bool preventOrphans = false)
        {
            var model = items.Where(item => item.FullPath.StartsWith("/sitecore/layout/"));

            if (preventOrphans && model != null && model.Count() > 0)
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
