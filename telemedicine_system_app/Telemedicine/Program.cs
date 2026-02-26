using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telemedicine.Controller;

namespace Telemedicine
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            /*try
            {
                // 1. Identify the path
                string exePath = AppDomain.CurrentDomain.BaseDirectory;
                string csvFolder = Path.Combine(exePath, "DataSeed");

                // DEBUG POPUP: Does the folder actually exist?
                if (!Directory.Exists(csvFolder))
                {
                    MessageBox.Show($"CRITICAL: DataSeed folder NOT found at: {csvFolder}", "Path Error");
                    // Don't exit yet, let's see if the form opens anyway
                }

                // 2. Run Initialization
                DatabaseInitializer.InitializeIfNeeded(csvFolder);

                // 3. Launch Form
                Application.Run(new First_screen());
            }
            catch (Exception ex)
            {
                // THIS WILL CATCH THE SILENT KILLER
                MessageBox.Show($"The application crashed during startup!\n\nMessage: {ex.Message}\n\nStackTrace: {ex.StackTrace}", "Fatal Error");
            }*/

            Application.Run(new First_screen());
        }
    }
}
