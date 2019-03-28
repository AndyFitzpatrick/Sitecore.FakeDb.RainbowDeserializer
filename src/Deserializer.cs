using Rainbow.Model;
using Rainbow.Storage.Yaml;
using Sitecore.Data;
using System;
using System.IO;
using System.Linq;

namespace Sitecore.FakeDb.RainbowSerialization
{
    /// <summary>
    /// Deserializes .yml serialized Sitecore items into FakeDb's DbItem and adds them
    /// to a passed in Db.  
    /// </summary>
    public static class Deserializer
    {
        public static DbItem DeserializeItem(FileInfo file)
        {
            DbItem deserialized = null;
            var formatter = new YamlSerializationFormatter(null, null);

            using (StreamReader sr = new StreamReader(file.FullName))
            {
                try
                {
                    IItemData item = formatter.ReadSerializedItem(sr.BaseStream, file.Name);
                    if (item != null)
                    {
                        deserialized = new DbItem(item.Name, new ID(item.Id), new ID(item.TemplateId));
                        deserialized.FullPath = item.Path;
                        deserialized.BranchId = new ID(item.BranchId);
                        deserialized.ParentID = new ID(item.ParentId);

                        foreach (var sharedField in item.SharedFields)
                            deserialized.Fields.Add(new ID(sharedField.FieldId), sharedField.Value);

                        foreach (var unversionedField in item.UnversionedFields)
                        {
                            if (unversionedField != null)
                            {
                                var field = unversionedField.Fields.Last();
                                deserialized.Fields.Add(new ID(field.FieldId), field.Value);
                            }
                        }

                        if (item.Versions != null && item.Versions.Count() > 0 && item.Versions.Last().Fields != null)
                        {
                            foreach (var field in item.Versions.Last().Fields)
                                deserialized.Fields.Add(new ID(field.FieldId), field.Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Unable to deserialize '{0}'", file.FullName), ex);
                }
            }

            return deserialized;
        }
    }
}