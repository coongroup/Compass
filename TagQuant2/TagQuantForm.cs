using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.ComponentModel;
using CSMSL;
using CSMSL.Analysis.Quantitation;
using CSMSL.IO;
using CSMSL.IO.Thermo;
using CSMSL.Spectral;
using LumenWorks.Framework.IO.Csv;
using MathNet.Numerics.LinearAlgebra.Double;

namespace TagQuant
{
    public partial class TagQuantForm : Form
    {
     
        BindingList<TagInformation> allTags  = new BindingList<TagInformation>(); 

        public TagQuantForm()
        {
            InitializeComponent();

            //dataGridView2.AutoGenerateColumns = false;
           

            allTags = new BindingList<TagInformation>();
            allTags.Add(new TagInformation(524, "TMT 126N","",524.5,324.25, TagSetType.TMTN));
            dataGridView2.DataSource = allTags;
        }

        private void TagQuantForm_Load(object sender, EventArgs e)
        {
            ComboBoxETDoptions.SelectedIndex = 3;
        }

        // Drag & enter files into listbox1
        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filepaths = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach(string filepath in filepaths)
                {
                    if(Path.GetExtension(filepath).Equals(".csv", StringComparison.InvariantCultureIgnoreCase) &&
                        !listBox1.Items.Contains(filepath))
                    {
                        e.Effect = DragDropEffects.Link;
                        break;
                    }
                }
            }
        }

        //Drag & drop files into listbox1
        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] filepaths = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach(string filepath in filepaths)
            {
                if(Path.GetExtension(filepath).Equals(".csv", StringComparison.InvariantCultureIgnoreCase) &&
                    !listBox1.Items.Contains(filepath))
                {
                    listBox1.Items.Add(filepath);
                    UpdateRawFileFolder(filepath);
                    UpdateOutputFolder(filepath);
                }
            }
        }

        // Add files into listbox1
        private void Add_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach(string filename in openFileDialog1.FileNames)
                {
                    listBox1.Items.Add(filename);
                    UpdateRawFileFolder(filename);
                    UpdateOutputFolder(filename);
                }
            }
        }

        // Update the output folder
        private void UpdateOutputFolder(string filepath)
        {
            if(textOutputFolder.Text == string.Empty)
            {
                textOutputFolder.Text = Path.GetDirectoryName(filepath);
            }
        }

        private void UpdateRawFileFolder(string filepath)
        {
            if (string.IsNullOrEmpty(textRawFolder.Text))
            {
                textRawFolder.Text = Path.GetDirectoryName(filepath);
            }
        }

        // Remove files from listbox1
        private void Remove_Click(object sender, EventArgs e)
        {
            while(listBox1.SelectedItems.Count > 0)
            {
                listBox1.Items.Remove(listBox1.SelectedItem);
            }
        }

        // Clear all files from listbox1
        private void Clear_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        // Find folder for raw files
        private void BrowseRaw_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textRawFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        // Determine whether or not TMT 2-Plex box has been checked
        private void radioBox_TMT2_CheckedChanged(object sender, EventArgs e)
        {
            if(radioBox_TMT2.Checked)
            {
                checkBox126.Enabled = true;
                checkBox127.Enabled = true;
                checkBox126.Checked = true;
                checkBox127.Checked = true;
                textBox126.Enabled = true;
                textBox127.Enabled = true;
                label10.Enabled = true;
                label9.Enabled = true;

                label16.Enabled = false;
                dataGridView1.Enabled = false;
            }
            else
            {
                checkBox126.Enabled = false;
                checkBox127.Enabled = false;
                checkBox126.Checked = false;
                checkBox127.Checked = false;
                textBox126.Enabled = false;
                textBox127.Enabled = false;
                label10.Enabled = false;
                label9.Enabled = false;

                label16.Enabled = true;
                dataGridView1.Enabled = true;
            }
        }

        // Determine whether or not ITRAQ 4-Plex box has been checked
        private void radioBox_iTRAQ4_CheckedChanged(object sender, EventArgs e)
        {
            if(radioBox_iTRAQ4.Checked)
            {
                checkBox114.Enabled = true;
                checkBox115.Enabled = true;
                checkBox116.Enabled = true;
                checkBox117.Enabled = true;
                checkBox114.Checked = true;
                checkBox115.Checked = true;
                checkBox116.Checked = true;
                checkBox117.Checked = true;
                textBox114.Enabled = true;
                textBox115.Enabled = true;
                textBox116.Enabled = true;
                textBox117.Enabled = true;
                label2.Enabled = true;
                label3.Enabled = true;

                dataGridView1.Enabled = true;
                dataGridView1.Rows.Add(4);
                dataGridView1.Rows[0].Cells[0].Value = "iTRAQ™ Reagent114";
                dataGridView1.Rows[1].Cells[0].Value = "iTRAQ™ Reagent115";
                dataGridView1.Rows[2].Cells[0].Value = "iTRAQ™ Reagent116";
                dataGridView1.Rows[3].Cells[0].Value = "iTRAQ™ Reagent117";

                dataGridView1.Rows[0].Cells[1].Value = "0.0";
                dataGridView1.Rows[1].Cells[1].Value = "0.0";
                dataGridView1.Rows[2].Cells[1].Value = "0.0";
                dataGridView1.Rows[3].Cells[1].Value = "0.1";

                dataGridView1.Rows[0].Cells[2].Value = "1.0";
                dataGridView1.Rows[1].Cells[2].Value = "2.0";
                dataGridView1.Rows[2].Cells[2].Value = "3.0";
                dataGridView1.Rows[3].Cells[2].Value = "4.0";

                dataGridView1.Rows[0].Cells[3].Value = "5.9";
                dataGridView1.Rows[1].Cells[3].Value = "5.6";
                dataGridView1.Rows[2].Cells[3].Value = "4.5";
                dataGridView1.Rows[3].Cells[3].Value = "3.5";

                dataGridView1.Rows[0].Cells[4].Value = "0.2";
                dataGridView1.Rows[1].Cells[4].Value = "0.1";
                dataGridView1.Rows[2].Cells[4].Value = "0.1";
                dataGridView1.Rows[3].Cells[4].Value = "0.1";
            }
            else
            {
                checkBox114.Checked = false;
                checkBox115.Checked = false;
                checkBox116.Checked = false;
                checkBox117.Checked = false;
                checkBox114.Enabled = false;
                checkBox115.Enabled = false;
                checkBox116.Enabled = false;
                checkBox117.Enabled = false;
                textBox114.Enabled = false;
                textBox115.Enabled = false;
                textBox116.Enabled = false;
                textBox117.Enabled = false;
                label2.Enabled = false;
                label3.Enabled = false;

                dataGridView1.Enabled = false;
                dataGridView1.Rows.Clear();
            }
        }

        // Determine whether or not TMT 6-Plex box has been checked
        private void radioBox_TMT6_CheckedChanged(object sender, EventArgs e)
        {
            if(radioBox_TMT6.Checked)
            {
                checkBox126.Checked = true;
                checkBox127.Checked = true;
                checkBox128.Checked = true;
                checkBox129.Checked = true;
                checkBox130.Checked = true;
                checkBox131.Checked = true;
                checkBox126.Enabled = true;
                checkBox127.Enabled = true;
                checkBox128.Enabled = true;
                checkBox129.Enabled = true;
                checkBox130.Enabled = true;
                checkBox131.Enabled = true;
                textBox126.Enabled = true;
                textBox127.Enabled = true;
                textBox128.Enabled = true;
                textBox129.Enabled = true;
                textBox130.Enabled = true;
                textBox131.Enabled = true;
                label10.Enabled = true;
                label11.Enabled = true;
                label12.Enabled = true;
                label9.Enabled = true;
                dataGridView1.Enabled = true;
                dataGridView1.Rows.Add(6);


                dataGridView1.Rows[0].Cells[0].Value = "TMT Reagent126";
                dataGridView1.Rows[1].Cells[0].Value = "TMT Reagent127";
                dataGridView1.Rows[2].Cells[0].Value = "TMT Reagent128";
                dataGridView1.Rows[3].Cells[0].Value = "TMT Reagent129";
                dataGridView1.Rows[4].Cells[0].Value = "TMT Reagent130";
                dataGridView1.Rows[5].Cells[0].Value = "TMT Reagent131";

                dataGridView1.Rows[0].Cells[1].Value = "0.00";
                dataGridView1.Rows[1].Cells[1].Value = "0.09";
                dataGridView1.Rows[2].Cells[1].Value = "0.09";
                dataGridView1.Rows[3].Cells[1].Value = "0.09";
                dataGridView1.Rows[4].Cells[1].Value = "0.19";
                dataGridView1.Rows[5].Cells[1].Value = "0.09";

                dataGridView1.Rows[0].Cells[2].Value = "0.09";
                dataGridView1.Rows[1].Cells[2].Value = "0.73";
                dataGridView1.Rows[2].Cells[2].Value = "1.28";
                dataGridView1.Rows[3].Cells[2].Value = "1.49";
                dataGridView1.Rows[4].Cells[2].Value = "1.95";
                dataGridView1.Rows[5].Cells[2].Value = "3.76";

                dataGridView1.Rows[0].Cells[3].Value = "8.54";
                dataGridView1.Rows[1].Cells[3].Value = "7.83";
                dataGridView1.Rows[2].Cells[3].Value = "6.59";
                dataGridView1.Rows[3].Cells[3].Value = "5.29";
                dataGridView1.Rows[4].Cells[3].Value = "4.74";
                dataGridView1.Rows[5].Cells[3].Value = "4.31";

                dataGridView1.Rows[0].Cells[4].Value = "0.54";
                dataGridView1.Rows[1].Cells[4].Value = "0.36";
                dataGridView1.Rows[2].Cells[4].Value = "0.46";
                dataGridView1.Rows[3].Cells[4].Value = "0.28";
                dataGridView1.Rows[4].Cells[4].Value = "0.19";
                dataGridView1.Rows[5].Cells[4].Value = "0.09";
            }
            else
            {
                checkBox126.Checked = false;
                checkBox127.Checked = false;
                checkBox128.Checked = false;
                checkBox129.Checked = false;
                checkBox130.Checked = false;
                checkBox131.Checked = false;
                checkBox126.Enabled = false;
                checkBox127.Enabled = false;
                checkBox128.Enabled = false;
                checkBox129.Enabled = false;
                checkBox130.Enabled = false;
                checkBox131.Enabled = false;
                textBox126.Enabled = false;
                textBox127.Enabled = false;
                textBox128.Enabled = false;
                textBox129.Enabled = false;
                textBox130.Enabled = false;
                textBox131.Enabled = false;
                label10.Enabled = false;
                label11.Enabled = false;
                label12.Enabled = false;
                label9.Enabled = false;

                dataGridView1.Enabled = false;
                dataGridView1.Rows.Clear();
            }
        }

        // Determine whether or not ITRAQ 8-Plex box has been checked
        private void radioBox_iTRAQ8_CheckedChanged(object sender, EventArgs e)
        {
            if(radioBox_iTRAQ8.Checked)
            {
                checkBox113.Checked = true;
                checkBox114.Checked = true;
                checkBox115.Checked = true;
                checkBox116.Checked = true;
                checkBox117.Checked = true;
                checkBox118.Checked = true;
                checkBox119.Checked = true;
                checkBox121.Checked = true;

                checkBox113.Enabled = true;
                checkBox114.Enabled = true;
                checkBox115.Enabled = true;
                checkBox116.Enabled = true;
                checkBox117.Enabled = true;
                checkBox118.Enabled = true;
                checkBox119.Enabled = true;
                checkBox121.Enabled = true;

                textBox113.Enabled = true;
                textBox114.Enabled = true;
                textBox115.Enabled = true;
                textBox116.Enabled = true;
                textBox117.Enabled = true;
                textBox118.Enabled = true;
                textBox119.Enabled = true;
                textBox121.Enabled = true;

                label2.Enabled = true;
                label3.Enabled = true;
                label15.Enabled = true;
                label14.Enabled = true;

                dataGridView1.Enabled = true;
                dataGridView1.Rows.Add(8);

                dataGridView1.Rows[0].Cells[0].Value = "iTRAQ™ Reagent113";
                dataGridView1.Rows[1].Cells[0].Value = "iTRAQ™ Reagent114";
                dataGridView1.Rows[2].Cells[0].Value = "iTRAQ™ Reagent115";
                dataGridView1.Rows[3].Cells[0].Value = "iTRAQ™ Reagent116";
                dataGridView1.Rows[4].Cells[0].Value = "iTRAQ™ Reagent117";
                dataGridView1.Rows[5].Cells[0].Value = "iTRAQ™ Reagent118";
                dataGridView1.Rows[6].Cells[0].Value = "iTRAQ™ Reagent119";
                dataGridView1.Rows[7].Cells[0].Value = "iTRAQ™ Reagent121";

                dataGridView1.Rows[0].Cells[1].Value = "0.00";
                dataGridView1.Rows[1].Cells[1].Value = "0.00";
                dataGridView1.Rows[2].Cells[1].Value = "0.00";
                dataGridView1.Rows[3].Cells[1].Value = "0.00";
                dataGridView1.Rows[4].Cells[1].Value = "0.06";
                dataGridView1.Rows[5].Cells[1].Value = "0.09";
                dataGridView1.Rows[6].Cells[1].Value = "0.14";
                dataGridView1.Rows[7].Cells[1].Value = "0.27";

                dataGridView1.Rows[0].Cells[2].Value = "0.00";
                dataGridView1.Rows[1].Cells[2].Value = "0.94";
                dataGridView1.Rows[2].Cells[2].Value = "1.88";
                dataGridView1.Rows[3].Cells[2].Value = "2.82";
                dataGridView1.Rows[4].Cells[2].Value = "3.77";
                dataGridView1.Rows[5].Cells[2].Value = "4.71";
                dataGridView1.Rows[6].Cells[2].Value = "5.66";
                dataGridView1.Rows[7].Cells[2].Value = "7.44";

                dataGridView1.Rows[0].Cells[3].Value = "6.89";
                dataGridView1.Rows[1].Cells[3].Value = "5.90";
                dataGridView1.Rows[2].Cells[3].Value = "4.90";
                dataGridView1.Rows[3].Cells[3].Value = "3.90";
                dataGridView1.Rows[4].Cells[3].Value = "2.88";
                dataGridView1.Rows[5].Cells[3].Value = "1.88";
                dataGridView1.Rows[6].Cells[3].Value = "0.87";
                dataGridView1.Rows[7].Cells[3].Value = "0.18";

                dataGridView1.Rows[0].Cells[4].Value = "0.22";
                dataGridView1.Rows[1].Cells[4].Value = "0.16";
                dataGridView1.Rows[2].Cells[4].Value = "0.10";
                dataGridView1.Rows[3].Cells[4].Value = "0.07";
                dataGridView1.Rows[4].Cells[4].Value = "0.00";
                dataGridView1.Rows[5].Cells[4].Value = "0.00";
                dataGridView1.Rows[6].Cells[4].Value = "0.00";
                dataGridView1.Rows[7].Cells[4].Value = "0.00";
            }
            else
            {
                checkBox113.Checked = false;
                checkBox114.Checked = false;
                checkBox115.Checked = false;
                checkBox116.Checked = false;
                checkBox117.Checked = false;
                checkBox118.Checked = false;
                checkBox119.Checked = false;
                checkBox121.Checked = false;
                checkBox127_N.Checked = false;
                checkBox129_N.Checked = false;
                checkBox128_N.Checked = false;
                checkBox130_N.Checked = false;

                checkBox113.Enabled = false;
                checkBox114.Enabled = false;
                checkBox115.Enabled = false;
                checkBox116.Enabled = false;
                checkBox117.Enabled = false;
                checkBox118.Enabled = false;
                checkBox119.Enabled = false;
                checkBox121.Enabled = false;

                textBox113.Enabled = false;
                textBox114.Enabled = false;
                textBox115.Enabled = false;
                textBox116.Enabled = false;
                textBox117.Enabled = false;
                textBox118.Enabled = false;
                textBox119.Enabled = false;
                textBox121.Enabled = false;

                label2.Enabled = false;
                label3.Enabled = false;
                label15.Enabled = false;
                label14.Enabled = false;

                dataGridView1.Enabled = false;
                dataGridView1.Rows.Clear();
            }
        }

        // Determine whether or not TMT 8-Plex box has been checked
        private void radioBox_TMT8_CheckedChanged(object sender, EventArgs e)
        {
            if (radioBox_TMT8.Checked)
            {
                checkBox126.Checked = true;
                checkBox127.Checked = true;
                checkBox127_N.Checked = true;
                checkBox128.Checked = true;
                checkBox129.Checked = true;
                checkBox129_N.Checked = true;
                checkBox130.Checked = true;
                checkBox131.Checked = true;
                checkBox126.Enabled = true;
                checkBox127.Enabled = true;
                checkBox127_N.Enabled = true;
                checkBox128.Enabled = true;
                checkBox129.Enabled = true;
                checkBox129_N.Enabled = true;
                checkBox130.Enabled = true;
                checkBox131.Enabled = true;
                textBox126.Enabled = true;
                textBox127.Enabled = true;
                textBox127_N.Enabled = true;
                textBox128.Enabled = true;
                textBox129.Enabled = true;
                textBox129_N.Enabled = true;
                textBox130.Enabled = true;
                textBox131.Enabled = true;
                label10.Enabled = true;
                label11.Enabled = true;
                label12.Enabled = true;
                label9.Enabled = true;
                dataGridView1.Enabled = true;
                dataGridView1.Rows.Add(8);

                dataGridView1.Rows[0].Cells[0].Value = "TMT Reagent126";
                dataGridView1.Rows[1].Cells[0].Value = "TMT Reagent127*";
                dataGridView1.Rows[2].Cells[0].Value = "TMT Reagent127";
                dataGridView1.Rows[3].Cells[0].Value = "TMT Reagent128";
                dataGridView1.Rows[4].Cells[0].Value = "TMT Reagent129*";
                dataGridView1.Rows[5].Cells[0].Value = "TMT Reagent129";
                dataGridView1.Rows[6].Cells[0].Value = "TMT Reagent130";
                dataGridView1.Rows[7].Cells[0].Value = "TMT Reagent131";

                dataGridView1.Rows[0].Cells[1].Value = "0.00";
                dataGridView1.Rows[1].Cells[1].Value = "0.00";
                dataGridView1.Rows[2].Cells[1].Value = "0.00";
                dataGridView1.Rows[3].Cells[1].Value = "0.00";
                dataGridView1.Rows[4].Cells[1].Value = "0.00";
                dataGridView1.Rows[5].Cells[1].Value = "0.00";
                dataGridView1.Rows[6].Cells[1].Value = "0.00";
                dataGridView1.Rows[7].Cells[1].Value = "0.20";

                dataGridView1.Rows[0].Cells[2].Value = "0.00";
                dataGridView1.Rows[1].Cells[2].Value = "0.50";
                dataGridView1.Rows[2].Cells[2].Value = "0.40";
                dataGridView1.Rows[3].Cells[2].Value = "1.10";
                dataGridView1.Rows[4].Cells[2].Value = "1.40";
                dataGridView1.Rows[5].Cells[2].Value = "1.70";
                dataGridView1.Rows[6].Cells[2].Value = "2.80";
                dataGridView1.Rows[7].Cells[2].Value = "3.20";

                dataGridView1.Rows[0].Cells[3].Value = "6.10";
                dataGridView1.Rows[1].Cells[3].Value = "6.70";
                dataGridView1.Rows[2].Cells[3].Value = "6.80";
                dataGridView1.Rows[3].Cells[3].Value = "4.20";
                dataGridView1.Rows[4].Cells[3].Value = "4.70";
                dataGridView1.Rows[5].Cells[3].Value = "4.10";
                dataGridView1.Rows[6].Cells[3].Value = "2.50";
                dataGridView1.Rows[7].Cells[3].Value = "2.80";

                dataGridView1.Rows[0].Cells[4].Value = "0.00";
                dataGridView1.Rows[1].Cells[4].Value = "0.00";
                dataGridView1.Rows[2].Cells[4].Value = "0.00";
                dataGridView1.Rows[3].Cells[4].Value = "0.00";
                dataGridView1.Rows[4].Cells[4].Value = "0.00";
                dataGridView1.Rows[5].Cells[4].Value = "0.00";
                dataGridView1.Rows[6].Cells[4].Value = "0.00";
                dataGridView1.Rows[7].Cells[4].Value = "0.00";
            }
            else
            {
                checkBox126.Checked = false;
                checkBox127.Checked = false;
                checkBox127_N.Checked = false;
                checkBox128.Checked = false;
                checkBox129.Checked = false;
                checkBox129_N.Checked = false;
                checkBox130.Checked = false;
                checkBox131.Checked = false;
                checkBox126.Enabled = false;
                checkBox127.Enabled = false;
                checkBox127_N.Enabled = false;
                checkBox128.Enabled = false;
                checkBox129.Enabled = false;
                checkBox129_N.Enabled = false;
                checkBox130.Enabled = false;
                checkBox131.Enabled = false;
                textBox126.Enabled = false;
                textBox127.Enabled = false;
                textBox127_N.Enabled = false;
                textBox128.Enabled = false;
                textBox129.Enabled = false;
                textBox129_N.Enabled = false;
                textBox130.Enabled = false;
                textBox131.Enabled = false;
                label10.Enabled = false;
                label11.Enabled = false;
                label12.Enabled = false;
                label9.Enabled = false;

                dataGridView1.Enabled = false;
                dataGridView1.Rows.Clear();
            }
        }

        // Determine whether or not TMT 10-Plex box has been checked
        private void radioBox_TMT10_CheckedChanged_1(object sender, EventArgs e)
        {
            if (radioBox_TMT10.Checked)
            {
                checkBox126.Checked = true;
                checkBox127.Checked = true;
                checkBox127_N.Checked = true;
                checkBox128.Checked = true;
                checkBox128_N.Checked = true;
                checkBox129.Checked = true;
                checkBox129_N.Checked = true;
                checkBox130.Checked = true;
                checkBox130_N.Checked = true;
                checkBox131.Checked = true;
                checkBox126.Enabled = true;
                checkBox127.Enabled = true;
                checkBox127_N.Enabled = true;
                checkBox128.Enabled = true;
                checkBox128_N.Enabled = true;
                checkBox129.Enabled = true;
                checkBox129_N.Enabled = true;
                checkBox130.Enabled = true;
                checkBox130_N.Enabled = true;
                checkBox131.Enabled = true;
                textBox126.Enabled = true;
                textBox127.Enabled = true;
                textBox127_N.Enabled = true;
                textBox128.Enabled = true;
                textBox128_N.Enabled = true;
                textBox129.Enabled = true;
                textBox129_N.Enabled = true;
                textBox130.Enabled = true;
                textBox130_N.Enabled = true;
                textBox131.Enabled = true;
                label10.Enabled = true;
                label11.Enabled = true;
                label12.Enabled = true;
                label9.Enabled = true;
                dataGridView1.Enabled = true;
                dataGridView1.Rows.Add(10);

                dataGridView1.Rows[0].Cells[0].Value = "TMT Reagent126";
                dataGridView1.Rows[1].Cells[0].Value = "TMT Reagent127*";
                dataGridView1.Rows[2].Cells[0].Value = "TMT Reagent127";
                dataGridView1.Rows[3].Cells[0].Value = "TMT Reagent128*";
                dataGridView1.Rows[4].Cells[0].Value = "TMT Reagent128";
                dataGridView1.Rows[5].Cells[0].Value = "TMT Reagent129*";
                dataGridView1.Rows[6].Cells[0].Value = "TMT Reagent129";
                dataGridView1.Rows[7].Cells[0].Value = "TMT Reagent130*";
                dataGridView1.Rows[8].Cells[0].Value = "TMT Reagent130";
                dataGridView1.Rows[9].Cells[0].Value = "TMT Reagent131";

                dataGridView1.Rows[0].Cells[1].Value = "0.00";
                dataGridView1.Rows[1].Cells[1].Value = "0.00";
                dataGridView1.Rows[2].Cells[1].Value = "0.00";
                dataGridView1.Rows[3].Cells[1].Value = "0.00";
                dataGridView1.Rows[4].Cells[1].Value = "0.00";
                dataGridView1.Rows[5].Cells[1].Value = "0.00";
                dataGridView1.Rows[6].Cells[1].Value = "0.00";
                dataGridView1.Rows[7].Cells[1].Value = "0.00";
                dataGridView1.Rows[8].Cells[1].Value = "0.00";
                dataGridView1.Rows[9].Cells[1].Value = "0.20";

                dataGridView1.Rows[0].Cells[2].Value = "0.00";
                dataGridView1.Rows[1].Cells[2].Value = "0.50";
                dataGridView1.Rows[2].Cells[2].Value = "0.40";
                dataGridView1.Rows[3].Cells[2].Value = "0.00";
                dataGridView1.Rows[4].Cells[2].Value = "1.10";
                dataGridView1.Rows[5].Cells[2].Value = "1.40";
                dataGridView1.Rows[6].Cells[2].Value = "1.70";
                dataGridView1.Rows[7].Cells[2].Value = "0.00";
                dataGridView1.Rows[8].Cells[2].Value = "2.80";
                dataGridView1.Rows[9].Cells[2].Value = "3.20";

                dataGridView1.Rows[0].Cells[3].Value = "6.10";
                dataGridView1.Rows[1].Cells[3].Value = "6.70";
                dataGridView1.Rows[2].Cells[3].Value = "6.80";
                dataGridView1.Rows[3].Cells[3].Value = "0.00";
                dataGridView1.Rows[4].Cells[3].Value = "4.20";
                dataGridView1.Rows[5].Cells[3].Value = "4.70";
                dataGridView1.Rows[6].Cells[3].Value = "4.10";
                dataGridView1.Rows[7].Cells[3].Value = "0.00";
                dataGridView1.Rows[8].Cells[3].Value = "2.50";
                dataGridView1.Rows[9].Cells[3].Value = "2.80";

                dataGridView1.Rows[0].Cells[4].Value = "0.00";
                dataGridView1.Rows[1].Cells[4].Value = "0.00";
                dataGridView1.Rows[2].Cells[4].Value = "0.00";
                dataGridView1.Rows[3].Cells[4].Value = "0.00";
                dataGridView1.Rows[4].Cells[4].Value = "0.00";
                dataGridView1.Rows[5].Cells[4].Value = "0.00";
                dataGridView1.Rows[6].Cells[4].Value = "0.00";
                dataGridView1.Rows[7].Cells[4].Value = "0.00";
                dataGridView1.Rows[8].Cells[4].Value = "0.00";
                dataGridView1.Rows[9].Cells[4].Value = "0.00";
            }
            else
            {
                checkBox126.Checked = false;
                checkBox127.Checked = false;
                checkBox127_N.Checked = false;
                checkBox128.Checked = false;
                checkBox129.Checked = false;
                checkBox129_N.Checked = false;
                checkBox130.Checked = false;
                checkBox131.Checked = false;
                checkBox126.Enabled = false;
                checkBox127.Enabled = false;
                checkBox127_N.Enabled = false;
                checkBox128.Enabled = false;
                checkBox128_N.Enabled = false;
                checkBox129.Enabled = false;
                checkBox129_N.Enabled = false;
                checkBox130.Enabled = false;
                checkBox130_N.Enabled = false;
                checkBox131.Enabled = false;
                textBox126.Enabled = false;
                textBox127.Enabled = false;
                textBox127_N.Enabled = false;
                textBox128.Enabled = false;
                textBox128_N.Enabled = false;
                textBox129.Enabled = false;
                textBox129_N.Enabled = false;
                textBox130.Enabled = false;
                textBox130_N.Enabled = false;
                textBox131.Enabled = false;
                label10.Enabled = false;
                label11.Enabled = false;
                label12.Enabled = false;
                label9.Enabled = false;

                dataGridView1.Enabled = false;
                dataGridView1.Rows.Clear();
            }
        }

        // Find output folder
        private void BrowseOutput_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog2.ShowDialog() == DialogResult.OK)
            {
                textOutputFolder.Text = folderBrowserDialog2.SelectedPath;
            }
        }

        private const double C12_C13_MASS_DIFFERENCE = 1.0033548378;
        private const double N14_N15_MASS_DIFFERENCE = 0.99999991;

        // Run TagQuant Program
        private void Quantify_Click(object sender, EventArgs e)
        {
            double ITerror = (double)numericUpDown1.Value;
            double FTerror = (double)numericUpDown2.Value;
            double error;

            var tagsToUse = new List<TagInformation>
            {
                new TagInformation(126, "126", textBox126.Text, 126.1283, 114.1279, TagSetType.TMTC),
                new TagInformation(127, "127N", textBox127_N.Text, 127.1253, 114.1279, TagSetType.TMTN),
                new TagInformation(127, "127C", textBox127.Text, 127.1316, 114.1279, TagSetType.TMTC),
                new TagInformation(128, "128N", textBox128_N.Text, 128.1287, 114.1279, TagSetType.TMTN),
                new TagInformation(128, "128C", textBox128.Text, 128.1350, 114.1279, TagSetType.TMTC),
                new TagInformation(129, "129N", textBox129_N.Text, 129.1320, 114.1279, TagSetType.TMTN),
                new TagInformation(129, "129C", textBox129.Text, 129.1383, 114.1279, TagSetType.TMTC),
                new TagInformation(130, "130N", textBox130_N.Text, 130.1354, 114.1279, TagSetType.TMTN),
                new TagInformation(130, "130C", textBox130.Text, 130.1417, 118.1415, TagSetType.TMTC),
                new TagInformation(131, "131", textBox130.Text, 131.1387, 119.1384, TagSetType.TMTC)
            };

            bool DontQuantifyETD = ComboBoxETDoptions.Text == "Don't Quantify";
            bool noisebandCap = noisebandcapCB.Checked; // Check for noise-band capping

            TagQuant tagQuant = new TagQuant(textOutputFolder.Text, textRawFolder.Text, listBox1.Items.OfType<string>(), tagsToUse, MassTolerance.FromDA(ITerror), MassTolerance.FromDA(FTerror), 0, noisebandCap);
            tagQuant.Run();
            return;

            panel1.Enabled = false;
            toolStripStatusLabel1.Text = "Running";
            Application.DoEvents();
            int FilesCount = listBox1.Items.Count;
            int FileCounter = 1;

           

            int ExceptionCount = 0;
            double IsoWindow = 0;
            double PreErrorAllowed = 0;
            //IsoWindow = double.Parse(windowTextBox.Text);   // Set isolation window
            //PreErrorAllowed = double.Parse(ppmTextBox.Text);    // Set allowed precursor error

            List<int> TagsInUse = new List<int>();
            double TagCount = 0;

            if(checkBox113.Checked)
            {
                TagsInUse.Add(113);
                TagCount++;
            }
            if(checkBox114.Checked)
            {
                TagsInUse.Add(114);
                TagCount++;
            }
            if(checkBox115.Checked)
            {
                TagsInUse.Add(115);
                TagCount++;
            }
            if(checkBox116.Checked)
            {
                TagsInUse.Add(116);
                TagCount++;
            }
            if(checkBox117.Checked)
            {
                TagsInUse.Add(117);
                TagCount++;
            }
            if(checkBox118.Checked)
            {
                TagsInUse.Add(118);
                TagCount++;
            }
            if(checkBox119.Checked)
            {
                TagsInUse.Add(119);
                TagCount++;
            }
            if(checkBox121.Checked)
            {
                TagsInUse.Add(121);
                TagCount++;
            }
            if(checkBox126.Checked)
            {
                TagsInUse.Add(126);
                TagCount++;
            }
            if(checkBox127.Checked)
            {
                TagsInUse.Add(127);
                TagCount++;
            }
            if(checkBox128.Checked)
            {
                TagsInUse.Add(128);
                TagCount++;
            }
            if(checkBox129.Checked)
            {
                TagsInUse.Add(129);
                TagCount++;
            }
            if(checkBox130.Checked)
            {
                TagsInUse.Add(130);
                TagCount++;
            }
            if(checkBox131.Checked)
            {
                TagsInUse.Add(131);
                TagCount++;
            }
            if(checkBox127_N.Checked)
            {
                TagsInUse.Add(132);
                TagCount++;
            }
            if(checkBox129_N.Checked)
            {
                TagsInUse.Add(133);
                TagCount++;
            }
            if(checkBox128_N.Checked)
            {
                TagsInUse.Add(134);
                TagCount++;
            }
            if(checkBox130_N.Checked)
            {
                TagsInUse.Add(135);
                TagCount++;
            }

            double TotalETDSignal = 0;
            double TotalCADSignal = 0;
            //string CleavagePattern = "\"" + textBoxCleavage.Text + "\"";

            // Purity correction factors for each of 10 possible rows and isotopic impurities (-2, -1, +1, +2)
            double pcf_R1_m2 = 0;
            double pcf_R2_m2 = 0;
            double pcf_R3_m2 = 0;
            double pcf_R4_m2 = 0;
            double pcf_R5_m2 = 0;
            double pcf_R6_m2 = 0;
            double pcf_R7_m2 = 0;
            double pcf_R8_m2 = 0;
            double pcf_R9_m2 = 0;
            double pcf_R10_m2 = 0;

            double pcf_R1_m1 = 0;
            double pcf_R2_m1 = 0;
            double pcf_R3_m1 = 0;
            double pcf_R4_m1 = 0;
            double pcf_R5_m1 = 0;
            double pcf_R6_m1 = 0;
            double pcf_R7_m1 = 0;
            double pcf_R8_m1 = 0;
            double pcf_R9_m1 = 0;
            double pcf_R10_m1 = 0;

            double pcf_R1_p1 = 0;
            double pcf_R2_p1 = 0;
            double pcf_R3_p1 = 0;
            double pcf_R4_p1 = 0;
            double pcf_R5_p1 = 0;
            double pcf_R6_p1 = 0;
            double pcf_R7_p1 = 0;
            double pcf_R8_p1 = 0;
            double pcf_R9_p1 = 0;
            double pcf_R10_p1 = 0;

            double pcf_R1_p2 = 0;
            double pcf_R2_p2 = 0;
            double pcf_R3_p2 = 0;
            double pcf_R4_p2 = 0;
            double pcf_R5_p2 = 0;
            double pcf_R6_p2 = 0;
            double pcf_R7_p2 = 0;
            double pcf_R8_p2 = 0;
            double pcf_R9_p2 = 0;
            double pcf_R10_p2 = 0;

            // Calculated value for each row (after purity correction)
            double AcP_R1 = 0;
            double AcP_R2 = 0;
            double AcP_R3 = 0;
            double AcP_R4 = 0;
            double AcP_R5 = 0;
            double AcP_R6 = 0;
            double AcP_R7 = 0;
            double AcP_R8 = 0;
            double AcP_R9 = 0;
            double AcP_R10 = 0;

            double CADCoefficientMatrixDeterminant = 0.0;
            double TMT10_CADCoefficientMatrixDeterminant = 0.0;
            //double ETDCoefficientMatrixDeterminant = 0.0;

            if(radioBox_iTRAQ4.Checked)
            {
                //Purity Correction Factors begin for iTRAQ 4-Plex by converting all to a decimal fraction
                pcf_R1_m2 = (Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value)) / 100;
                pcf_R2_m2 = (Convert.ToDouble(dataGridView1.Rows[1].Cells[1].Value)) / 100;
                pcf_R3_m2 = (Convert.ToDouble(dataGridView1.Rows[2].Cells[1].Value)) / 100;
                pcf_R4_m2 = (Convert.ToDouble(dataGridView1.Rows[3].Cells[1].Value)) / 100;

                pcf_R1_m1 = (Convert.ToDouble(dataGridView1.Rows[0].Cells[2].Value)) / 100;
                pcf_R2_m1 = (Convert.ToDouble(dataGridView1.Rows[1].Cells[2].Value)) / 100;
                pcf_R3_m1 = (Convert.ToDouble(dataGridView1.Rows[2].Cells[2].Value)) / 100;
                pcf_R4_m1 = (Convert.ToDouble(dataGridView1.Rows[3].Cells[2].Value)) / 100;

                pcf_R1_p1 = (Convert.ToDouble(dataGridView1.Rows[0].Cells[3].Value)) / 100;
                pcf_R2_p1 = (Convert.ToDouble(dataGridView1.Rows[1].Cells[3].Value)) / 100;
                pcf_R3_p1 = (Convert.ToDouble(dataGridView1.Rows[2].Cells[3].Value)) / 100;
                pcf_R4_p1 = (Convert.ToDouble(dataGridView1.Rows[3].Cells[3].Value)) / 100;

                pcf_R1_p2 = (Convert.ToDouble(dataGridView1.Rows[0].Cells[4].Value)) / 100;
                pcf_R2_p2 = (Convert.ToDouble(dataGridView1.Rows[1].Cells[4].Value)) / 100;
                pcf_R3_p2 = (Convert.ToDouble(dataGridView1.Rows[2].Cells[4].Value)) / 100;
                pcf_R4_p2 = (Convert.ToDouble(dataGridView1.Rows[3].Cells[4].Value)) / 100;

                // Find actual purity for each row/tag (1 minus sum of all impurities)
                AcP_R1 = (1 - (pcf_R1_m2 + pcf_R1_m1 + pcf_R1_p1 + pcf_R1_p2));
                AcP_R2 = (1 - (pcf_R2_m2 + pcf_R2_m1 + pcf_R2_p1 + pcf_R2_p2));
                AcP_R3 = (1 - (pcf_R3_m2 + pcf_R3_m1 + pcf_R3_p1 + pcf_R3_p2));
                AcP_R4 = (1 - (pcf_R4_m2 + pcf_R4_m1 + pcf_R4_p1 + pcf_R4_p2));

                // Convert matrix to 4x4
                Matrix CADCoefficientMatrix = DenseMatrix.OfArray(new double[,]{ 
                    { AcP_R1,       pcf_R2_m1,  pcf_R3_m2,  0           }, 
                    { pcf_R1_p1,    AcP_R2,     pcf_R3_m1,  pcf_R4_m2   }, 
                    { pcf_R1_p2,    pcf_R2_p1,  AcP_R3,     pcf_R4_m1   }, 
                    { 0    ,        pcf_R2_p2,  pcf_R3_p1,  AcP_R4      }, 
                });

                // Determine if coefficient is real
                CADCoefficientMatrixDeterminant = CADCoefficientMatrix.Determinant();
            }  // end iTRAQ purity correction factors

            if(radioBox_iTRAQ8.Checked)
            {
                //Purity Correction Factors begin for iTRAQ 8-Plex
                pcf_R1_m2 = (Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value)) / 100;
                pcf_R2_m2 = (Convert.ToDouble(dataGridView1.Rows[1].Cells[1].Value)) / 100;
                pcf_R3_m2 = (Convert.ToDouble(dataGridView1.Rows[2].Cells[1].Value)) / 100;
                pcf_R4_m2 = (Convert.ToDouble(dataGridView1.Rows[3].Cells[1].Value)) / 100;
                pcf_R5_m2 = (Convert.ToDouble(dataGridView1.Rows[4].Cells[1].Value)) / 100;
                pcf_R6_m2 = (Convert.ToDouble(dataGridView1.Rows[5].Cells[1].Value)) / 100;
                pcf_R7_m2 = (Convert.ToDouble(dataGridView1.Rows[6].Cells[1].Value)) / 100;
                pcf_R8_m2 = (Convert.ToDouble(dataGridView1.Rows[7].Cells[1].Value)) / 100;

                pcf_R1_m1 = (Convert.ToDouble(dataGridView1.Rows[0].Cells[2].Value)) / 100;
                pcf_R2_m1 = (Convert.ToDouble(dataGridView1.Rows[1].Cells[2].Value)) / 100;
                pcf_R3_m1 = (Convert.ToDouble(dataGridView1.Rows[2].Cells[2].Value)) / 100;
                pcf_R4_m1 = (Convert.ToDouble(dataGridView1.Rows[3].Cells[2].Value)) / 100;
                pcf_R5_m1 = (Convert.ToDouble(dataGridView1.Rows[4].Cells[2].Value)) / 100;
                pcf_R6_m1 = (Convert.ToDouble(dataGridView1.Rows[5].Cells[2].Value)) / 100;
                pcf_R7_m1 = (Convert.ToDouble(dataGridView1.Rows[6].Cells[2].Value)) / 100;
                pcf_R8_m1 = (Convert.ToDouble(dataGridView1.Rows[7].Cells[2].Value)) / 100;

                pcf_R1_p1 = (Convert.ToDouble(dataGridView1.Rows[0].Cells[3].Value)) / 100;
                pcf_R2_p1 = (Convert.ToDouble(dataGridView1.Rows[1].Cells[3].Value)) / 100;
                pcf_R3_p1 = (Convert.ToDouble(dataGridView1.Rows[2].Cells[3].Value)) / 100;
                pcf_R4_p1 = (Convert.ToDouble(dataGridView1.Rows[3].Cells[3].Value)) / 100;
                pcf_R5_p1 = (Convert.ToDouble(dataGridView1.Rows[4].Cells[3].Value)) / 100;
                pcf_R6_p1 = (Convert.ToDouble(dataGridView1.Rows[5].Cells[3].Value)) / 100;
                pcf_R7_p1 = (Convert.ToDouble(dataGridView1.Rows[6].Cells[3].Value)) / 100;
                pcf_R8_p1 = (Convert.ToDouble(dataGridView1.Rows[7].Cells[3].Value)) / 100;

                pcf_R1_p2 = (Convert.ToDouble(dataGridView1.Rows[0].Cells[4].Value)) / 100;
                pcf_R2_p2 = (Convert.ToDouble(dataGridView1.Rows[1].Cells[4].Value)) / 100;
                pcf_R3_p2 = (Convert.ToDouble(dataGridView1.Rows[2].Cells[4].Value)) / 100;
                pcf_R4_p2 = (Convert.ToDouble(dataGridView1.Rows[3].Cells[4].Value)) / 100;
                pcf_R5_p2 = (Convert.ToDouble(dataGridView1.Rows[4].Cells[4].Value)) / 100;
                pcf_R6_p2 = (Convert.ToDouble(dataGridView1.Rows[5].Cells[4].Value)) / 100;
                pcf_R7_p2 = (Convert.ToDouble(dataGridView1.Rows[6].Cells[4].Value)) / 100;
                pcf_R8_p2 = (Convert.ToDouble(dataGridView1.Rows[7].Cells[4].Value)) / 100;

                AcP_R1 = (1 - (pcf_R1_m2 + pcf_R1_m1 + pcf_R1_p1 + pcf_R1_p2));
                AcP_R2 = (1 - (pcf_R2_m2 + pcf_R2_m1 + pcf_R2_p1 + pcf_R2_p2));
                AcP_R3 = (1 - (pcf_R3_m2 + pcf_R3_m1 + pcf_R3_p1 + pcf_R3_p2));
                AcP_R4 = (1 - (pcf_R4_m2 + pcf_R4_m1 + pcf_R4_p1 + pcf_R4_p2));
                AcP_R5 = (1 - (pcf_R5_m2 + pcf_R5_m1 + pcf_R5_p1 + pcf_R5_p2));
                AcP_R6 = (1 - (pcf_R6_m2 + pcf_R6_m1 + pcf_R6_p1 + pcf_R6_p2));
                AcP_R7 = (1 - (pcf_R7_m2 + pcf_R7_m1 + pcf_R7_p1 + pcf_R7_p2));
                AcP_R8 = (1 - (pcf_R8_m2 + pcf_R8_m1 + pcf_R8_p1 + pcf_R8_p2));

                Matrix CADCoefficientMatrix = DenseMatrix.OfArray(new double[,]
                { 
                { AcP_R1,       pcf_R2_m1,      pcf_R3_m2,   0  ,                0 ,                 0,                 0,          0},
                { pcf_R1_p1,    AcP_R2,         pcf_R3_m1,   pcf_R4_m2,          0 ,                 0,                 0,          0},
                { pcf_R1_p2,    pcf_R2_p1,      AcP_R3,      pcf_R4_m1,          pcf_R5_m2,          0,                 0,          0},
                { 0    ,        pcf_R2_p2,      pcf_R3_p1,   AcP_R4,             pcf_R5_m1,          pcf_R6_m2,         0,          0}, 
                { 0    ,        0 ,             pcf_R3_p2,   pcf_R4_p1,          AcP_R5,             pcf_R6_m1,         pcf_R7_m2,  0}, 
                { 0    ,        0,              0,           pcf_R4_p2,          pcf_R5_p1,          AcP_R6,            pcf_R7_m1,  pcf_R8_m2}, 
                { 0    ,        0 ,             0,           0,                  pcf_R5_p2,          pcf_R6_p1,         AcP_R7,     pcf_R8_m1}, 
                { 0    ,        0,              0,           0,                  0,                  pcf_R6_p2,         pcf_R7_p1,  AcP_R8}
                });

                // Determine if coefficient is real
                CADCoefficientMatrixDeterminant = CADCoefficientMatrix.Determinant();
            }  // end iTRAQ (or TMT-8) purity correction factors

            if(radioBox_TMT6.Checked)
            {
                //Purity Correction Factors begin for TMT 6_Plex
                pcf_R1_m2 = (Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value) / 100);
                pcf_R2_m2 = (Convert.ToDouble(dataGridView1.Rows[1].Cells[1].Value) / 100);
                pcf_R3_m2 = (Convert.ToDouble(dataGridView1.Rows[2].Cells[1].Value) / 100);
                pcf_R4_m2 = (Convert.ToDouble(dataGridView1.Rows[3].Cells[1].Value) / 100);
                pcf_R5_m2 = (Convert.ToDouble(dataGridView1.Rows[4].Cells[1].Value) / 100);
                pcf_R6_m2 = (Convert.ToDouble(dataGridView1.Rows[5].Cells[1].Value) / 100);

                pcf_R1_m1 = (Convert.ToDouble(dataGridView1.Rows[0].Cells[2].Value) / 100);
                pcf_R2_m1 = (Convert.ToDouble(dataGridView1.Rows[1].Cells[2].Value) / 100);
                pcf_R3_m1 = (Convert.ToDouble(dataGridView1.Rows[2].Cells[2].Value) / 100);
                pcf_R4_m1 = (Convert.ToDouble(dataGridView1.Rows[3].Cells[2].Value) / 100);
                pcf_R5_m1 = (Convert.ToDouble(dataGridView1.Rows[4].Cells[2].Value) / 100);
                pcf_R6_m1 = (Convert.ToDouble(dataGridView1.Rows[5].Cells[2].Value) / 100);

                pcf_R1_p1 = (Convert.ToDouble(dataGridView1.Rows[0].Cells[3].Value) / 100);
                pcf_R2_p1 = (Convert.ToDouble(dataGridView1.Rows[1].Cells[3].Value) / 100);
                pcf_R3_p1 = (Convert.ToDouble(dataGridView1.Rows[2].Cells[3].Value) / 100);
                pcf_R4_p1 = (Convert.ToDouble(dataGridView1.Rows[3].Cells[3].Value) / 100);
                pcf_R5_p1 = (Convert.ToDouble(dataGridView1.Rows[4].Cells[3].Value) / 100);
                pcf_R6_p1 = (Convert.ToDouble(dataGridView1.Rows[5].Cells[3].Value) / 100);

                pcf_R1_p2 = (Convert.ToDouble(dataGridView1.Rows[0].Cells[4].Value) / 100);
                pcf_R2_p2 = (Convert.ToDouble(dataGridView1.Rows[1].Cells[4].Value) / 100);
                pcf_R3_p2 = (Convert.ToDouble(dataGridView1.Rows[2].Cells[4].Value) / 100);
                pcf_R4_p2 = (Convert.ToDouble(dataGridView1.Rows[3].Cells[4].Value) / 100);
                pcf_R5_p2 = (Convert.ToDouble(dataGridView1.Rows[4].Cells[4].Value) / 100);
                pcf_R6_p2 = (Convert.ToDouble(dataGridView1.Rows[5].Cells[4].Value) / 100);

                AcP_R1 = (1 - (pcf_R1_m2 + pcf_R1_m1 + pcf_R1_p1 + pcf_R1_p2));
                AcP_R2 = (1 - (pcf_R2_m2 + pcf_R2_m1 + pcf_R2_p1 + pcf_R2_p2));
                AcP_R3 = (1 - (pcf_R3_m2 + pcf_R3_m1 + pcf_R3_p1 + pcf_R3_p2));
                AcP_R4 = (1 - (pcf_R4_m2 + pcf_R4_m1 + pcf_R4_p1 + pcf_R4_p2));
                AcP_R5 = (1 - (pcf_R5_m2 + pcf_R5_m1 + pcf_R5_p1 + pcf_R5_p2));
                AcP_R6 = (1 - (pcf_R6_m2 + pcf_R6_m1 + pcf_R6_p1 + pcf_R6_p2));

                Matrix CADCoefficientMatrix = DenseMatrix.OfArray(new double[,]
                { 
                { AcP_R1,       pcf_R2_m1,      pcf_R3_m2,   0  ,                0 ,                 0          },
                { pcf_R1_p1,    AcP_R2,         pcf_R3_m1,   pcf_R4_m2,          0 ,                 0          },
                { pcf_R1_p2,    pcf_R2_p1,      AcP_R3,      pcf_R4_m1,          pcf_R5_m2,          0          },
                { 0    ,        pcf_R2_p2,      pcf_R3_p1,   AcP_R4,             pcf_R5_m1,          pcf_R6_m2  }, 
                { 0    ,        0 ,             pcf_R3_p2,   pcf_R4_p1,          AcP_R5,             pcf_R6_m1  }, 
                { 0    ,        0,              0,           pcf_R4_p2,          pcf_R5_p1,          AcP_R6     }, 
                });

                // Determine if coefficient is real
                CADCoefficientMatrixDeterminant = CADCoefficientMatrix.Determinant();
               
            }  // end iTRAQ purity correction factors
            
            if (radioBox_TMT8.Checked)
            {
                //Purity Correction Factors begin for TMT 6_Plex
                pcf_R1_m2 = (Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value) / 100);
                pcf_R2_m2 = (Convert.ToDouble(dataGridView1.Rows[2].Cells[1].Value) / 100);
                pcf_R3_m2 = (Convert.ToDouble(dataGridView1.Rows[3].Cells[1].Value) / 100);
                pcf_R4_m2 = (Convert.ToDouble(dataGridView1.Rows[5].Cells[1].Value) / 100);
                pcf_R5_m2 = (Convert.ToDouble(dataGridView1.Rows[6].Cells[1].Value) / 100);
                pcf_R6_m2 = (Convert.ToDouble(dataGridView1.Rows[7].Cells[1].Value) / 100);
                pcf_R7_m2 = (Convert.ToDouble(dataGridView1.Rows[1].Cells[1].Value) / 100); //TMT-8plex 127
                pcf_R8_m2 = (Convert.ToDouble(dataGridView1.Rows[4].Cells[1].Value) / 100); //TMT-8plex 129

                pcf_R1_m1 = (Convert.ToDouble(dataGridView1.Rows[0].Cells[2].Value) / 100);
                pcf_R2_m1 = (Convert.ToDouble(dataGridView1.Rows[2].Cells[2].Value) / 100);
                pcf_R3_m1 = (Convert.ToDouble(dataGridView1.Rows[3].Cells[2].Value) / 100);
                pcf_R4_m1 = (Convert.ToDouble(dataGridView1.Rows[5].Cells[2].Value) / 100);
                pcf_R5_m1 = (Convert.ToDouble(dataGridView1.Rows[6].Cells[2].Value) / 100);
                pcf_R6_m1 = (Convert.ToDouble(dataGridView1.Rows[7].Cells[2].Value) / 100);
                pcf_R7_m1 = (Convert.ToDouble(dataGridView1.Rows[1].Cells[2].Value) / 100); //TMT-8plex 127
                pcf_R8_m1 = (Convert.ToDouble(dataGridView1.Rows[4].Cells[2].Value) / 100); //TMT-8plex 129

                pcf_R1_p1 = (Convert.ToDouble(dataGridView1.Rows[0].Cells[3].Value) / 100);
                pcf_R2_p1 = (Convert.ToDouble(dataGridView1.Rows[2].Cells[3].Value) / 100);
                pcf_R3_p1 = (Convert.ToDouble(dataGridView1.Rows[3].Cells[3].Value) / 100);
                pcf_R4_p1 = (Convert.ToDouble(dataGridView1.Rows[5].Cells[3].Value) / 100);
                pcf_R5_p1 = (Convert.ToDouble(dataGridView1.Rows[6].Cells[3].Value) / 100);
                pcf_R6_p1 = (Convert.ToDouble(dataGridView1.Rows[7].Cells[3].Value) / 100);
                pcf_R7_p1 = (Convert.ToDouble(dataGridView1.Rows[1].Cells[3].Value) / 100); //TMT-8plex 127
                pcf_R8_p1 = (Convert.ToDouble(dataGridView1.Rows[4].Cells[3].Value) / 100); //TMT-8plex 129

                pcf_R1_p2 = (Convert.ToDouble(dataGridView1.Rows[0].Cells[4].Value) / 100);
                pcf_R2_p2 = (Convert.ToDouble(dataGridView1.Rows[2].Cells[4].Value) / 100);
                pcf_R3_p2 = (Convert.ToDouble(dataGridView1.Rows[3].Cells[4].Value) / 100);
                pcf_R4_p2 = (Convert.ToDouble(dataGridView1.Rows[5].Cells[4].Value) / 100);
                pcf_R5_p2 = (Convert.ToDouble(dataGridView1.Rows[6].Cells[4].Value) / 100);
                pcf_R6_p2 = (Convert.ToDouble(dataGridView1.Rows[7].Cells[4].Value) / 100);
                pcf_R7_p2 = (Convert.ToDouble(dataGridView1.Rows[1].Cells[4].Value) / 100); //TMT-8plex 127
                pcf_R8_p2 = (Convert.ToDouble(dataGridView1.Rows[4].Cells[4].Value) / 100); //TMT-8plex 129

                AcP_R1 = (1 - (pcf_R1_m2 + pcf_R1_m1 + pcf_R1_p1 + pcf_R1_p2));
                AcP_R2 = (1 - (pcf_R2_m2 + pcf_R2_m1 + pcf_R2_p1 + pcf_R2_p2));
                AcP_R3 = (1 - (pcf_R3_m2 + pcf_R3_m1 + pcf_R3_p1 + pcf_R3_p2));
                AcP_R4 = (1 - (pcf_R4_m2 + pcf_R4_m1 + pcf_R4_p1 + pcf_R4_p2));
                AcP_R5 = (1 - (pcf_R5_m2 + pcf_R5_m1 + pcf_R5_p1 + pcf_R5_p2));
                AcP_R6 = (1 - (pcf_R6_m2 + pcf_R6_m1 + pcf_R6_p1 + pcf_R6_p2)); 
                AcP_R7 = (1 - (pcf_R7_m2 + pcf_R7_m1 + pcf_R7_p1 + pcf_R7_p2)); //TMT-8plex 127
                AcP_R8 = (1 - (pcf_R8_m2 + pcf_R8_m1 + pcf_R8_p1 + pcf_R8_p2)); //TMT-8plex 129

                Matrix CADCoefficientMatrix = DenseMatrix.OfArray(new double[,]
                { 
                    { AcP_R1,       pcf_R2_m1,      pcf_R3_m2,   0  ,                0 ,                 0          },
                    { pcf_R1_p1,    AcP_R2,         pcf_R3_m1,   pcf_R4_m2,          0 ,                 0          },
                    { pcf_R1_p2,    pcf_R2_p1,      AcP_R3,      pcf_R4_m1,          pcf_R5_m2,          0          },
                    { 0    ,        pcf_R2_p2,      pcf_R3_p1,   AcP_R4,             pcf_R5_m1,          pcf_R6_m2  }, 
                    { 0    ,        0 ,             pcf_R3_p2,   pcf_R4_p1,          AcP_R5,             pcf_R6_m1  }, 
                    { 0    ,        0,              0,           pcf_R4_p2,          pcf_R5_p1,          AcP_R6     }, 
                });

                Matrix TMT10_CADCoefficientMatrix = DenseMatrix.OfArray(new double[,]
                { 
                    { AcP_R7,       pcf_R8_m2   },
                    { pcf_R7_p2,    AcP_R8     }, 
                });

                // Determine if coefficient is real
                CADCoefficientMatrixDeterminant = CADCoefficientMatrix.Determinant();
                TMT10_CADCoefficientMatrixDeterminant = TMT10_CADCoefficientMatrix.Determinant();
              
            }  // end TMT 8-plex purity correction factors
            
            if (radioBox_TMT10.Checked)
            {

                //double[,] data1 = new double[6,4];
                //for (int i = 0; i < 6; i ++)
                //{
                //    for (int j = 1; j <= 4; j++)
                //    {
                //        data1[i, j - 1] = Convert.ToDouble(dataGridView1.Rows[i].Cells[j].Value);
                //    }
                //}

                //IsobaricTagPurityCorrection correction = IsobaricTagPurityCorrection.Create(data1);
                
                //Purity Correction Factors begin for TMT 10_Plex
                pcf_R1_m2 = (Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value) / 100);
                pcf_R2_m2 = (Convert.ToDouble(dataGridView1.Rows[2].Cells[1].Value) / 100);
                pcf_R3_m2 = (Convert.ToDouble(dataGridView1.Rows[4].Cells[1].Value) / 100);
                pcf_R4_m2 = (Convert.ToDouble(dataGridView1.Rows[6].Cells[1].Value) / 100);
                pcf_R5_m2 = (Convert.ToDouble(dataGridView1.Rows[8].Cells[1].Value) / 100);
                pcf_R6_m2 = (Convert.ToDouble(dataGridView1.Rows[9].Cells[1].Value) / 100);
                pcf_R7_m2 = (Convert.ToDouble(dataGridView1.Rows[1].Cells[1].Value) / 100); //TMT-8plex 127
                pcf_R8_m2 = (Convert.ToDouble(dataGridView1.Rows[3].Cells[1].Value) / 100); //TMT-8plex 128
                pcf_R9_m2 = (Convert.ToDouble(dataGridView1.Rows[5].Cells[1].Value) / 100); //TMT-8plex 129
                pcf_R10_m2 = (Convert.ToDouble(dataGridView1.Rows[7].Cells[1].Value) / 100); //TMT-8plex 130

                pcf_R1_m1 = (Convert.ToDouble(dataGridView1.Rows[0].Cells[2].Value) / 100);
                pcf_R2_m1 = (Convert.ToDouble(dataGridView1.Rows[2].Cells[2].Value) / 100);
                pcf_R3_m1 = (Convert.ToDouble(dataGridView1.Rows[4].Cells[2].Value) / 100);
                pcf_R4_m1 = (Convert.ToDouble(dataGridView1.Rows[6].Cells[2].Value) / 100);
                pcf_R5_m1 = (Convert.ToDouble(dataGridView1.Rows[8].Cells[2].Value) / 100);
                pcf_R6_m1 = (Convert.ToDouble(dataGridView1.Rows[9].Cells[2].Value) / 100);
                pcf_R7_m1 = (Convert.ToDouble(dataGridView1.Rows[1].Cells[2].Value) / 100); //TMT-8plex 127
                pcf_R8_m1 = (Convert.ToDouble(dataGridView1.Rows[3].Cells[2].Value) / 100); //TMT-8plex 128
                pcf_R9_m1 = (Convert.ToDouble(dataGridView1.Rows[5].Cells[2].Value) / 100); //TMT-8plex 129
                pcf_R10_m1 = (Convert.ToDouble(dataGridView1.Rows[7].Cells[2].Value) / 100); //TMT-8plex 130

                pcf_R1_p1 = (Convert.ToDouble(dataGridView1.Rows[0].Cells[3].Value) / 100);
                pcf_R2_p1 = (Convert.ToDouble(dataGridView1.Rows[2].Cells[3].Value) / 100);
                pcf_R3_p1 = (Convert.ToDouble(dataGridView1.Rows[4].Cells[3].Value) / 100);
                pcf_R4_p1 = (Convert.ToDouble(dataGridView1.Rows[6].Cells[3].Value) / 100);
                pcf_R5_p1 = (Convert.ToDouble(dataGridView1.Rows[8].Cells[3].Value) / 100);
                pcf_R6_p1 = (Convert.ToDouble(dataGridView1.Rows[9].Cells[3].Value) / 100);
                pcf_R7_p1 = (Convert.ToDouble(dataGridView1.Rows[1].Cells[3].Value) / 100); //TMT-8plex 127
                pcf_R8_p1 = (Convert.ToDouble(dataGridView1.Rows[3].Cells[3].Value) / 100); //TMT-8plex 128
                pcf_R9_p1 = (Convert.ToDouble(dataGridView1.Rows[5].Cells[3].Value) / 100); //TMT-8plex 129
                pcf_R10_p1 = (Convert.ToDouble(dataGridView1.Rows[7].Cells[3].Value) / 100); //TMT-8plex 130

                pcf_R1_p2 = (Convert.ToDouble(dataGridView1.Rows[0].Cells[4].Value) / 100);
                pcf_R2_p2 = (Convert.ToDouble(dataGridView1.Rows[2].Cells[4].Value) / 100);
                pcf_R3_p2 = (Convert.ToDouble(dataGridView1.Rows[4].Cells[4].Value) / 100);
                pcf_R4_p2 = (Convert.ToDouble(dataGridView1.Rows[6].Cells[4].Value) / 100);
                pcf_R5_p2 = (Convert.ToDouble(dataGridView1.Rows[8].Cells[4].Value) / 100);
                pcf_R6_p2 = (Convert.ToDouble(dataGridView1.Rows[9].Cells[4].Value) / 100);
                pcf_R7_p2 = (Convert.ToDouble(dataGridView1.Rows[1].Cells[4].Value) / 100); //TMT-8plex 127
                pcf_R8_p2 = (Convert.ToDouble(dataGridView1.Rows[3].Cells[4].Value) / 100); //TMT-8plex 128
                pcf_R9_p2 = (Convert.ToDouble(dataGridView1.Rows[5].Cells[4].Value) / 100); //TMT-8plex 129
                pcf_R10_p2 = (Convert.ToDouble(dataGridView1.Rows[7].Cells[4].Value) / 100); //TMT-8plex 130

                AcP_R1 = (1 - (pcf_R1_m2 + pcf_R1_m1 + pcf_R1_p1 + pcf_R1_p2));
                AcP_R2 = (1 - (pcf_R2_m2 + pcf_R2_m1 + pcf_R2_p1 + pcf_R2_p2));
                AcP_R3 = (1 - (pcf_R3_m2 + pcf_R3_m1 + pcf_R3_p1 + pcf_R3_p2));
                AcP_R4 = (1 - (pcf_R4_m2 + pcf_R4_m1 + pcf_R4_p1 + pcf_R4_p2));
                AcP_R5 = (1 - (pcf_R5_m2 + pcf_R5_m1 + pcf_R5_p1 + pcf_R5_p2));
                AcP_R6 = (1 - (pcf_R6_m2 + pcf_R6_m1 + pcf_R6_p1 + pcf_R6_p2));
                AcP_R7 = (1 - (pcf_R7_m2 + pcf_R7_m1 + pcf_R7_p1 + pcf_R7_p2)); //TMT-8plex 127
                AcP_R8 = (1 - (pcf_R8_m2 + pcf_R8_m1 + pcf_R8_p1 + pcf_R8_p2)); //TMT-8plex 128
                AcP_R9 = (1 - (pcf_R9_m2 + pcf_R9_m1 + pcf_R9_p1 + pcf_R9_p2)); //TMT-8plex 129
                AcP_R10 = (1 - (pcf_R10_m2 + pcf_R10_m1 + pcf_R10_p1 + pcf_R10_p2)); //TMT-8plex 130

                Matrix CADCoefficientMatrix = DenseMatrix.OfArray(new double[,]
                { 
                    { AcP_R1,       pcf_R2_m1,      pcf_R3_m2,   0  ,                0 ,                 0          },
                    { pcf_R1_p1,    AcP_R2,         pcf_R3_m1,   pcf_R4_m2,          0 ,                 0          },
                    { pcf_R1_p2,    pcf_R2_p1,      AcP_R3,      pcf_R4_m1,          pcf_R5_m2,          0          },
                    { 0    ,        pcf_R2_p2,      pcf_R3_p1,   AcP_R4,             pcf_R5_m1,          pcf_R6_m2  }, 
                    { 0    ,        0 ,             pcf_R3_p2,   pcf_R4_p1,          AcP_R5,             pcf_R6_m1  }, 
                    { 0    ,        0,              0,           pcf_R4_p2,          pcf_R5_p1,          AcP_R6     }, 
                });

                Matrix TMT10_CADCoefficientMatrix = DenseMatrix.OfArray(new double[,]
                { 
                    { AcP_R7,       pcf_R8_m1,  pcf_R9_m2,   0              },
                    { pcf_R7_p1,    AcP_R8,     pcf_R9_m1,   pcf_R10_m2     },
                    { pcf_R7_p2,    pcf_R8_p1,  AcP_R9,      pcf_R10_m1     },
                    { 0     ,       pcf_R8_p2,  pcf_R9_p1,   AcP_R10         }, 
                });
                
                CADCoefficientMatrixDeterminant = CADCoefficientMatrix.Determinant();
                TMT10_CADCoefficientMatrixDeterminant = TMT10_CADCoefficientMatrix.Determinant();
             
                
            }  // end TMT 10-plex purity correction factors

            //Purity Correction Factors end

            // Create dictionary for peptide tag signals
            Dictionary<string, Dictionary<FragmentationMethod, double>> TotalTagSignal = new Dictionary<string, Dictionary<FragmentationMethod, double>>();
           
            double peptidecount = 0;
            //double ITerror = (double)numericUpDown1.Value;
            //double FTerror = (double)numericUpDown2.Value;
            //double error;

            // Create log file
            string LogOutPutName = Path.Combine(textOutputFolder.Text, "TagQuant_log.txt");
            StreamWriter log = new StreamWriter(LogOutPutName);
            log.AutoFlush = true;
            log.WriteLine("TagQuant PARAMETERS");
            log.WriteLine("Labeling Reagent: " + (radioBox_TMT2.Checked ? radioBox_TMT2.Text : (radioBox_iTRAQ4.Checked ? radioBox_iTRAQ4.Text : (radioBox_TMT6.Checked ? radioBox_TMT6.Text : radioBox_iTRAQ8.Text))));
            log.WriteLine();

            log.WriteLine("Tags and Samples Used");

            SortedDictionary<int, TagInformation> Samples = new SortedDictionary<int, TagInformation>();

            string sample = "unused";

            SortedList<double, TagInformation> SamplesAndTags = new SortedList<double, TagInformation>();

            /*
            if(radioBox_iTRAQ4.Checked)
            {
                for(int i = 114; i <= 117; i++)
                {
                    Application.DoEvents();
                    if(i == 114)
                    {
                        sample = textBox114.Text;
                        if(sample.Length == 0) { sample = "unused114"; }
                        if(!SamplesAndTags.ContainsKey(sample))
                        {
                            SamplesAndTags.Add(sample, i);
                        }
                    }
                    if(i == 115)
                    {
                        sample = textBox115.Text;
                        if(sample.Length == 0) { sample = "unused115"; }
                        if(!SamplesAndTags.ContainsKey(sample))
                        {
                            SamplesAndTags.Add(sample, i);
                        }
                    }
                    if(i == 116)
                    {
                        sample = textBox116.Text;
                        if(sample.Length == 0) { sample = "unused116"; }
                        if(!SamplesAndTags.ContainsKey(sample))
                        {
                            SamplesAndTags.Add(sample, i);
                        }
                    }
                    if(i == 117)
                    {
                        sample = textBox117.Text;
                        if(sample.Length == 0) { sample = "unused117"; }
                        if(!SamplesAndTags.ContainsKey(sample))
                        {
                            SamplesAndTags.Add(sample, i);
                        }
                    }
                    TagInformation TagInfo = new TagInformation(i, sample);
                    TotalTagSignal.Add(i, new Dictionary<FragmentationMethod, Double>());
                    TotalTagSignal[i].Add(FragmentationMethod.CAD, 0);
                    TotalTagSignal[i].Add(FragmentationMethod.ETD, 0);
                    Samples.Add(i, TagInfo);
                    log.WriteLine(i + "  " + sample);
                }
            }

            if(radioBox_iTRAQ8.Checked)
            {
                for(int i = 113; i <= 121; i++)
                {
                    if(i == 120)
                    {
                        i++;
                    }
                    Application.DoEvents();
                    if(i == 113)
                    {
                        sample = textBox113.Text;
                        if(sample.Length == 0) { sample = "unused113"; }
                        if(!SamplesAndTags.ContainsKey(sample))
                        {
                            SamplesAndTags.Add(sample, i);
                        }
                    }
                    if(i == 114)
                    {
                        sample = textBox114.Text;
                        if(sample.Length == 0) { sample = "unused114"; }
                        if(!SamplesAndTags.ContainsKey(sample))
                        {
                            SamplesAndTags.Add(sample, i);
                        }
                    }
                    if(i == 115)
                    {
                        sample = textBox115.Text;
                        if(sample.Length == 0) { sample = "unused115"; }
                        if(!SamplesAndTags.ContainsKey(sample))
                        {
                            SamplesAndTags.Add(sample, i);
                        }
                    }
                    if(i == 116)
                    {
                        sample = textBox116.Text;
                        if(sample.Length == 0) { sample = "unused116"; }
                        if(!SamplesAndTags.ContainsKey(sample))
                        {
                            SamplesAndTags.Add(sample, i);
                        }
                    }
                    if(i == 117)
                    {
                        sample = textBox117.Text;
                        if(sample.Length == 0) { sample = "unused117"; }
                        if(!SamplesAndTags.ContainsKey(sample))
                        {
                            SamplesAndTags.Add(sample, i);
                        }
                    }
                    if(i == 118)
                    {
                        sample = textBox118.Text;
                        if(sample.Length == 0) { sample = "unused118"; }
                        if(!SamplesAndTags.ContainsKey(sample))
                        {
                            SamplesAndTags.Add(sample, i);
                        }
                    }
                    if(i == 119)
                    {
                        sample = textBox119.Text;
                        if(sample.Length == 0) { sample = "unused119"; }
                        if(!SamplesAndTags.ContainsKey(sample))
                        {
                            SamplesAndTags.Add(sample, i);
                        }
                    }
                    if(i == 121)
                    {
                        sample = textBox121.Text;
                        if(sample.Length == 0) { sample = "unused121"; }
                        if(!SamplesAndTags.ContainsKey(sample))
                        {
                            SamplesAndTags.Add(sample, i);
                        }
                    }
                    TagInformation TagInfo = new TagInformation(i, sample);
                    TotalTagSignal.Add(i, new Dictionary<FragmentationMethod, Double>());
                    TotalTagSignal[i].Add(FragmentationMethod.CAD, 0);
                    TotalTagSignal[i].Add(FragmentationMethod.ETD, 0);
                    Samples.Add(i, TagInfo);
                    log.WriteLine(i + "  " + sample);
                }
            }

            if(radioBox_TMT6.Checked)
            {
                for(int i = 126; i <= 131; i++)
                {
                    Application.DoEvents();
                    if(i == 126)
                    {
                        sample = textBox126.Text;
                        if(sample.Length == 0) { sample = "unused126"; }
                        if(!SamplesAndTags.ContainsKey(sample))
                        {
                            SamplesAndTags.Add(sample, i);
                        }
                    }

                    if(i == 127)
                    {
                        sample = textBox127.Text;
                        if(sample.Length == 0) { sample = "unused127"; }
                        if(!SamplesAndTags.ContainsKey(sample))
                        {
                            SamplesAndTags.Add(sample, i);
                        }
                    }

                    if(i == 128)
                    {
                        sample = textBox128.Text;
                        if(sample.Length == 0) { sample = "unused128"; }
                        if(!SamplesAndTags.ContainsKey(sample))
                        {
                            SamplesAndTags.Add(sample, i);
                        }
                    }

                    if(i == 129)
                    {
                        sample = textBox129.Text;
                        if(sample.Length == 0) { sample = "unused129"; }
                        if(!SamplesAndTags.ContainsKey(sample))
                        {
                            SamplesAndTags.Add(sample, i);
                        }
                    }

                    if(i == 130)
                    {
                        sample = textBox130.Text;
                        if(sample.Length == 0) { sample = "unused130"; }
                        if(!SamplesAndTags.ContainsKey(sample))
                        {
                            SamplesAndTags.Add(sample, i);
                        }
                    }

                    if(i == 131)
                    {
                        sample = textBox131.Text;
                        if(sample.Length == 0) { sample = "unused131"; }
                        if(!SamplesAndTags.ContainsKey(sample))
                        {
                            SamplesAndTags.Add(sample, i);
                        }
                    }

                    TagInformation TagInfo = new TagInformation(i, sample);
                    TotalTagSignal.Add(i, new Dictionary<FragmentationMethod, Double>());
                    TotalTagSignal[i].Add(FragmentationMethod.CAD, 0);
                    TotalTagSignal[i].Add(FragmentationMethod.ETD, 0);
                    Samples.Add(i, TagInfo);
                    log.WriteLine(i + "  " + sample);
                }
            }

            if (radioBox_TMT8.Checked)
            {
                for (int i = 126; i <= 133; i++)
                {
                    Application.DoEvents();
                    if (i == 126)
                    {
                        sample = textBox126.Text;
                        if (sample.Length == 0) { sample = "unused126"; }
                        if (!SamplesAndTags.ContainsKey(sample))
                        {
                            SamplesAndTags.Add(sample, i);
                        }
                    }
                    if (i == 132)
                    {
                            sample = textBox127_N.Text;
                            if (sample.Length == 0) { sample = "unused127*"; }
                            if (!SamplesAndTags.ContainsKey(sample))
                            {
                                SamplesAndTags.Add(sample, i);
                            }
                    }
                    if (i == 127)
                    {
                        sample = textBox127.Text;
                        if (sample.Length == 0) { sample = "unused127"; }
                        if (!SamplesAndTags.ContainsKey(sample))
                        {
                            SamplesAndTags.Add(sample, i);
                        }
                    }
                    if (i == 128)
                    {
                        sample = textBox128.Text;
                        if (sample.Length == 0) { sample = "unused128"; }
                        if (!SamplesAndTags.ContainsKey(sample))
                        {
                            SamplesAndTags.Add(sample, i);
                        }
                    }

                    if (i == 129)
                    {
                        sample = textBox129.Text;
                        if (sample.Length == 0) { sample = "unused129"; }
                        if (!SamplesAndTags.ContainsKey(sample))
                        {
                            SamplesAndTags.Add(sample, i);
                        }
                    }
                    if (i == 133)
                    {
                        sample = textBox129_N.Text;
                        if (sample.Length == 0) { sample = "unused129*"; }
                        if (!SamplesAndTags.ContainsKey(sample))
                        {
                            SamplesAndTags.Add(sample, i);
                        }
                    }
                    if (i == 130)
                    {
                        sample = textBox130.Text;
                        if (sample.Length == 0) { sample = "unused130"; }
                        if (!SamplesAndTags.ContainsKey(sample))
                        {
                            SamplesAndTags.Add(sample, i);
                        }
                    }
                    if (i == 131)
                    {
                        sample = textBox131.Text;
                        if (sample.Length == 0) { sample = "unused131"; }
                        if (!SamplesAndTags.ContainsKey(sample))
                        {
                            SamplesAndTags.Add(sample, i);
                        }
                    }
                    
                    

                    TagInformation TagInfo = new TagInformation(i, sample);
                    TotalTagSignal.Add(i, new Dictionary<FragmentationMethod, Double>());
                    TotalTagSignal[i].Add(FragmentationMethod.CAD, 0);
                    TotalTagSignal[i].Add(FragmentationMethod.ETD, 0);

                    Samples.Add(i, TagInfo);
                    log.WriteLine(i + "  " + sample);
                }
            }
            */

            if (radioBox_TMT10.Checked)
            {
                var tags = new List<TagInformation>
                {
                    new TagInformation(126, "126", textBox126.Text, 126.1283, 114.1279, TagSetType.TMTC),
                    new TagInformation(127, "127N", textBox127_N.Text, 127.1253, 114.1279, TagSetType.TMTN),
                    new TagInformation(127, "127C", textBox127.Text, 127.1316, 114.1279, TagSetType.TMTC),
                    new TagInformation(128, "128N", textBox128_N.Text, 128.1287, 114.1279, TagSetType.TMTN),
                    new TagInformation(128, "128C", textBox128.Text, 128.1350, 114.1279, TagSetType.TMTC),
                    new TagInformation(129, "129N", textBox129_N.Text, 129.1320, 114.1279, TagSetType.TMTN),
                    new TagInformation(129, "129C", textBox129.Text, 129.1383, 114.1279, TagSetType.TMTC),
                    new TagInformation(130, "130N", textBox130_N.Text, 130.1354, 114.1279, TagSetType.TMTN),
                    new TagInformation(130, "130C", textBox130.Text, 130.1417, 118.1415, TagSetType.TMTC),
                    new TagInformation(131, "131", textBox130.Text, 131.1387, 119.1384, TagSetType.TMTC)
                };

                foreach (TagInformation tag in tags)
                {
                    SamplesAndTags.Add(tag.MassCAD, tag);
                    TotalTagSignal.Add(tag.TagName, new Dictionary<FragmentationMethod, Double>());
                    TotalTagSignal[tag.TagName].Add(FragmentationMethod.CAD, 0);
                    TotalTagSignal[tag.TagName].Add(FragmentationMethod.ETD, 0);
                }
            }

            log.WriteLine();

            // Print out purity correction factors (unless TMT 2-plex checked)
            if(!radioBox_TMT2.Checked)
            {
                log.WriteLine("Purity Correction Factors");
                for(int r = 0; r < dataGridView1.Rows.Count; r++)
                {
                    for(int c = 0; c < dataGridView1.Columns.Count; c++)
                    {
                        log.Write((string)dataGridView1.Rows[r].Cells[c].Value + '\t');
                    }
                    log.WriteLine();
                }
                log.WriteLine();
            }

            log.WriteLine("CAD Coefficient Matrix Determinant = " + CADCoefficientMatrixDeterminant);
            log.WriteLine("TMT10 CAD Coefficient Matrix Determinant = " + TMT10_CADCoefficientMatrixDeterminant);
            log.WriteLine();

            log.WriteLine("Input Files for Quantitation");
            

            foreach(string filename in listBox1.Items)
            {
                toolStripStatusLabel1.Text = "Working on File " + Convert.ToString(FileCounter) + " of " + Convert.ToString(FilesCount);
                Application.DoEvents();
                FileCounter++;
                //OPEN INPUT AND OUTPUT FILES

                StreamReader baseSteam = new StreamReader(filename);
                using (CsvReader reader = new CsvReader(baseSteam, true))
                {

                    string outputname = Path.Combine(textOutputFolder.Text,
                        Path.GetFileNameWithoutExtension(filename) + "_quant_temp.csv");
                    using (StreamWriter QuantTempFile = new StreamWriter(outputname))
                    {

                        log.WriteLine(filename);

                        //open raw file

                        string[] headerColumns = reader.GetFieldHeaders();
                        int headerCount = headerColumns.Length;
                        string header = string.Join(",", headerColumns);

                        StringBuilder sb = new StringBuilder();
                        sb.Append(header);
                        sb.Append(",Interference");
                       
                        foreach (TagInformation tag in SamplesAndTags.Values)
                        {
                            sb.AppendFormat(",{0} ({1} NL)", tag.SampleName, tag.TagName);
                        }

                        foreach (TagInformation tag in SamplesAndTags.Values)
                        {
                            sb.AppendFormat(",{0} ({1} dNL)", tag.SampleName, tag.TagName);
                        }
                        
                        //QuantTempFile.Write(",TQ_Total_int(all_tags)");

                        foreach (TagInformation tag in SamplesAndTags.Values)
                        {
                            sb.AppendFormat(",{0} ({1} PC)", tag.SampleName, tag.TagName);
                        }
                        

                        QuantTempFile.WriteLine(sb.ToString());

                        ThermoRawFile rawFile = null;

                        while (reader.ReadNextRecord()) // go through csv and raw file to extract the info we want
                        {
                            int scanNumber = int.Parse(reader["Spectrum number"]);
                            string filenameID = reader["Filename/id"];


                            string rawFileName = Path.Combine(textRawFolder.Text, filenameID.Split('.')[0] + ".raw");

                            // FIX ME

                            if (rawFile != null)
                            {
                                string RawFileCheck = rawFile.FilePath;
                                if (!RawFileCheck.Equals(rawFileName))
                                {
                                    rawFile.Dispose();
                                    if (!File.Exists(rawFileName))
                                    {
                                        throw new FileNotFoundException("Can't find file " + rawFileName);
                                    }

                                    rawFile = new ThermoRawFile(rawFileName);
                                }
                            }
                            else
                            {
                                if (!File.Exists(rawFileName))
                                {
                                    throw new FileNotFoundException("Can't find file " + rawFileName);
                                }

                                rawFile = new ThermoRawFile(rawFileName);
                            }

                            rawFile.Open();

                            // Set default fragmentation to CAD / HCD
                            FragmentationMethod ScanFragMethod = FragmentationMethod.CAD;

                            if (filenameID.Contains(".ETD."))
                            {
                                ScanFragMethod = FragmentationMethod.ETD;
                                if (ComboBoxETDoptions.Text == "Use Scan Before")
                                {
                                    scanNumber = scanNumber - 1;
                                    ScanFragMethod = FragmentationMethod.CAD;
                                }
                                if (ComboBoxETDoptions.Text == "Use Scan After")
                                {
                                    scanNumber = scanNumber + 1;
                                    ScanFragMethod = FragmentationMethod.CAD;
                                }
                            }

                            error = ITerror;
                            if (filenameID.Contains(".FTMS"))
                            {
                                error = FTerror;
                            }

                            // perform functions on raw file

                            // 1.) Determine Purity
                            double Purity = 0;
                            double PreInt = 0;
                            double TotalInt = 0;

                            // Get the scan object for the sequence ms2 scan
                            MsnDataScan seqScan = rawFile[scanNumber] as MsnDataScan;

                            if (seqScan == null)
                            {
                                throw new ArgumentException("Not an MS2 scan");
                            }

                            double injectionTime = seqScan.InjectionTime;
                            var massSpectrum = seqScan.MassSpectrum;

                            // Find the closest parent survery scan


                            //if(surveyScan != null)
                            //{   
                            //    double Charge = Convert.ToDouble(column_values[11]);
                            //    double IsoPreMZ = Convert.ToDouble(column_values[15]);
                            //    Range<double> range = new Range<double>(IsoPreMZ - 0.5 * IsoWindow, IsoPreMZ + 0.5 * IsoWindow);
                            //    List<MZPeak> peaks = null;

                            //    if (surveyScan.MassSpectrum.TryGetPeaks(out peaks, range))
                            //    {
                            //        foreach (MZPeak peak in peaks)
                            //        {
                            //            double difference = (peak.MZ - IsoPreMZ) * Charge;
                            //            double difference_Rounded = Math.Round(difference);
                            //            double expected_difference = difference_Rounded * C12_C13_MASS_DIFFERENCE;
                            //            double Difference_ppm = (Math.Abs((expected_difference - difference) / (IsoPreMZ * Charge))) * 1000000;

                            //            if (Difference_ppm <= PreErrorAllowed)
                            //            {
                            //                PreInt += peak.Intensity;
                            //            }
                            //            TotalInt += peak.Intensity;
                            //        }
                            //    }

                            //    Purity = PreInt / TotalInt;
                            //}


                            Dictionary<string, double> nlScanIntensityHash = new Dictionary<string, double>();
                            Dictionary<string, double> dNlScanIntensityHash = new Dictionary<string, double>();
                            Dictionary<string, double> pcfScanIntensityHash = new Dictionary<string, double>();

                            double TotalScanIntCounts = 0;

                            foreach (TagInformation tag in SamplesAndTags.Values)
                            {
                                nlScanIntensityHash.Add(tag.TagName, 0);
                                dNlScanIntensityHash.Add(tag.TagName, 0);
                                pcfScanIntensityHash.Add(tag.TagName, 0);
                            }

                            foreach (TagInformation tag in SamplesAndTags.Values)
                            {

                                double tagvalue = ScanFragMethod == FragmentationMethod.ETD
                                    ? tag.MassEtd
                                    : tag.MassCAD;
                            
                                var peak = massSpectrum.GetClosestPeak(tagvalue, error);

                                double nlCounts = peak != null ? peak.Intensity : 0;
                                if(noisebandCap) {

}
                                double dNlCounts = nlCounts*injectionTime;

                                nlScanIntensityHash[tag.TagName] += nlCounts;
                                dNlScanIntensityHash[tag.TagName] += dNlCounts;
                            }

                            switch (ScanFragMethod)
                            {
                                case FragmentationMethod.CAD:
                                    //if (radioBox_iTRAQ4.Checked)
                                    //{
                                    //    Matrix Delta_R1_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {
                                    //            dNlScanIntensityHash[114], dNlScanIntensityHash[115],
                                    //            dNlScanIntensityHash[116],
                                    //            dNlScanIntensityHash[117]
                                    //        },
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4}
                                    //    });

                                    //    Matrix Delta_R2_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0},
                                    //        {
                                    //            dNlScanIntensityHash[114], dNlScanIntensityHash[115],
                                    //            dNlScanIntensityHash[116],
                                    //            dNlScanIntensityHash[117]
                                    //        },
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4}
                                    //    });

                                    //    Matrix Delta_R3_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2},
                                    //        {
                                    //            dNlScanIntensityHash[114], dNlScanIntensityHash[115],
                                    //            dNlScanIntensityHash[116],
                                    //            dNlScanIntensityHash[117]
                                    //        },
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4}
                                    //    });

                                    //    Matrix Delta_R4_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1},
                                    //        {
                                    //            dNlScanIntensityHash[114], dNlScanIntensityHash[115],
                                    //            dNlScanIntensityHash[116],
                                    //            dNlScanIntensityHash[117]
                                    //        }
                                    //    });

                                    //    try
                                    //    {
                                    //        double DeterminantR1 = Delta_R1_CAD.Determinant();
                                    //        double DeterminantR2 = Delta_R2_CAD.Determinant();
                                    //        double DeterminantR3 = Delta_R3_CAD.Determinant();
                                    //        double DeterminantR4 = Delta_R4_CAD.Determinant();

                                    //        // Calculate the purity correction factor for each channel
                                    //        pcfScanIntensityHash[114] = (DeterminantR1/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[115] = (DeterminantR2/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[116] = (DeterminantR3/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[117] = (DeterminantR4/CADCoefficientMatrixDeterminant);
                                    //    }
                                    //    catch (DivideByZeroException)
                                    //    {
                                    //        pcfScanIntensityHash[114] = dNlScanIntensityHash[114];
                                    //        pcfScanIntensityHash[115] = dNlScanIntensityHash[115];
                                    //        pcfScanIntensityHash[116] = dNlScanIntensityHash[116];
                                    //        pcfScanIntensityHash[117] = dNlScanIntensityHash[117];
                                    //    }

                                    //    TotalScanIntCounts += pcfScanIntensityHash[114] + pcfScanIntensityHash[115] +
                                    //                          pcfScanIntensityHash[116] + pcfScanIntensityHash[117];
                                    //}
                                    //if (radioBox_iTRAQ8.Checked)
                                    //{
                                    //    Matrix Delta_R1_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {
                                    //            dNlScanIntensityHash[113], dNlScanIntensityHash[114],
                                    //            dNlScanIntensityHash[115],
                                    //            dNlScanIntensityHash[116], dNlScanIntensityHash[117],
                                    //            dNlScanIntensityHash[118],
                                    //            dNlScanIntensityHash[119], dNlScanIntensityHash[121]
                                    //        },
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0, 0, 0},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0, 0, 0},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2, 0, 0},
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1, pcf_R5_p2, 0},
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6, pcf_R6_p1, pcf_R6_p2},
                                    //        {0, 0, 0, 0, pcf_R7_m2, pcf_R7_m1, AcP_R7, pcf_R7_p1},
                                    //        {0, 0, 0, 0, 0, pcf_R8_m2, pcf_R8_m1, AcP_R8}
                                    //    });

                                    //    Matrix Delta_R2_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0, 0, 0},
                                    //        {
                                    //            dNlScanIntensityHash[113], dNlScanIntensityHash[114],
                                    //            dNlScanIntensityHash[115],
                                    //            dNlScanIntensityHash[116], dNlScanIntensityHash[117],
                                    //            dNlScanIntensityHash[118],
                                    //            dNlScanIntensityHash[119], dNlScanIntensityHash[121]
                                    //        },
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0, 0, 0},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2, 0, 0},
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1, pcf_R5_p2, 0},
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6, pcf_R6_p1, pcf_R6_p2},
                                    //        {0, 0, 0, 0, pcf_R7_m2, pcf_R7_m1, AcP_R7, pcf_R7_p1},
                                    //        {0, 0, 0, 0, 0, pcf_R8_m2, pcf_R8_m1, AcP_R8}
                                    //    });

                                    //    Matrix Delta_R3_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0, 0, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0, 0, 0},
                                    //        {
                                    //            dNlScanIntensityHash[113], dNlScanIntensityHash[114],
                                    //            dNlScanIntensityHash[115],
                                    //            dNlScanIntensityHash[116], dNlScanIntensityHash[117],
                                    //            dNlScanIntensityHash[118],
                                    //            dNlScanIntensityHash[119], dNlScanIntensityHash[121]
                                    //        },
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2, 0, 0},
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1, pcf_R5_p2, 0},
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6, pcf_R6_p1, pcf_R6_p2},
                                    //        {0, 0, 0, 0, pcf_R7_m2, pcf_R7_m1, AcP_R7, pcf_R7_p1},
                                    //        {0, 0, 0, 0, 0, pcf_R8_m2, pcf_R8_m1, AcP_R8}
                                    //    });

                                    //    Matrix Delta_R4_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0, 0, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0, 0, 0},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0, 0, 0},
                                    //        {
                                    //            dNlScanIntensityHash[113], dNlScanIntensityHash[114],
                                    //            dNlScanIntensityHash[115],
                                    //            dNlScanIntensityHash[116], dNlScanIntensityHash[117],
                                    //            dNlScanIntensityHash[118],
                                    //            dNlScanIntensityHash[119], dNlScanIntensityHash[121]
                                    //        },
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1, pcf_R5_p2, 0},
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6, pcf_R6_p1, pcf_R6_p2},
                                    //        {0, 0, 0, 0, pcf_R7_m2, pcf_R7_m1, AcP_R7, pcf_R7_p1},
                                    //        {0, 0, 0, 0, 0, pcf_R8_m2, pcf_R8_m1, AcP_R8}
                                    //    });

                                    //    Matrix Delta_R5_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0, 0, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0, 0, 0},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0, 0, 0},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2, 0, 0},
                                    //        {
                                    //            dNlScanIntensityHash[113], dNlScanIntensityHash[114],
                                    //            dNlScanIntensityHash[115],
                                    //            dNlScanIntensityHash[116], dNlScanIntensityHash[117],
                                    //            dNlScanIntensityHash[118],
                                    //            dNlScanIntensityHash[119], dNlScanIntensityHash[121]
                                    //        },
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6, pcf_R6_p1, pcf_R6_p2},
                                    //        {0, 0, 0, 0, pcf_R7_m2, pcf_R7_m1, AcP_R7, pcf_R7_p1},
                                    //        {0, 0, 0, 0, 0, pcf_R8_m2, pcf_R8_m1, AcP_R8}
                                    //    });

                                    //    Matrix Delta_R6_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0, 0, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0, 0, 0},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0, 0, 0},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2, 0, 0},
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1, pcf_R5_p2, 0},
                                    //        {
                                    //            dNlScanIntensityHash[113], dNlScanIntensityHash[114],
                                    //            dNlScanIntensityHash[115],
                                    //            dNlScanIntensityHash[116], dNlScanIntensityHash[117],
                                    //            dNlScanIntensityHash[118],
                                    //            dNlScanIntensityHash[119], dNlScanIntensityHash[121]
                                    //        },
                                    //        {0, 0, 0, 0, pcf_R7_m2, pcf_R7_m1, AcP_R7, pcf_R7_p1},
                                    //        {0, 0, 0, 0, 0, pcf_R8_m2, pcf_R8_m1, AcP_R8}
                                    //    });

                                    //    Matrix Delta_R7_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {

                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0, 0, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0, 0, 0},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0, 0, 0},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2, 0, 0},
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1, pcf_R5_p2, 0},
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6, pcf_R6_p1, pcf_R6_p2},
                                    //        {
                                    //            dNlScanIntensityHash[113], dNlScanIntensityHash[114],
                                    //            dNlScanIntensityHash[115],
                                    //            dNlScanIntensityHash[116], dNlScanIntensityHash[117],
                                    //            dNlScanIntensityHash[118],
                                    //            dNlScanIntensityHash[119], dNlScanIntensityHash[121]
                                    //        },
                                    //        {0, 0, 0, 0, 0, pcf_R8_m2, pcf_R8_m1, AcP_R8}
                                    //    });

                                    //    Matrix Delta_R8_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0, 0, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0, 0, 0},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0, 0, 0},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2, 0, 0},
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1, pcf_R5_p2, 0},
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6, pcf_R6_p1, pcf_R6_p2},
                                    //        {0, 0, 0, 0, pcf_R7_m2, pcf_R7_m1, AcP_R7, pcf_R7_p1},
                                    //        {
                                    //            dNlScanIntensityHash[113], dNlScanIntensityHash[114],
                                    //            dNlScanIntensityHash[115],
                                    //            dNlScanIntensityHash[116], dNlScanIntensityHash[117],
                                    //            dNlScanIntensityHash[118],
                                    //            dNlScanIntensityHash[119], dNlScanIntensityHash[121]
                                    //        }
                                    //    });

                                    //    try
                                    //    {
                                    //        double DeterminantR1 = Delta_R1_CAD.Determinant();
                                    //        double DeterminantR2 = Delta_R2_CAD.Determinant();
                                    //        double DeterminantR3 = Delta_R3_CAD.Determinant();
                                    //        double DeterminantR4 = Delta_R4_CAD.Determinant();
                                    //        double DeterminantR5 = Delta_R5_CAD.Determinant();
                                    //        double DeterminantR6 = Delta_R6_CAD.Determinant();
                                    //        double DeterminantR7 = Delta_R7_CAD.Determinant();
                                    //        double DeterminantR8 = Delta_R8_CAD.Determinant();

                                    //        pcfScanIntensityHash[113] = (DeterminantR1/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[114] = (DeterminantR2/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[115] = (DeterminantR3/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[116] = (DeterminantR4/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[117] = (DeterminantR5/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[118] = (DeterminantR6/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[119] = (DeterminantR7/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[121] = (DeterminantR8/CADCoefficientMatrixDeterminant);
                                    //    }
                                    //    catch (DivideByZeroException)
                                    //    {
                                    //        pcfScanIntensityHash[113] = dNlScanIntensityHash[113];
                                    //        pcfScanIntensityHash[114] = dNlScanIntensityHash[114];
                                    //        pcfScanIntensityHash[115] = dNlScanIntensityHash[115];
                                    //        pcfScanIntensityHash[116] = dNlScanIntensityHash[116];
                                    //        pcfScanIntensityHash[117] = dNlScanIntensityHash[117];
                                    //        pcfScanIntensityHash[118] = dNlScanIntensityHash[118];
                                    //        pcfScanIntensityHash[119] = dNlScanIntensityHash[119];
                                    //        pcfScanIntensityHash[121] = dNlScanIntensityHash[121];
                                    //    }

                                    //    TotalScanIntCounts += pcfScanIntensityHash[113] + pcfScanIntensityHash[114] +
                                    //                          pcfScanIntensityHash[115] + pcfScanIntensityHash[116] +
                                    //                          pcfScanIntensityHash[117] + pcfScanIntensityHash[118] +
                                    //                          pcfScanIntensityHash[119] + pcfScanIntensityHash[121];
                                    //}
                                    //if (radioBox_TMT6.Checked)
                                    //{
                                    //    Matrix Delta_R1_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {
                                    //            dNlScanIntensityHash[126], dNlScanIntensityHash[127],
                                    //            dNlScanIntensityHash[128],
                                    //            dNlScanIntensityHash[129], dNlScanIntensityHash[130],
                                    //            dNlScanIntensityHash[131]
                                    //        },
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                    //    });

                                    //    Matrix Delta_R2_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                    //        {
                                    //            dNlScanIntensityHash[126], dNlScanIntensityHash[127],
                                    //            dNlScanIntensityHash[128],
                                    //            dNlScanIntensityHash[129], dNlScanIntensityHash[130],
                                    //            dNlScanIntensityHash[131]
                                    //        },
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                    //    });

                                    //    Matrix Delta_R3_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                    //        {
                                    //            dNlScanIntensityHash[126], dNlScanIntensityHash[127],
                                    //            dNlScanIntensityHash[128],
                                    //            dNlScanIntensityHash[129], dNlScanIntensityHash[130],
                                    //            dNlScanIntensityHash[131]
                                    //        },
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                    //    });

                                    //    Matrix Delta_R4_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                    //        {
                                    //            dNlScanIntensityHash[126], dNlScanIntensityHash[127],
                                    //            dNlScanIntensityHash[128],
                                    //            dNlScanIntensityHash[129], dNlScanIntensityHash[130],
                                    //            dNlScanIntensityHash[131]
                                    //        },
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                    //    });

                                    //    Matrix Delta_R5_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                    //        {
                                    //            dNlScanIntensityHash[126], dNlScanIntensityHash[127],
                                    //            dNlScanIntensityHash[128],
                                    //            dNlScanIntensityHash[129], dNlScanIntensityHash[130],
                                    //            dNlScanIntensityHash[131]
                                    //        },
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                    //    });

                                    //    Matrix Delta_R6_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                    //        {
                                    //            dNlScanIntensityHash[126], dNlScanIntensityHash[127],
                                    //            dNlScanIntensityHash[128],
                                    //            dNlScanIntensityHash[129], dNlScanIntensityHash[130],
                                    //            dNlScanIntensityHash[131]
                                    //        }
                                    //    });

                                    //    try
                                    //    {
                                    //        double DeterminantR1 = Delta_R1_CAD.Determinant();
                                    //        double DeterminantR2 = Delta_R2_CAD.Determinant();
                                    //        double DeterminantR3 = Delta_R3_CAD.Determinant();
                                    //        double DeterminantR4 = Delta_R4_CAD.Determinant();
                                    //        double DeterminantR5 = Delta_R5_CAD.Determinant();
                                    //        double DeterminantR6 = Delta_R6_CAD.Determinant();

                                    //        pcfScanIntensityHash[126] = (DeterminantR1/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[127] = (DeterminantR2/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[128] = (DeterminantR3/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[129] = (DeterminantR4/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[130] = (DeterminantR5/CADCoefficientMatrixDeterminant);
                                    //        pcfScanIntensityHash[131] = (DeterminantR6/CADCoefficientMatrixDeterminant);

                                    //    }
                                    //    catch (DivideByZeroException)
                                    //    {
                                    //        ExceptionCount++;
                                    //        pcfScanIntensityHash[126] = dNlScanIntensityHash[126];
                                    //        pcfScanIntensityHash[127] = dNlScanIntensityHash[127];
                                    //        pcfScanIntensityHash[128] = dNlScanIntensityHash[128];
                                    //        pcfScanIntensityHash[129] = dNlScanIntensityHash[129];
                                    //        pcfScanIntensityHash[130] = dNlScanIntensityHash[130];
                                    //        pcfScanIntensityHash[131] = dNlScanIntensityHash[131];
                                    //    }

                                    //    TotalScanIntCounts += pcfScanIntensityHash[126] + pcfScanIntensityHash[127] +
                                    //                          pcfScanIntensityHash[128] + pcfScanIntensityHash[129] +
                                    //                          pcfScanIntensityHash[130] + pcfScanIntensityHash[131];
                                    //}
                                    //if (radioBox_TMT8.Checked)
                                    //{
                                    //    Matrix Delta_R1_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {
                                    //            dNlScanIntensityHash[126], dNlScanIntensityHash[127],
                                    //            dNlScanIntensityHash[128],
                                    //            dNlScanIntensityHash[129], dNlScanIntensityHash[130],
                                    //            dNlScanIntensityHash[131]
                                    //        },
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                    //    });

                                    //    Matrix Delta_R2_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                    //        {
                                    //            dNlScanIntensityHash[126], dNlScanIntensityHash[127],
                                    //            dNlScanIntensityHash[128],
                                    //            dNlScanIntensityHash[129], dNlScanIntensityHash[130],
                                    //            dNlScanIntensityHash[131]
                                    //        },
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                    //    });

                                    //    Matrix Delta_R3_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                    //        {
                                    //            dNlScanIntensityHash[126], dNlScanIntensityHash[127],
                                    //            dNlScanIntensityHash[128],
                                    //            dNlScanIntensityHash[129], dNlScanIntensityHash[130],
                                    //            dNlScanIntensityHash[131]
                                    //        },
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                    //    });

                                    //    Matrix Delta_R4_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                    //        {
                                    //            dNlScanIntensityHash[126], dNlScanIntensityHash[127],
                                    //            dNlScanIntensityHash[128],
                                    //            dNlScanIntensityHash[129], dNlScanIntensityHash[130],
                                    //            dNlScanIntensityHash[131]
                                    //        },
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                    //    });

                                    //    Matrix Delta_R5_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                    //        {
                                    //            dNlScanIntensityHash[126], dNlScanIntensityHash[127],
                                    //            dNlScanIntensityHash[128],
                                    //            dNlScanIntensityHash[129], dNlScanIntensityHash[130],
                                    //            dNlScanIntensityHash[131]
                                    //        },
                                    //        {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                    //    });

                                    //    Matrix Delta_R6_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                    //        {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                    //        {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                    //        {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                    //        {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                    //        {
                                    //            dNlScanIntensityHash[126], dNlScanIntensityHash[127],
                                    //            dNlScanIntensityHash[128],
                                    //            dNlScanIntensityHash[129], dNlScanIntensityHash[130],
                                    //            dNlScanIntensityHash[131]
                                    //        }
                                    //    });

                                    //    Matrix Delta_R7_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {dNlScanIntensityHash[132], dNlScanIntensityHash[133]},
                                    //        {pcf_R8_m2, AcP_R8}
                                    //    });

                                    //    Matrix Delta_R8_CAD = DenseMatrix.OfArray(new double[,]
                                    //    {
                                    //        {AcP_R7, pcf_R7_p2},
                                    //        {dNlScanIntensityHash[132], dNlScanIntensityHash[133]}
                                    //    });

                                 
                                    //    TotalScanIntCounts +=  pcfScanIntensityHash[126] = (Delta_R1_CAD.Determinant()/CADCoefficientMatrixDeterminant);
                                    //    TotalScanIntCounts +=    pcfScanIntensityHash[127] = (Delta_R2_CAD.Determinant()/CADCoefficientMatrixDeterminant);
                                    //    TotalScanIntCounts +=    pcfScanIntensityHash[128] = (Delta_R3_CAD.Determinant()/CADCoefficientMatrixDeterminant);
                                    //    TotalScanIntCounts +=    pcfScanIntensityHash[129] = (Delta_R4_CAD.Determinant()/CADCoefficientMatrixDeterminant);
                                    //    TotalScanIntCounts +=    pcfScanIntensityHash[130] = (Delta_R5_CAD.Determinant()/CADCoefficientMatrixDeterminant);
                                    //    TotalScanIntCounts +=    pcfScanIntensityHash[131] = (Delta_R6_CAD.Determinant()/CADCoefficientMatrixDeterminant);
                                    //    TotalScanIntCounts +=    pcfScanIntensityHash[132] = (Delta_R7_CAD.Determinant()/TMT10_CADCoefficientMatrixDeterminant);
                                    //    TotalScanIntCounts +=    pcfScanIntensityHash[133] = (Delta_R8_CAD.Determinant()/TMT10_CADCoefficientMatrixDeterminant);

                                        
                                        
                                    //    //ExceptionCount++;
                                    //    //pcfScanIntensityHash[126] = dNlScanIntensityHash[126];
                                    //    //pcfScanIntensityHash[127] = dNlScanIntensityHash[127];
                                    //    //pcfScanIntensityHash[128] = dNlScanIntensityHash[128];
                                    //    //pcfScanIntensityHash[129] = dNlScanIntensityHash[129];
                                    //    //pcfScanIntensityHash[130] = dNlScanIntensityHash[130];
                                    //    //pcfScanIntensityHash[131] = dNlScanIntensityHash[131];
                                    //    //pcfScanIntensityHash[132] = dNlScanIntensityHash[132];
                                    //    //pcfScanIntensityHash[133] = dNlScanIntensityHash[133];
                                    //}
                                    if (radioBox_TMT10.Checked)
                                    {
                                        Matrix Delta_R1_CAD = DenseMatrix.OfArray(new double[,]
                                        {
                                            {
                                                dNlScanIntensityHash["126"], dNlScanIntensityHash["127C"],
                                                dNlScanIntensityHash["128C"], dNlScanIntensityHash["129C"],
                                                dNlScanIntensityHash["130C"], dNlScanIntensityHash["131"]
                                            },
                                            {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                            {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                            {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                            {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                            {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                        });

                                        Matrix Delta_R2_CAD = DenseMatrix.OfArray(new double[,]
                                        {
                                            {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                            {
                                                dNlScanIntensityHash["126"], dNlScanIntensityHash["127C"],
                                                dNlScanIntensityHash["128C"], dNlScanIntensityHash["129C"],
                                                dNlScanIntensityHash["130C"], dNlScanIntensityHash["131"]
                                            },
                                            {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                            {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                            {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                            {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                        });

                                        Matrix Delta_R3_CAD = DenseMatrix.OfArray(new double[,]
                                        {
                                            {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                            {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                            {
                                                dNlScanIntensityHash["126"], dNlScanIntensityHash["127C"],
                                                dNlScanIntensityHash["128C"], dNlScanIntensityHash["129C"],
                                                dNlScanIntensityHash["130C"], dNlScanIntensityHash["131"]
                                            },
                                            {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                            {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                            {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                        });

                                        Matrix Delta_R4_CAD = DenseMatrix.OfArray(new double[,]
                                        {
                                            {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                            {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                            {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                            {
                                                dNlScanIntensityHash["126"], dNlScanIntensityHash["127C"],
                                                dNlScanIntensityHash["128C"], dNlScanIntensityHash["129C"],
                                                dNlScanIntensityHash["130C"], dNlScanIntensityHash["131"]
                                            },
                                            {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                            {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                        });

                                        Matrix Delta_R5_CAD = DenseMatrix.OfArray(new double[,]
                                        {
                                            {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                            {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                            {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                            {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                            {
                                                dNlScanIntensityHash["126"], dNlScanIntensityHash["127C"],
                                                dNlScanIntensityHash["128C"], dNlScanIntensityHash["129C"],
                                                dNlScanIntensityHash["130C"], dNlScanIntensityHash["131"]
                                            },
                                            {0, 0, 0, pcf_R6_m2, pcf_R6_m1, AcP_R6}
                                        });

                                        Matrix Delta_R6_CAD = DenseMatrix.OfArray(new double[,]
                                        {
                                            {AcP_R1, pcf_R1_p1, pcf_R1_p2, 0, 0, 0},
                                            {pcf_R2_m1, AcP_R2, pcf_R2_p1, pcf_R2_p2, 0, 0},
                                            {pcf_R3_m2, pcf_R3_m1, AcP_R3, pcf_R3_p1, pcf_R3_p2, 0},
                                            {0, pcf_R4_m2, pcf_R4_m1, AcP_R4, pcf_R4_p1, pcf_R4_p2},
                                            {0, 0, pcf_R5_m2, pcf_R5_m1, AcP_R5, pcf_R5_p1},
                                            {
                                                dNlScanIntensityHash["126"], dNlScanIntensityHash["127C"],
                                                dNlScanIntensityHash["128C"], dNlScanIntensityHash["129C"],
                                                dNlScanIntensityHash["130C"], dNlScanIntensityHash["131"]
                                            }
                                        });

                                        Matrix Delta_R7_CAD = DenseMatrix.OfArray(new double[,]
                                        {
                                            {
                                                dNlScanIntensityHash["127N"], dNlScanIntensityHash["128N"],
                                                dNlScanIntensityHash["129N"], dNlScanIntensityHash["130N"]
                                            },
                                            {pcf_R7_p1, AcP_R8, pcf_R9_m1, pcf_R10_m2},
                                            {pcf_R7_p2, pcf_R8_p1, AcP_R9, pcf_R10_m1},
                                            {0, pcf_R8_p2, pcf_R9_p1, AcP_R10},
                                        });

                                        Matrix Delta_R8_CAD = DenseMatrix.OfArray(new double[,]
                                        {
                                            {AcP_R7, pcf_R8_m1, pcf_R9_m2, 0},
                                            {
                                                dNlScanIntensityHash["127N"], dNlScanIntensityHash["128N"],
                                                dNlScanIntensityHash["129N"], dNlScanIntensityHash["130N"]
                                            },
                                            {pcf_R7_p2, pcf_R8_p1, AcP_R9, pcf_R10_m1},
                                            {0, pcf_R8_p2, pcf_R9_p1, AcP_R10},
                                        });

                                        Matrix Delta_R9_CAD = DenseMatrix.OfArray(new double[,]
                                        {
                                            {AcP_R7, pcf_R8_m1, pcf_R9_m2, 0},
                                            {pcf_R7_p1, AcP_R8, pcf_R9_m1, pcf_R10_m2},
                                            {
                                                dNlScanIntensityHash["127N"], dNlScanIntensityHash["128N"],
                                                dNlScanIntensityHash["129N"], dNlScanIntensityHash["130N"]
                                            },
                                            {0, pcf_R8_p2, pcf_R9_p1, AcP_R10},
                                        });

                                        Matrix Delta_R10_CAD = DenseMatrix.OfArray(new double[,]
                                        {
                                            {AcP_R7, pcf_R8_m1, pcf_R9_m2, 0},
                                            {pcf_R7_p1, AcP_R8, pcf_R9_m1, pcf_R10_m2},
                                            {pcf_R7_p2, pcf_R8_p1, AcP_R9, pcf_R10_m1},
                                            {
                                                dNlScanIntensityHash["127N"], dNlScanIntensityHash["128N"],
                                                dNlScanIntensityHash["129N"], dNlScanIntensityHash["130N"]
                                            },
                                        });

                                        TotalScanIntCounts += pcfScanIntensityHash["126"] = (Delta_R1_CAD.Determinant() / CADCoefficientMatrixDeterminant);
                                        TotalScanIntCounts += pcfScanIntensityHash["127N"] = (Delta_R2_CAD.Determinant() / CADCoefficientMatrixDeterminant);
                                        TotalScanIntCounts += pcfScanIntensityHash["127C"] = (Delta_R7_CAD.Determinant() / TMT10_CADCoefficientMatrixDeterminant);
                                        TotalScanIntCounts += pcfScanIntensityHash["128N"] = (Delta_R3_CAD.Determinant() / CADCoefficientMatrixDeterminant);
                                        TotalScanIntCounts += pcfScanIntensityHash["128C"] = (Delta_R8_CAD.Determinant() / TMT10_CADCoefficientMatrixDeterminant);
                                        TotalScanIntCounts += pcfScanIntensityHash["129N"] = (Delta_R4_CAD.Determinant() / CADCoefficientMatrixDeterminant);
                                        TotalScanIntCounts += pcfScanIntensityHash["129C"] = (Delta_R9_CAD.Determinant() / TMT10_CADCoefficientMatrixDeterminant);
                                        TotalScanIntCounts += pcfScanIntensityHash["130N"] = (Delta_R5_CAD.Determinant() / CADCoefficientMatrixDeterminant);
                                        TotalScanIntCounts += pcfScanIntensityHash["130C"] = (Delta_R10_CAD.Determinant() / TMT10_CADCoefficientMatrixDeterminant);
                                        TotalScanIntCounts += pcfScanIntensityHash["131"] = (Delta_R6_CAD.Determinant() / CADCoefficientMatrixDeterminant);
                                    

                                    }
                                    break;
                                    /*
                                case FragmentationMethod.ETD:
                                    if (radioBox_iTRAQ4.Checked)
                                    {
                                        pcfScanIntensityHash[114] = dNlScanIntensityHash[114];
                                        pcfScanIntensityHash[115] = dNlScanIntensityHash[115];
                                        pcfScanIntensityHash[116] = dNlScanIntensityHash[116];
                                        pcfScanIntensityHash[117] = dNlScanIntensityHash[117];
                                    }
                                    if (radioBox_iTRAQ8.Checked)
                                    {
                                        pcfScanIntensityHash[114] = dNlScanIntensityHash[113];
                                        pcfScanIntensityHash[114] = dNlScanIntensityHash[114];
                                        pcfScanIntensityHash[115] = dNlScanIntensityHash[115];
                                        pcfScanIntensityHash[116] = dNlScanIntensityHash[116];
                                        pcfScanIntensityHash[117] = dNlScanIntensityHash[117];
                                        pcfScanIntensityHash[118] = dNlScanIntensityHash[118];
                                        pcfScanIntensityHash[119] = dNlScanIntensityHash[119];
                                        pcfScanIntensityHash[121] = dNlScanIntensityHash[121];
                                    }
                                    if (radioBox_TMT6.Checked)
                                    {
                                        pcfScanIntensityHash[126] = dNlScanIntensityHash[126];
                                        pcfScanIntensityHash[127] = dNlScanIntensityHash[127];
                                        pcfScanIntensityHash[128] = dNlScanIntensityHash[128];
                                        pcfScanIntensityHash[129] = dNlScanIntensityHash[129];
                                        pcfScanIntensityHash[130] = dNlScanIntensityHash[130];
                                        pcfScanIntensityHash[131] = dNlScanIntensityHash[131];
                                    }
                                    break;
                                     */
                            }

                            peptidecount++;

                            sb.Clear();

                            for (int i = 0; i < headerCount; i++)
                            {
                                sb.Append(reader[i]);
                                sb.Append(',');
                            }
                            sb.Append(Purity);

                            // Write out NL values
                            foreach (TagInformation tag in SamplesAndTags.Values)
                            {
                                sb.Append(',');
                                if (DontQuantifyETD && ScanFragMethod == FragmentationMethod.ETD)
                                {
                                    sb.Append("NA");
                                }
                                else
                                {
                                    sb.Append(nlScanIntensityHash[tag.TagName]);
                                }
                            }

                            // Write out dNL
                            foreach (TagInformation tag in SamplesAndTags.Values)
                            {
                                sb.Append(',');
                                if (DontQuantifyETD && ScanFragMethod == FragmentationMethod.ETD)
                                {
                                    sb.Append("NA");
                                }
                                else
                                {
                                    sb.Append(dNlScanIntensityHash[tag.TagName]);
                                }
                            }

                            //sb.Append(",");
                            //// Write in Total Tag Intensity
                            //if (DontQuantifyETD && ScanFragMethod == FragmentationMethod.ETD)
                            //{
                            //    sb.Append("NA");
                            //}
                            //else
                            //{
                            //    sb.Append(TotalScanIntCounts);
                            //}

                            //Add the purity corrected signal from each channel if it is in use to the total counts for that channel
                            MassRange noiseRange = new MassRange(100, 250);
                            List<MZPeak> noisePeaks = null;
                            int labeledNoisePeakCount;

                            foreach (TagInformation tag in SamplesAndTags.Values)
                            {
                                noisePeaks = null;
                                labeledNoisePeakCount = 0;
                                sb.Append(',');
                                if (DontQuantifyETD && ScanFragMethod == FragmentationMethod.ETD)
                                {
                                    sb.Append("NA");
                                }
                                else
                                {
                                    if (noisebandCap)
                                    {
                                        if (nlScanIntensityHash[tag.TagName] == 0)
                                        {
                                            pcfScanIntensityHash[tag.TagName] = 0;
                                            if (seqScan.MassSpectrum.TryGetPeaks(noiseRange, out noisePeaks))
                                            {
                                                foreach (MZPeak peak in noisePeaks)
                                                {
                                                    if (peak is ThermoLabeledPeak)
                                                    {
                                                        pcfScanIntensityHash[tag.TagName] +=
                                                            ((ThermoLabeledPeak) peak).Noise;
                                                        labeledNoisePeakCount++;
                                                    }
                                                }
                                                pcfScanIntensityHash[tag.TagName] = (pcfScanIntensityHash[tag.TagName] /
                                                                                   labeledNoisePeakCount)*injectionTime;
                                            }
                                        }
                                    }

                                    sb.Append(pcfScanIntensityHash[tag.TagName]);
                                }

                               
                                // Add to total channel signal for that fragmentation method
                                TotalTagSignal[tag.TagName][ScanFragMethod] += pcfScanIntensityHash[tag.TagName];

                                // Add to the total signal for that fragmentation method
                                if (ScanFragMethod == FragmentationMethod.ETD)
                                {
                                    TotalETDSignal += pcfScanIntensityHash[tag.TagName];
                                }
                                if (ScanFragMethod == FragmentationMethod.CAD)
                                {
                                    TotalCADSignal += pcfScanIntensityHash[tag.TagName];
                                }
                                
                            }

                            QuantTempFile.WriteLine(sb.ToString());
                            progressBar1.Value = (int) (((double) baseSteam.BaseStream.Position/baseSteam.BaseStream.Length)*100);

                        }
                    }
                }
            }

            double ExpectedRatio = 1 / TagCount;
            Application.DoEvents();
            Dictionary<string, Dictionary<FragmentationMethod, double>> NormalizationHash = new Dictionary<string, Dictionary<FragmentationMethod, double>>();
            double maxValue = 0;
            toolStripStatusLabel1.Text = "Calculating Normalization Values";

            foreach (TagInformation tag in SamplesAndTags.Values)
            {
                NormalizationHash.Add(tag.TagName, new Dictionary<FragmentationMethod, double>());
                NormalizationHash[tag.TagName].Add(FragmentationMethod.CAD, ExpectedRatio / (TotalTagSignal[tag.TagName][FragmentationMethod.CAD] / TotalCADSignal));
                NormalizationHash[tag.TagName].Add(FragmentationMethod.ETD, 0);
                //NormalizationHash[TagUsed][FragmentationMethod.CAD] = ExpectedRatio / (TotalTagSignal[TagUsed][FragmentationMethod.CAD] / TotalCADSignal);
                
                // Keep track of max channel signal
                if (NormalizationHash[tag.TagName][FragmentationMethod.CAD] > maxValue)
                {
                    maxValue = NormalizationHash[tag.TagName][FragmentationMethod.CAD];
                }
            }

            log.WriteLine();
            log.WriteLine("Tag m/z \tTotal Signal \tNoramlization Value \tNormalized to Max");

            foreach (TagInformation tag in SamplesAndTags.Values)
            {                 
                double percentValue =  (NormalizationHash[tag.TagName][FragmentationMethod.CAD]) / maxValue;

                StringBuilder sb = new StringBuilder();
                sb.Append(tag.TagName);
                sb.Append("\t ");
                sb.Append(TotalTagSignal[tag.TagName][FragmentationMethod.CAD].ToString("g4"));
                sb.Append("\t ");
                sb.Append(NormalizationHash[tag.TagName][FragmentationMethod.CAD].ToString("f4"));
                sb.Append("\t " );
                sb.Append(percentValue.ToString("f4"));
                log.WriteLine(sb.ToString());          

                if(TotalETDSignal > 0)
                {
                    NormalizationHash[tag.TagName][FragmentationMethod.ETD] = ExpectedRatio / (TotalTagSignal[tag.TagName][FragmentationMethod.ETD] / TotalETDSignal);

                    log.WriteLine(tag.TagName + " Total Signal ETD = " + TotalTagSignal[tag.TagName][FragmentationMethod.ETD] + ", Normalization value (ETD) = " + NormalizationHash[tag.TagName][FragmentationMethod.ETD]);
                }
                else
                {
                    //log.WriteLine(TagUsed + " Total Signal ETD = " + TotalTagSignal[TagUsed][FragmentationMethod.ETD] + "," + " Normalization value (ETD) = NaN");
                }
            }
            log.WriteLine();

            log.Close();


            int tagnumber = 0;
            if (radioBox_iTRAQ4.Checked)
            {
                tagnumber = 4;
            }
            if (radioBox_iTRAQ8.Checked)
            {
                tagnumber = 8;
            }
            if (radioBox_TMT6.Checked)
            {
                tagnumber = 6;
            }
            if (radioBox_TMT8.Checked)
            {
                tagnumber = 8;
            }
            if (radioBox_TMT10.Checked)
            {
                tagnumber = 10;
            }

            ////Re open files and perform normalization
            foreach(string filename in listBox1.Items)
            {
                toolStripStatusLabel1.Text = "Applying Normalization Values";
                string quantfile = Path.Combine(textOutputFolder.Text, Path.GetFileNameWithoutExtension(filename) + "_quant_temp.csv");
                StreamReader sr = new StreamReader(quantfile);
                using (CsvReader reader = new CsvReader(sr, true))
                {

                    string outputname = Path.Combine(textOutputFolder.Text, Path.GetFileNameWithoutExtension(filename) + "_quant.csv");
                    using (StreamWriter sw = new StreamWriter(outputname))
                    {
                        string[] headerFields = reader.GetFieldHeaders();
                        int headerCount = headerFields.Length;
                        int firstQuantColumn = headerCount - tagnumber;
                        string header = string.Join(",", headerFields);
                        sw.Write(header);

                        foreach (TagInformation tag in SamplesAndTags.Values)
                        {
                            sw.Write(",{0} ({1} PCN)", tag.SampleName, tag.TagName);
                        }

                        //sw.Write(",Cleavage Site");
                        sw.WriteLine();


                        while (reader.ReadNextRecord()) // go through csv and raw file to extract the info we want
                        {
                            StringBuilder sb = new StringBuilder();
                            StringBuilder sb2 = new StringBuilder();

                            string fileNameID = reader["Filename/id"];
                            FragmentationMethod ScanFragMethod = FragmentationMethod.CAD;
                            if (fileNameID.Contains(".ETD."))
                            {
                                ScanFragMethod = FragmentationMethod.ETD;
                                if ((ComboBoxETDoptions.Text == "Use Scan Before") ||
                                    (ComboBoxETDoptions.Text == "Use Scan After"))
                                {
                                    ScanFragMethod = FragmentationMethod.CAD;
                                }
                            }
                            int j = 0;
                            for (int i = 0; i < headerCount; i++)
                            {
                                string value = reader[i];
                                sb.Append(value);
                                sb.Append(',');
                                if (i < firstQuantColumn) 
                                    continue;
                                
                                if (value == "NA")
                                {
                                    sb2.Append("NA");
                                }
                                else
                                {
                                    double UnnormalizedValue = double.Parse(value);
                                    double NormalizedValue = UnnormalizedValue;

                                    TagInformation tag = SamplesAndTags.Values[j++];
                                    NormalizedValue = UnnormalizedValue * NormalizationHash[tag.TagName][ScanFragMethod];
                                    
                                    sb2.Append(NormalizedValue);
                                }
                                sb2.Append(',');
                            }
                            sb2.Remove(sb2.Length - 1, 1);
                            //sb2.Append(',');
                            //sb2.Append(CleavagePattern);
                            sw.WriteLine(sb.ToString() + sb2.ToString());
                        }
                    }
                }
                File.Delete(quantfile);
            }
            panel1.Enabled = true;
            toolStripStatusLabel1.Text = "done";
        }  
        
        private static int sortByAscendingIntensity(KeyValuePair<double, double> left, KeyValuePair<double, double> right)
        {
            return left.Key.CompareTo(right.Key);
        }

        private void TagQuantForm_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void TagQuantForm_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) 
                return;

            string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);
            foreach (string filename in files)
            {
                listBox1.Items.Add(filename);
                UpdateRawFileFolder(filename);
                UpdateOutputFolder(filename);
            }
        }

        private void textBox113_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

    }
}