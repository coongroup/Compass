using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CsvNeuterer
{
    public partial class Form1 : Form
    {
        private static ModificationDictionary modifications;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            modifications = new ModificationDictionary(Path.Combine(Application.StartupPath, "mods.xml"));
            Peptide.SetModifications(modifications);
            UpdateModsListboxes();
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filepaths = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach(string filepath in filepaths)
                {
                    if(Path.GetExtension(filepath).Equals(".csv", StringComparison.InvariantCultureIgnoreCase) &&
                        !lstOmssaCsvFiles.Items.Contains(filepath))
                    {
                        e.Effect = DragDropEffects.Link;
                        break;
                    }
                    else if(Path.GetExtension(filepath).Equals(".xml", StringComparison.InvariantCultureIgnoreCase))
                    {
                        e.Effect = DragDropEffects.Link;
                        break;
                    }
                }
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] filepaths = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach(string filepath in filepaths)
            {
                if(Path.GetExtension(filepath).Equals(".csv", StringComparison.InvariantCultureIgnoreCase) &&
                    !lstOmssaCsvFiles.Items.Contains(filepath))
                {
                    lstOmssaCsvFiles.Items.Add(filepath);
                }
                else if(Path.GetExtension(filepath).Equals(".xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    modifications.ReadModificationsFromXmlFile(filepath, true);
                    Peptide.SetModifications(modifications);
                    UpdateModsListboxes();
                }
            }
        }

        private void btnBrowseMods_Click(object sender, EventArgs e)
        {
            if(ofdModsXml.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                modifications.ReadModificationsFromXmlFile(ofdModsXml.FileName, true);
                Peptide.SetModifications(modifications);
                UpdateModsListboxes();
            }
        }

        private void UpdateModsListboxes()
        {
            lstAllLightModifications.Items.Clear();
            lstAllMediumModifications.Items.Clear();
            lstAllHeavyModifications.Items.Clear();
            lstSelectedLightFixedModifications.Items.Clear();
            lstSelectedMediumFixedModifications.Items.Clear();
            lstSelectedHeavyFixedModifications.Items.Clear();

            lstAllLightModifications.DisplayMember = "Text";
            lstAllMediumModifications.DisplayMember = "Text";
            lstAllHeavyModifications.DisplayMember = "Text";
            lstSelectedLightFixedModifications.DisplayMember = "Text";
            lstSelectedMediumFixedModifications.DisplayMember = "Text";
            lstSelectedHeavyFixedModifications.DisplayMember = "Text";
            foreach(Modification modification in modifications.Values)
            {
                ListViewItem list_view_item = new ListViewItem(modification.ToString());
                list_view_item.Tag = modification;
                lstAllLightModifications.Items.Add(list_view_item);
                lstAllMediumModifications.Items.Add(list_view_item);
                lstAllHeavyModifications.Items.Add(list_view_item);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if(ofdOmssaCsvFiles.ShowDialog() == DialogResult.OK)
            {
                lstOmssaCsvFiles.Items.AddRange(ofdOmssaCsvFiles.FileNames);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            while(lstOmssaCsvFiles.SelectedIndices.Count > 0)
            {
                lstOmssaCsvFiles.Items.RemoveAt(lstOmssaCsvFiles.SelectedIndices[0]);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lstOmssaCsvFiles.Items.Clear();
        }

        private void btnLightMoveRight_Click(object sender, EventArgs e)
        {
            while(lstAllLightModifications.SelectedItems.Count > 0)
            {
                lstSelectedLightFixedModifications.Items.Add(lstAllLightModifications.SelectedItem);
                lstAllLightModifications.Items.Remove(lstAllLightModifications.SelectedItem);
            }
        }

        private void btnLightMoveLeft_Click(object sender, EventArgs e)
        {
            while(lstSelectedLightFixedModifications.SelectedItems.Count > 0)
            {
                lstAllLightModifications.Items.Add(lstSelectedLightFixedModifications.SelectedItem);
                lstSelectedLightFixedModifications.Items.Remove(lstSelectedLightFixedModifications.SelectedItem);
            }
        }

        private void btnMediumMoveRight_Click(object sender, EventArgs e)
        {
            while(lstAllMediumModifications.SelectedItems.Count > 0)
            {
                lstSelectedMediumFixedModifications.Items.Add(lstAllMediumModifications.SelectedItem);
                lstAllMediumModifications.Items.Remove(lstAllMediumModifications.SelectedItem);
            }
        }

        private void btnMediumMoveLeft_Click(object sender, EventArgs e)
        {
            while(lstSelectedMediumFixedModifications.SelectedItems.Count > 0)
            {
                lstAllMediumModifications.Items.Add(lstSelectedMediumFixedModifications.SelectedItem);
                lstSelectedMediumFixedModifications.Items.Remove(lstSelectedMediumFixedModifications.SelectedItem);
            }
        }

        private void btnHeavyMoveRight_Click(object sender, EventArgs e)
        {
            while(lstAllHeavyModifications.SelectedItems.Count > 0)
            {
                lstSelectedHeavyFixedModifications.Items.Add(lstAllHeavyModifications.SelectedItem);
                lstAllHeavyModifications.Items.Remove(lstAllHeavyModifications.SelectedItem);
            }
        }

        private void btnHeavyMoveLeft_Click(object sender, EventArgs e)
        {
            while(lstSelectedHeavyFixedModifications.SelectedItems.Count > 0)
            {
                lstAllHeavyModifications.Items.Add(lstSelectedHeavyFixedModifications.SelectedItem);
                lstSelectedHeavyFixedModifications.Items.Remove(lstSelectedHeavyFixedModifications.SelectedItem);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            List<string> csv_filepaths = new List<string>(lstOmssaCsvFiles.Items.Count);
            foreach(string csv_filepath in lstOmssaCsvFiles.Items)
            {
                csv_filepaths.Add(csv_filepath);
            }
            List<Modification> light_fixed_modifications = new List<Modification>(lstSelectedLightFixedModifications.Items.Count);
            foreach(ListViewItem checked_item in lstSelectedLightFixedModifications.Items)
            {
                light_fixed_modifications.Add((Modification)checked_item.Tag);
            }
            List<Modification> medium_fixed_modifications = new List<Modification>(lstSelectedMediumFixedModifications.Items.Count);
            foreach(ListViewItem checked_item in lstSelectedMediumFixedModifications.Items)
            {
                medium_fixed_modifications.Add((Modification)checked_item.Tag);
            }
            List<Modification> heavy_fixed_modifications = new List<Modification>(lstSelectedHeavyFixedModifications.Items.Count);
            foreach(ListViewItem checked_item in lstSelectedHeavyFixedModifications.Items)
            {
                heavy_fixed_modifications.Add((Modification)checked_item.Tag);
            }

            CsvNeuterer csv_neuterer = new CsvNeuterer(csv_filepaths, light_fixed_modifications, medium_fixed_modifications, heavy_fixed_modifications);
            csv_neuterer.Neuter();
            MessageBox.Show("Finished!");
        }
    }
}
