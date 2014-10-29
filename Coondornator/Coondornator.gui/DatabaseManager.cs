using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compass.Coondornator
{
    public partial class DatabaseManager : Form
    {
        public DatabaseManager()
        {
            InitializeComponent();
            localMachineOutputDirectoryBox.ForeColor = Color.Gray;
            SetDate();
        }

        private void SetDate()
        {
            var dateTime = DateTime.Now.ToString("MM/dd/yy");
            string[] dtParts = dateTime.Split('/');
            monthTextBox.Text = dtParts[0];
            dayTextBox.Text = dtParts[1];
            yearTextBox.Text = dtParts[2];
        }

        private void taxIDLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MessageBox.Show(
             "Taxonomic ID# should be a 6-digit number with leading zeroes.\n\nExample - Mus musculus Taxonomic ID#: 010090\n\nWould you like more information?", "Taxonomic ID", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start("http://www.ncbi.nlm.nih.gov/Taxonomy/Browser/wwwtax.cgi");
            }
        }

        private void genusLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MessageBox.Show(
            "Genus is the first half of a scientific name.\n\nExample - Mus musculus genus: Mus\n\nWould you like more information?","Genus", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start("http://www.uniprot.org/help/taxonomy#organism-denomination");
            }
        }

        private void speciesLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MessageBox.Show(
           "Species is the second half of a scientific name.\n\nExample - Mus musculus species: musculus\n\nWould you like more information?", "Species", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start("http://www.uniprot.org/help/taxonomy#organism-denomination");
            }
        }

        private void consortiumLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Database Source. Examples: Uniprot,\nEnsembl,GenBank,etc.");
        }

        private void isoformLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("YES: Protein isoforms present in database.\n\nNO: Database contains only canonical sequences.");
        }

        private void localMachineCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (localMachineCheckBox.Checked)
            {
                localMachineOutputBrowseButton.Enabled = true;
                localMachineOutputDirectoryBox.Enabled = true;
            }
            else
            {
                localMachineOutputBrowseButton.Enabled = false;
                localMachineOutputDirectoryBox.Enabled = false;
            }
        }

        private void serverCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (serverCheckBox.Checked)
            {
                taxIDLabel.Enabled = true;
                taxIDTextBox.Enabled = true;
                genusLabel.Enabled = true;
                genusTextBox.Enabled = true;
                speciesLabel.Enabled = true;
                speciesTextBox.Enabled = true;
                consortiumLabel.Enabled = true;
                consortiumTextBox.Enabled = true;
                isoformLabel.Enabled = true;
                isoformsComboBox.Enabled = true;
                infoTextBox.Enabled = true;
                infoLabel.Enabled = true;
                monthTextBox.Enabled = true;
                dayTextBox.Enabled = true;
                yearTextBox.Enabled = true;
                downloadLabel.Enabled = true;
            }
            else
            {
                taxIDLabel.Enabled = false;
                taxIDTextBox.Enabled = false;
                genusLabel.Enabled = false;
                genusTextBox.Enabled = false;
                speciesLabel.Enabled = false;
                speciesTextBox.Enabled = false;
                consortiumLabel.Enabled = false;
                consortiumTextBox.Enabled = false;
                isoformLabel.Enabled = false;
                isoformsComboBox.Enabled = false;
                infoTextBox.Enabled = false;
                infoLabel.Enabled = false;
                monthTextBox.Enabled = false;
                dayTextBox.Enabled = false;
                yearTextBox.Enabled = false;
                downloadLabel.Enabled = false;
            }
        }

        private void localMachineOutputDirectoryBox_Click(object sender, EventArgs e)
        {
            if (localMachineOutputDirectoryBox.Text.Equals("Output File Location"))
            {
                localMachineOutputDirectoryBox.ForeColor = Color.Black;
                localMachineOutputDirectoryBox.Text = "";
            }
        }

        private void monthTextBox_Click(object sender, EventArgs e)
        {
            if (monthTextBox.Text.Equals("MM"))
            {
                monthTextBox.Text = "";
                monthTextBox.ForeColor = Color.Black;
            }
        }

        private void dayTextBox_Click(object sender, EventArgs e)
        {
            if (dayTextBox.Text.Equals("DD"))
            {
                dayTextBox.Text = "";
                dayTextBox.ForeColor = Color.Black;
            }
        }

        private void yearTextBox_Click(object sender, EventArgs e)
        {
            if (yearTextBox.Text.Equals("YYYY"))
            {
                yearTextBox.Text = "";
                yearTextBox.ForeColor = Color.Black;
            }
        }

        private void excludeNTermCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (excludeNTermCheckBox.Checked)
            {
                nTermMethCheckBox.Enabled = true;
            }
            if (!excludeNTermCheckBox.Checked)
            {
                nTermMethCheckBox.Checked = false;
                nTermMethCheckBox.Enabled = false;
            }
        }

        private void browseButtonOne_Click(object sender, EventArgs e)
        {
              openFileDialog1.Filter = "FASTA Database Files (*.fasta)|*.fasta";
              if (openFileDialog1.ShowDialog() == DialogResult.OK)
              {
                  foreach (var file in openFileDialog1.FileNames)
                  {
                      fastaTextBox.ForeColor = Color.Black;
                      fastaTextBox.Text = file;
                  }
              }
        }

        private void localMachineOutputBrowseButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                localMachineOutputDirectoryBox.ForeColor = Color.Black;
                localMachineOutputDirectoryBox.Text = folderBrowserDialog1.SelectedPath.ToString();
            }
        }
    }
}
