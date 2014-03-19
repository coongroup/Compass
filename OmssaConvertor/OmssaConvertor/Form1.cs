using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSMSL;
using CSMSL.Proteomics;
using CSMSL.IO.Thermo;

namespace OmssaConvertor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Convertor convertor = new Convertor();

            convertor.Protease = Protease.Trypsin;
            convertor.FastaDatabaseFile = @"C:\Users\Derek\Documents\Compass Tests\OmssaConvertor\CONCAT_Uniprot_yeast_canonicalIsoforms_20jan2013.fasta";
            convertor.MissedClevages = 3;
            convertor.FixedMods.Add(3);
            ThermoRawFile rawFile = new ThermoRawFile(@"C:\Users\Derek\Documents\Compass Tests\OmssaConvertor\10sep2013_yeast_control_2.raw");
            convertor.Convert(@"C:\Users\Derek\Documents\Compass Tests\OmssaConvertor\10sep2013_yeast_control_2_ITMS_HCD30.00.csv", rawFile);


        }
    }
}
