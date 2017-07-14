using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class ClientServerAccess : IServerBusiness
    {
        private ClientRPCManager RPCManager { get; set; }

        public ClientServerAccess(string ipAddress, int port)
        {
            RPCManager = new ClientRPCManager(ipAddress, port);
        }

        public void ChangeAdministratorGroup()
        {
            RPCManager.SendMessage(System.Reflection.MethodBase.GetCurrentMethod());
        }

        public bool Connect(string username, string password)
        {
            return (bool)RPCManager.SendMessage(System.Reflection.MethodBase.GetCurrentMethod(),
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

        public void CreateUser(string username, string password)
        {
            RPCManager.SendMessage(System.Reflection.MethodBase.GetCurrentMethod(),
                username,
                password
                );
        }

        public void DeleteClientFromGroup()
        {
            throw new NotImplementedException();
        }

        public void GetClientLists()
        {
            throw new NotImplementedException();
        }

        public void GetGroupList()
        {
            throw new NotImplementedException();
        }

        public void GetNotification()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void SendClientGroupInvitation()
        {
            throw new NotImplementedException();
        }

        public void SendClientGroupRequest()
        {
            throw new NotImplementedException();
        }
    }
}
