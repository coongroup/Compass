using System.IO;

namespace OmssaNavigator
{
    public class DataBaseFile : File
    {
        private string phr;
        private string pin;
        private string psq;
        private string pal;

        public string FullPathWithoutExtension
        {
            get
            {
                return Path.Combine(BasePath, Name);
            }
        }

        public DataBaseFile(string filePath)
            : base(filePath)
        {
            phr = FullPathWithoutExtension + ".phr";
            pin = FullPathWithoutExtension + ".pin";
            psq = FullPathWithoutExtension + ".psq";
            pal = FullPathWithoutExtension + ".pal";
        }
    }
}