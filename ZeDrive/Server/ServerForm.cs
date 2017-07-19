using Server.TcpCommunication;
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

namespace Server
{
    public partial class ServerForm : Form
    {
        ServerBusiness business;
        ServerDispatcher dispatcher;

        public ServerForm()
        {
            InitializeComponent();
        }

        private void ServerForm_Load(object sender, EventArgs e)
        {

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            business = new ServerBusiness("GroupsSave", "ClientSave");
            dispatcher = new ServerDispatcher(business, "127.0.0.1", 4242);
        }
    }
}
