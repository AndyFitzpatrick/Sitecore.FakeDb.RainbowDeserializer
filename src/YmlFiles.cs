using System.Collections.Generic;
using System.IO;

namespace Sitecore.FakeDb.RainbowDeserializer
{
    public static class YmlFiles
    {
        public static List<DbItem> ToDbItems(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (path.EndsWith(".yml") && File.Exists(path))
                    return ToDbItems(new FileInfo(path));
                else if (Directory.Exists(path))
                    return ToDbItems(new DirectoryInfo(path));
            }

            return new List<DbItem>();
        }

        public static List<DbItem> ToDbItems(DirectoryInfo info)
        {
            List<DbItem> items = new List<DbItem>();

            if (info != null)
            {
                var files = info.GetFiles("*.yml", SearchOption.TopDirectoryOnly);

                if (files != null && files.Length > 0)
                {
                    foreach (FileInfo file in files)
                        items.AddRange(ToDbItems(file));
                }
                else
                {
                    var subdirectories = info.GetDirectories();

                    if (subdirectories != null && subdirectories.Length > 0)
                    {
                        foreach (var subdirectory in subdirectories)
                            items.AddRange(ToDbItems(subdirectory));
                    }
                }
            }

            return items;
        }

        public static List<DbItem> ToDbItems(FileInfo file)
        {
            List<DbItem> items = new List<DbItem>();

            if (file != null)
            {
                DbItem deserialized = Deserializer.DeserializeItem(file, Settings.Language);

                if (deserialized != null)
                {
                    items.Add(deserialized);

                    var folder = new DirectoryInfo(file.Directory.FullName + "\\" + file.Name.Replace(".yml", ""));
                    if (folder.Exists)
                        items.AddRange(RecursivelyFindFiles(folder));
                }
            }

            return items;
        }

        private static List<DbItem> RecursivelyFindFiles(DirectoryInfo info)
        {
            List<DbItem> items = new List<DbItem>();

            if (info != null)
            {
                foreach (FileInfo file in info.GetFiles("*.yml", SearchOption.TopDirectoryOnly))
                    items.AddRange(ToDbItems(file));
            }

            return items;
        }
    }
}