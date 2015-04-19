using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GPAR
{
    public static class Program
    {
        public static Settings Settings = new Settings();
        private static string SettingsPath = "Settings.json";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Settings = Settings.Load(SettingsPath);
            Application.Run(new MainForm());
            Settings.Save(SettingsPath);
        }
    }
}