using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ShareLibrary.Communication;
using ShareLibrary.Models;
//using System.Threading.Tasks;

namespace Server
{
    public class ServerBusiness : IServerBusiness
    {
        private readonly JobExecuter _jobExecuter;
        private readonly DataStore _dataStore;
        private readonly List<PendingAction> _pendingActions;
        //private Task _jobExecuterTask;

        /// <summary>
        /// Instanciates the server business
        /// </summary>
        /// <param name="groupsSaveFileName">Groups save file name on disk</param>
        /// <param name="clientsSaveFileName">Clients save file name on disk</param>
        public ServerBusiness(string groupsSaveFileName, string clientsSaveFileName)
        {
            _dataStore = new DataStore(groupsSaveFileName, clientsSaveFileName);
            _pendingActions = new List<PendingAction>();

            _jobExecuter = new JobExecuter(_dataStore);
            //_jobExecuterTask = Task.Factory.StartNew(() => _jobExecuter.Execute());
        }

        /// <summary>
        /// Saves the datastore on disk on server destruction
        /// </summary>
        ~ServerBusiness()
        {
            _dataStore.Save();
        }

        /// <summary>
        /// Connects a user on the server
        /// </summary>
        /// <param name="username">Name of the user to connect</param>
        /// <param name="password">Password of the user</param>
        /// <returns>Whether user credentials are valid</returns>
        public bool Connect(string username, string password)
        {
            bool authorized;
            if (!ParametersHasEmpty(username, password))
            {
                authorized = _dataStore.CheckCredentials(username, password);
            }
            else throw new ArgumentException("Les paramètres ne doivent pas être vides.");
            return authorized;
        }

        /// <summary>
        /// Creates a user on the server
        /// </summary>
        /// <param name="username">Name of the user</param>
        /// <param name="password">Password of the user</param>
        public void CreateUser(string username, string password)
        {
            if (!ParametersHasEmpty(username, password))
            {
                _dataStore.CreateUser(username, password);
            }
            else throw new ArgumentException("Les paramètres ne doivent pas être vides.");
        }

        /// <summary>
        /// Creates a group on the server
        /// </summary>
        /// <param name="groupName">Name of the group</param>
        /// <param name="description">Description of the group</param>
        /// <param name="username">Name of the user creating the group (gets admin rights)</param>
        public void CreateGroup(string groupName, string description, string username)
        {
            if (!ParametersHasEmpty(groupName, description, username))
            {
                _dataStore.CreateGroup(groupName, description, username);
            }
            else throw new ArgumentException("Les paramètres ne doivent pas être vides.");
        }

        /// <summary>
        /// Gets the list on clients from the server
        /// </summary>
        /// <returns>List of clients</returns>
        public List<Client> GetClientList()
        {
            return _dataStore.Clients;
        }

        /// <summary>
        /// Gets the list on clients that were last seen less than 5mins ago
        /// </summary>
        /// <returns>List of online clients</returns>
        public List<Client> GetOnlineClientsList()
        {
            return _dataStore.Clients.Where(c => c.LastSeen >= DateTime.Now.AddMinutes(-5)).ToList();
        }

        /// <summary>
        /// Gets the list of groups from the server
        /// </summary>
        /// <returns>List of groups</returns>
        public List<Group> GetGroupList()
        {
            return _dataStore.Groups;
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

            _jobExecuter.Add(job);
            stopWaitHandle.WaitOne();

            _dataStore.UpdateLastSeen(job.Username);

            return response;
        }

        /// <summary>
        /// Sends an invitation to join a group to a user
        /// </summary>
        /// <param name="adminUsername">Name of the user (admin) that sends the invite</param>
        /// <param name="invitedUser">Name of the user that is invited</param>
        /// <param name="groupName">Name of the group that the user is invited to</param>
        public void SendClientGroupInvitation(string adminUsername, string invitedUser, string groupName)
        {
            if (!ParametersHasEmpty(adminUsername, invitedUser, groupName))
            {
                if (!_dataStore.CheckAdminRights(adminUsername, groupName))
                {
                    throw new ArgumentException("Vous n'avez pas les droits requis.");
                }
                if (!_dataStore.CheckUserInGroup(invitedUser, groupName))
                {
                   _pendingActions.Add(new PendingAction { ClientName = invitedUser, GroupName = groupName, ActionType = ActionTypes.Invite});
                }
                else throw new ArgumentException("L'utilisateur est déjà dans le groupe.");
            }
            else throw new ArgumentException("Les paramètres ne doivent pas être vides.");
        }

        /// <summary>
        /// Sends a request to join a group to the group admin
        /// </summary>
        /// <param name="username">Name of the user that sends the request</param>
        /// <param name="groupName">Name of the group that the user wants to join</param>
        public void SendClientGroupRequest(string username, string groupName)
        {
            if (!ParametersHasEmpty(username, groupName))
            {
                if (!_dataStore.CheckUserInGroup(username, groupName))
                {
                    _pendingActions.Add(new PendingAction { ClientName = username, GroupName = groupName, ActionType = ActionTypes.Request });
                }
                else throw new ArgumentException("L'utilisateur est déjà dans le groupe.");
            }
            else throw new ArgumentException("Les paramètres ne doivent pas être vides.");
        }
        
