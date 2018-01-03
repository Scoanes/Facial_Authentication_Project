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

            // adding both user controls to the panel
            pnl_Main.Controls.Add(ucMainPage.Instance);
            ucMainPage.Instance.Dock = DockStyle.Fill;

            pnl_Main.Controls.Add(ucEnrolPage.Instance);
            ucEnrolPage.Instance.Dock = DockStyle.Fill;

            // setting default to main page
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
                // change the panel contents
                ucEnrolPage.Instance.BringToFront();

                // update button text and vars
                btn_userControlSwitch.Text = "Main Page";
                isMainPage = false;

                // close down any objects in the main page
                ucMainPage.Instance.mainPageReset();
                ucEnrolPage.Instance.EnrolPageStart();
            }
            else
            {
                // Swap to Main Page
                // change the panel contents
                ucMainPage.Instance.BringToFront();

                // update button text and vars
                btn_userControlSwitch.Text = "Enrol User";
                isMainPage = true;

                // close down any objects in the enrol page
                ucEnrolPage.Instance.EnrolPageStop();
            }
        }
    }
}
