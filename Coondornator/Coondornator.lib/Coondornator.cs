
namespace Compass.Coondornator
{
    public class Coondornator
    {
        public const string DefaultFileServer = "144.92.52.55";

        public const string CondorRootDirectory = "/home/GENERAL/Condor/"; //  "/home/Groups/Condor/";
        public const string CondorBinDirectory = CondorRootDirectory + "bin/";
        public const string OmssaDefaultExecutable = "omssacl";
        public const string OmssaFilePath = CondorBinDirectory + OmssaDefaultExecutable;

        public const string CondorSubmitExecutable = "/usr/bin/condor_submit";

        public const string NullDevice = "/dev/null";
        
        public const string UserCondorDirectory = CondorRootDirectory + "searches/";
        //public const string UserCondorTempDirectory = UserCondorDirectory + "temp/";
       
        public const string DefaultOmssaModsXML = CondorBinDirectory + "mods.xml";
        public const string DefaultOmssaUserModsXML = CondorBinDirectory + "usermods.xml";

        public const string CondorLogDirectory = CondorRootDirectory + "log/"; //CondorDatabaseDirectory + "Logs/";
        public const string CondorMasterLog = CondorLogDirectory + "omssacl.log";

        public const string CondorDatabaseDirectory = CondorRootDirectory + "databases/";

    }
}
