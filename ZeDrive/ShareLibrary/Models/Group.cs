using System.Collections.Generic;

namespace ShareLibrary
{
    public class Group
    {
        public string Name;
        public string Description;
        public Client Administrator;
        public List<Client> Members;
        public List<File> Files;
    }
}
