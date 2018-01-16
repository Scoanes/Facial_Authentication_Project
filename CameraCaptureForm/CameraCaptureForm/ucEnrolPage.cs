using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CameraCaptureForm
{
    public partial class ucEnrolPage : UserControl
    {
        private VideoCapture cameraCaptureEnrol;
        private Rectangle[] facesFromFrame;
        int count = 0;

        public static Mat CleanFrame { get; set; }
        public static Rectangle[] AllFaces { get; set; }

        // singleton instance for user control class
        private static ucEnrolPage instance;
        public static ucEnrolPage Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ucEnrolPage();
                }
                return instance;
            }
        }

        public ucEnrolPage()
        {
            InitializeComponent();

            // Initialize out other Variables
            CleanFrame = new Mat();
            
            // starts the camera feed when constructed
            cameraCaptureEnrol = new VideoCapture();
            cameraCaptureEnrol.ImageGrabbed += CameraCapture_ImageGrabbed;
            cameraCaptureEnrol.Start();
        }

        public void EnrolPageStop()
        {
            if (cameraCaptureEnrol != null)
            {
                // Stopping the camera capturing frames and reset image to null
                cameraCaptureEnrol.Stop();
                pb_FaceOutput.Image = null;
            }
        }

        public void EnrolPageStart()
        {
            // starting the camera capture again
            cameraCaptureEnrol.Start();

            // setting the buttons to default state
            btn_capture.Enabled = true;
            btn_EnrolUser.Enabled = false;
            btn_NextFace.Enabled = false;
            btn_PrevFace.Enabled = false;
        }

        private void CameraCapture_ImageGrabbed(object sender, EventArgs e)
        {
            // Getting the image from the camera
            Mat capturedImage = new Mat();
            cameraCaptureEnrol.Retrieve(capturedImage);
            var convertedCapture = capturedImage.ToImage<Bgr, byte>();

            // setting the Image
            pb_CameraFeed.Image = BackendGuiUtility.DetectFaces(convertedCapture);
        }

        private void btn_capture_Click(object sender, EventArgs e)
        {
            // only want to update the faces from the frame when captured
            cameraCaptureEnrol.Retrieve(CleanFrame);
            count = 0;
            facesFromFrame = AllFaces;
            FaceDisplayCoordinator(count);
        }

        private void btn_EnrolUser_Click(object sender, EventArgs e)
        {
            // Gets the detected face from the camera feed
            var faceImage = BackendGuiUtility.getFaceFromFeed(count, facesFromFrame);

            BackendGuiUtility.SaveUserImage(faceImage, tBox_UserName.Text);
        }

        private void btn_NextFace_Click(object sender, EventArgs e)
        {
            // all we want to do here is increment the count number and call the coordinator
            count++;

            // extra safety check here to make sure we don't go out of bounds
            if (count == facesFromFrame.Length)
            {
                count = 0;
            }

            FaceDisplayCoordinator(count);
        }

        private void btn_PrevFace_Click(object sender, EventArgs e)
        {
            // all we want to do here is decrement the count number and call the coordinator
            count--;

            // extra safety check here to make sure we don't go out of bounds
            if (count < 0)
            {
                count = facesFromFrame.Length - 1;
            }

            FaceDisplayCoordinator(count);
        }

        private void FaceDisplayCoordinator(int count)
        {
            switch (facesFromFrame.Length)
            {
                // check for 0 faces detected
                case 0:
                    // error showing no faces detected
                    MessageBox.Show("No faces were detected", "Error When Capturing Frame", MessageBoxButtons.OK);
                    break;
                case 1:
                    // no need to enable the next/prev buttons
                    btn_EnrolUser.Enabled = true;
                    tBox_UserName.Enabled = true;
                    // but will disable in case they are left on
                    btn_NextFace.Enabled = false;
                    btn_PrevFace.Enabled = false;

                    pb_FaceOutput.Image = BackendGuiUtility.getFaceFromFeed(0, facesFromFrame);
                    break;
                default:
                    // enable the next/prev buttons for choosing the face
                    btn_EnrolUser.Enabled = true;
                    tBox_UserName.Enabled = true;
                    btn_NextFace.Enabled = true;
                    btn_PrevFace.Enabled = true;

                    pb_FaceOutput.Image = BackendGuiUtility.getFaceFromFeed(count, facesFromFrame);
                    break;
            }
        }
    }
}
