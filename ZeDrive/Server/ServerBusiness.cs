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
        private string groupsSaveFileName;
        private string clientsSaveFileName;
        private List<Group> groups;
        private List<Client> clients;
        private JobExecuter jobExecuter;

        public ServerBusiness(string groupsSaveFileName, string clientsSaveFileName)
        {
            jobExecuter = new JobExecuter();
            Task.Factory.StartNew(() => jobExecuter.Execute());


            this.groupsSaveFileName = groupsSaveFileName;
            this.clientsSaveFileName = clientsSaveFileName;
            try
            {
                if (string.IsNullOrEmpty(groupsSaveFileName) || string.IsNullOrEmpty(clientsSaveFileName))
                {
                    throw new ArgumentNullException("Noms de fichiers invalides");
                }

                if (System.IO.File.Exists(groupsSaveFileName))
                {
                    LoadGroups();
                }
                else
                {
                    groups = new List<Group>();
                }
                if (System.IO.File.Exists(clientsSaveFileName))
                {
                    LoadClients();
                }
                else
                {
                    clients = new List<Client>();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        ~ServerBusiness()
        {
            Save();
        }

        public void Save()
        {
            using (var writer = new System.IO.StreamWriter(groupsSaveFileName))
            {
                var serializer = new XmlSerializer(groups.GetType());
                serializer.Serialize(writer, groups);
                writer.Flush();
            }
            using (var writer = new System.IO.StreamWriter(clientsSaveFileName))
            {
                var serializer = new XmlSerializer(clients.GetType());
                serializer.Serialize(writer, clients);
                writer.Flush();
            }
        }

        private void LoadGroups()
        {
            using (var stream = System.IO.File.OpenRead(groupsSaveFileName))
            {
                var serializer = new XmlSerializer(typeof(List<Group>));
                groups = serializer.Deserialize(stream) as List<Group>;
            }
        }

        private void LoadClients()
        {
            using (var stream = System.IO.File.OpenRead(clientsSaveFileName))
            {
                var serializer = new XmlSerializer(typeof(List<Client>));
                clients = serializer.Deserialize(stream) as List<Client>;
            }
        }

        public bool Connect(string username, string password)
        {
            bool authorized = false;
            if (clients.Where(c => c.Name == username && c.Password == password).Count() > 0)
            {
                authorized = true;
                UpdateLastSeen(username);
            }
            return authorized;
        }

        public void CreateUser(string username, string password)
        {
            if (!ParametersHasEmpty(username, password))
            {
                if (clients.Where(c => c.Name == username).Count() == 0)
                {
                    clients.Add(new Client { Name = username, Password = password, LastSeen = DateTime.Now });
                }
                else
                {
                    throw new ArgumentException($"Le client {username} existe déjà.");
                }
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
                if (groups.Where(c => c.Name == name).Count() == 0)
                {
                    groups.Add(new Group { Name = name, Description = description, Administrator = clients.Where(c => c.Name == name).FirstOrDefault()});
                    UpdateLastSeen(username);
                }
                else
                {
                    throw new ArgumentException($"Le groupe {username} existe déjà.");
                }
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

        private void UpdateLastSeen(string username)
        {
            clients.Where(c => c.Name == username).FirstOrDefault().LastSeen = DateTime.Now;
        }

        private bool HasAdminRights(string username, string groupName)
        {
            bool hasAdminRights = false;

            string adminOfGroup = groups.Where(g => g.Name == groupName).FirstOrDefault().Administrator.Name;
            if (adminOfGroup == username)
            {
                hasAdminRights = true;
            }

            return hasAdminRights;
        }

        private bool ParametersHasEmpty(params string[] parameters)
        {
            foreach (string p in parameters)
            {
                if (string.IsNullOrEmpty(p))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
