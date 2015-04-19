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