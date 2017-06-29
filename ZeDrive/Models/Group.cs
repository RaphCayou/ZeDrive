using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
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
