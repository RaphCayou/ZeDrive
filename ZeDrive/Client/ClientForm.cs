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
        private const int TIMER_TICK = 1000;
        private const int SYNC_INTERVAL = 15;
        private const string MESSAGE_ERREUR = "Une erreur inattendue est survenue.";
        private int currentTick = 0;
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
            syncTimer.Interval = TIMER_TICK;
        }

        private void syncTimer_Tick(object sender, EventArgs e)
        {
            ++currentTick;
            if (currentTick % SYNC_INTERVAL == 0)
            {
                Sync();
                currentTick = 0;
            }
            TimeUntilNextSync.Text = (SYNC_INTERVAL - currentTick % SYNC_INTERVAL).ToString();
        }

        private void Sync()
        {
            try
            {

                client.SyncWithServer();
                UpdateConnectedUser(client.GetClientsList());
                foreach (PendingAction action in client.GetPendingActions())
                {
                    if (action.ActionType == ActionTypes.Invite)
                    {
                        DialogResult dialogResult =
                            MessageBox.Show($"Voulez-vous rejoindre le group \"{action.GroupName}\" ?",
                                "Nouvelle invitation!", MessageBoxButtons.YesNo);
                        client.AcceptInvitation(action.GroupName, dialogResult == DialogResult.Yes);
                    }
                    else
                    {
                        DialogResult dialogResult =
                            MessageBox.Show(
                                $"Acceptez-vous que {action.ClientName} rejoinde le group \"{action.GroupName}\" ?",
                                "Nouvelle demande!", MessageBoxButtons.YesNo);
                        client.AcknowledgeRequest(action.ClientName, action.GroupName,
                            dialogResult == DialogResult.Yes);
                    }
                }
                UpdateGroupInformation();
                client.Save();
            }
            catch (Exception exception)
            {
                MessageBox.Show(MESSAGE_ERREUR + Environment.NewLine + exception.Message);
            }
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
                MessageBox.Show(MESSAGE_ERREUR + Environment.NewLine + exception.ToString() + Environment.NewLine + exception.Message);
            }
        }

        private void ConnectUser_Click(object sender, EventArgs e)
        {
            ConnectUserToServer();
        }

        private void CreateUser_Click(object sender, EventArgs e)
        {
            try
            {
                if (client.CreateUser(UserName.Text, Password.Text))
                {
                    ConnectUserToServer();
                }
                else
                {
                    MessageBox.Show("Le nom d'usager est déjà utilisé.");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(MESSAGE_ERREUR + Environment.NewLine + exception.Message);
            }
        }

        private void ConnectUserToServer()
        {
            try
            {
                if (client.Connect(UserName.Text, Password.Text))
                {
                    GroupsInformationGroup.Enabled = true;
                    CurrentUserGroup.Enabled = false;
                    Sync();
                    syncTimer.Start();
                }
                else
                {
                    MessageBox.Show("Nom d'usager ou mot de passe invalide.");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(MESSAGE_ERREUR + Environment.NewLine + exception.Message);
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
            try
            {
                List<Group> allGroups = client.GetGroupList();
                GroupList.DataSource = allGroups;
            }
            catch (Exception exception)
            {
                MessageBox.Show(MESSAGE_ERREUR + Environment.NewLine + exception.Message);
            }
        }

        private void GroupList_SelectedValueChanged(object sender, EventArgs e)
        {
            UpdateGroupInformation();
        }

        private void UpdateGroupInformation()
        {
            currentGroup = (Group)GroupList.SelectedItem;
            if (currentGroup != null)
            {
                try
                {
                    currentGroup = client.GetGroupInfo(currentGroup.Name);
                    GroupDescription.Text = currentGroup.Description;
                    //User not in group, so he can ask to join.
                    JoinGroup.Enabled = !currentGroup.Members.Exists(client1 => client1.Name == UserName.Text);
                    if (currentGroup.Administrator.Name == UserName.Text)
                    {
                        AdminGroup.Enabled = true;
                        List<string> groupMembers = currentGroup.Members.Select(client1 => client1.Name).ToList();
                        GroupClientList.DataSource = groupMembers;
                        List<string> userNotInGroup = client.GetClientsList().Select(client1 => client1.Name).ToList();
                        userNotInGroup.RemoveAll(name => groupMembers.Contains(name));
                        AllUsersList.DataSource = userNotInGroup;
                    }
                    else
                    {
                        AdminGroup.Enabled = false;
                        AllUsersList.DataSource = null;
                        GroupClientList.DataSource = null;
                    }

                }
                catch (Exception exception)
                {
                    MessageBox.Show(MESSAGE_ERREUR + Environment.NewLine + exception.Message);
                }
            }
        }

        private void UpdateConnectedUser(List<ShareLibrary.Models.Client> onlineClients)
        {
            OnlineUsers.Clear();
            foreach (ShareLibrary.Models.Client onlineClient in onlineClients)
            {
                OnlineUsers.SelectionColor = onlineClient.LastSeen.AddMinutes(2) < DateTime.Now ? Color.DarkOrange : Color.Green;
                OnlineUsers.AppendText(onlineClient.Name + Environment.NewLine);
            }
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            client?.Save();
        }

        private void JoinGroup_Click(object sender, EventArgs e)
        {
            try
            {
                client.SendJoinGroupRequest(UserName.Text, currentGroup.Name);
            }
            catch (Exception exception)
            {
                MessageBox.Show(MESSAGE_ERREUR + Environment.NewLine + exception.Message);
            }
        }

        private void ChangeAdmin_Click(object sender, EventArgs e)
        {
            try
            {
                client.ChangeAdministratorGroup((string)GroupClientList.SelectedItem, currentGroup.Name);
                UpdateGroupInformation();
            }
            catch (Exception exception)
            {
                MessageBox.Show(MESSAGE_ERREUR + Environment.NewLine + exception.Message);
            }
        }

        private void KickFromGroup_Click(object sender, EventArgs e)
        {
            try
            {
                client.KickClientFromGroup((string)GroupClientList.SelectedItem, currentGroup.Name);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            UpdateGroupInformation();
        }

        private void InviteToGroup_Click(object sender, EventArgs e)
        {
            try
            {
                client.SendGroupInvitation((string)AllUsersList.SelectedItem, currentGroup.Name);
            }
            catch (Exception exception)
            {
                MessageBox.Show(MESSAGE_ERREUR + Environment.NewLine + exception.Message);
            }
        }

        private void CreateGroup_Click(object sender, EventArgs e)
        {
            try
            {
                if (!client.CreateGroup(NewGroupName.Text.Trim(), NewGroupDescription.Text.Trim()))
                {
                    MessageBox.Show("Ce nom de groupe existe déjà.");
                }
                else
                {
                    NewGroupName.Text = "";
                    NewGroupDescription.Text = "";
                    MessageBox.Show("Le groupe a été créé.");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(MESSAGE_ERREUR + Environment.NewLine + exception.Message);
            }
        }
    }
}
