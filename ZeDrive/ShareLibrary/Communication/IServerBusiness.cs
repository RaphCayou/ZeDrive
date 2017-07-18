using ShareLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public interface IServerBusiness
    {
        bool Connect(string username, string password);
        void CreateUser(string username, string password);
        void CreateGroup(string name, string description, string username);
        List<Client> GetClientLists();
        List<Group> GetGroupList();
        void SendClientGroupInvitation(string adminUsername, string invitedUser, string groupName);
        void SendClientGroupRequest(string username, string groupName);
        void KickUserFromGroup(string adminUsername, string username, string groupName);
        void GetNotification();
        void AcknowledgeRequest(string adminUsername, string username, string group, bool accept);
        void AcknowledgeInvite(string username, string group, bool accept);
        void ChangeAdministratorGroup(string usernameCurrentAdmin, string usernameFutureAdmin, string groupName);
        Tuple<string, string> GetSaveFilesNames();
    }
}
