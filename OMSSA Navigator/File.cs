using System.IO;

namespace OmssaNavigator
{
    public class File
    {
        public string FilePath { get; set; }
        public string Name { get { return Path.GetFileNameWithoutExtension(FilePath); } }
        public string NameWithExt { get { return Path.GetFileName(FilePath); } }
        public string BasePath { get { return Path.GetDirectoryName(FilePath); } }

        public File(string filePath)
        {
            FilePath = filePath;
        }
    }
}
