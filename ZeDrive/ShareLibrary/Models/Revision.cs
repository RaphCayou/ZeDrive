using System;
using System.IO;
using System.Linq;

namespace ShareLibrary.Models
{
    [Serializable]
    public enum Action
    {
        Create,
        Modify,
        Delete,
    }
    [Serializable]
    public class Revision
    {
        public Action Action;
        public FileInfo File;
        public string GroupName;
        public byte[] Data;

        /// <summary>
        /// Apply the revision the local file.
        /// </summary>
        /// <param name="rootFolderPath">Path to the folder of all groups folders.</param>
        public void Apply(string rootFolderPath)
        {
            string filePath = Path.Combine(rootFolderPath, GroupName, File.Name);
            switch (Action)
            {
                case Action.Create:
                case Action.Modify:
                    System.IO.File.WriteAllBytes(filePath, Data);
                    System.IO.File.SetCreationTime(filePath, File.CreationDate);
                    System.IO.File.SetLastWriteTime(filePath, File.LastModificationDate);
                    break;
                case Action.Delete:
                    System.IO.File.Delete(filePath);
                    break;
                default:
                    throw new ArgumentException("Action non supportée.");
            }
        }

        public override bool Equals(Object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            Revision r = (Revision)obj;
            return 
                Action.Equals(r.Action) && 
                Data.SequenceEqual(r.Data) && 
                File.Equals(r.File) && 
                GroupName.Equals(r.GroupName);
        }
    }
}
