namespace OmssaNavigator
{
    public class ResultFile : File
    {
        public string DestinationDirectory { get; set; }

        public ResultFile(string filePath, string destDirectory)
            : base(filePath)
        {
            DestinationDirectory = destDirectory;
        }
    }
}