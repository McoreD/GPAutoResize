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
        public List<string> FailedFiles { get; private set; }

        public delegate void ProgressEventHandler(int current, int max);

        public event ProgressEventHandler ProgressChanged;

        private readonly Settings settings = Program.Settings;
        private readonly object progressLock = new object();

        public void Start()
        {
            if (IsWorking) return;

            IsWorking = true;
            FailedFiles = new List<string>();

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

                if (settings.MaximumPixels == 0)
                {
                    MessageBox.Show("Maximum number of pixels is not configured.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                ResizePhotosInFolder(settings.PhotosLocation);

                if (FailedFiles.Count > 0)
                {
                    DebugHelper.WriteLine("Failed image files:\r\n" + string.Join("\r\n", FailedFiles));
                    //ResizePhotos(FailedFiles);
                }
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

            if (files.Count > 0)
            {
                ResizePhotos(files);
            }
        }

        private void ResizePhotos(IEnumerable<string> files)
        {
            int current = 0;
            int max = files.Count();

            OnProgressChanged(current, max);

            Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = settings.MaxDegreeOfParallelism }, filePath =>
            {
                try
                {
                    using (Image img = Image.FromFile(filePath))
                    {
                        bool resizePending = img.Width > settings.MaximumPixels || img.Height > settings.MaximumPixels;

                        if (resizePending)
                        {
                            if (settings.BackupOriginalFiles)
                            {
                                string newPath = filePath.Replace(settings.PhotosLocation, settings.BackupLocation);
                                Helpers.CreateDirectoryIfNotExist(newPath);
                                if (!File.Exists(newPath)) File.Copy(filePath, newPath);
                            }

                            using (Image img2 = ImageHelpers.ResizeImageLimit(img, settings.MaximumPixels))
                            {
                                DebugHelper.WriteLine("Resized {0} to {1}x{2} on thread {3}", Path.GetFileName(filePath), img2.Width, img2.Height, Thread.CurrentThread.ManagedThreadId.ToString("D4"));
                                img2.SaveJPG(filePath, settings.PhotoQuality);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    FailedFiles.Add(filePath);
                    DebugHelper.WriteException(ex, "Try reducing MaxDegreeOfParallelism");
                }
                finally
                {
                    lock (progressLock)
                    {
                        current++;
                        OnProgressChanged(current, max);
                    }
                }
            });
        }
    }
}