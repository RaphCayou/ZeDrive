using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ShareLibrary;
using ShareLibrary.Communication;
using ShareLibrary.Models;

namespace Server
{
    public class ServerBusiness : IServerBusiness
    {
        private JobExecuter jobExecuter;
        private DataStore dataStore;
        private List<PendingAction> pendingActions;
        private Task jobExecuterTask;

        public ServerBusiness(string groupsSaveFileName, string clientsSaveFileName)
        {
            dataStore = new DataStore(groupsSaveFileName, clientsSaveFileName);

            jobExecuter = new JobExecuter(dataStore);
            jobExecuterTask = Task.Factory.StartNew(() => jobExecuter.Execute());

            pendingActions = new List<PendingAction>();
        }

        ~ServerBusiness()
        {
            dataStore.Save();
        }

        public bool Connect(string username, string password)
        {
            return dataStore.CheckCredentials(username, password);
        }

        public void CreateUser(string username, string password)
        {
            if (!ParametersHasEmpty(username, password))
            {
                dataStore.CreateUser(username, password);
            }
            else throw new ArgumentException("Les paramètres ne doivent pas être vides.");
        }

        public void CreateGroup(string name, string description, string username)
        {
            if (!ParametersHasEmpty(name, description, username))
            {
                dataStore.CreateGroup(name, description, username);
            }
            else throw new ArgumentException("Les paramètres ne doivent pas être vides.");
        }

        public List<Client> GetClientLists()
        {
            return dataStore.Clients;
        }

        public List<Group> GetGroupList()
        {
            return dataStore.Groups;
        }

        /// <summary>
        /// Update server history,
        /// Send Revision,
        /// Save history on disk
        /// </summary>
        public List<Revision> UpdateServerHistory(Job job)
        {
            AutoResetEvent stopWaitHandle = new AutoResetEvent(false);
            List<Revision> response = new List<Revision>();

            job.CallBack = revisionList =>
            {
                response = revisionList;
                stopWaitHandle.Set();
            };

            jobExecuter.Add(job);
            stopWaitHandle.WaitOne();

            return response;
        }

        public void SendClientGroupInvitation(string adminUsername, string invitedUser, string groupName)
        {
            if (!ParametersHasEmpty(adminUsername, invitedUser, groupName))
            {
                if (!dataStore.CheckAdminRights(adminUsername, groupName))
                {
                    throw new ArgumentException("Vous n'avez pas les droits d'administrateur.");
                }
                if (!dataStore.CheckUserInGroup(invitedUser, groupName))
                {
                   pendingActions.Add(new PendingAction { ClientName = invitedUser, GroupName = groupName, ActionType = ActionTypes.Invite});
                }
                else throw new ArgumentException("L'utilisateur est déjà dans le groupe.");
            }
            else throw new ArgumentException("Les paramètres ne doivent pas être vides.");
        }

        public void SendClientGroupRequest(string username, string groupName)
        {
            if (!ParametersHasEmpty(username, groupName))
            {
                if (!dataStore.CheckUserInGroup(username, groupName))
                {
                    pendingActions.Add(new PendingAction { ClientName = username, GroupName = groupName, ActionType = ActionTypes.Request });
                }
                else throw new ArgumentException("L'utilisateur est déjà dans le groupe.");
            }
            else throw new ArgumentException("Les paramètres ne doivent pas être vides.");
        }
        
        public void KickUserFromGroup(string adminUsername, string username, string groupName)
        {
            if (!ParametersHasEmpty(adminUsername, username, groupName))
            {
                if (!dataStore.CheckAdminRights(adminUsername, groupName)) throw new ArgumentException("Vous n'avez pas les droits d'administrateur.");
                if (!dataStore.CheckUserInGroup(username, groupName)) throw new ArgumentException("L'utilisateur n'est pas dans le groupe.");
                dataStore.RemoveUserFromGroup(adminUsername, username, groupName);
            }
            else throw new ArgumentException("Les paramètres ne doivent pas être vides.");
        }

        /// <summary>
        /// Check for group join request and group invitation
        /// </summary>
        public List<PendingAction> GetNotification(string username)
        {
            List<PendingAction> userPendingActions = new List<PendingAction>();

            //Invites sent to the user
            userPendingActions.AddRange(pendingActions.Where(p => p.ActionType == ActionTypes.Invite && p.ClientName == username));

            //Requests by ClientName towards admin of GroupName
            userPendingActions.AddRange(pendingActions.Where(p => p.ActionType == ActionTypes.Request && username == dataStore.GetGroupAdmin(p.GroupName)));

            return userPendingActions;
        }

        public void AcknowledgeRequest(string adminUsername, string username, string group, bool accept)
        {
            //TODO accepter la requete, faire l'ActionTypes et supprimer le pending
        }

        public void AcknowledgeInvite(string username, string group, bool accept)
        {
            //TODO accepter la requete, faire l'ActionTypes et supprimer le pending
        }

        public void ChangeAdministratorGroup(string usernameCurrentAdmin, string usernameFutureAdmin, string groupName)
        {
            if (!ParametersHasEmpty(usernameCurrentAdmin, usernameFutureAdmin, groupName))
            {
                if (!dataStore.CheckAdminRights(usernameCurrentAdmin, groupName)) throw new ArgumentException("Vous n'avez pas les droits d'administrateur.");
                if (dataStore.CheckUserInGroup(usernameFutureAdmin, groupName))
                {
                    dataStore.ChangeAdminForGroup(usernameCurrentAdmin, usernameFutureAdmin, groupName);
                }
                else throw new ArgumentException("L'utilisateur n'est pas dans le groupe.");
            }
            else throw new ArgumentException("Les paramètres ne doivent pas être vides.");
        }

        private static bool ParametersHasEmpty(params string[] parameters)
        {
            return parameters.Any(string.IsNullOrEmpty);
        }

        public Tuple<string, string> GetSaveFilesNames()
        {
            return dataStore.GetSaveFilesNames();
        }
    }
}
