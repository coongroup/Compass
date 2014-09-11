using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSMSL.Util;
using IMP.PhosphoRS;

namespace Coon.Compass.Lotor
{
    public class PhosphoRS : ThreadManagement.IDataConection
    {
       readonly AminoAcidModification _modificationToLocalize;
        private readonly List<Tuple<PeptideSpectrumMatch, List<Tuple<int, List<int>>>>> _psms;
        public Dictionary<int, string> Sequences; 
        int _currentNr = 0;
        private const double masstolerance = 0.2;
        private const int maxIsoformCount = 500;
        private const int maxPTMCount = 20;
        private const int maxPackageSize = 3000;
        private static int peptideID = 1;
        public ThreadManagement PhosphoRS;
        public int TotalPsms { get { return _psms.Count; } }
        
        public SampleDataSource(AminoAcidModification modificationToLocalize, IEnumerable<PSM> psms)
        {
            _modificationToLocalize = modificationToLocalize;
            _psms = new List<Tuple<PeptideSpectrumMatch, List<Tuple<int, List<int>>>>>();
            Sequences = new Dictionary<int, string>();
            foreach (PeptideSpectrumMatch psm in psms)
            {
                var modLocations = psm.Sequence.GetPositionsOf(_modificationToLocalize.TargetAminoAcids, true).Select(n => n - 1).ToArray(); // need to be 0-based
                int ptms = psm.Sequence.GetPositionsOf(_modificationToLocalize).Count;
                var id2ModMap = new List<Tuple<int, List<int>>>();
                foreach (var combo in Combinatorics.Combinations(modLocations, ptms))
                {
                    int id = peptideID++;
                    id2ModMap.Add(new Tuple<int, List<int>>(id, new List<int>(combo)));
                    Sequences.Add(id, ConvertString(psm.sequenceString, combo));
                }
             
                _psms.Add(new Tuple<PeptideSpectrumMatch, List<Tuple<int, List<int>>>>(psm, id2ModMap));
            }
        }

        private string ConvertString(string baseString, IEnumerable<int> positions)
        {
            char[] seq = baseString.ToCharArray();
            foreach (int pos in positions)
            {
                seq[pos] = char.ToLower(seq[pos]);
            }
            return new string(seq);
        }

        private readonly BlockingCollection<ThreadManagement.progressMessage> _progressMessageQueue = 
            new BlockingCollection<ThreadManagement.progressMessage>(new ConcurrentQueue<ThreadManagement.progressMessage>());
        
        public List<ThreadManagement.SpectraPackageItem> GetNewDataPackage(int maxSizeOfPackage, out int numberOfSpectraPacked, out int numberOfPeptidesPacked)
        {
            numberOfSpectraPacked = 0;
            numberOfPeptidesPacked = 0;
            if (_currentNr >= _psms.Count)
                return null;

            List<ThreadManagement.SpectraPackageItem> package = new List<ThreadManagement.SpectraPackageItem>();
            for (int i = _currentNr; i < _psms.Count && i - _currentNr < maxSizeOfPackage; i++)
            {
                package.Add(new ThreadManagement.SpectraPackageItem(_psms[i].Item1, 1.0 / _psms.Count, _psms[i].Item2));
            }

            _currentNr += package.Count;
            numberOfSpectraPacked = package.Count;
            numberOfPeptidesPacked = package.Count;

            return package;
        }

        public BlockingCollection<ThreadManagement.progressMessage> GetProgressMessageQueue()
        {
            return _progressMessageQueue;
        }

        public void Start()
        {
            _currentNr = 0;
            var searchCancel = new CancellationTokenSource();
            Task t2 = Task.Factory.StartNew(() =>
            {
                try
                {
                    ThreadManagement.progressMessage msg;
                    double lastProgress;

                    lastProgress = -1;
                    while (!_progressMessageQueue.IsCompleted)
                    {
                        msg = _progressMessageQueue.Take();
                        if (msg.type == ThreadManagement.progressMessage.typeOfMessage.stringMessage)
                        {
                            if (msg.message != null)
                            {
                                //here the string message can be used to for output. 
                                //print(msg.message)
                            }
                        }
                        else
                        {
                            //spectraProcessed is not a integer. 
                            //see GetNewDataPackage. 
                            //after each spetra this funtion will be called (asynchronously). 
                            var currentProgress = msg.spectraProcessed / TotalPsms;

                            if (currentProgress >= lastProgress + 0.001)
                            {//just send progress reports in 1% steps... 
                                lastProgress = currentProgress;
                                //use this data to print the progress in a progress bar. 
                                //print_progress(lastprogress * 100);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }, searchCancel.Token);

            PhosphoRS = new ThreadManagement(this, searchCancel, maxIsoformCount, maxPTMCount, "automatic", masstolerance, _modificationToLocalize, TotalPsms);
         
            PhosphoRS.StartPTMLocalisation();
        }
    }
}
