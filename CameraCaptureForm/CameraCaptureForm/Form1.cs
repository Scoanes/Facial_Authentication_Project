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
        // Declaring constants
        static string haarFaceFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "haarcascade_frontalface_default.xml");
        static string haarEyeFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "haarcascade_eye.xml");

        VideoCapture cameraCapture;

        // Creating classifiers here
        CascadeClassifier faceClassifier = new CascadeClassifier(haarFaceFile);
        CascadeClassifier eyeClassifier = new CascadeClassifier(haarEyeFile);

        public Form1()
        {
            InitializeComponent();
        }

        private void btn_StartCapture_Click(object sender, EventArgs e)
        {
            if(cameraCapture == null)
            {
                cameraCapture = new VideoCapture();
            }

            cameraCapture.ImageGrabbed += CameraCapture_ImageGrabbed;
            cameraCapture.Start();
        }

        private void CameraCapture_ImageGrabbed(object sender, EventArgs e)
        {
            // Getting the image from the camera
            Mat capturedImage = new Mat();
            cameraCapture.Retrieve(capturedImage);
            var convertedCapture = capturedImage.ToImage<Bgr, byte>();

            // Face detection
            var greyImage = convertedCapture.Convert<Gray, byte>();
            var allFaces = faceClassifier.DetectMultiScale(greyImage, 1.1, 10);
            var allEyes = eyeClassifier.DetectMultiScale(greyImage, 1.1, 10);

            foreach(var face in allFaces)
            {
                convertedCapture.Draw(face, new Bgr(Color.Green), 2);
            }

            foreach (var eye in allEyes)
            {
                convertedCapture.Draw(eye, new Bgr(Color.Purple), 2);
            }

            pictureBox1.Image = convertedCapture.Bitmap;
        }

        private void btn_StopCapture_Click(object sender, EventArgs e)
        {
            if(cameraCapture != null)
            {
                cameraCapture = null;
            }
        }
    }
}