        /// <summary>
        /// Kicks a user from a group
        /// </summary>
        /// <param name="adminUsername">Name of the admininistrator of the group</param>
        /// <param name="username">Name of the user to kick</param>
        /// <param name="groupName">Name of the group</param>
        public void KickUserFromGroup(string adminUsername, string username, string groupName)
        {
            if (!ParametersHasEmpty(adminUsername, username, groupName))
            {
                if (!_dataStore.CheckAdminRights(adminUsername, groupName)) throw new ArgumentException("Vous n'avez pas les droits requis.");
                if (!_dataStore.CheckUserInGroup(username, groupName)) throw new ArgumentException("L'utilisateur n'est pas dans le groupe.");
                _dataStore.RemoveUserFromGroup(adminUsername, username, groupName);
            }
            else throw new ArgumentException("Les paramètres ne doivent pas être vides.");
        }

        /// <summary>
        /// Gets the notification to prompt the user
        /// </summary>
        /// <param name="username">Name of the user to check for notifications</param>
        /// <returns>List of pending actions to prompt the user</returns>
        public List<PendingAction> GetNotification(string username)
        {
            List<PendingAction> userPendingActions = new List<PendingAction>();
            if (!ParametersHasEmpty(username))
            {
                //Invites sent to the user
                userPendingActions.AddRange(_pendingActions.Where(p => p.ActionType == ActionTypes.Invite && p.ClientName == username));

                //Requests by ClientName towards admin of GroupName
                userPendingActions.AddRange(_pendingActions.Where(p => p.ActionType == ActionTypes.Request && username == _dataStore.GetGroupAdmin(p.GroupName)));
            }
            else throw new ArgumentException("Les paramètres ne doivent pas être vides.");
            return userPendingActions;
        }

        /// <summary>
        /// Acknowledges a request to join a group
        /// </summary>
        /// <param name="adminUsername">Name of the admin that accepts of denies the request</param>
        /// <param name="username">Name of the user that requested to join the group</param>
        /// <param name="groupName">Name of the group that the user wanted to join</param>
        /// <param name="accept">Response to the request from the administrator</param>
        public void AcknowledgeRequest(string adminUsername, string username, string groupName, bool accept)
        {
            if (!ParametersHasEmpty(username, groupName))
            {
                if (!_dataStore.CheckAdminRights(adminUsername, groupName))
                {
                    if (accept)
                    {
                        _dataStore.AddUserToGroup(username, groupName);
                    }
                }
                else throw new ArgumentException("Vous n'avez pas les droits requis.");
                _pendingActions.RemoveAll(p => p.ActionType == ActionTypes.Request && p.GroupName == groupName && p.ClientName == username);
            }
            else throw new ArgumentException("Les paramètres ne doivent pas être vides.");
        }

        /// <summary>
        /// Acknowledges an invitation to join a group
        /// </summary>
        /// <param name="username">Name of the user that got invited to a group</param>
        /// <param name="groupName">Name of the group that the user was invited to</param>
        /// <param name="accept">Response to the request from the user</param>
        public void AcknowledgeInvite(string username, string groupName, bool accept)
        {
            if (!ParametersHasEmpty(username, groupName))
            {
                if (!_dataStore.CheckUserInGroup(username, groupName))
                {
                    if (accept)
                    {
                        _dataStore.AddUserToGroup(username, groupName);
                    }
                }
                else throw new ArgumentException("Vous êtes déjà dans le groupe.");
                _pendingActions.RemoveAll(p => p.ActionType == ActionTypes.Invite && p.GroupName == groupName && p.ClientName == username);
            }
            else throw new ArgumentException("Les paramètres ne doivent pas être vides.");
        }

        /// <summary>
        /// Changes the administrator of a group
        /// </summary>
        /// <param name="usernameCurrentAdmin">Name of the current administrator that requests the change</param>
        /// <param name="usernameFutureAdmin">Name of the user that will get administrator rights</param>
        /// <param name="groupName">Name of the impacted group</param>
        public void ChangeAdministratorGroup(string usernameCurrentAdmin, string usernameFutureAdmin, string groupName)
        {
            if (!ParametersHasEmpty(usernameCurrentAdmin, usernameFutureAdmin, groupName))
            {
                if (!_dataStore.CheckAdminRights(usernameCurrentAdmin, groupName)) throw new ArgumentException("Vous n'avez pas les droits d'administrateur.");
                if (_dataStore.CheckUserInGroup(usernameFutureAdmin, groupName))
                {
                    _dataStore.ChangeAdminForGroup(usernameCurrentAdmin, usernameFutureAdmin, groupName);
                }
                else throw new ArgumentException("L'utilisateur n'est pas dans le groupe.");
            }
            else throw new ArgumentException("Les paramètres ne doivent pas être vides.");
        }

        /// <summary>
        /// Validates that the parameters are non empty and non null strings
        /// </summary>
        /// <param name="parameters">String parameters to validate</param>
        /// <returns>True if a parameter is empty or null</returns>
        private static bool ParametersHasEmpty(params string[] parameters)
        {
            return parameters.Any(string.IsNullOrEmpty);
        }

        /// <summary>
        /// Gets the save files names on the disk
        /// </summary>
        /// <returns>Save files names on disk</returns>
        public Tuple<string, string> GetSaveFilesNames()
        {
            return _dataStore.GetSaveFilesNames();
        }
    }
}
