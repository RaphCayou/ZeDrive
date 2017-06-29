using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public enum Action
    {
        Create,
        Modify,
        Delete,
    }
    public class Revision
    {
        public Action Action;
        public File File;
        public Group Group;
    }
}
