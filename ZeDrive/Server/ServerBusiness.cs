using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public ServerBusiness(string groupsSaveFileName, string clientsSaveFileName)
        {
            jobExecuter = new JobExecuter();
            Task.Factory.StartNew(() => jobExecuter.Execute());

            dataStore = new DataStore(groupsSaveFileName, clientsSaveFileName);
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

        public void GetClientLists() { }
        public void GetGroupList() { }

        /// <summary>
        /// Update server history,
        /// Send Revision,
        /// Save history on disk
        /// </summary>
        public void UpdateServerHistory(Job job)
        {
            jobExecuter.Add(job);
        }

        public void SendClientGroupInvitation() { }
        public void SendClientGroupRequest() { }
        /// <summary>
        /// Check for group join request and group invitation
        /// </summary>
        public void GetNotification() { }
        public void ChangeAdministratorGroup() { }
        public void DeleteClientFromGroup() { }

        private bool HasAdminRights(string username, string groupName)
        {
            return dataStore.CheckAdminRights(username, groupName);
        }

        private static bool ParametersHasEmpty(params string[] parameters)
        {
            return parameters.Any(string.IsNullOrEmpty);
        }
    }
}
