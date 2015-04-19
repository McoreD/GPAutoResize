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