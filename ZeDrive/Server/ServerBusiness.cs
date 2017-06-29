using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ServerBusiness
    {
        public void Connect() { }
        public void CreateUser() { }
        public void CreateGroup() { }
        public void GetClientLists() { }
        public void GetGroupList() { }
        /// <summary>
        /// Update server history,
        /// Send Revision,
        /// Save history on disk
        /// </summary>
        public void UpdateServerHistory() { }
        public void SendClientGroupInvitation() { }
        public void SendClientGroupRequest() { }
        /// <summary>
        /// Check for group join request and group invitation
        /// </summary>
        public void GetNotification() { }
        public void ChangeAdministratorGroup() { }
        public void DeleteClientFromGroup() { }
    }
}
