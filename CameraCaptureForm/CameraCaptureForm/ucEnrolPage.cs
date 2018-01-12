using Emgu.CV;
using Emgu.CV.Structure;
using FaceAuthenticators;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CameraCaptureForm
{
    public partial class ucEnrolPage : UserControl
    {
        // Declaring constants
        static string haarFaceFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "haarcascade_frontalface_default.xml");

        static string enrolLocation = @"C:\Users\RockInTheBox\Documents\University\Project\TestEnrolLocation";

        VideoCapture cameraCaptureEnrol;
        Mat cleanFrame = new Mat();
        Rectangle[] allFaces;
        Rectangle[] facesFromFrame;
        int count = 0;

        // Creating classifiers here
        CascadeClassifier faceClassifier = new CascadeClassifier(haarFaceFile);

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

            // Face detection
            var greyImage = convertedCapture.Convert<Gray, byte>();
            allFaces = faceClassifier.DetectMultiScale(greyImage, 1.1, 10);

            foreach (var face in allFaces)
            {
                convertedCapture.Draw(face, new Bgr(Color.Green), 2);
            }

            pb_CameraFeed.Image = convertedCapture.Bitmap;
        }

        private void btn_capture_Click(object sender, EventArgs e)
        {
            // only want to update the faces from the frame when captured
            cameraCaptureEnrol.Retrieve(cleanFrame);
            count = 0;
            facesFromFrame = allFaces;
            faceDisplayCoordinator(count);
        }

        private void btn_EnrolUser_Click(object sender, EventArgs e)
        {
            var faceImage = getFaceFromFeed(count, facesFromFrame);
            //faceImage.SetResolution(EigenfaceAuthenticator.imageWidth, EigenfaceAuthenticator.imageHeight);
            //faceImage.Save(Path.Combine(enrolLocation, tBox_UserName.Text + ".jpg"));
            
            Image<Bgr, byte> userToEnrol = new Image<Bgr, byte>(RecognizerUtility.imageWidth, RecognizerUtility.imageHeight)
            {
                Bitmap = faceImage
            };
            
            //userToEnrol.Resize(EigenfaceAuthenticator.imageWidth, EigenfaceAuthenticator.imageHeight, Emgu.CV.CvEnum.Inter.Linear);
            userToEnrol.Save(Path.Combine(enrolLocation, tBox_UserName.Text + ".jpg"));
        }

        private void btn_NextFace_Click(object sender, EventArgs e)
        {
            // all we want to do here is increment the count number and call the coordinator
            count++;
            if(count == facesFromFrame.Length)
            {
                count = 0;
            }
            faceDisplayCoordinator(count);
        }

        private void btn_PrevFace_Click(object sender, EventArgs e)
        {
            // all we want to do here is decrement the count number and call the coordinator
            count--;
            if (count < 0)
            {
                count = facesFromFrame.Length - 1;
            }
            faceDisplayCoordinator(count);
        }

        private void faceDisplayCoordinator(int count)
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

                    pb_FaceOutput.Image = getFaceFromFeed(0, facesFromFrame);
                    break;
                default:
                    // enable the next/prev buttons for choosing the face
                    btn_EnrolUser.Enabled = true;
                    tBox_UserName.Enabled = true;
                    btn_NextFace.Enabled = true;
                    btn_PrevFace.Enabled = true;

                    pb_FaceOutput.Image = getFaceFromFeed(count, facesFromFrame);
                    break;
            }
        }

        private Bitmap getFaceFromFeed(int faceIndex, Rectangle[] facesToDisplay)
        {
            // have to do a new retrieve that hasn't had the rectangle drawn on it
            Bitmap faceImage = new Bitmap(cleanFrame.Bitmap);

            // create copy as we are altering properties of it that we don't want in the master copy
            var areaOfFace = facesToDisplay[faceIndex];

            // these changes are here because the face detector doesn't capture the entire face correctly
            // all these do is add extra padding to height and width to ensure the entire face is captured
            areaOfFace.X -= 25;
            areaOfFace.Y -= 50;
            areaOfFace.Height = RecognizerUtility.imageHeight;
            areaOfFace.Width = RecognizerUtility.imageWidth;
            
            return faceImage.Clone(areaOfFace, faceImage.PixelFormat);
        }
    }
}
