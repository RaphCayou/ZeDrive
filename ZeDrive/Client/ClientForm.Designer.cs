namespace Client
{
    partial class ClientForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.syncTimer = new System.Windows.Forms.Timer(this.components);
            this.ConnectToServer = new System.Windows.Forms.Button();
            this.CurrentUserGroup = new System.Windows.Forms.GroupBox();
            this.TimeUntilNextSync = new System.Windows.Forms.Label();
            this.IsUserConnectText = new System.Windows.Forms.Label();
            this.CreateUser = new System.Windows.Forms.Button();
            this.ConnectUser = new System.Windows.Forms.Button();
            this.Password = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.UserName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ServerConnexionGroup = new System.Windows.Forms.GroupBox();
            this.IsConnectText = new System.Windows.Forms.Label();
            this.RootFolderPath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ServerPort = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.ServerAddress = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.AdminGroup = new System.Windows.Forms.GroupBox();
            this.InviteToGroup = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.AllUsersList = new System.Windows.Forms.ListBox();
            this.KickFromGroup = new System.Windows.Forms.Button();
            this.ChangeAdmin = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.GroupClientList = new System.Windows.Forms.ListBox();
            this.GroupsInformationGroup = new System.Windows.Forms.GroupBox();
            this.NewGroupDescription = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.CreateGroup = new System.Windows.Forms.Button();
            this.NewGroupName = new System.Windows.Forms.TextBox();
            this.JoinGroup = new System.Windows.Forms.Button();
            this.GroupDescription = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.GroupList = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.OnlineUsers = new System.Windows.Forms.RichTextBox();
            this.CurrentUserGroup.SuspendLayout();
            this.ServerConnexionGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ServerPort)).BeginInit();
            this.AdminGroup.SuspendLayout();
            this.GroupsInformationGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // syncTimer
            // 
            this.syncTimer.Tick += new System.EventHandler(this.syncTimer_Tick);
            // 
            // ConnectToServer
            // 
            this.ConnectToServer.Location = new System.Drawing.Point(866, 11);
            this.ConnectToServer.Name = "ConnectToServer";
            this.ConnectToServer.Size = new System.Drawing.Size(75, 23);
            this.ConnectToServer.TabIndex = 0;
            this.ConnectToServer.Text = "Connexion";
            this.ConnectToServer.UseVisualStyleBackColor = true;
            this.ConnectToServer.Click += new System.EventHandler(this.ConnectToServer_Click);
            // 
            // CurrentUserGroup
            // 
            this.CurrentUserGroup.Controls.Add(this.TimeUntilNextSync);
            this.CurrentUserGroup.Controls.Add(this.IsUserConnectText);
            this.CurrentUserGroup.Controls.Add(this.CreateUser);
            this.CurrentUserGroup.Controls.Add(this.ConnectUser);
            this.CurrentUserGroup.Controls.Add(this.Password);
            this.CurrentUserGroup.Controls.Add(this.label5);
            this.CurrentUserGroup.Controls.Add(this.UserName);
            this.CurrentUserGroup.Controls.Add(this.label1);
            this.CurrentUserGroup.Location = new System.Drawing.Point(12, 61);
            this.CurrentUserGroup.Name = "CurrentUserGroup";
            this.CurrentUserGroup.Size = new System.Drawing.Size(1048, 46);
            this.CurrentUserGroup.TabIndex = 1;
            this.CurrentUserGroup.TabStop = false;
            this.CurrentUserGroup.Text = "Usager courant";
            this.CurrentUserGroup.EnabledChanged += new System.EventHandler(this.CurrentUserGroup_EnabledChanged);
            // 
            // TimeUntilNextSync
            // 
            this.TimeUntilNextSync.AutoSize = true;
            this.TimeUntilNextSync.Location = new System.Drawing.Point(909, 20);
            this.TimeUntilNextSync.Name = "TimeUntilNextSync";
            this.TimeUntilNextSync.Size = new System.Drawing.Size(19, 13);
            this.TimeUntilNextSync.TabIndex = 15;
            this.TimeUntilNextSync.Text = "15";
            // 
            // IsUserConnectText
            // 
            this.IsUserConnectText.AutoSize = true;
            this.IsUserConnectText.ForeColor = System.Drawing.Color.DarkGreen;
            this.IsUserConnectText.Location = new System.Drawing.Point(764, 16);
            this.IsUserConnectText.Name = "IsUserConnectText";
            this.IsUserConnectText.Size = new System.Drawing.Size(56, 13);
            this.IsUserConnectText.TabIndex = 14;
            this.IsUserConnectText.Text = "Connecté!";
            this.IsUserConnectText.Visible = false;
            // 
            // CreateUser
            // 
            this.CreateUser.Location = new System.Drawing.Point(956, 11);
            this.CreateUser.Name = "CreateUser";
            this.CreateUser.Size = new System.Drawing.Size(86, 23);
            this.CreateUser.TabIndex = 13;
            this.CreateUser.Text = "Créer l\'usager";
            this.CreateUser.UseVisualStyleBackColor = true;
            this.CreateUser.Click += new System.EventHandler(this.CreateUser_Click);
            // 
            // ConnectUser
            // 
            this.ConnectUser.Location = new System.Drawing.Point(683, 11);
            this.ConnectUser.Name = "ConnectUser";
            this.ConnectUser.Size = new System.Drawing.Size(75, 23);
            this.ConnectUser.TabIndex = 12;
            this.ConnectUser.Text = "Connexion";
            this.ConnectUser.UseVisualStyleBackColor = true;
            this.ConnectUser.Click += new System.EventHandler(this.ConnectUser_Click);
            // 
            // Password
            // 
            this.Password.Location = new System.Drawing.Point(459, 13);
            this.Password.Name = "Password";
            this.Password.Size = new System.Drawing.Size(218, 20);
            this.Password.TabIndex = 8;
            this.Password.UseSystemPasswordChar = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(378, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Mot de passe:";
            // 
            // UserName
            // 
            this.UserName.Location = new System.Drawing.Point(87, 13);
            this.UserName.Name = "UserName";
            this.UserName.Size = new System.Drawing.Size(285, 20);
            this.UserName.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Nom d\'usager:";
            // 
            // ServerConnexionGroup
            // 
            this.ServerConnexionGroup.Controls.Add(this.IsConnectText);
            this.ServerConnexionGroup.Controls.Add(this.RootFolderPath);
            this.ServerConnexionGroup.Controls.Add(this.label4);
            this.ServerConnexionGroup.Controls.Add(this.ServerPort);
            this.ServerConnexionGroup.Controls.Add(this.label3);
            this.ServerConnexionGroup.Controls.Add(this.ConnectToServer);
            this.ServerConnexionGroup.Controls.Add(this.ServerAddress);
            this.ServerConnexionGroup.Controls.Add(this.label2);
            this.ServerConnexionGroup.Location = new System.Drawing.Point(12, 12);
            this.ServerConnexionGroup.Name = "ServerConnexionGroup";
            this.ServerConnexionGroup.Size = new System.Drawing.Size(1048, 43);
            this.ServerConnexionGroup.TabIndex = 2;
            this.ServerConnexionGroup.TabStop = false;
            this.ServerConnexionGroup.Text = "Connexion au serveur";
            this.ServerConnexionGroup.EnabledChanged += new System.EventHandler(this.ServerConnexionGroup_EnabledChanged);
            // 
            // IsConnectText
            // 
            this.IsConnectText.AutoSize = true;
            this.IsConnectText.ForeColor = System.Drawing.Color.DarkGreen;
            this.IsConnectText.Location = new System.Drawing.Point(947, 16);
            this.IsConnectText.Name = "IsConnectText";
            this.IsConnectText.Size = new System.Drawing.Size(56, 13);
            this.IsConnectText.TabIndex = 15;
            this.IsConnectText.Text = "Connecté!";
            this.IsConnectText.Visible = false;
            // 
            // RootFolderPath
            // 
            this.RootFolderPath.Location = new System.Drawing.Point(642, 13);
            this.RootFolderPath.Name = "RootFolderPath";
            this.RootFolderPath.Size = new System.Drawing.Size(218, 20);
            this.RootFolderPath.TabIndex = 11;
            this.RootFolderPath.Text = "./";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(495, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(141, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Adresse du dossier principal:";
            // 
            // ServerPort
            // 
            this.ServerPort.Location = new System.Drawing.Point(425, 14);
            this.ServerPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.ServerPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ServerPort.Name = "ServerPort";
            this.ServerPort.Size = new System.Drawing.Size(64, 20);
            this.ServerPort.TabIndex = 9;
            this.ServerPort.Value = new decimal(new int[] {
            4242,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(337, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Port du serveur:";
            // 
            // ServerAddress
            // 
            this.ServerAddress.Location = new System.Drawing.Point(113, 13);
            this.ServerAddress.Name = "ServerAddress";
            this.ServerAddress.Size = new System.Drawing.Size(218, 20);
            this.ServerAddress.TabIndex = 7;
            this.ServerAddress.Text = "127.0.0.1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Adresse du serveur:";
            // 
            // AdminGroup
            // 
            this.AdminGroup.Controls.Add(this.InviteToGroup);
            this.AdminGroup.Controls.Add(this.label10);
            this.AdminGroup.Controls.Add(this.AllUsersList);
            this.AdminGroup.Controls.Add(this.KickFromGroup);
            this.AdminGroup.Controls.Add(this.ChangeAdmin);
            this.AdminGroup.Controls.Add(this.label9);
            this.AdminGroup.Controls.Add(this.GroupClientList);
            this.AdminGroup.Enabled = false;
            this.AdminGroup.Location = new System.Drawing.Point(9, 183);
            this.AdminGroup.Name = "AdminGroup";
            this.AdminGroup.Size = new System.Drawing.Size(784, 335);
            this.AdminGroup.TabIndex = 3;
            this.AdminGroup.TabStop = false;
            this.AdminGroup.Text = "Gestion d\'administrateur";
            // 
            // InviteToGroup
            // 
            this.InviteToGroup.Location = new System.Drawing.Point(605, 157);
            this.InviteToGroup.Name = "InviteToGroup";
            this.InviteToGroup.Size = new System.Drawing.Size(170, 23);
            this.InviteToGroup.TabIndex = 20;
            this.InviteToGroup.Text = "Inviter à ce groupe";
            this.InviteToGroup.UseVisualStyleBackColor = true;
            this.InviteToGroup.Click += new System.EventHandler(this.InviteToGroup_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(393, 16);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(161, 13);
            this.label10.TabIndex = 23;
            this.label10.Text = "Usagers non membre du groupe:";
            // 
            // AllUsersList
            // 
            this.AllUsersList.FormattingEnabled = true;
            this.AllUsersList.Location = new System.Drawing.Point(396, 29);
            this.AllUsersList.Name = "AllUsersList";
            this.AllUsersList.Size = new System.Drawing.Size(203, 290);
            this.AllUsersList.TabIndex = 22;
            // 
            // KickFromGroup
            // 
            this.KickFromGroup.Location = new System.Drawing.Point(218, 197);
            this.KickFromGroup.Name = "KickFromGroup";
            this.KickFromGroup.Size = new System.Drawing.Size(172, 23);
            this.KickFromGroup.TabIndex = 21;
            this.KickFromGroup.Text = "Enlever du groupe";
            this.KickFromGroup.UseVisualStyleBackColor = true;
            this.KickFromGroup.Click += new System.EventHandler(this.KickFromGroup_Click);
            // 
            // ChangeAdmin
            // 
            this.ChangeAdmin.Location = new System.Drawing.Point(218, 97);
            this.ChangeAdmin.Name = "ChangeAdmin";
            this.ChangeAdmin.Size = new System.Drawing.Size(172, 23);
            this.ChangeAdmin.TabIndex = 20;
            this.ChangeAdmin.Text = "Rendre administrateur du groupe";
            this.ChangeAdmin.UseVisualStyleBackColor = true;
            this.ChangeAdmin.Click += new System.EventHandler(this.ChangeAdmin_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 16);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(140, 13);
            this.label9.TabIndex = 16;
            this.label9.Text = "Usagers membre du groupe:";
            // 
            // GroupClientList
            // 
            this.GroupClientList.FormattingEnabled = true;
            this.GroupClientList.Location = new System.Drawing.Point(9, 29);
            this.GroupClientList.Name = "GroupClientList";
            this.GroupClientList.Size = new System.Drawing.Size(203, 290);
            this.GroupClientList.TabIndex = 0;
            // 
            // GroupsInformationGroup
            // 
            this.GroupsInformationGroup.Controls.Add(this.NewGroupDescription);
            this.GroupsInformationGroup.Controls.Add(this.label12);
            this.GroupsInformationGroup.Controls.Add(this.label11);
            this.GroupsInformationGroup.Controls.Add(this.CreateGroup);
            this.GroupsInformationGroup.Controls.Add(this.NewGroupName);
            this.GroupsInformationGroup.Controls.Add(this.JoinGroup);
            this.GroupsInformationGroup.Controls.Add(this.GroupDescription);
            this.GroupsInformationGroup.Controls.Add(this.label8);
            this.GroupsInformationGroup.Controls.Add(this.label6);
            this.GroupsInformationGroup.Controls.Add(this.GroupList);
            this.GroupsInformationGroup.Controls.Add(this.AdminGroup);
            this.GroupsInformationGroup.Location = new System.Drawing.Point(12, 113);
            this.GroupsInformationGroup.Name = "GroupsInformationGroup";
            this.GroupsInformationGroup.Size = new System.Drawing.Size(800, 524);
            this.GroupsInformationGroup.TabIndex = 4;
            this.GroupsInformationGroup.TabStop = false;
            this.GroupsInformationGroup.Text = "Gestion des groupes";
            // 
            // NewGroupDescription
            // 
            this.NewGroupDescription.Location = new System.Drawing.Point(171, 122);
            this.NewGroupDescription.Multiline = true;
            this.NewGroupDescription.Name = "NewGroupDescription";
            this.NewGroupDescription.Size = new System.Drawing.Size(415, 55);
            this.NewGroupDescription.TabIndex = 21;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 131);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(159, 13);
            this.label12.TabIndex = 23;
            this.label12.Text = "Description du nouveau groupe:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 99);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(128, 13);
            this.label11.TabIndex = 22;
            this.label11.Text = "Nom du nouveau groupe:";
            // 
            // CreateGroup
            // 
            this.CreateGroup.Location = new System.Drawing.Point(592, 120);
            this.CreateGroup.Name = "CreateGroup";
            this.CreateGroup.Size = new System.Drawing.Size(192, 23);
            this.CreateGroup.TabIndex = 22;
            this.CreateGroup.Text = "Créer un groupe";
            this.CreateGroup.UseVisualStyleBackColor = true;
            this.CreateGroup.Click += new System.EventHandler(this.CreateGroup_Click);
            // 
            // NewGroupName
            // 
            this.NewGroupName.Location = new System.Drawing.Point(171, 92);
            this.NewGroupName.Name = "NewGroupName";
            this.NewGroupName.Size = new System.Drawing.Size(415, 20);
            this.NewGroupName.TabIndex = 20;
            // 
            // JoinGroup
            // 
            this.JoinGroup.Location = new System.Drawing.Point(592, 19);
            this.JoinGroup.Name = "JoinGroup";
            this.JoinGroup.Size = new System.Drawing.Size(192, 23);
            this.JoinGroup.TabIndex = 19;
            this.JoinGroup.Text = "Rejoindre ce groupe";
            this.JoinGroup.UseVisualStyleBackColor = true;
            this.JoinGroup.Click += new System.EventHandler(this.JoinGroup_Click);
            // 
            // GroupDescription
            // 
            this.GroupDescription.Location = new System.Drawing.Point(75, 50);
            this.GroupDescription.Name = "GroupDescription";
            this.GroupDescription.Size = new System.Drawing.Size(718, 43);
            this.GroupDescription.TabIndex = 18;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 50);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(63, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Description:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(210, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Choisissez un groupe pour faire une action.";
            // 
            // GroupList
            // 
            this.GroupList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GroupList.FormattingEnabled = true;
            this.GroupList.Location = new System.Drawing.Point(222, 19);
            this.GroupList.Name = "GroupList";
            this.GroupList.Size = new System.Drawing.Size(363, 21);
            this.GroupList.TabIndex = 4;
            this.GroupList.DropDown += new System.EventHandler(this.GroupList_DropDown);
            this.GroupList.SelectedValueChanged += new System.EventHandler(this.GroupList_SelectedValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label7.Location = new System.Drawing.Point(818, 119);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(132, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Nom des usagers en ligne:";
            // 
            // OnlineUsers
            // 
            this.OnlineUsers.Location = new System.Drawing.Point(818, 135);
            this.OnlineUsers.Name = "OnlineUsers";
            this.OnlineUsers.ReadOnly = true;
            this.OnlineUsers.Size = new System.Drawing.Size(242, 502);
            this.OnlineUsers.TabIndex = 0;
            this.OnlineUsers.Text = "";
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1072, 649);
            this.Controls.Add(this.OnlineUsers);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.GroupsInformationGroup);
            this.Controls.Add(this.ServerConnexionGroup);
            this.Controls.Add(this.CurrentUserGroup);
            this.Name = "ClientForm";
            this.Text = "Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClientForm_FormClosing);
            this.Load += new System.EventHandler(this.ClientForm_Load);
            this.CurrentUserGroup.ResumeLayout(false);
            this.CurrentUserGroup.PerformLayout();
            this.ServerConnexionGroup.ResumeLayout(false);
            this.ServerConnexionGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ServerPort)).EndInit();
            this.AdminGroup.ResumeLayout(false);
            this.AdminGroup.PerformLayout();
            this.GroupsInformationGroup.ResumeLayout(false);
            this.GroupsInformationGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer syncTimer;
        private System.Windows.Forms.Button ConnectToServer;
        private System.Windows.Forms.GroupBox CurrentUserGroup;
        private System.Windows.Forms.TextBox UserName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox ServerConnexionGroup;
        private System.Windows.Forms.NumericUpDown ServerPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox ServerAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox AdminGroup;
        private System.Windows.Forms.GroupBox GroupsInformationGroup;
        private System.Windows.Forms.TextBox RootFolderPath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox Password;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button ConnectUser;
        private System.Windows.Forms.Button CreateUser;
        private System.Windows.Forms.Label IsUserConnectText;
        private System.Windows.Forms.Label IsConnectText;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox GroupList;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RichTextBox OnlineUsers;
        private System.Windows.Forms.Button JoinGroup;
        private System.Windows.Forms.Label GroupDescription;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ListBox GroupClientList;
        private System.Windows.Forms.Button InviteToGroup;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ListBox AllUsersList;
        private System.Windows.Forms.Button KickFromGroup;
        private System.Windows.Forms.Button ChangeAdmin;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox NewGroupDescription;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button CreateGroup;
        private System.Windows.Forms.TextBox NewGroupName;
        private System.Windows.Forms.Label TimeUntilNextSync;
    }
}

