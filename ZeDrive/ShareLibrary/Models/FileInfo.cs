using System;

namespace ShareLibrary.Models
{
    [Serializable]
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

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ CreationDate.GetHashCode();
                hashCode = (hashCode * 397) ^ LastModificationDate.GetHashCode();
                return hashCode;
            }
        }
    }
}
