using System.Collections.Generic;
using OmssaLib;

namespace OmssaNavigator
{
    public class DtaFile : File
    {
        public ArgumentLine Arguments { get; set; }
        public List<ResultFile> ResultFiles { get; private set; }

        public DtaFile(string filePath)
            : base(filePath)
        {
            ResultFiles = new List<ResultFile>();
        }

        public void AddResult(ResultFile result)
        {
            ResultFiles.Add(result);
        }
    }
}