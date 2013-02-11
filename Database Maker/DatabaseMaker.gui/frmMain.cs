using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using CSMSL.IO;

namespace Coon.Compass.DatabaseMaker
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            fastaLB.DataSource = FastaFiles;
        }

        private BindingList<string> FastaFiles = new BindingList<string>();

        private void frmMain_DragEnter(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Link;
            }
        }

        private void frmMain_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] faFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
                LoadFasta(faFiles);
            }            
        }

        /**
         * Method to load Fasta files out of all the added files.
         * // TODO: Avoid adding same files.
         **/
        private void LoadFasta(IEnumerable<string> faFiles)
        {           
            foreach (string item in faFiles)
            {
                // Get the file attributes
                FileAttributes attr = File.GetAttributes(item);

                // Check whether it is a directory or a file
                if ((attr & FileAttributes.Directory) != 0) // It is a directory
                {
                    // Add all the fasta files within that directory. Please note that we 
                    // are not looking for fasta files recursively in this directory.
                    string[] fileEntries = Directory.GetFiles(item);
                    foreach (string fileName in fileEntries)
                        addFastaFile(fileName);
                }
                else // It is a file
                    addFastaFile(item);
            }

            // Show the 'Merge Output' CheckBox as enabled only if more than 
            // one fasta files have been added.
            mergeoutputCB.Enabled = (FastaFiles.Count > 1);

            if (FastaFiles.Count < 1)
                MessageBox.Show("Sorry, you added NO FASTA Files!");
            else
            {
                // [Output Folder Text Box] Show the Path of Directory from which files are added.
                try
                {
                    // Reading the path of any dragged file should be sufficient, thus just reading the first file.
                    string childFile = "";
                    foreach (string fileName in faFiles)
                    {
                        childFile = fileName; 
                        break;
                    }

                    txtOutput.Text = Path.GetDirectoryName(childFile);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine("WE SHOULD NOT BE COMING HERE!!! {0}", e);
                }
            }
        }

        private void addFastaFile(string fileName)
        {
            string ext = Path.GetExtension(fileName);
            if (ext.Contains("fa")) // TODO: Find out what other formats of Fasta Files are allowed.
            {
                FastaFiles.Add(fileName);
            }
        }

        private void btnBrowseFasta_Click(object sender, EventArgs e)
        {
            if(fastaD.ShowDialog() == DialogResult.OK)
            {
                LoadFasta(fastaD.FileNames);
            }
        }

        private void radTarget_CheckedChanged(object sender, EventArgs e)
        {
            grpDecoyDatabaseMethod.Enabled = !radTarget.Checked;
        }

        private void radDecoy_CheckedChanged(object sender, EventArgs e)
        {
            grpDecoyDatabaseMethod.Enabled = radDecoy.Checked;            
        }

        private void radConcatenated_CheckedChanged(object sender, EventArgs e)
        {
            grpDecoyDatabaseMethod.Enabled = radConcatenated.Checked;       
        }

        private void chkExcludeNTerminus_CheckedChanged(object sender, EventArgs e)
        {
            chkOnlyIfNTerminusIsMethionine.Enabled = chkExcludeNTerminus.Checked;
            if(!chkExcludeNTerminus.Checked) {
                chkOnlyIfNTerminusIsMethionine.Enabled = false;
            }
        }

        private void btnBrowseOutput_Click(object sender, EventArgs e)
        {
            if(outputD.ShowDialog() == DialogResult.OK)
            {
                txtOutput.Text = outputD.SelectedPath;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DatabaseMakerOptions options = new DatabaseMakerOptions();            
                      
            if(radDecoy.Checked)
            {
                options.OutputType = DatabaseType.Decoy;               
            }
            else if(radConcatenated.Checked)
            {
                options.OutputType = DatabaseType.Concatenated;                
            }
            else
            {
                options.OutputType = DatabaseType.Target;               
            }
          
            if(radShuffle.Checked)
            {
                options.DecoyType = DecoyDatabaseMethod.Shuffle;
            }
            else if(radRandom.Checked)
            {
                options.DecoyType = DecoyDatabaseMethod.Random;              
            }
            else
            {
                options.DecoyType = DecoyDatabaseMethod.Reverse;                
            }
            options.ExcludeNTerminalResidue = chkExcludeNTerminus.Checked;
            options.ExcludeNTerminalMethionine = chkOnlyIfNTerminusIsMethionine.Checked;
            options.BlastDatabase = chkBlast.Checked;
            options.DoNotMergeFiles = !mergeoutputCB.Checked;
            options.OutputFastaFile = txtOutput.Text;
            if (options.OutputFastaFile == string.Empty)
            {
                options.OutputFastaFile = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }

            DatabaseMaker database_maker = new DatabaseMaker(options);

            database_maker.CreateDatabase();
        }

        /**
         * Callback to 'Clear' Button
         **/
        private void clearBtn_Click(object sender, EventArgs e)
        {
            // Clear the Fasta Files List
            FastaFiles.Clear();

            // Show the Merge Output CheckBox as disabled., as it of no use right now.
            mergeoutputCB.Enabled = false;

            // Clearing the 'Output Folder' TextBox
            txtOutput.Text = "";
        }
    }

}