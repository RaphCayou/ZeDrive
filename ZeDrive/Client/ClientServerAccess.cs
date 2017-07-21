using System;
using System.Collections.Generic;
using ShareLibrary.Communication;
using ShareLibrary.Models;
using ShareLibrary.Summary;

namespace Client
{
    public class ClientServerAccess : IServerBusiness
    {
        // ReSharper disable once InconsistentNaming
        private ClientRPCManager RPCManager { get; set; }

        public ClientServerAccess(string ipAddress, int port)
        {
            RPCManager = new ClientRPCManager(ipAddress, port);
        }

        public bool Connect(string username, string password)
        {
            return (bool)RPCManager.SendMessage(System.Reflection.MethodBase.GetCurrentMethod(),
                username,
                password
                );
        }
        public bool IsAvailable()
        {
            return (bool)RPCManager.SendMessage(System.Reflection.MethodBase.GetCurrentMethod());
        }

        public bool CreateUser(string username, string password)
        {
            return (bool)RPCManager.SendMessage(System.Reflection.MethodBase.GetCurrentMethod(),
                username,
                password
                );
        }

        public bool CreateGroup(string name, string description, string username)
        {
            return (bool)RPCManager.SendMessage(System.Reflection.MethodBase.GetCurrentMethod(),
                name,
                description,
                username
                );
        }

        public List<ShareLibrary.Models.Client> GetClientList()
        {
            return (List<ShareLibrary.Models.Client>)RPCManager.SendMessage(System.Reflection.MethodBase.GetCurrentMethod());
        }

        public List<ShareLibrary.Models.Client> GetOnlineClientsList()
        {
            return (List<ShareLibrary.Models.Client>)RPCManager.SendMessage(System.Reflection.MethodBase.GetCurrentMethod());
        }

        public List<Group> GetGroupList()
        {
            return (List<Group>)RPCManager.SendMessage(System.Reflection.MethodBase.GetCurrentMethod());
        }

        public List<Group> GetGroupListForClient(string username)
        {
            return (List<Group>)RPCManager.SendMessage(System.Reflection.MethodBase.GetCurrentMethod(), username);
        }

        public Group GetGroupInfo(string groupName)
        {
            return (Group)RPCManager.SendMessage(System.Reflection.MethodBase.GetCurrentMethod(), groupName);
        }

        public void SendClientGroupInvitation(string adminUsername, string invitedUser, string groupName)
        {
            RPCManager.SendMessage(System.Reflection.MethodBase.GetCurrentMethod(),
                adminUsername,
                invitedUser,
                groupName);
        }

        public void SendClientGroupRequest(string username, string groupName)
        {
            RPCManager.SendMessage(System.Reflection.MethodBase.GetCurrentMethod(),
                username,
                groupName);
        }

        public void KickUserFromGroup(string adminUsername, string username, string groupName)
        {
            RPCManager.SendMessage(System.Reflection.MethodBase.GetCurrentMethod(),
                adminUsername,
                username,
                groupName);
        }

        public List<PendingAction> GetNotification(string username)
        {
            return (List<PendingAction>)RPCManager.SendMessage(System.Reflection.MethodBase.GetCurrentMethod(),
                username);
        }

        public void AcknowledgeRequest(string adminUsername, string username, string group, bool accept)
        {
            RPCManager.SendMessage(System.Reflection.MethodBase.GetCurrentMethod(),
                adminUsername,
                username, 
                group,
                accept);
        }

        public void AcknowledgeInvite(string username, string group, bool accept)
        {
            RPCManager.SendMessage(System.Reflection.MethodBase.GetCurrentMethod(),
                username,
                group,
                accept);
        }

        public void ChangeAdministratorGroup(string usernameCurrentAdmin, string usernameFutureAdmin, string groupName)
        {
            RPCManager.SendMessage(System.Reflection.MethodBase.GetCurrentMethod(),
                usernameCurrentAdmin,
                usernameFutureAdmin,
                groupName);
        }

        public List<Revision> UpdateServerHistory(string username, List<Revision> revisions, List<GroupSummary> groupSummaries)
        {
            return (List<Revision>)RPCManager.SendMessage(System.Reflection.MethodBase.GetCurrentMethod(),
                username,
                revisions,
                groupSummaries);
        }
    }
}
