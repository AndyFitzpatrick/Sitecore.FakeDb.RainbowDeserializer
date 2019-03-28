using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.FakeDb.RainbowSerialization
{
    public static class MediaItems
    {
        public static IEnumerable<DbItem> Get(List<DbItem> items, bool preventOrphans = false)
        {
            var model = items.Where(item => item.FullPath.StartsWith("/sitecore/media library/"));

            if (preventOrphans && model != null && model.Count() > 0)
            {
                foreach (var item in model)
                {
                    if (item != null && !items.Any(t => t.ID == item.ParentID))
                        item.ParentID = ItemIDs.MediaLibraryRoot;
                }
            }

            return model;
        }
    }
}