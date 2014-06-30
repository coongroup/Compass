using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSMSL;
using CSMSL.Chemistry;
using CSMSL.IO;
using CSMSL.IO.Thermo;
using CSMSL.Spectral;
using ZedGraph;

namespace Coon.Compass.DtaGenerator
{
    public partial class DtaViewerForm : Form
    {
        private ThermoRawFile _rawFile;

        private BindingList<int> _scanNumbers;

        public DtaViewerForm()
        {
            InitializeComponent();

           
            //_dtas = new BindingList<Dta>();
            //listBox1.DataSource = _dtas;
            //listBox1.DisplayMember = "ID";

            _scanNumbers = new BindingList<int>();
            listBox1.DataSource = _scanNumbers;
            zedGraphControl1.ZoomEvent += ZoomEvent;
            zedGraphControl2.ZoomEvent += ZoomEvent;
            
            zedGraphControl1.GraphPane.XAxis.Title.Text = "m/z";
            zedGraphControl2.GraphPane.XAxis.Title.Text = "m/z";
            zedGraphControl1.GraphPane.YAxis.Title.Text = "Intensity";
            zedGraphControl2.GraphPane.YAxis.Title.Text = "Intensity";

            zedGraphControl1.GraphPane.XAxis.MajorTic.IsInside = false;
            zedGraphControl1.GraphPane.XAxis.MinorTic.IsInside = false;
            zedGraphControl2.GraphPane.XAxis.MajorTic.IsInside = false;
            zedGraphControl2.GraphPane.XAxis.MinorTic.IsInside = false;

            RestoreSettings();
        }

        private void RestoreSettings()
        {
            numericUpDown1.Value = (decimal)DtaGenerator.LOW_PRECURSOR_CLEANING_WINDOW_MZ;
            numericUpDown2.Value = (decimal)DtaGenerator.HIGH_PRECURSOR_CLEANING_WINDOW_MZ;
            numericUpDown3.Value = (decimal)DtaGenerator.LOW_NEUTRAL_LOSS_CLEANING_WINDOW_DA;
            numericUpDown4.Value = (decimal)DtaGenerator.HIGH_PRECURSOR_CLEANING_WINDOW_MZ;

           
            if (_currentIndex >= 0)
            {
                Plot(_currentIndex, false);
            }
        }

        private void ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {
            if (sender == zedGraphControl1)
            {
                newState.ApplyState(zedGraphControl2.GraphPane);
                zedGraphControl2.Invalidate();
                zedGraphControl2.AxisChange();
            } else {
                newState.ApplyState(zedGraphControl1.GraphPane);
                zedGraphControl1.Invalidate();
                zedGraphControl1.AxisChange();
            }
        }
        
        private void Plot(int spectrumNumber, bool rescale = true)
        {
            zedGraphControl1.GraphPane.CurveList.Clear();
            zedGraphControl2.GraphPane.CurveList.Clear();

            var rawSpectrum = _rawFile.GetSpectrum(spectrumNumber);

            Polarity polarity = _rawFile.GetPolarity(spectrumNumber);
            double precursorMZ = _rawFile.GetPrecusorMz(spectrumNumber);
            int precursorZ = _rawFile.GetPrecusorCharge(spectrumNumber);
            double precursorMass = Mass.MassFromMz(precursorMZ, precursorZ);
        
            textBox2.Text = precursorMZ.ToString("f5");
            textBox3.Text = ((polarity == Polarity.Positive) ? "+" : "") + precursorZ.ToString("N0");
            textBox4.Text = precursorMass.ToString("f5");

            List<IRange<double>> mzRangesToRemove = new List<IRange<double>>();

            if (checkBox1.Checked)
            {
                double lowMZ = (double)numericUpDown1.Value;
                double highMZ = (double)numericUpDown2.Value;
                DtaGenerator.CleanPrecursor(mzRangesToRemove, precursorMZ, lowMZ, highMZ);
            }

            if (checkBox2.Checked)
            {
                double lowMZ = (double)numericUpDown3.Value;
                double highMZ = (double)numericUpDown4.Value;
                DtaGenerator.CleanETD(mzRangesToRemove, precursorMass, precursorZ, lowMZ, highMZ);
            }
            
            var cleanSpectrum = rawSpectrum.Filter(mzRangesToRemove);
   
            zedGraphControl1.GraphPane.AddStick("Raw", rawSpectrum.GetMasses(), rawSpectrum.GetIntensities(), Color.Black);
            zedGraphControl2.GraphPane.AddStick("Cleaned", cleanSpectrum.GetMasses(), cleanSpectrum.GetIntensities(), Color.Black);

            if (rescale)
            {
                zedGraphControl1.GraphPane.XAxis.Scale.Min = rawSpectrum.FirstMz;
                zedGraphControl2.GraphPane.XAxis.Scale.Min = rawSpectrum.FirstMz;
                zedGraphControl1.GraphPane.XAxis.Scale.Max = rawSpectrum.LastMZ;
                zedGraphControl2.GraphPane.XAxis.Scale.Max = rawSpectrum.LastMZ;
            }

            zedGraphControl1.GraphPane.Title.Text = _rawFile.GetScanFilter(spectrumNumber);
            //zedGraphControl2.GraphPane.Title.Text = dta.Name;

            zedGraphControl1.Invalidate();
            zedGraphControl2.Invalidate();

            zedGraphControl1.AxisChange();
            zedGraphControl2.AxisChange();

            zedGraphControl1.SetScrollRangeFromData();
            zedGraphControl2.SetScrollRangeFromData();
            
            _currentIndex = spectrumNumber;
        }

        private void LoadRawFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;
           
            _rawFile = new ThermoRawFile(filePath);
            _rawFile.Open();

             _scanNumbers.Clear();
            _scanNumbers.RaiseListChangedEvents = false;
            for (int i = _rawFile.FirstSpectrumNumber; i < _rawFile.LastSpectrumNumber; i++)
            {
                if (_rawFile.GetMsnOrder(i) > 1)
                {
                    _scanNumbers.Add(i);
                }
            }
            _scanNumbers.RaiseListChangedEvents = true;
            _scanNumbers.ResetBindings();
           
            
            textBox1.Text = filePath;
        }

    

        private void DtaViewerForm_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] data = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (data == null)
                    return;
                if (data.Any(f => Path.GetExtension(f).Equals(".raw") || Path.GetExtension(f).Equals(".txt")))
                {
                    e.Effect = DragDropEffects.All;
                }
            }
        }

        private void DtaViewerForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] data = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (data == null)
                    return;

                LoadRawFile(data.First(f => Path.GetExtension(f).Equals(".raw")));
         
            }
        }

        private int _currentIndex = -1;

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            object o = listBox1.SelectedValue;
            if (o == null)
                return;
            var i = (int)o;

            Plot(i);
        }

        private void Refreshed(object sender, EventArgs e)
        {
            if (_currentIndex >= 0)
            {
                Plot(_currentIndex, false);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RestoreSettings();
        }
    }
}
