using System;
using System.Collections.Generic;
using ShareLibrary.Communication;
using ShareLibrary.Models;

namespace Client
{
    public class ClientServerAccess : IServerBusiness
    {
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

        public void CreateUser(string username, string password)
        {
            RPCManager.SendMessage(System.Reflection.MethodBase.GetCurrentMethod(),
                username,
                password
                );
        }

        public void CreateGroup(string name, string description, string username)
        {
            RPCManager.SendMessage(System.Reflection.MethodBase.GetCurrentMethod(),
                name,
                description,
                username
                );
        }

        public List<ShareLibrary.Models.Client> GetClientLists()
        {
            return (List<ShareLibrary.Models.Client>)RPCManager.SendMessage(System.Reflection.MethodBase.GetCurrentMethod());
        }

        public List<Group> GetGroupList()
        {
            throw new NotImplementedException();
        }

        public void SendClientGroupInvitation(string adminUsername, string invitedUser, string groupName)
        {
            throw new NotImplementedException();
        }

        public void SendClientGroupRequest(string username, string groupName)
        {
            throw new NotImplementedException();
        }

        public void KickUserFromGroup(string adminUsername, string username, string groupName)
        {
            throw new NotImplementedException();
        }

        public List<PendingAction> GetNotification(string username)
        {
            return (List<PendingAction>)RPCManager.SendMessage(System.Reflection.MethodBase.GetCurrentMethod());
        }

        public void AcknowledgeRequest(string adminUsername, string username, string group, bool accept)
        {
            throw new NotImplementedException();
        }

        public void AcknowledgeInvite(string username, string group, bool accept)
        {
            throw new NotImplementedException();
        }

        public void ChangeAdministratorGroup(string usernameCurrentAdmin, string usernameFutureAdmin, string groupName)
        {
            throw new NotImplementedException();
        }
    }
}
