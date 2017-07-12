using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ShareLibrary;
using ShareLibrary.Models;

namespace Server
{
    public class ServerBusiness
    {
        private JobExecuter jobExecuter;
        private DataStore dataStore;
        private Task jobExecuterTask;

        public ServerBusiness(string groupsSaveFileName, string clientsSaveFileName)
        {
            dataStore = new DataStore(groupsSaveFileName, clientsSaveFileName);

            jobExecuter = new JobExecuter(dataStore);
            jobExecuterTask = Task.Factory.StartNew(() => jobExecuter.Execute());
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
            else
            {
                throw new ArgumentException("Les paramètres ne doivent pas être vides.");
            }
        }

        public void CreateGroup(string name, string description, string username)
        {
            if (!ParametersHasEmpty(name, description, username))
            {
                dataStore.CreateGroup(name, description, username);
            }
            else
            {
                throw new ArgumentException("Les paramètres ne doivent pas être vides.");
            }
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
        public SyncTransmission UpdateServerHistory(Job job)
        {
            AutoResetEvent stopWaitHandle = new AutoResetEvent(false);
            SyncTransmission response = new SyncTransmission();

            job.CallBack = syncT =>
            {
                response = syncT;
                stopWaitHandle.Set();
            };

            jobExecuter.Add(job);
            stopWaitHandle.WaitOne();

            return response;
        }

        public void SendClientGroupInvitation() { }
        public void SendClientGroupRequest() { }
        /// <summary>
        /// Check for group join request and group invitation
        /// </summary>
        public void GetNotification() { }

        public void ChangeAdministratorGroup(string usernameCurrentAdmin, string usernameFutureAdmin, string groupName)
        {
            if (!ParametersHasEmpty(usernameCurrentAdmin, usernameFutureAdmin, groupName))
            {
                if (!dataStore.CheckAdminRights(usernameCurrentAdmin, groupName)) return;
                if (dataStore.CheckUserInGroup(usernameFutureAdmin, groupName))
                {
                    dataStore.ChangeAdminForGroup(usernameCurrentAdmin, usernameFutureAdmin, groupName);
                }
                else
                {
                    //invite user to group
                }
            }
            else
            {
                throw new ArgumentException("Les paramètres ne doivent pas être vides.");
            }
        }

        public void DeleteClientFromGroup(string adminUserName, string username, string groupName)
        {
            if (!ParametersHasEmpty(adminUserName, username, groupName))
            {
                if (!dataStore.CheckAdminRights(adminUserName, groupName)) return;
                if (!dataStore.CheckUserInGroup(username, groupName)) return;
                dataStore.RemoveUserFromGroup(adminUserName, username, groupName);
            }
            else
            {
                throw new ArgumentException("Les paramètres ne doivent pas être vides.");
            }
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
