using System;
using System.IO;
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
    }
}
