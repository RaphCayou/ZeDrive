using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server;
using ShareLibrary.Communication;
using ShareLibrary.Models;
using ShareLibrary.Summary;
using ShareLibrary.Utils;
using Action = ShareLibrary.Models.Action;

namespace Client
{
    public class ClientBusiness
    {
        private const string GROUP_SUMMARY_FILE = "lastGroupsSummaries.xml";
        private readonly string rootFolderPath;
        private string userName;
        private List<GroupSummary> lastGroupsSummaries;

        private readonly IServerBusiness access;

        public ClientBusiness(string rootFolderPath)
        {
            this.rootFolderPath = rootFolderPath;
            lastGroupsSummaries = DiskAccessUtils.LoadFromDiskOrConstrucDefault<List<GroupSummary>>(Path.Combine(rootFolderPath, GROUP_SUMMARY_FILE));
        }

        public ClientBusiness(string rootFolderPath, string serverAddress, int serverPort) : this(rootFolderPath)
        {
            access = new ClientServerAccess(serverAddress, serverPort);
            access.IsAvailable();
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

        public bool CreateUser(string newUserName, string password)
        {
            return access.CreateUser(newUserName, password);
        }

        public bool CreateGroup(string groupName, string groupDescription)
        {
            return access.CreateGroup(groupName, groupDescription, userName);
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

        public void AcknowledgeRequest(string requestUserName, string groupName, bool isAccepted)
        {
            access.AcknowledgeRequest(userName, requestUserName, groupName, isAccepted);
        }

        public List<PendingAction> GetPendingActions()
        {
            return access.GetNotification(userName);
        }

        /// <summary>
        /// Generate and push revisions list
        /// </summary>
        /// <returns>Revision list of the server.</returns>
        public List<Revision> UpdateServerHistory()
        {
            List<Revision> localRevisions = new List<Revision>();
            List<GroupSummary> newGroupSummaries = new List<GroupSummary>();
            //Creating the revision list
            foreach (GroupSummary group in lastGroupsSummaries)
            {
                GroupSummary updateSummary = new GroupSummary(group.GroupName, rootFolderPath);
                localRevisions.AddRange(updateSummary.GenerateRevisions(group));
                newGroupSummaries.Add(updateSummary);
            }
            lastGroupsSummaries = newGroupSummaries;
            List<Revision> serverRevisions = access.UpdateServerHistory(userName, localRevisions, lastGroupsSummaries);
            return serverRevisions;
        }

        /// <summary>
        /// Updating local version of the files based on the revisions received by the server.
        /// </summary>
        /// <param name="revisions">The revisions to apply localy.</param>
        public void UpdateLocalFiles(List<Revision> revisions)
        {
            if (revisions == null) return;

            foreach (Revision revision in revisions)
            {
                lastGroupsSummaries.First(summary => summary.GroupName == revision.GroupName).Update(revision);
                revision.Apply(rootFolderPath);
            }
        }

        public void SyncWithServer()
        {
            GetGroupsUpdate();
            UpdateLocalFiles(UpdateServerHistory());
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
            List<string> groupListUpdate = access.GetGroupListForClient(userName).Select(group => group.Name).ToList();
            IEnumerable<string> newGroups = groupListUpdate.Except(lastGroupsSummaries.Select(summary => summary.GroupName));
            foreach (string newGroup in newGroups)
            {
                Directory.CreateDirectory(Path.Combine(rootFolderPath, newGroup));
                lastGroupsSummaries.Add(new GroupSummary(newGroup, rootFolderPath));
            }
            List<string> removedGroups = lastGroupsSummaries.Select(summary => summary.GroupName).Except(groupListUpdate).ToList();
            foreach (string removedGroup in removedGroups)
            {
                lastGroupsSummaries.RemoveAll(summary => summary.GroupName == removedGroup);
                Directory.Delete(Path.Combine(rootFolderPath, removedGroup), true);
            }
        }

        public List<Group> GetGroupList()
        {
            return access.GetGroupList();
        }

        public Group GetGroupInfo(string groupName)
        {
            return access.GetGroupInfo(groupName);
        }

        public List<ShareLibrary.Models.Client> GetClientsList()
        {
            return access.GetClientList();
        }

        public List<ShareLibrary.Models.Client> GetOnlineClients()
        {
            return access.GetOnlineClientsList();
        }

        public void Save()
        {
            //We do not want to save an empty list.
            if (lastGroupsSummaries != null && lastGroupsSummaries.Any())
            {
                DiskAccessUtils.SaveToDisk(Path.Combine(rootFolderPath, GROUP_SUMMARY_FILE), lastGroupsSummaries);
            }
        }
    }
}
