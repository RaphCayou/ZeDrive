using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
        private string rootPath;
        public ClientForm()
        {
            InitializeComponent();
        }

        private void ClientForm_Load(object sender, EventArgs e)
        {
            CurrentUserGroup.Enabled = false;
            //GroupsInformationGroup.Enabled = false;  // TODO JP reactivate that line
        }

        private void syncTimer_Tick(object sender, EventArgs e)
        {
            client.SyncWithServer();

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
            IsUserConnectText.Visible = CurrentUserGroup.Enabled;
        }

        private void ServerConnexionGroup_EnabledChanged(object sender, EventArgs e)
        {
            IsConnectText.Visible = ServerConnexionGroup.Enabled;
        }

        private void GroupList_DropDown(object sender, EventArgs e)
        {
            List<Group> allGroups = client.GetGroupList();
            GroupList.DataSource = allGroups;
        }

        private void GroupList_SelectedValueChanged(object sender, EventArgs e)
        {
            // TODO update the UI based on the new selected group.
            Group currentGroup = (Group)GroupList.SelectedItem;
            AdminGroup.Enabled = currentGroup.Administrator.Name == UserName.Text;

        }

        private void UpdateConnectedUser(List<ShareLibrary.Models.Client> onlineClients)
        {
            OnlineUsers.Clear();
            foreach (ShareLibrary.Models.Client onlineClient in onlineClients)
            {
                if (onlineClient.LastSeen.AddMinutes(2) < DateTime.Now)
                {
                    OnlineUsers.SelectionColor = Color.Yellow;
                }
                else
                {
                    OnlineUsers.SelectionColor = Color.Green;
                }
                OnlineUsers.AppendText(onlineClient.Name);
            }
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // TODO faire la sauvegarde des derniers group summaries.
        }
    }
}
