using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public interface IServerBusiness
    {
        void Save();
        bool Connect(string username, string password);
        void CreateUser(string username, string password);
        void CreateGroup(string name, string description, string username);
        void GetClientLists();
        void GetGroupList();
        void SendClientGroupInvitation();
        void SendClientGroupRequest();
        void GetNotification();
        void ChangeAdministratorGroup();
        void DeleteClientFromGroup();

    }
}
