#region License Information (GPL v3)

/*
    Google+ Auto Resize - A program that allows you to resize photos larger than
    2048 pixels (by default) in width or height so that it does not count towards
    your Google Drive storage.

    Copyright (C) 2015 ShareX Developers

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using ShareX.HelpersLib;
using System;
using System.IO;
using System.Windows.Forms;

namespace GPAR
{
    public static class Program
    {
        public static readonly string LogFilePath = Path.Combine(Path.GetDirectoryName(Application.StartupPath), string.Format("{0}-{1}-debug.log", Application.ProductName, DateTime.Now.ToString("yyyyMMdd")));

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
            DebugHelper.Logger.SaveLog(LogFilePath);
        }
    }
}