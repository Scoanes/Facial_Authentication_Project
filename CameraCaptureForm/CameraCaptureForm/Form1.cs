using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.Structure;
using System.IO;

namespace CameraCaptureForm
{
    public partial class Form1 : Form
    {

        bool isMainPage;

        public Form1()
        {
            InitializeComponent();

            // Setting up default form contents

            pnl_Main.Controls.Add(ucMainPage.Instance);
            ucMainPage.Instance.Dock = DockStyle.Fill;
            ucMainPage.Instance.BringToFront();

            this.isMainPage = true;
            btn_userControlSwitch.Text = "Enrol User";
        }

        private void btn_userControlSwitch_Click(object sender, EventArgs e)
        {
            // if statement here which changes the panel contents
            if (isMainPage)
            {
                // Swap to Enrol Page
                // dispose of all user controls in panel
                foreach (Control control in pnl_Main.Controls)
                {
                    control.Dispose();
                }

                // change the panel contents
                pnl_Main.Controls.Add(ucEnrolPage.Instance);
                ucEnrolPage.Instance.Dock = DockStyle.Fill;
                ucEnrolPage.Instance.BringToFront();

                // update button text and vars
                btn_userControlSwitch.Text = "Main Page";
                isMainPage = false;
            }
            else
            {
                // Swap to Main Page
                // dispose of all user controls in panel
                foreach (Control control in pnl_Main.Controls)
                {
                    control.Dispose();
                }

                // change the panel contents
                pnl_Main.Controls.Add(ucMainPage.Instance);
                ucMainPage.Instance.Dock = DockStyle.Fill;
                ucMainPage.Instance.BringToFront();

                // update button text and vars
                btn_userControlSwitch.Text = "Enrol User";
                isMainPage = true;
            }
        }
    }
}
