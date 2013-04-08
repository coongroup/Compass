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
            Console.WriteLine(submitFile.SubmitFileHeader("executable", "requirements"));
            submitFile.Requirements.Add("Derek Rocks");
            submitFile.Requirements.Add("Biking is fun"); 
            string result = submitFile.ToString();

            Console.WriteLine("User Password");
            string password = GetPassword();
            Console.WriteLine();
            using (ServerConnection connection = new ServerConnection("coongrp.ad.biotech.wisc.edu", "dbailey", password))
            {
                Console.WriteLine(string.Join("\n", connection.GetBlastDatabases()));
            }
            Console.ReadKey();
            
        }

        private static string GetPassword()
        {
            StringBuilder pwd = new StringBuilder();
            while (true)
            {
                ConsoleKeyInfo i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (i.Key == ConsoleKey.Backspace)
                {              
                    pwd.Remove(pwd.Length - 1, 1);                
                    Console.Write("\b \b");
                }
                else
                {
                    pwd.Append(i.KeyChar);
                    Console.Write("*");
                }
            }
            return pwd.ToString();
        }
    }
}
