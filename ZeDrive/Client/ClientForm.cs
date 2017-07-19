using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
            client.UpdateServerHistory();
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
            // TODO Add the real groups informations
            GroupList.DataSource = new List<string>{"1", "2", "3", "4", "5", "1", "1", "1"};
        }

        private void GroupList_SelectedValueChanged(object sender, EventArgs e)
        {

        }
    }
}
