using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareLibrary.Models;
using Action = ShareLibrary.Models.Action;

namespace Client
{
    public partial class ClientForm : Form
    {
        private ClientBusiness client;
        private Group currentGroup;
        private string rootPath;
        public ClientForm()
        {
            InitializeComponent();
        }

        private void ClientForm_Load(object sender, EventArgs e)
        {
            CurrentUserGroup.Enabled = false;
            GroupsInformationGroup.Enabled = false;
        }

        private void syncTimer_Tick(object sender, EventArgs e)
        {
            client.SyncWithServer();
            UpdateConnectedUser(client.GetClientsList());
        }

        private void ConnectToServer_Click(object sender, EventArgs e)
        {
            string serverAddress = ServerAddress.Text;
            int serverPort = Convert.ToInt32(ServerPort.Value);
            rootPath = RootFolderPath.Text;
            try
            {
                client = new ClientBusiness(rootPath, serverAddress, serverPort);
                CurrentUserGroup.Enabled = true;
                ServerConnexionGroup.Enabled = false;
            }
            catch (SocketException socketException)
            {
                MessageBox.Show(socketException.Message);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString() + Environment.NewLine + exception.Message);
            }
        }

        private void ConnectUser_Click(object sender, EventArgs e)
        {
            ConnectUserToServer();
        }

        private void CreateUser_Click(object sender, EventArgs e)
        {
            client.CreateUser(UserName.Text, Password.Text);
            ConnectUserToServer();
        }

        private void ConnectUserToServer()
        {
            if (client.Connect(UserName.Text, Password.Text))
            {
                GroupsInformationGroup.Enabled = true;
                CurrentUserGroup.Enabled = false;
            }
            else
            {
                MessageBox.Show("Nom d'usager ou mot de passe invalide.");
            }
        }

        private void CurrentUserGroup_EnabledChanged(object sender, EventArgs e)
        {
            IsUserConnectText.Visible = !CurrentUserGroup.Enabled;
        }

        private void ServerConnexionGroup_EnabledChanged(object sender, EventArgs e)
        {
            IsConnectText.Visible = !ServerConnexionGroup.Enabled;
        }

        private void GroupList_DropDown(object sender, EventArgs e)
        {
            List<Group> allGroups = client.GetGroupList();
            GroupList.DataSource = allGroups;
        }

        private void GroupList_SelectedValueChanged(object sender, EventArgs e)
        {
            currentGroup = (Group)GroupList.SelectedItem;
            if (currentGroup != null)
            {
                GroupDescription.Text = currentGroup.Description;
                //User not in group, so he can ask to join.
                JoinGroup.Enabled = !currentGroup.Members.Exists(client1 => client1.Name == UserName.Text);
                if (currentGroup.Administrator.Name == UserName.Text)
                {
                    AdminGroup.Enabled = true;
                    List<string> groupMembers = currentGroup.Members.Select(client1 => client1.Name).ToList();
                    GroupClientList.DataSource = groupMembers;
                    AllUsersList.DataSource = client.GetClientsList().Select(client1 => client1.Name).ToList().RemoveAll(name => groupMembers.Contains(name));
                }
                else
                {
                    AdminGroup.Enabled = false;
                }
            }
        }

        private void UpdateConnectedUser(List<ShareLibrary.Models.Client> onlineClients)
        {
            OnlineUsers.Clear();
            foreach (ShareLibrary.Models.Client onlineClient in onlineClients)
            {
                OnlineUsers.SelectionColor = onlineClient.LastSeen.AddMinutes(2) < DateTime.Now ? Color.Yellow : Color.Green;
                OnlineUsers.AppendText(onlineClient.Name);
            }
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            client?.Save();
        }

        private void JoinGroup_Click(object sender, EventArgs e)
        {
            client.SendJoinGroupRequest(UserName.Text, currentGroup.Name);
        }

        private void ChangeAdmin_Click(object sender, EventArgs e)
        {
            client.ChangeAdministratorGroup((string)GroupClientList.SelectedItem, currentGroup.Name);
        }

        private void KickFromGroup_Click(object sender, EventArgs e)
        {
            client.KickClientFromGroup((string)GroupClientList.SelectedItem, currentGroup.Name);
        }

        private void InviteToGroup_Click(object sender, EventArgs e)
        {
            client.SendGroupInvitation((string)AllUsersList.SelectedItem, currentGroup.Name);
        }

        private void CreateGroup_Click(object sender, EventArgs e)
        {
            client.CreateGroup(NewGroupName.Text, NewGroupDescription.Text);
        }
    }
}
