using System;
using ShareLibrary.Models;

namespace ShareLibrary.Models
{
    [Serializable]
    public class PendingAction
    {
        public string ClientName { get; set; }
        public string GroupName { get; set; }
        public ActionType ActionType { get; set; }
    }
}
