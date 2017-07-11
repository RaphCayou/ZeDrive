using System.Collections.Generic;
using ShareLibrary.Summary;

namespace ShareLibrary.Models
{
    public class SyncTransmission
    {
        public string Username;
        public List<Revision> Revisions;
        public List<GroupSummary> GroupSummaries;
    }
}
