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
            Console.WriteLine(submitFile.SubmitFileHeader("hi nick", "hi nick"));
            submitFile.Requirements.Add("Derek Rocks");
            submitFile.Requirements.Add("Biking is fun");


            string result = submitFile.ToString();
        }
    }
}
