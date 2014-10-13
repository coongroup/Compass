using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace Coon.Compass
{
    partial class frmAbout : Form
    {
        private DateTime AssemblyDateTime
        {
            get
            {
                Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                System.IO.FileInfo file_info = new System.IO.FileInfo(assembly.Location);
                DateTime last_modified = file_info.LastWriteTime;
                return last_modified;
            }
        }

        public frmAbout()
        {
            InitializeComponent();

            label1.Parent = pictureBox1;
            label2.Parent = pictureBox1;
            linkLabel1.Parent = pictureBox1;
            linkLabel2.Parent = pictureBox1;
            linkLabel3.Parent = pictureBox1;
            label3.Parent = pictureBox1;
            label4.Parent = pictureBox1;
            btnOK.Parent = pictureBox1;

            label2.Text = "Version " + CompassForm.GetRunningVersion() + Environment.NewLine + AssemblyDateTime.ToLongDateString();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://onlinelibrary.wiley.com/doi/10.1002/pmic.201000616/abstract");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/dbaileychess/Compass/blob/develop/VERSION.txt");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.gnu.org/licenses/gpl-3.0.txt");
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
