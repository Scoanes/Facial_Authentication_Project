using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using FaceAuthenticators;

namespace CameraCaptureForm
{
    public partial class ucMainPage : UserControl
    {
        // Declaring constants
        static string haarFaceFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "haarcascade_frontalface_default.xml");
        static string haarEyeFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "haarcascade_eye.xml");

        VideoCapture cameraCaptureMain;

        // Creating classifiers here
        CascadeClassifier faceClassifier = new CascadeClassifier(haarFaceFile);
        //CascadeClassifier eyeClassifier = new CascadeClassifier(haarEyeFile);

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
            EigenfaceAuthenticator.TrainEigenfaceAuthenticator();
            pb_CameraFeed.InitialImage = null;
        }

        public void mainPageReset()
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
            mainPageReset();
        }

        private void CameraCapture_ImageGrabbed(object sender, EventArgs e)
        {
            // Getting the image from the camera
            Mat capturedImage = new Mat();
            cameraCaptureMain.Retrieve(capturedImage);
            var convertedCapture = capturedImage.ToImage<Bgr, byte>();

            // Face detection
            var greyImage = convertedCapture.Convert<Gray, byte>();
            var allFaces = faceClassifier.DetectMultiScale(greyImage, 1.1, 10);
            //var allEyes = eyeClassifier.DetectMultiScale(greyImage, 1.1, 10);

            foreach (var face in allFaces)
            {
                convertedCapture.Draw(face, new Bgr(Color.Green), 2);
            }
            /*
            foreach (var eye in allEyes)
            {
                convertedCapture.Draw(eye, new Bgr(Color.Purple), 2);
            }
            */

            // sets the camera output as the image
            pb_CameraFeed.Image = convertedCapture.Bitmap;
        }
    }
}
