using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CameraCaptureForm
{
    public partial class ucMainPage : UserControl
    {
        VideoCapture cameraCaptureMain;

        // singleton instance for user control class
        private static ucMainPage instance;

        public static ucMainPage Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new ucMainPage();
                }
                return instance;
            }
        }
        
        public ucMainPage()
        {
            InitializeComponent();
            BackendGuiUtility.eigenRecognizer.TrainAuthenticator();
            pb_CameraFeed.InitialImage = null;

            // adding the authenticators to the combo box
            cbox_AuthSelector.Items.Add(BackendGuiUtility.eigenRecognizer);
            cbox_AuthSelector.Items.Add(new EigenFaceRecognizer());
            cbox_AuthSelector.Items.Add(new FisherFaceRecognizer());
            cbox_AuthSelector.Items.Add(new LBPHFaceRecognizer());
        }

        public void MainPageReset()
        {
            if(cameraCaptureMain != null)
            {
                cameraCaptureMain.Stop();

                pb_CameraFeed.Image = new Bitmap(pb_CameraFeed.ClientSize.Height, pb_CameraFeed.ClientSize.Width);

                btn_Start.Enabled = true;
                btn_Stop.Enabled = false;
            }
        }

        private void btn_Start_Click(object sender, EventArgs e)
        {
            // create cameraCapture object if not already created
            if (cameraCaptureMain == null)
            {
                cameraCaptureMain = new VideoCapture();

            }

            // sets the function that captures the camera feed and starts the capture
            cameraCaptureMain.ImageGrabbed += CameraCapture_ImageGrabbed;
            cameraCaptureMain.Start();

            btn_Start.Enabled = false;
            btn_Stop.Enabled = true;
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            MainPageReset();
        }

        private void btn_ChooseAuth_Click(object sender, EventArgs e)
        {
            BackendGuiUtility.SetAuthenticator(cbox_AuthSelector.SelectedItem);
        }

        private void CameraCapture_ImageGrabbed(object sender, EventArgs e)
        {
            // Getting the image from the camera
            Mat capturedImage = new Mat();
            cameraCaptureMain.Retrieve(capturedImage);
            var convertedCapture = capturedImage.ToImage<Bgr, byte>();

            // This is where it detects and predicts the faces
            convertedCapture = BackendGuiUtility.DetectAndPredictFaces(convertedCapture);

            // sets the camera output as the image
            pb_CameraFeed.Image = convertedCapture.Bitmap;
        }
    }
}
