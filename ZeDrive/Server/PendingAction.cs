using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareLibrary.Models;

namespace Server
{
    class PendingAction
    {
        public string clientName { get; set; }
        public string groupName { get; set; }
        public ActionType.Action action { get; set; }
    }
}
