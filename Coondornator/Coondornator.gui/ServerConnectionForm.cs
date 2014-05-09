using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compass.Coondornator
{
    public partial class ServerConnectionForm : Form
    {

        private CoondornatorForm Parent;

        public ServerConnectionForm(CoondornatorForm parent)
        {
            InitializeComponent();

            Parent = parent;
           
            // Attempt to load defaults
            string name = Properties.Settings.Default.UserName;
            string host = Properties.Settings.Default.Host;
            string password = ServerConnection.ToInsecureString(ServerConnection.DecryptString(Properties.Settings.Default.Password));
            
            if (string.IsNullOrWhiteSpace(host))
            {
                host = Coondornator.DefaultFileServer;
            }

            textBox1.Text = host;
            textBox2.Text = name;
            textBox3.Text = password;

            checkBox1.Checked = true;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            
            bool storeAsDefault = checkBox1.Checked;

            string host = textBox1.Text;
            string userName = textBox2.Text;
            string password = ServerConnection.EncryptString(ServerConnection.ToSecureString(textBox3.Text));

            if (storeAsDefault)
            {
                Properties.Settings.Default.UserName = userName;
                Properties.Settings.Default.Host = host;
                Properties.Settings.Default.Password = password;
                Properties.Settings.Default.Save();
            }

            bool result = await Parent.ConnectAsync(userName, host, password);
            if (!result)
            {
                button1.Enabled = true;
            }
            else
            {
                Close();
            }
        }
    }
}
