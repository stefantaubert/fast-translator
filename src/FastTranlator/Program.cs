using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FastTranslator
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            int wert = 0;
            if (args.Length == 1)
                try { wert = Convert.ToInt32(args[0]); }
                catch { }
            Application.Run(new Form1(wert));
        }
    }
}
