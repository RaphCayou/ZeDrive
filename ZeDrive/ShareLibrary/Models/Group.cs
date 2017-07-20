using System;
using System.Collections.Generic;

namespace ShareLibrary.Models
{
    [Serializable]
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
