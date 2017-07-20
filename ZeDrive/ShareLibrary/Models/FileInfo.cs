using System;

namespace ShareLibrary.Models
{
    public class FileInfo
    {
        public string Name;
        public DateTime CreationDate;
        public DateTime LastModificationDate;

        public override bool Equals(Object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            FileInfo f = (FileInfo)obj;
            return
                Name.Equals(f.Name) &&
                CreationDate.Equals(f.CreationDate) &&
                LastModificationDate.Equals(f.LastModificationDate);
        }
    }
}
