using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPAR
{
    public class Settings : SettingsBase<Settings>
    {
        [Category("Settings"), DefaultValue(true), Description("Include subfolders")]
        public bool IncludeSubfolders { get; set; }

        [Category("Upload"), Description("Files with these file extensions will be resized.")]
        [Editor("System.Windows.Forms.Design.StringCollectionEditor,System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public List<string> ImageExtensions { get; set; }

        [Category("Settings"), DefaultValue(2048), Description("Maximum pixels in the resized photo in width or height")]
        public int MaximumPixels { get; set; }

        [Category("Settings"), DefaultValue(true), Description("Move original files to the backup location.")]
        [Editor(typeof(DirectoryNameEditor), typeof(UITypeEditor))]
        public bool BackupOriginalFiles { get; set; }

        [Category("Settings"), Description("Location of the backup folder for original files")]
        [Editor(typeof(DirectoryNameEditor), typeof(UITypeEditor))]
        public string BackupLocation { get; set; }

        [Category("Settings"), Description("Location of the photos folder")]
        [Editor(typeof(DirectoryNameEditor), typeof(UITypeEditor))]
        public string PhotosLocation { get; set; }

        [Category("Settings"), DefaultValue(99), Description("Image quality")]
        public int PhotoQuality { get; set; }

        public Settings()
        {
            ImageExtensions = new List<string>();
            this.ApplyDefaultPropertyValues();
        }
    }
}