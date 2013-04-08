using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compass.Coondornator
{
    class Program
    {
        static void Main(string[] args)
        {
            CondorSubmitFile submitFile = new CondorSubmitFile();
            submitFile.Requirements.Add("Derek Rocks");
            submitFile.Requirements.Add("Biking is fun");


            string result = submitFile.ToString();

          
            
        }
    }
}
