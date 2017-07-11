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
    internal class DataStore
    {
        private readonly string _groupsSaveFileName;
        private readonly string _clientsSaveFileName;
        private List<Group> _groups;
        private List<Client> _clients;

        public DataStore(string groupsSaveFileName, string clientsSaveFileName)
        {
            _groupsSaveFileName = groupsSaveFileName;
            _clientsSaveFileName = clientsSaveFileName;

            if (string.IsNullOrEmpty(groupsSaveFileName) || string.IsNullOrEmpty(clientsSaveFileName))
            {
                throw new ArgumentNullException("Noms de fichiers vides ou invalides");
            }

            if (System.IO.File.Exists(groupsSaveFileName))
            {
                LoadGroups();
            }
            else
            {
                _groups = new List<Group>();
            }
            if (System.IO.File.Exists(clientsSaveFileName))
            {
                LoadClients();
            }
            else
            {
                _clients = new List<Client>();
            }
        }

        public void CreateUser(string username, string password)
        {
            if (_clients.Count(c => c.Name == username) == 0)
            {
                _clients.Add(new Client { Name = username, Password = password, LastSeen = DateTime.Now });
            }
            else
            {
                throw new ArgumentException($"Le client {username} existe déjà.");
            }
        }

        public void CreateGroup(string name, string description, string username)
        {
            if (_groups.Count(c => c.Name == name) == 0)
            {
                _groups.Add(new Group { Name = name, Description = description, Administrator = _clients.FirstOrDefault(c => c.Name == name) });
                UpdateLastSeen(username);
            }
            else
            {
                throw new ArgumentException($"Le groupe {username} existe déjà.");
            }
        }

        private void LoadGroups()
        {
            using (var stream = System.IO.File.OpenRead(_groupsSaveFileName))
            {
                var serializer = new XmlSerializer(typeof(List<Group>));
                _groups = serializer.Deserialize(stream) as List<Group>;
            }
        }

        private void LoadClients()
        {
            using (var stream = System.IO.File.OpenRead(_clientsSaveFileName))
            {
                var serializer = new XmlSerializer(typeof(List<Client>));
                _clients = serializer.Deserialize(stream) as List<Client>;
            }
        }

        public bool CheckCredentials(string username, string password)
        {
            bool authorized = false;
            if (_clients.Count(c => c.Name == username && c.Password == password) > 0)
            {
                authorized = true;
                UpdateLastSeen(username);
            }
            return authorized;
        }

        public bool CheckAdminRights(string username, string groupName)
        {
            bool hasAdminRights = false;

            string adminOfGroup = _groups.FirstOrDefault(g => g.Name == groupName).Administrator.Name;
            if (adminOfGroup == username)
            {
                hasAdminRights = true;
            }

            return hasAdminRights;
        }

        private void UpdateLastSeen(string username)
        {
            _clients.FirstOrDefault(c => c.Name == username).LastSeen = DateTime.Now;
        }

        public void Save()
        {
            using (var writer = new System.IO.StreamWriter(_groupsSaveFileName))
            {
                var serializer = new XmlSerializer(_groups.GetType());
                serializer.Serialize(writer, _groups);
                writer.Flush();
            }
            using (var writer = new System.IO.StreamWriter(_clientsSaveFileName))
            {
                var serializer = new XmlSerializer(_clients.GetType());
                serializer.Serialize(writer, _clients);
                writer.Flush();
            }
        }
    }
}
