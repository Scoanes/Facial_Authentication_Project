using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using FaceAuthenticators;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace CameraCaptureForm
{
    public class BackendGuiUtility
    {
        // Declaring constants
        public static string haarFaceFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "haarcascade_frontalface_default.xml");
        public static EigenfaceAuthenticator eigenRecognizer = new EigenfaceAuthenticator();

        // Define our authenticators here
        private static bool isOwnAuthenticator;
        private static FaceRecognizer emguAuthenticator;
        private static IOwnAuthenticators ownAuthenticator;

        // Declare these for the Emgu authenticators
        private static VectorOfMat faceVector;
        private static List<string> faceLabels;
        private static VectorOfInt indexLocations;

        // Creating classifiers here
        static CascadeClassifier faceClassifier = new CascadeClassifier(haarFaceFile);

        public static Bitmap GetFaceFromFeed(int faceIndex, Rectangle[] facesToDisplay)
        {
            // have to do a new retrieve that hasn't had the rectangle drawn on it
            Bitmap faceImage = new Bitmap(ucEnrolPage.CleanFrame.Bitmap);

            // create copy as we are altering properties of it that we don't want in the master copy
            var areaOfFace = facesToDisplay[faceIndex];
            areaOfFace = ReshapeRectangle(areaOfFace);

            return faceImage.Clone(areaOfFace, faceImage.PixelFormat);
        }

        public static Rectangle ReshapeRectangle(Rectangle areaOfFace)
        {
            // these changes are here because the face detector doesn't capture the entire face correctly
            // all these do is add extra padding to height and width to ensure the entire face is captured
            areaOfFace.X -= 25;
            areaOfFace.Y -= 50;
            areaOfFace.Height = RecognizerUtility.imageHeight;
            areaOfFace.Width = RecognizerUtility.imageWidth;

            return areaOfFace;
        }

        public static Image<Bgr, byte> DetectAndPredictFaces(Image<Bgr, byte> cameraCapture)
        {
            // Face detection
            var greyImage = cameraCapture.Convert<Gray, byte>();
            var allFaces = faceClassifier.DetectMultiScale(greyImage, 1.1, 10);

            for (int i = 0; i < allFaces.Length; i++)
            {
                cameraCapture.Draw(allFaces[i], new Bgr(Color.Green), 2);

                // update the rectange to fir the accepted size
                allFaces[i] = ReshapeRectangle(allFaces[i]);

                // Copy the image from the feed into the recognizer and get the predicted name
                cameraCapture.ROI = allFaces[i];
                var nameToDisplay = GetAuthenticatorPrediction(cameraCapture.Clone().Convert<Gray, byte>());
                CvInvoke.cvResetImageROI(cameraCapture);

                // Display the name of the prediction
                using (Graphics graphics = Graphics.FromImage(cameraCapture.Bitmap))
                {
                    using (Font arialFont = new Font("Arial", 10))
                    {
                        graphics.DrawString(nameToDisplay, arialFont, Brushes.Blue, allFaces[i].X + 25, allFaces[i].Y + 25);
                    }
                }
            }

            return cameraCapture;
        }

        public static Bitmap DetectFaces(Image<Bgr, byte> cameraCapture)
        {
            // Face detection
            var greyImage = cameraCapture.Convert<Gray, byte>();
            ucEnrolPage.AllFaces = faceClassifier.DetectMultiScale(greyImage, 1.1, 10);

            foreach (var face in ucEnrolPage.AllFaces)
            {
                cameraCapture.Draw(face, new Bgr(Color.Green), 2);
            }

            return cameraCapture.Bitmap;
        }

        public static void SaveUserImage(Bitmap userImage, string userName)
        {
            // Creates the image to be saved to disk
            Image<Bgr, byte> userToEnrol = new Image<Bgr, byte>(RecognizerUtility.imageWidth, RecognizerUtility.imageHeight)
            {
                Bitmap = userImage
            };

            var filePath = Path.Combine(RecognizerUtility.rootEnrolImagesFolder, userName.ToLower());

            // check to see if the user already has a folder, creates one if not
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            // Get the current number of images in the directory
            var currentCount = Directory.GetFiles(filePath).Length;
            currentCount++;

            // Finally, save the image
            userToEnrol.Save(Path.Combine(filePath, currentCount + ".jpg"));
        }

        public static void SetAndTrainAuthenticator(object authenticator)
        {
            // if the object being passed is emgu
            if (authenticator.ToString().Contains("Emgu"))
            {
                isOwnAuthenticator = false;
                emguAuthenticator = (FaceRecognizer) authenticator;

                var totalImages = RecognizerUtility.GetTrainingImagesAmount();

                // assinging size here for sake of speed
                VectorOfMat faceVector = new VectorOfMat(totalImages);
                List<string> faceLabels = new List<string>(totalImages);
                VectorOfInt indexLocations = new VectorOfInt(totalImages);

                RecognizerUtility.GetAllImageVectorsAndLabels(RecognizerUtility.rootEnrolImagesFolder, ref faceVector, ref faceLabels, ref indexLocations);

                // train the authenticator
                emguAuthenticator.Train(faceVector, indexLocations);
            }
            // if the authenticator is our own type
            else
            {
                // cast to our own type here for use
                isOwnAuthenticator = true;
                ownAuthenticator = (IOwnAuthenticators) authenticator;

                // train the authenticator
                ownAuthenticator.TrainAuthenticator();
            }
        }

        private static string GetAuthenticatorPrediction(Image<Gray, byte> faceImage)
        {
            if (isOwnAuthenticator)
            {
                // much simpler
                return ownAuthenticator.PredictImage(faceImage);
            }
            else
            {
                // have to convert to a list of Mat, to then convert to VectorOfMat datatype...
                Mat[] convertedImage = new Mat[]
                {
                    faceImage.Mat
                };
                var imageVector = new VectorOfMat(convertedImage);

                // and even then, prediction does not support string datatype, so will only be int, signifying the index value
                var indexValue = emguAuthenticator.Predict(imageVector);

                return faceLabels[indexValue.Label];
            }
        }
    }
}
