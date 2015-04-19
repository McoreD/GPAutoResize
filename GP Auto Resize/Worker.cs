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

            foreach (string filePath in files)
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
                        }
                    }
                    else if (img.Height > settings.MaximumPixels)
                    {
                        float perc = (float)settings.MaximumPixels / (float)img.Height * 100;

                        using (Image img2 = ImageHelpers.ResizeImageByPercentage(img, perc))
                        {
                            img2.SaveJPG(filePath, settings.PhotoQuality);
                        }
                    }
                }
            }

            if (settings.IncludeSubfolders)
                Directory.GetDirectories(folderPath).ForEach(ResizePhotosInFolderAsync);
        }
    }
}