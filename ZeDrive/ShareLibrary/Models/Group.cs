using System.Collections.Generic;

namespace ShareLibrary.Models
{
    public class Group
    {
        public string Name;
        public string Description;
        public Client Administrator;
        public List<Client> Members;
        public List<FileInfo> Files;

        public override string ToString()
        {
            return Name;
        }
    }
}
