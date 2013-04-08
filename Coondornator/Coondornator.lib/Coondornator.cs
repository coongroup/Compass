using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compass.Coondornator
{
    public class Coondornator
    {
        public static readonly string DefaultFileServer = @"\\coongrp.ad.biotech.wisc.edu\";

        public static readonly string CondorRootDirectory = "/home/Groups/Condor/";
        public static readonly string OmssaDefaultExecutable = "omssacl";
        public static readonly string CondorSubmitExecutable = "/usr/bin/condor_submit";

        public static readonly string NullDevice = "/dev/null";
        public static readonly string UserCondorDirectory = "./Condor/";
        public static readonly string UserCondorTempDirectory = UserCondorDirectory + "temp/";

        public static readonly string GlobalArgumentLineFileName = CondorRootDirectory + "OmssaArgumentLines.xml";
        public static readonly string DefaultOmssaModsXML = CondorRootDirectory + "mods.xml";
        public static readonly string DefaultOmssaUserModsXML = CondorRootDirectory + "usermods.xml";

        public static readonly string CondorLogDirectory = CondorDatabaseDirectory + "Logs/";
        public static readonly string CondorMasterLog = CondorLogDirectory + "omssacl.log";

        public static readonly string CondorDatabaseDirectory = CondorRootDirectory + "Databases/";
        public static readonly string CondorFastaDatabaseDirectory = CondorDatabaseDirectory + "Fasta/";

    }
}
