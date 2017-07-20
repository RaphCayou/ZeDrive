using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ShareLibrary.Utils
{
    public static class DiskAccessUtils
    {
        public static T LoadFromDisk<T>(string filename)
        {
            T loadedObject;
            using (var stream = File.OpenRead(filename))
            {
                var serializer = new XmlSerializer(typeof(T));
                loadedObject = (T)serializer.Deserialize(stream);
            }
            return loadedObject;
        }

        public static void SaveToDisk(string filename, Object objectToSave)
        {
            using (var writer = new StreamWriter(filename))
            {
                var serializer = new XmlSerializer(objectToSave.GetType());
                serializer.Serialize(writer, objectToSave);
                writer.Flush();
            }
        }

        public static T LoadFromDiskOrConstrucDefault<T>(string filename) where T : new()
        {
            T loadedObject;
            if (File.Exists(filename))
            {
                using (var stream = File.OpenRead(filename))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    XmlReader reader = new XmlTextReader(stream);
                    if (serializer.CanDeserialize(reader))
                    {
                        loadedObject = (T)serializer.Deserialize(reader);
                    }
                    else
                    {
                        loadedObject = new T();
                    }
                }
            }
            else
            {
                loadedObject = new T();
            }
            return loadedObject;
        }
    }
}
