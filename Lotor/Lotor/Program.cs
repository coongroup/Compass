using System;
using System.Windows.Forms;

namespace Coon.Compass.Lotor
{
    class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new lotorForm());
        }
    }
}
