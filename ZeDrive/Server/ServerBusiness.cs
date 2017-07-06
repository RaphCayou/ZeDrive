using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Server
{
    public class ServerBusiness
    {
        private string groupsSaveFileName;
        private string clientsSaveFileName;
        private List<Models.Group> groups;
        private List<Models.Client> clients;

        public ServerBusiness(string groupsSaveFileName, string clientsSaveFileName)
        {
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
                    groups = new List<Models.Group>();
                }
                if (System.IO.File.Exists(clientsSaveFileName))
                {
                    LoadClients();
                }
                else
                {
                    clients = new List<Models.Client>();
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
                var serializer = new XmlSerializer(typeof(List<Models.Group>));
                groups = serializer.Deserialize(stream) as List<Models.Group>;
            }
        }

        private void LoadClients()
        {
            using (var stream = System.IO.File.OpenRead(clientsSaveFileName))
            {
                var serializer = new XmlSerializer(typeof(List<Models.Client>));
                clients = serializer.Deserialize(stream) as List<Models.Client>;
            }
        }

        public void Connect() { }

        public void CreateUser(string username, string password)
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                if (clients.Where(c => c.Name == username).Count() == 0)
                {
                    clients.Add(new Models.Client { Name = username, Password = password, LastSeen = DateTime.Now });
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
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(description) && !string.IsNullOrEmpty(username))
            {
                if (groups.Where(c => c.Name == name).Count() == 0)
                {
                    groups.Add(new Models.Group { Name = name, Description = description, Administrator = clients.Where(c => c.Name == name).FirstOrDefault() });
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
        public void UpdateServerHistory() { }
        public void SendClientGroupInvitation() { }
        public void SendClientGroupRequest() { }
        /// <summary>
        /// Check for group join request and group invitation
        /// </summary>
        public void GetNotification() { }
        public void ChangeAdministratorGroup() { }
        public void DeleteClientFromGroup() { }
    }
}
