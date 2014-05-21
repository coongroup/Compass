using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSMSL.IO;
using CSMSL.IO.Thermo;
using CSMSL.Spectral;
using ZedGraph;

namespace Coon.Compass.DtaGenerator
{
    public partial class DtaViewerForm : Form
    {
        private ThermoRawFile _rawFile;
        private DtaReader _dtaFile;
        
        private BindingList<Dta> _dtas;

        public DtaViewerForm()
        {
            InitializeComponent();

            _dtas = new BindingList<Dta>();
            listBox1.DataSource = _dtas;
            listBox1.DisplayMember = "ID";
            zedGraphControl1.ZoomEvent += ZoomEvent;
            zedGraphControl2.ZoomEvent += ZoomEvent;
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


        private void Plot(Dta dta)
        {
            MSDataScan scan = _rawFile[dta.ID];

            zedGraphControl1.GraphPane.CurveList.Clear();
            zedGraphControl2.GraphPane.CurveList.Clear();

            var rawSpectrum = scan.MassSpectrum;
            var cleanSpectrum = dta.Spectrum;

            zedGraphControl1.GraphPane.AddStick("Raw", rawSpectrum.GetMasses(), rawSpectrum.GetIntensities(), Color.Black);
            zedGraphControl2.GraphPane.AddStick("Cleaned", cleanSpectrum.GetMasses(), cleanSpectrum.GetIntensities(), Color.Black);

            double minmz = Math.Min(rawSpectrum.FirstMz, cleanSpectrum.FirstMz);
            double maxmz = Math.Max(rawSpectrum.LastMZ, cleanSpectrum.LastMZ);

            zedGraphControl1.GraphPane.XAxis.Scale.Min = minmz;
            zedGraphControl2.GraphPane.XAxis.Scale.Min = minmz;
            zedGraphControl1.GraphPane.XAxis.Scale.Max = maxmz;
            zedGraphControl2.GraphPane.XAxis.Scale.Max = maxmz;

            zedGraphControl1.Invalidate();
            zedGraphControl1.AxisChange();

            zedGraphControl2.Invalidate();
            zedGraphControl2.AxisChange();
        }

        private void LoadRawFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;
           
            _rawFile = new ThermoRawFile(filePath);
            _rawFile.Open();
            textBox1.Text = filePath;
        }

        private Task<List<Dta>> LoadDtaFile(string filePath)
        {
            Task<List<Dta>> t = new Task<List<Dta>>(() =>
            {
                List<Dta> dtas = new List<Dta>();
                int counter = 0;
                using (_dtaFile = new DtaReader(filePath))
                {
                    foreach (var dta in _dtaFile.ReadNextDta())
                    {
                        dtas.Add(dta);
                        if (counter++ > 500)
                            break;
                    }
                }
                return dtas;
            });
            t.Start();
  
            textBox2.Text = filePath;
            return t;
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

        private async void DtaViewerForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] data = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (data == null)
                    return;

                LoadRawFile(data.First(f => Path.GetExtension(f).Equals(".raw")));
                List<Dta> dtas = await LoadDtaFile(data.First(f => Path.GetExtension(f).Equals(".txt")));

                _dtas.RaiseListChangedEvents = false;
                _dtas.Clear();
                foreach (var dta in dtas)
                {
                    _dtas.Add(dta);
                }
                _dtas.RaiseListChangedEvents = true;
                _dtas.ResetBindings();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dta = listBox1.SelectedValue as Dta;
            if (dta == null)
                return;

            Plot(dta);
        }
    }
}
