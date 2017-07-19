using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLibrary;
using ShareLibrary.Models;
using ShareLibrary.Summary;

namespace Server
{
    public class Job
    {
        public string Username;
        public List<Revision> Revisions;
        public List<GroupSummary> GroupSummaries;

        public Action<List<Revision>> CallBack;
    }
}
