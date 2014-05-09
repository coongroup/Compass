
namespace Compass.Coondornator
{
    public class Coondornator
    {
        public const string DefaultFileServer = "coongrp.ad.biotech.wisc.edu";

        public const string CondorRootDirectory = "/home/Groups/Condor/";
        public const string OmssaDefaultExecutable = "omssacl";
        public const string OmssaFilePath = CondorRootDirectory + OmssaDefaultExecutable;

        public const string CondorSubmitExecutable = "/usr/bin/condor_submit";

        public const string NullDevice = "/dev/null";
        public const string UserCondorDirectory = "./Condor/";
        public const string UserCondorTempDirectory = UserCondorDirectory + "temp/";

        public const string GlobalArgumentLineFileName = CondorRootDirectory + "OmssaArgumentLines.xml";
        public const string DefaultOmssaModsXML = CondorRootDirectory + "mods.xml";
        public const string DefaultOmssaUserModsXML = CondorRootDirectory + "usermods.xml";

        public const string CondorLogDirectory = CondorDatabaseDirectory + "Logs/";
        public const string CondorMasterLog = CondorLogDirectory + "omssacl.log";

        public const string CondorDatabaseDirectory = CondorRootDirectory + "Databases/";
        public const string CondorFastaDatabaseDirectory = CondorDatabaseDirectory + "Fasta/";

    }
}
