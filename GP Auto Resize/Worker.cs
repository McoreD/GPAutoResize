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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GPAR
{
    public class Worker
    {
        private static Settings settings = Program.Settings;

        public static void Start()
        {
            if (!Directory.Exists(settings.PhotosLocation))
            {
                MessageBox.Show("Photos location is not configured.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (settings.BackupOriginalFiles && !Directory.Exists(settings.BackupLocation))
            {
                MessageBox.Show("Backup location is not configured.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ResizePhotosInFolderAsync(settings.PhotosLocation);
        }

        private static void ResizePhotosInFolderAsync(string folderPath)
        {
            Thread thread = new Thread(() =>
            {
                ResizePhotosInFolder(folderPath);
            });
            thread.Start();
        }

        private static void ResizePhotosInFolder(string folderPath)
        {
            List<string> files = new List<string>();

            settings.ImageExtensions.ForEach(ext => files.AddRange(Directory.GetFiles(folderPath, "*." + ext)));

            Parallel.ForEach(files, filePath =>
            {
                using (Image img = Image.FromFile(filePath))
                {
                    if ((img.Width > settings.MaximumPixels || img.Height > settings.MaximumPixels) && settings.BackupOriginalFiles)
                    {
                        string newPath = filePath.Replace(settings.PhotosLocation, settings.BackupLocation);
                        Helpers.CreateDirectoryIfNotExist(newPath);
                        File.Copy(filePath, newPath);
                    }

                    if (img.Width > img.Height && img.Width > settings.MaximumPixels)
                    {
                        float perc = (float)settings.MaximumPixels / (float)img.Width * 100;

                        using (Image img2 = ImageHelpers.ResizeImageByPercentage(img, perc))
                        {
                            img2.SaveJPG(filePath, settings.PhotoQuality);
                            Console.WriteLine("Resized {0} on thread {1}", Path.GetFileName(filePath), Thread.CurrentThread.ManagedThreadId);
                        }
                    }
                    else if (img.Height > settings.MaximumPixels)
                    {
                        float perc = (float)settings.MaximumPixels / (float)img.Height * 100;

                        using (Image img2 = ImageHelpers.ResizeImageByPercentage(img, perc))
                        {
                            img2.SaveJPG(filePath, settings.PhotoQuality);
                            Console.WriteLine("Resized {0} on thread {1}", Path.GetFileName(filePath), Thread.CurrentThread.ManagedThreadId);
                        }
                    }
                }
            });

            if (settings.IncludeSubfolders)
                Directory.GetDirectories(folderPath).ForEach(ResizePhotosInFolderAsync);
        }
    }
}