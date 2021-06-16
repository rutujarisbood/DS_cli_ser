
// Name: Rutuja Sanjay Risbood
// UTA ID: 1001843943
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    public static class Program
    {
        public static bool isMainServer = false;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        public static async Task Main()
        {
            using (Mutex mutex = new Mutex(false, "MainServer"))
            {
                if (!mutex.WaitOne(0, false))
                {
                    isMainServer = true;
                }

                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new ServerUIForm1());
            }
        }
    }
}
