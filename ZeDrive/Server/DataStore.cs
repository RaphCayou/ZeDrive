using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ShareLibrary.Models;
using FileInfo = ShareLibrary.Models.FileInfo;

namespace Server
{
    internal class DataStore
    {
        private readonly string _groupsSaveFileName;
        private readonly string _clientsSaveFileName;
        public List<Group> Groups { get; private set; }
        public List<Client> Clients { get; private set; }

        public DataStore(string groupsSaveFileName, string clientsSaveFileName)
        {
            if (string.IsNullOrEmpty(groupsSaveFileName) || string.IsNullOrEmpty(clientsSaveFileName))
            {
                throw new ArgumentNullException("Noms de fichiers vides ou invalides");
            }

            _groupsSaveFileName = Path.GetFileNameWithoutExtension(groupsSaveFileName) + ".xml";
            _clientsSaveFileName = Path.GetFileNameWithoutExtension(clientsSaveFileName) + ".xml";

            if (File.Exists(_groupsSaveFileName))
            {
                LoadGroups();
            }
            else
            {
                Groups = new List<Group>();
            }
            if (File.Exists(_clientsSaveFileName))
            {
                LoadClients();
            }
            else
            {
                Clients = new List<Client>();
            }
        }

        public void CreateUser(string username, string password)
        {
            if (Clients.Count(c => c.Name == username) == 0)
            {
                Clients.Add(new Client { Name = username, Password = password, LastSeen = DateTime.Now });
            }
            else
            {
                throw new ArgumentException($"Le client {username} existe déjà.");
            }
        }

        public void CreateGroup(string name, string description, string username)
        {
            if (Groups.Count(c => c.Name == name) == 0)
            {
                Groups.Add(new Group
                {
                    Name = name,
                    Description = description,
                    Administrator = Clients.FirstOrDefault(c => c.Name == name),
                    Files = new List<FileInfo>(),
                    Members = new List<Client>{ Clients.FirstOrDefault(c => c.Name == username) }
                });
                UpdateLastSeen(username);
            }
            else
            {
                throw new ArgumentException($"Le groupe {username} existe déjà.");
            }
        }

        private void LoadGroups()
        {
            using (var stream = File.OpenRead(_groupsSaveFileName))
            {
                var serializer = new XmlSerializer(typeof(List<Group>));
                Groups = serializer.Deserialize(stream) as List<Group>;
            }
        }

        private void LoadClients()
        {
            using (var stream = File.OpenRead(_clientsSaveFileName))
            {
                var serializer = new XmlSerializer(typeof(List<Client>));
                Clients = serializer.Deserialize(stream) as List<Client>;
            }
        }

        public bool CheckCredentials(string username, string password)
        {
            bool authorized = false;
            if (Clients.Count(c => c.Name == username) > 0)
            {
                if (Clients.Count(c => c.Password == password) > 0)
                {
                    authorized = true;
                    UpdateLastSeen(username);
                }
            }
            else
            {
                //TODO Crée client
            }
            return authorized;
        }

        public bool CheckAdminRights(string username, string groupName)
        {
            UpdateLastSeen(username);
            return Groups.FirstOrDefault(g => g.Name == groupName).Administrator.Name == username;
        }

        public bool CheckUserInGroup(string username, string groupName)
        {
            UpdateLastSeen(username);
            return Groups.FirstOrDefault(g => g.Name == groupName).Members.Count(c => c.Name == username) > 0;
        }

        public void ChangeAdminForGroup(string usernameOldAdmin, string usernameNewAdmin, string groupName)
        {
            UpdateLastSeen(usernameOldAdmin);
            Groups.FirstOrDefault(g => g.Name == groupName).Administrator = Clients.FirstOrDefault(c => c.Name == usernameNewAdmin);
        }

        public void AddUserToGroup(string username, string groupName)
        {
            UpdateLastSeen(username);
            Groups.FirstOrDefault(g => g.Name == groupName)?.Members.Add(Clients.FirstOrDefault(c => c.Name == username));
        }

        public void RemoveUserFromGroup(string usernameAdmin, string username, string groupName)
        {
            UpdateLastSeen(usernameAdmin);
            Groups.FirstOrDefault(g => g.Name == groupName)?.Members.Remove(Groups.FirstOrDefault(g => g.Name == groupName)?.Members.FirstOrDefault(c => c.Name == username));
        }

        private void UpdateLastSeen(string username)
        {
            Clients.FirstOrDefault(c => c.Name == username).LastSeen = DateTime.Now;
        }

        public void Save()
        {
            using (var writer = new StreamWriter(_groupsSaveFileName))
            {
                var serializer = new XmlSerializer(Groups.GetType());
                serializer.Serialize(writer, Groups);
                writer.Flush();
            }
            using (var writer = new StreamWriter(_clientsSaveFileName))
            {
                var serializer = new XmlSerializer(Clients.GetType());
                serializer.Serialize(writer, Clients);
                writer.Flush();
            }
        }

        public Tuple<string, string> GetSaveFilesNames()
        {
            return new Tuple<string, string>(_groupsSaveFileName, _clientsSaveFileName);
        }
    }
}
