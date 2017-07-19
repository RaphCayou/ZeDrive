using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server;
using ShareLibrary.Communication;
using ShareLibrary.Models;
using ShareLibrary.Summary;
using Action = ShareLibrary.Models.Action;

namespace Client
{
    public class ClientBusiness
    {
        private string rootFolderPath;
        private string userName;
        private Dictionary<string, GroupSummary> lastGroupsSummaries;

        private IServerBusiness access;

        public ClientBusiness(string rootFolderPath)
        {
            this.rootFolderPath = rootFolderPath;
        }

        public ClientBusiness(string rootFolderPath, string serverAddress, int serverPort)
        {
            this.rootFolderPath = rootFolderPath;
            access = new ClientServerAccess(serverAddress, serverPort);
            lastGroupsSummaries = new Dictionary<string, GroupSummary>();
        }

        public bool Connect(string user, string password)
        {
            bool validConnect = access.Connect(user, password);
            if (validConnect)
            {
                userName = user;
            }
            return validConnect;
        }

        public void CreateUser(string newUserName, string password)
        {
            access.CreateUser(newUserName, password);
        }

        public void CreateGroup(string groupName, string groupDescription)
        {
            access.CreateGroup(groupName, groupDescription, userName);
        }

        public void SendJoinGroupRequest(string invitedUser, string groupName)
        {
            access.SendClientGroupRequest(userName, groupName);
        }

        public void SendGroupInvitation(string invitedUser, string groupName)
        {
            access.SendClientGroupInvitation(userName, invitedUser, groupName);
        }

        public void AcceptInvitation(string groupName, bool isAccepted)
        {
            access.AcknowledgeInvite(userName, groupName, isAccepted);
        }

        /// <summary>
        /// Generate and push revisions list
        /// </summary>
        /// <returns>Revision list of the server.</returns>
        public List<Revision> UpdateServerHistory()
        {
            List<Revision> serveeRevisions = new List<Revision>();
            //Creating the revision list
            //TODO JP call on access the update server
            return serveeRevisions;
        }

        /// <summary>
        /// Updating local version of the files based on the revisions received by the server.
        /// </summary>
        /// <param name="revisions">The revisions to apply localy.</param>
        public void UpdateLocalFiles(List<Revision> revisions)
        {
            foreach (Revision revision in revisions)
            {
                revision.Apply(rootFolderPath);
            }
        }

        public void SyncWithServer()
        {
            UpdateLocalFiles(UpdateServerHistory());
            GetGroupsUpdate();
        }

        public void ChangeAdministratorGroup(string newAdmin, string groupName)
        {
            access.ChangeAdministratorGroup(userName, newAdmin, groupName);
        }

        public void KickClientFromGroup(string removedUser, string groupName)
        {
            access.KickUserFromGroup(userName, removedUser, groupName);
        }

        /// <summary>
        /// We fetch the groups updates.(Invitations, kicks, join completed and newly admin)
        /// </summary>
        public void GetGroupsUpdate()
        {
            // TODO JP call on access the getGroup list then apply the local modification based the group list
        }

        public List<Group> GetGroupList()
        {
            return access.GetGroupList();
        }

        public List<ShareLibrary.Models.Client> GetClientsList()
        {
            return access.GetClientList();
        }
    }
}
