using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ShareLibrary.Models;
using ShareLibrary.Utils;
using FileInfo = ShareLibrary.Models.FileInfo;
// ReSharper disable NotResolvedInText

namespace Server
{
    internal class DataStore
    {
        private readonly string _groupsSaveFilePath;
        private readonly string _clientsSaveFilePath;

        /// <summary>
        /// List of Groups that gets saved to disk
        /// </summary>
        public List<Group> Groups { get; private set; }

        /// <summary>
        /// List of Clients that gets saved to disk
        /// </summary>
        public List<Client> Clients { get; private set; }

        /// <summary>
        /// Instanciates the datastore
        /// Creates or loads existing Groups and Clients
        /// </summary>
        /// <param name="groupsSaveFilePath">Groups save file name on disk</param>
        /// <param name="clientsSaveFilePath">Clients save file name on disk</param>
        public DataStore(string groupsSaveFilePath, string clientsSaveFilePath)
        {
            if (string.IsNullOrEmpty(groupsSaveFilePath) || string.IsNullOrEmpty(clientsSaveFilePath))
            {
                throw new ArgumentNullException("Noms de fichiers vides ou invalides");
            }

            _groupsSaveFilePath = Path.ChangeExtension(groupsSaveFilePath, ".xml");
            _clientsSaveFilePath = Path.ChangeExtension(clientsSaveFilePath, ".xml");

            if (File.Exists(_groupsSaveFilePath))
            {
                LoadGroups();
            }
            else
            {
                Groups = new List<Group>();
            }
            if (File.Exists(_clientsSaveFilePath))
            {
                LoadClients();
            }
            else
            {
                Clients = new List<Client>();
            }
        }

        /// <summary>
        /// Creates the user in datastore (adds a new client)
        /// </summary>
        /// <param name="username">Username of the user</param>
        /// <param name="password">Password of the user</param>
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

        /// <summary>
        /// Creates the group in datastore
        /// </summary>
        /// <param name="groupName">Name of the group</param>
        /// <param name="description">Description of the group</param>
        /// <param name="username">Username of the user creating the group (gets admin rights)</param>
        public void CreateGroup(string groupName, string description, string username)
        {
            if (Groups.Count(c => c.Name == groupName) == 0)
            {
                Groups.Add(new Group
                {
                    Name = groupName,
                    Description = description,
                    Administrator = Clients.FirstOrDefault(c => c.Name == username),
                    Files = new List<FileInfo>(),
                    Members = new List<Client>{ Clients.FirstOrDefault(c => c.Name == username) }
                });
                UpdateLastSeen(username);
            }
            else
            {
                throw new ArgumentException($"Le groupe {groupName} existe déjà.");
            }
        }

        /// <summary>
        /// Loads the Groups save file from disk
        /// </summary>
        private void LoadGroups()
        {
            Groups = DiskAccessUtils.LoadFromDisk<List<Group>>(_groupsSaveFilePath);
        }

        /// <summary>
        /// Loads the Clients save file from disk
        /// </summary>
        private void LoadClients()
        {
            Clients = DiskAccessUtils.LoadFromDisk<List<Client>>(_clientsSaveFilePath);
        }

        /// <summary>
        /// Verifies that the user has entered credentials matching what is stored
        /// </summary>
        /// <param name="username">Username of the user</param>
        /// <param name="password">Password of the user</param>
        /// <returns>Whether user credentials match stored</returns>
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
            return authorized;
        }

        /// <summary>
        /// Verifies that the user has admin rights for the group
        /// </summary>
        /// <param name="username">Username of the user</param>
        /// <param name="groupName">Name of the group to validate rights on</param>
        /// <returns>Whether user has admin rights for the group</returns>
        public bool CheckAdminRights(string username, string groupName)
        {
            UpdateLastSeen(username);
            return Groups.FirstOrDefault(g => g.Name == groupName)?.Administrator.Name == username;
        }

        /// <summary>
        /// Gets the username who owns admin rights for the group
        /// </summary>
        /// <param name="groupName">Name of the group</param>
        /// <returns>Name of the group administrator</returns>
        public string GetGroupAdmin(string groupName)
        {
            return Groups.FirstOrDefault(g => g.Name == groupName)?.Administrator.Name;
        }

        /// <summary>
        /// Verifies that the user is in the group
        /// </summary>
        /// <param name="username">Username of the user</param>
        /// <param name="groupName">Name of the group</param>
        /// <returns>Whether user is in the group</returns>
        public bool CheckUserInGroup(string username, string groupName)
        {
            UpdateLastSeen(username);
            return Groups.FirstOrDefault(g => g.Name == groupName)?.Members.Count(c => c.Name == username) > 0;
        }

        /// <summary>
        /// Changes the administrator for the group
        /// </summary>
        /// <param name="usernameOldAdmin">Old administrator of the group</param>
        /// <param name="usernameNewAdmin">New administrator for the group</param>
        /// <param name="groupName">Name of the group</param>
        public void ChangeAdminForGroup(string usernameOldAdmin, string usernameNewAdmin, string groupName)
        {
            UpdateLastSeen(usernameOldAdmin);
            var group = Groups.FirstOrDefault(g => g.Name == groupName);
            if (group != null) group.Administrator = Clients.FirstOrDefault(c => c.Name == usernameNewAdmin);
        }

        /// <summary>
        /// Adds a user to a group
        /// </summary>
        /// <param name="username">Name of the user</param>
        /// <param name="groupName">Name of the group</param>
        public void AddUserToGroup(string username, string groupName)
        {
            UpdateLastSeen(username);
            Groups.FirstOrDefault(g => g.Name == groupName)?.Members.Add(Clients.FirstOrDefault(c => c.Name == username));
        }

        /// <summary>
        /// Removes a user from a group
        /// </summary>
        /// <param name="usernameAdmin">Name of the group administrator</param>
        /// <param name="username">Name of the user</param>
        /// <param name="groupName">Name of the group</param>
        public void RemoveUserFromGroup(string usernameAdmin, string username, string groupName)
        {
            UpdateLastSeen(usernameAdmin);
            Groups.FirstOrDefault(g => g.Name == groupName)?.Members.Remove(Groups.FirstOrDefault(g => g.Name == groupName)?.Members.FirstOrDefault(c => c.Name == username));
        }

        /// <summary>
        /// Updates the last seen date for a user
        /// </summary>
        /// <param name="username">User to update the date</param>
        public void UpdateLastSeen(string username)
        {
            var client = Clients.FirstOrDefault(c => c.Name == username);
            if (client != null)
                client.LastSeen = DateTime.Now;
        }

        /// <summary>
        /// Saves the Groups and Clients lists on disk
        /// </summary>
        public void Save()
        {
            DiskAccessUtils.SaveToDisk(_groupsSaveFilePath, Groups);
            DiskAccessUtils.SaveToDisk(_clientsSaveFilePath, Clients);
        }

        /// <summary>
        /// Gets the save files names on disk
        /// </summary>
        /// <returns>Save files names on disk</returns>
        public Tuple<string, string> GetSaveFilesNames()
        {
            return new Tuple<string, string>(_groupsSaveFilePath, _clientsSaveFilePath);
        }
    }
}
