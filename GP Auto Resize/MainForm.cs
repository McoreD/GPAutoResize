﻿#region License Information (GPL v3)

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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GPAR
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            if (Program.Settings.ImageExtensions.Count == 0)
            {
                Program.Settings.ImageExtensions.Add("jpg");
                Program.Settings.ImageExtensions.Add("jpeg");
            }
            pgApp.SelectedObject = Program.Settings;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Worker.Start();
        }
    }
}