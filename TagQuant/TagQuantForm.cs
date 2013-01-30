using CSML;
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using CoonThermo.IO;
using Coon;
using Coon.Spectra;

namespace TagQuant
{
    public partial class TagQuantForm : Form
    {
        public TagQuantForm()
        {
            InitializeComponent();        
        }

        private void TagQuantForm_Load(object sender, EventArgs e)
        {
            ComboBoxETDoptions.SelectedIndex = 3;
        }

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

        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] filepaths = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach(string filepath in filepaths)
            {
                if(Path.GetExtension(filepath).Equals(".csv", StringComparison.InvariantCultureIgnoreCase) &&
                    !listBox1.Items.Contains(filepath))
                {
                    listBox1.Items.Add(filepath);
                    UpdateOutputFolder(filepath);
                }
            }
        }

        private void Add_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach(string filename in openFileDialog1.FileNames)
                {
                    listBox1.Items.Add(filename);
                    UpdateOutputFolder(filename);
                }
            }
        }

        private void UpdateOutputFolder(string filepath)
        {
            if(textOutputFolder.Text == string.Empty)
            {
                textOutputFolder.Text = Path.GetDirectoryName(filepath);
            }
        }

        private void Remove_Click(object sender, EventArgs e)
        {
            while(listBox1.SelectedItems.Count > 0)
            {
                listBox1.Items.Remove(listBox1.SelectedItem);
            }
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void BrowseRaw_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textRawFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

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

        private void BrowseOutput_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog2.ShowDialog() == DialogResult.OK)
            {
                textOutputFolder.Text = folderBrowserDialog2.SelectedPath;
            }
        }

        private const double C12_C13_MASS_DIFFERENCE = 1.0033548378;

        private void Quantify_Click(object sender, EventArgs e)
        {
            panel1.Enabled = false;
            toolStripStatusLabel1.Text = "Running";
            Application.DoEvents();
            int FilesCount = listBox1.Items.Count;
            int FileCounter = 1;

            bool noisebandCap = noisebandcapCB.Checked;

            int ExceptionCount = 0;
            double IsoWindow = 0;
            double PreErrorAllowed = 0;
            IsoWindow = double.Parse(windowTextBox.Text);
            PreErrorAllowed = double.Parse(ppmTextBox.Text);

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

            double TotalETDSignal = 0;
            double TotalCADSignal = 0;
            string CleavagePattern = "\"" + textBoxCleavage.Text + "\"";

            double pcf_R1_m2 = 0;
            double pcf_R2_m2 = 0;
            double pcf_R3_m2 = 0;
            double pcf_R4_m2 = 0;
            double pcf_R5_m2 = 0;
            double pcf_R6_m2 = 0;
            double pcf_R7_m2 = 0;
            double pcf_R8_m2 = 0;

            double pcf_R1_m1 = 0;
            double pcf_R2_m1 = 0;
            double pcf_R3_m1 = 0;
            double pcf_R4_m1 = 0;
            double pcf_R5_m1 = 0;
            double pcf_R6_m1 = 0;
            double pcf_R7_m1 = 0;
            double pcf_R8_m1 = 0;

            double pcf_R1_p1 = 0;
            double pcf_R2_p1 = 0;
            double pcf_R3_p1 = 0;
            double pcf_R4_p1 = 0;
            double pcf_R5_p1 = 0;
            double pcf_R6_p1 = 0;
            double pcf_R7_p1 = 0;
            double pcf_R8_p1 = 0;

            double pcf_R1_p2 = 0;
            double pcf_R2_p2 = 0;
            double pcf_R3_p2 = 0;
            double pcf_R4_p2 = 0;
            double pcf_R5_p2 = 0;
            double pcf_R6_p2 = 0;
            double pcf_R7_p2 = 0;
            double pcf_R8_p2 = 0;

            double AcP_R1 = 0;
            double AcP_R2 = 0;
            double AcP_R3 = 0;
            double AcP_R4 = 0;
            double AcP_R5 = 0;
            double AcP_R6 = 0;
            double AcP_R7 = 0;
            double AcP_R8 = 0;

            double CADCoefficientMatrixDeterminant = 0.0;
            //double ETDCoefficientMatrixDeterminant = 0.0;

            if(radioBox_iTRAQ4.Checked)
            {
                //Purity Correction Factors begin
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

                AcP_R1 = (1 - (pcf_R1_m2 + pcf_R1_m1 + pcf_R1_p1 + pcf_R1_p2));
                AcP_R2 = (1 - (pcf_R2_m2 + pcf_R2_m1 + pcf_R2_p1 + pcf_R2_p2));
                AcP_R3 = (1 - (pcf_R3_m2 + pcf_R3_m1 + pcf_R3_p1 + pcf_R3_p2));
                AcP_R4 = (1 - (pcf_R4_m2 + pcf_R4_m1 + pcf_R4_p1 + pcf_R4_p2));

                Matrix CADCoefficientMatrix = new Matrix(new double[,]{ 
            { AcP_R1,       pcf_R2_m1,  pcf_R3_m2,  0           }, 
            { pcf_R1_p1,    AcP_R2,     pcf_R3_m1,  pcf_R4_m2   }, 
            { pcf_R1_p2,    pcf_R2_p1,  AcP_R3,     pcf_R4_m1   }, 
            { 0    ,        pcf_R2_p2,  pcf_R3_p1,  AcP_R4      }, 
            });

                // Determine if coefficient is real
                CADCoefficientMatrixDeterminant = CADCoefficientMatrix.Determinant().Re;
                if(CADCoefficientMatrix.Determinant().IsReal())
                {
                    CADCoefficientMatrixDeterminant = CADCoefficientMatrix.Determinant().Re;
                }
                else
                {
                    throw new Exception("Unreal determinant!");
                }
            }  // end iTRAQ purity correction factors

            if(radioBox_iTRAQ8.Checked)
            {
                //Purity Correction Factors begin
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

                Matrix CADCoefficientMatrix = new Matrix(new double[,]
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
                CADCoefficientMatrixDeterminant = CADCoefficientMatrix.Determinant().Re;
                if(CADCoefficientMatrix.Determinant().IsReal())
                {
                    CADCoefficientMatrixDeterminant = CADCoefficientMatrix.Determinant().Re;
                }
                else
                {
                    throw new Exception("Unreal determinant!");
                }
            }  // end iTRAQ purity correction factors

            if(radioBox_TMT6.Checked)
            {
                //Purity Correction Factors begin
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

                Matrix CADCoefficientMatrix = new Matrix(new double[,]
                { 
                { AcP_R1,       pcf_R2_m1,      pcf_R3_m2,   0  ,                0 ,                 0          },
                { pcf_R1_p1,    AcP_R2,         pcf_R3_m1,   pcf_R4_m2,          0 ,                 0          },
                { pcf_R1_p2,    pcf_R2_p1,      AcP_R3,      pcf_R4_m1,          pcf_R5_m2,          0          },
                { 0    ,        pcf_R2_p2,      pcf_R3_p1,   AcP_R4,             pcf_R5_m1,          pcf_R6_m2  }, 
                { 0    ,        0 ,             pcf_R3_p2,   pcf_R4_p1,          AcP_R5,             pcf_R6_m1  }, 
                { 0    ,        0,              0,           pcf_R4_p2,          pcf_R5_p1,          AcP_R6     }, 
                });

                // Determine if coefficient is real
                CADCoefficientMatrixDeterminant = CADCoefficientMatrix.Determinant().Re;
                if(CADCoefficientMatrix.Determinant().IsReal())
                {
                    CADCoefficientMatrixDeterminant = CADCoefficientMatrix.Determinant().Re;
                }
                else
                {
                    throw new Exception("Unreal determinant!");
                }
            }  // end iTRAQ purity correction factors

            //Purity Correction Factors end

            Dictionary<int, Dictionary<FragmentationMethod, double>> TotalTagSignal = new Dictionary<int, Dictionary<FragmentationMethod, double>>();
            double peptidecount = 0;
            double ITerror = (double)numericUpDown1.Value;
            double FTerror = (double)numericUpDown2.Value;
            double error;
            string LogOutPutName = Path.Combine(textOutputFolder.Text, "TagQuant_log.txt");
            StreamWriter log = new StreamWriter(LogOutPutName);
            log.AutoFlush = true;
            log.WriteLine("TagQuant PARAMETERS");
            log.WriteLine("Labeling Reagent: " + (radioBox_TMT2.Checked ? radioBox_TMT2.Text : (radioBox_iTRAQ4.Checked ? radioBox_iTRAQ4.Text : (radioBox_TMT6.Checked ? radioBox_TMT6.Text : radioBox_iTRAQ8.Text))));
            log.WriteLine();

            log.WriteLine("Tags and Samples Used");

            SortedDictionary<int, TagInformation> Samples = new SortedDictionary<int, TagInformation>();

            string sample = "unused";

            //SortedList<string, int> SamplesAndTags = new SortedList<string, int>();
            Dictionary<string, int> SamplesAndTags = new Dictionary<string, int>();

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

            log.WriteLine();

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
            log.WriteLine();

            log.WriteLine("Input Files for Quantitation");
            

            foreach(string filename in listBox1.Items)
            {
                toolStripStatusLabel1.Text = "Working on File " + Convert.ToString(FileCounter) + " of " + Convert.ToString(FilesCount);
                Application.DoEvents();
                FileCounter++;
                //OPEN INPUT AND OUTPUT FILES
                FileStream file = new FileStream(filename, FileMode.Open);
                StreamReader csvInputFile = new StreamReader(file);
                string outputname = Path.Combine(textOutputFolder.Text, Path.GetFileNameWithoutExtension(filename) + "_quant_temp.csv");
                StreamWriter QuantTempFile = new StreamWriter(outputname);

                log.WriteLine(filename);

                //open raw file

                string header = csvInputFile.ReadLine();      // read header and add appropriate header line to output file
                QuantTempFile.Write(header);
                //QuantTempFile.Write(",Precursor Purity");
                QuantTempFile.Write(",Interference");

                foreach(KeyValuePair<string, int> KVP in SamplesAndTags)
                {
                    Application.DoEvents();
                    QuantTempFile.Write(",TQ_" + KVP.Value + "_" + KVP.Key + "_NL");
                }

                foreach(KeyValuePair<string, int> KVP in SamplesAndTags)
                {
                    Application.DoEvents();
                    QuantTempFile.Write(",TQ_" + KVP.Value + "_" + KVP.Key + "_dNL");
                }

                QuantTempFile.Write(",TQ_Total_int(all_tags)");

                foreach(KeyValuePair<string, int> KVP in SamplesAndTags)
                {
                    Application.DoEvents();
                    QuantTempFile.Write(",TQ_" + KVP.Value + "_" + KVP.Key + "_PC");
                }               

                QuantTempFile.WriteLine();

                ThermoRawFile rawFile = null;

                while(csvInputFile.Peek() != -1)         // go through csv and raw file to extract the info we want
                {
                    string FPline = csvInputFile.ReadLine();
                    string[] column_values = Regex.Split(FPline, @",(?!(?<=(?:^|,)\s*\x22(?:[^\x22]|\x22\x22|\\\x22)*,)(?:[^\x22]|\x22\x22|\\\x22)*\x22\s*(?:,|$))"); // crazy regex to parse CSV with internal double quotes from http://regexlib.com/REDetails.aspx?regexp_id=621
                    int scan_number = int.Parse(column_values[0]);

                    string RawFileName = Path.Combine(textRawFolder.Text, column_values[1].Substring(0, column_values[1].IndexOf(".")) + ".raw");
                    if(textRawFolder.Text != null && textRawFolder.Text != string.Empty && Directory.Exists(textRawFolder.Text))
                    {
                        RawFileName = Path.Combine(textRawFolder.Text, column_values[1].Substring(0, column_values[1].IndexOf(".")) + ".raw");
                    }
                    else
                    {
                        RawFileName = Path.Combine(Directory.GetParent(filename).ToString(), column_values[1].Substring(0, column_values[1].IndexOf(".")) + ".raw");
                    }

                    if (rawFile != null)
                    {
                        string RawFileCheck = rawFile.RawFilepath;
                        if (!RawFileCheck.Equals(RawFileName))
                        {
                            rawFile.Close();
                            if (!File.Exists(RawFileName))
                            {
                                throw new FileNotFoundException("Can't find file " + RawFileName);
                            }

                            rawFile = new ThermoRawFile(RawFileName);
                        }
                    }
                    else
                    {
                        if (!File.Exists(RawFileName))
                        {
                            throw new FileNotFoundException("Can't find file " + RawFileName);
                        }

                        rawFile = new ThermoRawFile(RawFileName);
                    }
                    
                
                    // Set default fragmentation to CAD / HCD
                    FragmentationMethod ScanFragMethod = FragmentationMethod.CAD;

                    string csvFilenameInformation = column_values[1];

                    if (csvFilenameInformation.Contains(".ETD."))
                    {
                        ScanFragMethod = FragmentationMethod.ETD;
                        if(ComboBoxETDoptions.Text == "Use Scan Before")
                        {
                            scan_number = scan_number - 1;
                            ScanFragMethod = FragmentationMethod.CAD;
                        }
                        if(ComboBoxETDoptions.Text == "Use Scan After")
                        {
                            scan_number = scan_number + 1;
                            ScanFragMethod = FragmentationMethod.CAD;
                        }
                    }                                       

                    error = ITerror;
                    if (csvFilenameInformation.Contains(".FTMS"))
                    {
                        error = FTerror;
                    }

                    // perform functions on raw file

                    // 1.) Determine Purity
                    double Purity = 0;
                    double PreInt = 0;
                    double TotalInt = 0;
                      
                    // Get the scan object for the sequence ms2 scan
                    ThermoRawFileScan seqScan = rawFile[scan_number];

                    // Find the closest parent survery scan
                    ThermoRawFileScan surveyScan = seqScan.GetPreviousMsnScan(1);

                    if(surveyScan != null)
                    {   
                        double Charge = Convert.ToDouble(column_values[11]);
                        double IsoPreMZ = Convert.ToDouble(column_values[15]);
                        MZRange range = new MZRange(IsoPreMZ - 0.5 * IsoWindow, IsoPreMZ + 0.5 * IsoWindow);
                        List<IPeak> peaks = null;

                        if (surveyScan.Spectrum.TryGetPeaks(out peaks, range))
                        {
                            foreach (IPeak peak in peaks)
                            {
                                double difference = (peak.MZ - IsoPreMZ) * Charge;
                                double difference_Rounded = Math.Round(difference);
                                double expected_difference = difference_Rounded * C12_C13_MASS_DIFFERENCE;
                                double Difference_ppm = (Math.Abs((expected_difference - difference) / (IsoPreMZ * Charge))) * 1000000;

                                if (Difference_ppm <= PreErrorAllowed)
                                {
                                    PreInt += peak.Intensity;
                                }
                                TotalInt += peak.Intensity;
                            }
                        }
                                               
                        Purity = PreInt / TotalInt;
                    }
                                                 
                    double InjectionTime = Convert.ToDouble(seqScan.ScanTrailer["Ion Injection Time (ms):"]);
                    Dictionary<int, double> NL_ScanIntensityHash = new Dictionary<int, double>();
                    Dictionary<int, double> dNL_ScanIntensityHash = new Dictionary<int, double>();
                    Dictionary<int, double> PCF_ScanIntensityHash = new Dictionary<int, double>();
                    List<TagInformation> noisebandcapDic = new List<TagInformation>();

                    double TotalScanIntCounts = 0;

                    foreach(KeyValuePair<int, TagInformation> KVP in Samples)
                    {
                        Application.DoEvents();
                        if(!dNL_ScanIntensityHash.ContainsKey(KVP.Key))
                        {
                            NL_ScanIntensityHash.Add(KVP.Key, 0);
                            dNL_ScanIntensityHash.Add(KVP.Key, 0);
                            PCF_ScanIntensityHash.Add(KVP.Key, 0);
                        }
                    }

                    foreach (KeyValuePair<int, TagInformation> KVP in Samples)
                    {
                        double tagvalue = KVP.Value.TagMassCAD;
                        
                        if (ScanFragMethod == FragmentationMethod.ETD)
                        {
                            tagvalue = KVP.Value.TagMassETD;
                        }
                        MZRange range = new MZRange(tagvalue - error, tagvalue + error);
                        List<IPeak> peaks = null;
                        double NL_Counts = 0;
                        if (seqScan.Spectrum.TryGetPeaks(out peaks, range))
                        {
                            foreach (IPeak peak in peaks)
                            {
                                NL_Counts += peak.Intensity;
                            }
                        }
                        else
                        {
                            if (noisebandCap)
                            {
                                IPeak peak = seqScan.Spectrum.GetNearestPeak(tagvalue);
                                if (peak is LabeledPeak)
                                {
                                    NL_Counts = ((LabeledPeak)peak).Noise;
                                }
                                else
                                {
                                    NL_Counts = 1;
                                }
                                noisebandcapDic.Add(KVP.Value);
                            }
                            else
                            {
                                NL_Counts = 1;
                            }
                        }

                        NL_ScanIntensityHash[KVP.Key] += NL_Counts;
                      
                        double dNL_Counts = NL_Counts * InjectionTime;
                        dNL_ScanIntensityHash[KVP.Key] += dNL_Counts;
                    }

                    if((ScanFragMethod == FragmentationMethod.CAD) && (radioBox_iTRAQ4.Checked))
                    {
                        Matrix Delta_R1_CAD = new Matrix(new double[,]
                        {
                            {dNL_ScanIntensityHash[114],dNL_ScanIntensityHash[115], dNL_ScanIntensityHash[116],dNL_ScanIntensityHash[117]},
                            {pcf_R2_m1,     AcP_R2,         pcf_R2_p1,      pcf_R2_p2},
                            {pcf_R3_m2,     pcf_R3_m1,      AcP_R3,         pcf_R3_p1},
                            {0,             pcf_R4_m2,      pcf_R4_m1,      AcP_R4}
                        });

                        Matrix Delta_R2_CAD = new Matrix(new double[,]
                        {
                            {AcP_R1,        pcf_R1_p1,      pcf_R1_p2,      0},
                            {dNL_ScanIntensityHash[114],dNL_ScanIntensityHash[115], dNL_ScanIntensityHash[116],dNL_ScanIntensityHash[117]},
                            {pcf_R3_m2,     pcf_R3_m1,      AcP_R3,         pcf_R3_p1},
                            {0,             pcf_R4_m2,      pcf_R4_m1,      AcP_R4}
                        });

                        Matrix Delta_R3_CAD = new Matrix(new double[,]
                        {
                            {AcP_R1,        pcf_R1_p1,       pcf_R1_p2,      0},
                            {pcf_R2_m1,     AcP_R2,          pcf_R2_p1,      pcf_R2_p2},
                            {dNL_ScanIntensityHash[114],dNL_ScanIntensityHash[115], dNL_ScanIntensityHash[116],dNL_ScanIntensityHash[117]},
                            {0,             pcf_R4_m2,      pcf_R4_m1,      AcP_R4}
                        });

                        Matrix Delta_R4_CAD = new Matrix(new double[,]
                        {
                            {AcP_R1,        pcf_R1_p1,      pcf_R1_p2,      0},
                            {pcf_R2_m1,     AcP_R2,         pcf_R2_p1,      pcf_R2_p2},
                            {pcf_R3_m2,     pcf_R3_m1,      AcP_R3,         pcf_R3_p1},
                            {dNL_ScanIntensityHash[114],dNL_ScanIntensityHash[115], dNL_ScanIntensityHash[116],dNL_ScanIntensityHash[117]}
                        });

                        try
                        {
                            double DeterminantR1 = Delta_R1_CAD.Determinant().Re;
                            double DeterminantR2 = Delta_R2_CAD.Determinant().Re;
                            double DeterminantR3 = Delta_R3_CAD.Determinant().Re;
                            double DeterminantR4 = Delta_R4_CAD.Determinant().Re;

                            PCF_ScanIntensityHash[114] = (DeterminantR1 / CADCoefficientMatrixDeterminant);
                            PCF_ScanIntensityHash[115] = (DeterminantR2 / CADCoefficientMatrixDeterminant);
                            PCF_ScanIntensityHash[116] = (DeterminantR3 / CADCoefficientMatrixDeterminant);
                            PCF_ScanIntensityHash[117] = (DeterminantR4 / CADCoefficientMatrixDeterminant);
                        }
                        catch(DivideByZeroException)
                        {
                            Console.WriteLine(FPline);
                            PCF_ScanIntensityHash[114] = dNL_ScanIntensityHash[114];
                            PCF_ScanIntensityHash[115] = dNL_ScanIntensityHash[115];
                            PCF_ScanIntensityHash[116] = dNL_ScanIntensityHash[116];
                            PCF_ScanIntensityHash[117] = dNL_ScanIntensityHash[117];
                        }

                        TotalScanIntCounts += PCF_ScanIntensityHash[114] + PCF_ScanIntensityHash[115] + PCF_ScanIntensityHash[116] + PCF_ScanIntensityHash[117];
                    }

                    Application.DoEvents();

                    if((ScanFragMethod == FragmentationMethod.CAD) && (radioBox_iTRAQ8.Checked))
                    {
                        Matrix Delta_R1_CAD = new Matrix(new double[,]
                        {
                            {dNL_ScanIntensityHash[113],dNL_ScanIntensityHash[114], dNL_ScanIntensityHash[115],dNL_ScanIntensityHash[116], dNL_ScanIntensityHash[117],dNL_ScanIntensityHash[118],dNL_ScanIntensityHash[119],dNL_ScanIntensityHash[121]},
                            {pcf_R2_m1,     AcP_R2,         pcf_R2_p1,      pcf_R2_p2,       0,             0,                  0,              0},
                            {pcf_R3_m2,     pcf_R3_m1,      AcP_R3,         pcf_R3_p1,       pcf_R3_p2,     0,                  0,              0},
                            {0,             pcf_R4_m2,      pcf_R4_m1,      AcP_R4,          pcf_R4_p1,     pcf_R4_p2,          0,              0},
                            {0,             0,              pcf_R5_m2,      pcf_R5_m1,       AcP_R5,        pcf_R5_p1,          pcf_R5_p2,      0},
                            {0,             0,              0,              pcf_R6_m2,       pcf_R6_m1,     AcP_R6,             pcf_R6_p1,      pcf_R6_p2},
                            {0,             0,              0,              0,               pcf_R7_m2,     pcf_R7_m1,          AcP_R7,         pcf_R7_p1},
                            {0,             0,              0,              0,               0,             pcf_R8_m2,          pcf_R8_m1,      AcP_R8}
                        });

                        Matrix Delta_R2_CAD = new Matrix(new double[,]
                        {
                            {AcP_R1,        pcf_R1_p1,      pcf_R1_p2,      0,               0,             0,                  0,              0},  
                            {dNL_ScanIntensityHash[113],dNL_ScanIntensityHash[114], dNL_ScanIntensityHash[115],dNL_ScanIntensityHash[116], dNL_ScanIntensityHash[117],dNL_ScanIntensityHash[118],dNL_ScanIntensityHash[119],dNL_ScanIntensityHash[121]},
                            {pcf_R3_m2,     pcf_R3_m1,      AcP_R3,         pcf_R3_p1,       pcf_R3_p2,     0,                  0,              0},
                            {0,             pcf_R4_m2,      pcf_R4_m1,      AcP_R4,          pcf_R4_p1,     pcf_R4_p2,          0,              0},
                            {0,             0,              pcf_R5_m2,      pcf_R5_m1,       AcP_R5,        pcf_R5_p1,          pcf_R5_p2,      0},
                            {0,             0,              0,              pcf_R6_m2,       pcf_R6_m1,     AcP_R6,             pcf_R6_p1,      pcf_R6_p2},
                            {0,             0,              0,              0,               pcf_R7_m2,     pcf_R7_m1,          AcP_R7,         pcf_R7_p1},
                            {0,             0,              0,              0,               0,             pcf_R8_m2,          pcf_R8_m1,      AcP_R8}
                        });

                        Matrix Delta_R3_CAD = new Matrix(new double[,]
                        {
                            {AcP_R1,        pcf_R1_p1,      pcf_R1_p2,      0,               0,             0,                  0,              0},  
                            {pcf_R2_m1,     AcP_R2,         pcf_R2_p1,      pcf_R2_p2,       0,             0,                  0,              0},
                            {dNL_ScanIntensityHash[113],dNL_ScanIntensityHash[114], dNL_ScanIntensityHash[115],dNL_ScanIntensityHash[116], dNL_ScanIntensityHash[117],dNL_ScanIntensityHash[118],dNL_ScanIntensityHash[119],dNL_ScanIntensityHash[121]},
                            {0,             pcf_R4_m2,      pcf_R4_m1,      AcP_R4,          pcf_R4_p1,     pcf_R4_p2,          0,              0},
                            {0,             0,              pcf_R5_m2,      pcf_R5_m1,       AcP_R5,        pcf_R5_p1,          pcf_R5_p2,      0},
                            {0,             0,              0,              pcf_R6_m2,       pcf_R6_m1,     AcP_R6,             pcf_R6_p1,      pcf_R6_p2},
                            {0,             0,              0,              0,               pcf_R7_m2,     pcf_R7_m1,          AcP_R7,         pcf_R7_p1},
                            {0,             0,              0,              0,               0,             pcf_R8_m2,          pcf_R8_m1,      AcP_R8}
                        });

                        Matrix Delta_R4_CAD = new Matrix(new double[,]
                        {
                            {AcP_R1,        pcf_R1_p1,      pcf_R1_p2,      0,               0,             0,                  0,              0},  
                            {pcf_R2_m1,     AcP_R2,         pcf_R2_p1,      pcf_R2_p2,       0,             0,                  0,              0},
                            {pcf_R3_m2,     pcf_R3_m1,      AcP_R3,         pcf_R3_p1,       pcf_R3_p2,     0,                  0,              0},
                            {dNL_ScanIntensityHash[113],dNL_ScanIntensityHash[114], dNL_ScanIntensityHash[115],dNL_ScanIntensityHash[116], dNL_ScanIntensityHash[117],dNL_ScanIntensityHash[118],dNL_ScanIntensityHash[119],dNL_ScanIntensityHash[121]},
                            {0,             0,              pcf_R5_m2,      pcf_R5_m1,       AcP_R5,        pcf_R5_p1,          pcf_R5_p2,      0},
                            {0,             0,              0,              pcf_R6_m2,       pcf_R6_m1,     AcP_R6,             pcf_R6_p1,      pcf_R6_p2},
                            {0,             0,              0,              0,               pcf_R7_m2,     pcf_R7_m1,          AcP_R7,         pcf_R7_p1},
                            {0,             0,              0,              0,               0,             pcf_R8_m2,          pcf_R8_m1,      AcP_R8}
                        });

                        Matrix Delta_R5_CAD = new Matrix(new double[,]
                        {
                            {AcP_R1,        pcf_R1_p1,      pcf_R1_p2,      0,               0,             0,                  0,              0},  
                            {pcf_R2_m1,     AcP_R2,         pcf_R2_p1,      pcf_R2_p2,       0,             0,                  0,              0},
                            {pcf_R3_m2,     pcf_R3_m1,      AcP_R3,         pcf_R3_p1,       pcf_R3_p2,     0,                  0,              0},
                            {0,             pcf_R4_m2,      pcf_R4_m1,      AcP_R4,          pcf_R4_p1,     pcf_R4_p2,          0,              0},
                            {dNL_ScanIntensityHash[113],dNL_ScanIntensityHash[114], dNL_ScanIntensityHash[115],dNL_ScanIntensityHash[116], dNL_ScanIntensityHash[117],dNL_ScanIntensityHash[118],dNL_ScanIntensityHash[119],dNL_ScanIntensityHash[121]},
                            {0,             0,              0,              pcf_R6_m2,       pcf_R6_m1,     AcP_R6,             pcf_R6_p1,      pcf_R6_p2},
                            {0,             0,              0,              0,               pcf_R7_m2,     pcf_R7_m1,          AcP_R7,         pcf_R7_p1},
                            {0,             0,              0,              0,               0,             pcf_R8_m2,          pcf_R8_m1,      AcP_R8}
                        });

                        Matrix Delta_R6_CAD = new Matrix(new double[,]
                        {
                            {AcP_R1,        pcf_R1_p1,      pcf_R1_p2,      0,               0,             0,                  0,              0},  
                            {pcf_R2_m1,     AcP_R2,         pcf_R2_p1,      pcf_R2_p2,       0,             0,                  0,              0},
                            {pcf_R3_m2,     pcf_R3_m1,      AcP_R3,         pcf_R3_p1,       pcf_R3_p2,     0,                  0,              0},
                            {0,             pcf_R4_m2,      pcf_R4_m1,      AcP_R4,          pcf_R4_p1,     pcf_R4_p2,          0,              0},
                            {0,             0,              pcf_R5_m2,      pcf_R5_m1,       AcP_R5,        pcf_R5_p1,          pcf_R5_p2,      0},
                            {dNL_ScanIntensityHash[113],dNL_ScanIntensityHash[114], dNL_ScanIntensityHash[115],dNL_ScanIntensityHash[116], dNL_ScanIntensityHash[117],dNL_ScanIntensityHash[118],dNL_ScanIntensityHash[119],dNL_ScanIntensityHash[121]},
                            {0,             0,              0,              0,               pcf_R7_m2,     pcf_R7_m1,          AcP_R7,         pcf_R7_p1},
                            {0,             0,              0,              0,               0,             pcf_R8_m2,          pcf_R8_m1,      AcP_R8}
                        });

                        Matrix Delta_R7_CAD = new Matrix(new double[,]
                        {
                                                
                            {AcP_R1,        pcf_R1_p1,      pcf_R1_p2,      0,               0,             0,                  0,              0},  
                            {pcf_R2_m1,     AcP_R2,         pcf_R2_p1,      pcf_R2_p2,       0,             0,                  0,              0},
                            {pcf_R3_m2,     pcf_R3_m1,      AcP_R3,         pcf_R3_p1,       pcf_R3_p2,     0,                  0,              0},
                            {0,             pcf_R4_m2,      pcf_R4_m1,      AcP_R4,          pcf_R4_p1,     pcf_R4_p2,          0,              0},
                            {0,             0,              pcf_R5_m2,      pcf_R5_m1,       AcP_R5,        pcf_R5_p1,          pcf_R5_p2,      0},
                            {0,             0,              0,              pcf_R6_m2,       pcf_R6_m1,     AcP_R6,             pcf_R6_p1,      pcf_R6_p2},
                           {dNL_ScanIntensityHash[113],dNL_ScanIntensityHash[114], dNL_ScanIntensityHash[115],dNL_ScanIntensityHash[116], dNL_ScanIntensityHash[117],dNL_ScanIntensityHash[118],dNL_ScanIntensityHash[119],dNL_ScanIntensityHash[121]},
                            {0,             0,              0,              0,               0,             pcf_R8_m2,          pcf_R8_m1,      AcP_R8}
                        });

                        Matrix Delta_R8_CAD = new Matrix(new double[,]
                        {
                            {AcP_R1,        pcf_R1_p1,      pcf_R1_p2,      0,               0,             0,                  0,              0},  
                            {pcf_R2_m1,     AcP_R2,         pcf_R2_p1,      pcf_R2_p2,       0,             0,                  0,              0},
                            {pcf_R3_m2,     pcf_R3_m1,      AcP_R3,         pcf_R3_p1,       pcf_R3_p2,     0,                  0,              0},
                            {0,             pcf_R4_m2,      pcf_R4_m1,      AcP_R4,          pcf_R4_p1,     pcf_R4_p2,          0,              0},
                            {0,             0,              pcf_R5_m2,      pcf_R5_m1,       AcP_R5,        pcf_R5_p1,          pcf_R5_p2,      0},
                            {0,             0,              0,              pcf_R6_m2,       pcf_R6_m1,     AcP_R6,             pcf_R6_p1,      pcf_R6_p2},
                            {0,             0,              0,              0,               pcf_R7_m2,     pcf_R7_m1,          AcP_R7,         pcf_R7_p1},
                            {dNL_ScanIntensityHash[113],dNL_ScanIntensityHash[114], dNL_ScanIntensityHash[115],dNL_ScanIntensityHash[116], dNL_ScanIntensityHash[117],dNL_ScanIntensityHash[118],dNL_ScanIntensityHash[119],dNL_ScanIntensityHash[121]}
                        });

                        try
                        {
                            double DeterminantR1 = Delta_R1_CAD.Determinant().Re;
                            double DeterminantR2 = Delta_R2_CAD.Determinant().Re;
                            double DeterminantR3 = Delta_R3_CAD.Determinant().Re;
                            double DeterminantR4 = Delta_R4_CAD.Determinant().Re;
                            double DeterminantR5 = Delta_R5_CAD.Determinant().Re;
                            double DeterminantR6 = Delta_R6_CAD.Determinant().Re;
                            double DeterminantR7 = Delta_R7_CAD.Determinant().Re;
                            double DeterminantR8 = Delta_R8_CAD.Determinant().Re;

                            PCF_ScanIntensityHash[113] = (DeterminantR1 / CADCoefficientMatrixDeterminant);
                            PCF_ScanIntensityHash[114] = (DeterminantR2 / CADCoefficientMatrixDeterminant);
                            PCF_ScanIntensityHash[115] = (DeterminantR3 / CADCoefficientMatrixDeterminant);
                            PCF_ScanIntensityHash[116] = (DeterminantR4 / CADCoefficientMatrixDeterminant);
                            PCF_ScanIntensityHash[117] = (DeterminantR5 / CADCoefficientMatrixDeterminant);
                            PCF_ScanIntensityHash[118] = (DeterminantR6 / CADCoefficientMatrixDeterminant);
                            PCF_ScanIntensityHash[119] = (DeterminantR7 / CADCoefficientMatrixDeterminant);
                            PCF_ScanIntensityHash[121] = (DeterminantR8 / CADCoefficientMatrixDeterminant);
                        }
                        catch(DivideByZeroException)
                        {
                            Console.WriteLine(FPline);
                            PCF_ScanIntensityHash[113] = dNL_ScanIntensityHash[113];
                            PCF_ScanIntensityHash[114] = dNL_ScanIntensityHash[114];
                            PCF_ScanIntensityHash[115] = dNL_ScanIntensityHash[115];
                            PCF_ScanIntensityHash[116] = dNL_ScanIntensityHash[116];
                            PCF_ScanIntensityHash[117] = dNL_ScanIntensityHash[117];
                            PCF_ScanIntensityHash[118] = dNL_ScanIntensityHash[118];
                            PCF_ScanIntensityHash[119] = dNL_ScanIntensityHash[119];
                            PCF_ScanIntensityHash[121] = dNL_ScanIntensityHash[121];
                        }

                        TotalScanIntCounts += PCF_ScanIntensityHash[113] + PCF_ScanIntensityHash[114] + PCF_ScanIntensityHash[115] + PCF_ScanIntensityHash[116] + PCF_ScanIntensityHash[117] + PCF_ScanIntensityHash[118] + PCF_ScanIntensityHash[119] + PCF_ScanIntensityHash[121];
                    }

                    if((ScanFragMethod == FragmentationMethod.CAD) && (radioBox_TMT6.Checked))
                    {
                        Matrix Delta_R1_CAD = new Matrix(new double[,]
                        {
                            {dNL_ScanIntensityHash[126],dNL_ScanIntensityHash[127], dNL_ScanIntensityHash[128],dNL_ScanIntensityHash[129], dNL_ScanIntensityHash[130],dNL_ScanIntensityHash[131]},
                            {pcf_R2_m1,     AcP_R2,         pcf_R2_p1,      pcf_R2_p2,       0,             0},
                            {pcf_R3_m2,     pcf_R3_m1,      AcP_R3,         pcf_R3_p1,       pcf_R3_p2,     0},
                            {0,             pcf_R4_m2,      pcf_R4_m1,      AcP_R4,          pcf_R4_p1,     pcf_R4_p2},
                            {0,             0,              pcf_R5_m2,      pcf_R5_m1,       AcP_R5,        pcf_R5_p1},
                            {0,             0,              0,              pcf_R6_m2,       pcf_R6_m1,     AcP_R6}
                        });

                        Matrix Delta_R2_CAD = new Matrix(new double[,]
                        {
                            {AcP_R1,        pcf_R1_p1,      pcf_R1_p2,      0,               0,              0},  
                            {dNL_ScanIntensityHash[126],dNL_ScanIntensityHash[127], dNL_ScanIntensityHash[128],dNL_ScanIntensityHash[129], dNL_ScanIntensityHash[130],dNL_ScanIntensityHash[131]},
                            {pcf_R3_m2,     pcf_R3_m1,      AcP_R3,         pcf_R3_p1,       pcf_R3_p2,     0},
                            {0,             pcf_R4_m2,      pcf_R4_m1,      AcP_R4,          pcf_R4_p1,     pcf_R4_p2},
                            {0,             0,              pcf_R5_m2,      pcf_R5_m1,       AcP_R5,        pcf_R5_p1},
                            {0,             0,              0,              pcf_R6_m2,       pcf_R6_m1,     AcP_R6}
                        });

                        Matrix Delta_R3_CAD = new Matrix(new double[,]
                        {
                            {AcP_R1,        pcf_R1_p1,      pcf_R1_p2,      0,               0,              0},  
                            {pcf_R2_m1,     AcP_R2,         pcf_R2_p1,      pcf_R2_p2,       0,             0},
                            {dNL_ScanIntensityHash[126],dNL_ScanIntensityHash[127], dNL_ScanIntensityHash[128],dNL_ScanIntensityHash[129], dNL_ScanIntensityHash[130],dNL_ScanIntensityHash[131]},
                            {0,             pcf_R4_m2,      pcf_R4_m1,      AcP_R4,          pcf_R4_p1,     pcf_R4_p2},
                            {0,             0,              pcf_R5_m2,      pcf_R5_m1,       AcP_R5,        pcf_R5_p1},
                            {0,             0,              0,              pcf_R6_m2,       pcf_R6_m1,     AcP_R6}
                        });

                        Matrix Delta_R4_CAD = new Matrix(new double[,]
                        {
                            {AcP_R1,        pcf_R1_p1,      pcf_R1_p2,      0,               0,              0},  
                            {pcf_R2_m1,     AcP_R2,         pcf_R2_p1,      pcf_R2_p2,       0,             0},
                            {pcf_R3_m2,     pcf_R3_m1,      AcP_R3,         pcf_R3_p1,       pcf_R3_p2,     0},
                            {dNL_ScanIntensityHash[126],dNL_ScanIntensityHash[127], dNL_ScanIntensityHash[128],dNL_ScanIntensityHash[129], dNL_ScanIntensityHash[130],dNL_ScanIntensityHash[131]},
                            {0,             0,              pcf_R5_m2,      pcf_R5_m1,       AcP_R5,        pcf_R5_p1},
                            {0,             0,              0,              pcf_R6_m2,       pcf_R6_m1,     AcP_R6}
                        });

                        Matrix Delta_R5_CAD = new Matrix(new double[,]
                        {
                            {AcP_R1,        pcf_R1_p1,      pcf_R1_p2,      0,               0,              0},  
                            {pcf_R2_m1,     AcP_R2,         pcf_R2_p1,      pcf_R2_p2,       0,             0},
                            {pcf_R3_m2,     pcf_R3_m1,      AcP_R3,         pcf_R3_p1,       pcf_R3_p2,     0},
                            {0,             pcf_R4_m2,      pcf_R4_m1,      AcP_R4,          pcf_R4_p1,     pcf_R4_p2},
                            {dNL_ScanIntensityHash[126],dNL_ScanIntensityHash[127], dNL_ScanIntensityHash[128],dNL_ScanIntensityHash[129], dNL_ScanIntensityHash[130],dNL_ScanIntensityHash[131]},
                            {0,             0,              0,              pcf_R6_m2,       pcf_R6_m1,     AcP_R6}
                        });

                        Matrix Delta_R6_CAD = new Matrix(new double[,]
                        {
                            {AcP_R1,        pcf_R1_p1,      pcf_R1_p2,      0,               0,              0},  
                            {pcf_R2_m1,     AcP_R2,         pcf_R2_p1,      pcf_R2_p2,       0,             0},
                            {pcf_R3_m2,     pcf_R3_m1,      AcP_R3,         pcf_R3_p1,       pcf_R3_p2,     0},
                            {0,             pcf_R4_m2,      pcf_R4_m1,      AcP_R4,          pcf_R4_p1,     pcf_R4_p2},
                            {0,             0,              pcf_R5_m2,      pcf_R5_m1,       AcP_R5,        pcf_R5_p1},
                            {dNL_ScanIntensityHash[126],dNL_ScanIntensityHash[127], dNL_ScanIntensityHash[128],dNL_ScanIntensityHash[129], dNL_ScanIntensityHash[130],dNL_ScanIntensityHash[131]}
                        });

                        try
                        {
                            double DeterminantR1 = Delta_R1_CAD.Determinant().Re;
                            double DeterminantR2 = Delta_R2_CAD.Determinant().Re;
                            double DeterminantR3 = Delta_R3_CAD.Determinant().Re;
                            double DeterminantR4 = Delta_R4_CAD.Determinant().Re;
                            double DeterminantR5 = Delta_R5_CAD.Determinant().Re;
                            double DeterminantR6 = Delta_R6_CAD.Determinant().Re;

                            PCF_ScanIntensityHash[126] = (DeterminantR1 / CADCoefficientMatrixDeterminant);
                            PCF_ScanIntensityHash[127] = (DeterminantR2 / CADCoefficientMatrixDeterminant);
                            PCF_ScanIntensityHash[128] = (DeterminantR3 / CADCoefficientMatrixDeterminant);
                            PCF_ScanIntensityHash[129] = (DeterminantR4 / CADCoefficientMatrixDeterminant);
                            PCF_ScanIntensityHash[130] = (DeterminantR5 / CADCoefficientMatrixDeterminant);
                            PCF_ScanIntensityHash[131] = (DeterminantR6 / CADCoefficientMatrixDeterminant);
                          
                        }
                        catch(DivideByZeroException)
                        {
                            Console.WriteLine(FPline);
                            ExceptionCount++;
                            PCF_ScanIntensityHash[126] = dNL_ScanIntensityHash[126];
                            PCF_ScanIntensityHash[127] = dNL_ScanIntensityHash[127];
                            PCF_ScanIntensityHash[128] = dNL_ScanIntensityHash[128];
                            PCF_ScanIntensityHash[129] = dNL_ScanIntensityHash[129];
                            PCF_ScanIntensityHash[130] = dNL_ScanIntensityHash[130];
                            PCF_ScanIntensityHash[131] = dNL_ScanIntensityHash[131];
                        }

                        TotalScanIntCounts += PCF_ScanIntensityHash[126] + PCF_ScanIntensityHash[127] + PCF_ScanIntensityHash[128] + PCF_ScanIntensityHash[129] + PCF_ScanIntensityHash[130] + PCF_ScanIntensityHash[131];
                    }

                    if((ScanFragMethod == FragmentationMethod.ETD) && (radioBox_iTRAQ4.Checked))
                    {
                        PCF_ScanIntensityHash[114] = dNL_ScanIntensityHash[114];
                        PCF_ScanIntensityHash[115] = dNL_ScanIntensityHash[115];
                        PCF_ScanIntensityHash[116] = dNL_ScanIntensityHash[116];
                        PCF_ScanIntensityHash[117] = dNL_ScanIntensityHash[117];
                    }

                    if((ScanFragMethod == FragmentationMethod.ETD) && (radioBox_iTRAQ8.Checked))
                    {
                        PCF_ScanIntensityHash[114] = dNL_ScanIntensityHash[113];
                        PCF_ScanIntensityHash[114] = dNL_ScanIntensityHash[114];
                        PCF_ScanIntensityHash[115] = dNL_ScanIntensityHash[115];
                        PCF_ScanIntensityHash[116] = dNL_ScanIntensityHash[116];
                        PCF_ScanIntensityHash[117] = dNL_ScanIntensityHash[117];
                        PCF_ScanIntensityHash[118] = dNL_ScanIntensityHash[118];
                        PCF_ScanIntensityHash[119] = dNL_ScanIntensityHash[119];
                        PCF_ScanIntensityHash[121] = dNL_ScanIntensityHash[121];
                    }

                    if((ScanFragMethod == FragmentationMethod.ETD) && (radioBox_TMT6.Checked))
                    {
                        PCF_ScanIntensityHash[126] = dNL_ScanIntensityHash[126];
                        PCF_ScanIntensityHash[127] = dNL_ScanIntensityHash[127];
                        PCF_ScanIntensityHash[128] = dNL_ScanIntensityHash[128];
                        PCF_ScanIntensityHash[129] = dNL_ScanIntensityHash[129];
                        PCF_ScanIntensityHash[130] = dNL_ScanIntensityHash[130];
                        PCF_ScanIntensityHash[131] = dNL_ScanIntensityHash[131];
                    }

                    peptidecount += 1;
                   

                    if (noisebandCap)
                    {  
                        StringBuilder sb = new StringBuilder();
                        foreach (TagInformation tInfo in noisebandcapDic)
                        {
                            sb.Append(tInfo.ClassTag);
                            sb.Append(" ");
                        }
                        if (sb.Length <= 0)
                        {                            
                            column_values[14] = "";
                        }
                        else
                        {
                            sb.Remove(sb.Length - 1, 1);
                            column_values[14] = sb.ToString();                            
                        }
                        sb.Clear();
                        for (int i = 0; i < column_values.Length; i++)
                        {
                            sb.Append(column_values[i]);
                            sb.Append(',');
                        }
                        sb.Append(Purity);
                        QuantTempFile.Write(sb.ToString());                        
                    }
                    else
                    {
                        QuantTempFile.Write(FPline + "," + Purity.ToString());
                    }

                    foreach(KeyValuePair<string, int> KVP in SamplesAndTags)
                    {
                        Application.DoEvents();
                        if((ComboBoxETDoptions.Text == "Don't Quantify") && (ScanFragMethod == FragmentationMethod.ETD))
                        {
                            QuantTempFile.Write(",NA");
                        }
                        else
                        {
                            QuantTempFile.Write("," + NL_ScanIntensityHash[KVP.Value]);
                        }
                    }

                    foreach(KeyValuePair<string, int> KVP in SamplesAndTags)
                    {
                        Application.DoEvents();
                        if((ComboBoxETDoptions.Text == "Don't Quantify") && (ScanFragMethod == FragmentationMethod.ETD))
                        {
                            QuantTempFile.Write(",NA");
                        }
                        else
                        {
                            QuantTempFile.Write("," + dNL_ScanIntensityHash[KVP.Value]);
                        }
                    }

                    if((ComboBoxETDoptions.Text == "Don't Quantify") && (ScanFragMethod == FragmentationMethod.ETD))
                    {
                        QuantTempFile.Write(",NA");
                    }
                    else
                    {
                        QuantTempFile.Write("," + TotalScanIntCounts);
                    }

                    //Add the purity corrected signal from each channel if it is in use to the total counts for that channel

                    foreach(KeyValuePair<string, int> KVP in SamplesAndTags)
                    {
                        Application.DoEvents();
                        if((ComboBoxETDoptions.Text == "Don't Quantify") && (ScanFragMethod == FragmentationMethod.ETD))
                        {
                            QuantTempFile.Write(",NA");
                        }
                        else
                        {
                            QuantTempFile.Write("," + PCF_ScanIntensityHash[KVP.Value]);
                        }

                        if(TagsInUse.Contains(KVP.Value))
                        {
                            TotalTagSignal[KVP.Value][ScanFragMethod] += PCF_ScanIntensityHash[KVP.Value];
                            if(ScanFragMethod == FragmentationMethod.ETD)
                            {
                                TotalETDSignal += PCF_ScanIntensityHash[KVP.Value];
                            }
                            if(ScanFragMethod == FragmentationMethod.CAD)
                            {                              
                                TotalCADSignal += PCF_ScanIntensityHash[KVP.Value];
                            }
                        }
                    }

                    QuantTempFile.WriteLine();

                    progressBar1.Value = (int)(((double)csvInputFile.BaseStream.Position / csvInputFile.BaseStream.Length) * 100);
                    Application.DoEvents();
                }

                //close all three files

                //raw.Close();
                csvInputFile.Close();
                QuantTempFile.Close();
            }

            double ExpectedRatio = 1 / TagCount;
            Application.DoEvents();
            Dictionary<int, Dictionary<FragmentationMethod, double>> NormalizationHash = new Dictionary<int, Dictionary<FragmentationMethod, double>>();
            double maxValue = 0;
            toolStripStatusLabel1.Text = "Calculating Normalization Values";
            foreach (int TagUsed in TagsInUse)
            {
                NormalizationHash.Add(TagUsed, new Dictionary<FragmentationMethod, double>());
                NormalizationHash[TagUsed].Add(FragmentationMethod.CAD, ExpectedRatio / (TotalTagSignal[TagUsed][FragmentationMethod.CAD] / TotalCADSignal));
                NormalizationHash[TagUsed].Add(FragmentationMethod.ETD, 0);
                //NormalizationHash[TagUsed][FragmentationMethod.CAD] = ExpectedRatio / (TotalTagSignal[TagUsed][FragmentationMethod.CAD] / TotalCADSignal);
                if (NormalizationHash[TagUsed][FragmentationMethod.CAD] > maxValue)
                {
                    maxValue = NormalizationHash[TagUsed][FragmentationMethod.CAD];
                }
            }

            log.WriteLine();
            log.WriteLine("Tag m/z \tTotal Signal \tNoramlization Value \tNormalized to Max");           

            foreach(int TagUsed in TagsInUse)
            {                 
                double percentValue =  (NormalizationHash[TagUsed][FragmentationMethod.CAD]) / maxValue;

                StringBuilder sb = new StringBuilder();
                sb.Append(TagUsed);
                sb.Append("\t ");
                sb.Append(TotalTagSignal[TagUsed][FragmentationMethod.CAD].ToString("g4"));
                sb.Append("\t ");
                sb.Append(NormalizationHash[TagUsed][FragmentationMethod.CAD].ToString("f4"));
                sb.Append("\t " );
                sb.Append(percentValue.ToString("f4"));
                log.WriteLine(sb.ToString());          

                if(TotalETDSignal > 0)
                {
                    NormalizationHash[TagUsed][FragmentationMethod.ETD] = ExpectedRatio / (TotalTagSignal[TagUsed][FragmentationMethod.ETD] / TotalETDSignal);

                    log.WriteLine(TagUsed + " Total Signal ETD = " + TotalTagSignal[TagUsed][FragmentationMethod.ETD] + ", Normalization value (ETD) = " + NormalizationHash[TagUsed][FragmentationMethod.ETD]);
                }
                else
                {
                    //log.WriteLine(TagUsed + " Total Signal ETD = " + TotalTagSignal[TagUsed][FragmentationMethod.ETD] + "," + " Normalization value (ETD) = NaN");
                }
            }
            log.WriteLine();

            log.Close();

            ////Re open files and perform normalization

            foreach(string filename in listBox1.Items)
            {
                toolStripStatusLabel1.Text = "Applying Normalization Values";
                Application.DoEvents();
                string quantfile = Path.Combine(textOutputFolder.Text, Path.GetFileNameWithoutExtension(filename) + "_quant_temp.csv");
                StreamReader sr = new StreamReader(quantfile);
                string outputname = Path.Combine(textOutputFolder.Text, Path.GetFileNameWithoutExtension(filename) + "_quant.csv");
                StreamWriter sw = new StreamWriter(outputname);
                string header = sr.ReadLine();      // read header and add appropriate header line to output file
                sw.Write(header);

                foreach(KeyValuePair<string, int> KVP in SamplesAndTags)
                {
                    Application.DoEvents();
                    sw.Write(",TQ_" + KVP.Value + "_" + KVP.Key + "_PCN");
                }
                sw.Write(",Cleavage Site");               
                sw.WriteLine();
                while(sr.Peek() != -1)         // go through csv and raw file to extract the info we want
                {
                    Application.DoEvents();
                    string FPline = sr.ReadLine();
                    string[] column_values = Regex.Split(FPline, @",(?!(?<=(?:^|,)\s*\x22(?:[^\x22]|\x22\x22|\\\x22)*,)(?:[^\x22]|\x22\x22|\\\x22)*\x22\s*(?:,|$))"); // crazy regex to parse CSV with internal double quotes from http://regexlib.com/REDetails.aspx?regexp_id=621
                    sw.Write(FPline);
                    FragmentationMethod ScanFragMethod = FragmentationMethod.CAD;
                    if(column_values[1].Contains(".ETD."))
                    {
                        ScanFragMethod = FragmentationMethod.ETD;
                        if((ComboBoxETDoptions.Text == "Use Scan Before") || (ComboBoxETDoptions.Text == "Use Scan After"))
                        {
                            ScanFragMethod = FragmentationMethod.CAD;
                        }
                    }

                    int tagnumber = 0;
                    if(radioBox_iTRAQ4.Checked)
                    {
                        tagnumber = 4;
                    }
                    if(radioBox_iTRAQ8.Checked)
                    {
                        tagnumber = 8;
                    }
                    if(radioBox_TMT6.Checked)
                    {
                        tagnumber = 6;
                    }
                    int FirstQuantColumn = column_values.Length - tagnumber;

                    int i = 0;
                    foreach(KeyValuePair<string, int> KVP in SamplesAndTags)
                    {
                        Application.DoEvents();
                        if(column_values[FirstQuantColumn + i] == "NA")
                        {
                            sw.Write(",NA");
                            i += 1;
                        }
                        else
                        {
                            double UnnormalizedValue = 0;
                            UnnormalizedValue = Convert.ToDouble(column_values[FirstQuantColumn + i]);                        
                            double NormalizedValue = 0;
                            if(NormalizationHash.ContainsKey(KVP.Value))
                            {
                                NormalizedValue = UnnormalizedValue * NormalizationHash[KVP.Value][ScanFragMethod];
                                double whatIsThis = NormalizationHash[KVP.Value][ScanFragMethod];
                            }
                            else
                            {
                                NormalizedValue = UnnormalizedValue;
                            }
                            sw.Write("," + NormalizedValue);
                            i += 1;
                            
                        }
                    }
                    
                                      
                    sw.Write("," + CleavagePattern);                   
                    sw.WriteLine();
                }
                sw.Close();
                sr.Close();
                File.Delete(quantfile);
            }
            panel1.Enabled = true;
            toolStripStatusLabel1.Text = "done";
        }  
        
        private static int sortByAscendingIntensity(KeyValuePair<double, double> left, KeyValuePair<double, double> right)
        {
            return left.Key.CompareTo(right.Key);
        }
    }
}