using ShareLibrary.Models;

namespace ShareLibrary.Models
{
    public class PendingAction
    {
        public string ClientName { get; set; }
        public string GroupName { get; set; }
        public ActionTypes ActionType { get; set; }
    }
}
