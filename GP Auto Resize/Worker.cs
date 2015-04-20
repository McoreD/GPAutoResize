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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GPAR
{
    public class Worker
    {
        public bool IsWorking { get; private set; }

        public delegate void ProgressEventHandler(int current, int max);

        public event ProgressEventHandler ProgressChanged;

        private readonly Settings settings = Program.Settings;
        private readonly object progressLock = new object();

        public void Start()
        {
            if (IsWorking) return;

            IsWorking = true;

            try
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

                ResizePhotosInFolder(settings.PhotosLocation);
            }
            finally
            {
                IsWorking = false;
            }
        }

        protected void OnProgressChanged(int current, int max)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(current, max);
            }
        }

        private void ResizePhotosInFolder(string folderPath)
        {
            List<string> files = new List<string>();

            SearchOption searchOption = settings.IncludeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            settings.ImageExtensions.ForEach(ext => files.AddRange(Directory.GetFiles(folderPath, "*." + ext, searchOption)));

            if (!string.IsNullOrEmpty(settings.ExcludeFilesWithWord))
            {
                files = files.Where(x => !x.Contains(settings.ExcludeFilesWithWord)).ToList();
            }

            int current = 0;
            int max = files.Count;

            OnProgressChanged(current, max);

            Parallel.ForEach(files, filePath =>
            {
                try
                {
                    using (Image img = Image.FromFile(filePath))
                    {
                        if ((img.Width > settings.MaximumPixels || img.Height > settings.MaximumPixels) && settings.BackupOriginalFiles)
                        {
                            string newPath = filePath.Replace(settings.PhotosLocation, settings.BackupLocation);
                            Helpers.CreateDirectoryIfNotExist(newPath);
                            if (!File.Exists(newPath)) File.Copy(filePath, newPath);
                        }

                        float perc;

                        if (img.Width > img.Height && img.Width > settings.MaximumPixels)
                        {
                            perc = (float)settings.MaximumPixels / img.Width * 100;
                        }
                        else if (img.Height > settings.MaximumPixels)
                        {
                            perc = (float)settings.MaximumPixels / img.Height * 100;
                        }
                        else
                        {
                            return;
                        }

                        using (Image img2 = ImageHelpers.ResizeImageByPercentage(img, perc))
                        {
                            img2.SaveJPG(filePath, settings.PhotoQuality);
                            Console.WriteLine("Resized {0} on thread {1}", Path.GetFileName(filePath), Thread.CurrentThread.ManagedThreadId);
                        }
                    }

                    lock (progressLock)
                    {
                        current++;
                        OnProgressChanged(current, max);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }
    }
}