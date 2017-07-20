using System.Collections.Generic;
using ShareLibrary.Models;
using ShareLibrary.Summary;

namespace ShareLibrary.Communication
{
    public interface IServerBusiness
    {
        bool Connect(string username, string password);
        void CreateUser(string username, string password);
        void CreateGroup(string name, string description, string username);
        List<Client> GetClientList();
        List<Client> GetOnlineClientsList();
        List<Group> GetGroupList();
        List<Group> GetGroupListForClient(string username);
        void SendClientGroupInvitation(string adminUsername, string invitedUser, string groupName);
        void SendClientGroupRequest(string username, string groupName);
        void KickUserFromGroup(string adminUsername, string username, string groupName);
        List<PendingAction> GetNotification(string username);
        void AcknowledgeRequest(string adminUsername, string username, string groupName, bool accept);
        void AcknowledgeInvite(string username, string groupName, bool accept);
        void ChangeAdministratorGroup(string usernameCurrentAdmin, string usernameFutureAdmin, string groupName);
        List<Revision> UpdateServerHistory(string username, List<Revision> revisions, List<GroupSummary> groupSummaries);
    }
}
