using CameraCaptureForm;
using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using FaceAuthenticators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace TestRunner
{
    public class TestRunner
    {
        private static string testImagesRootFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestImages");
        private static string trimmedTestImagesRootFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TrimmedTestImages");
        private static int numOfTestImagesPerPerson = 5;
        private static int numOfImagesPerPerson = 15;
        private static int numOfTrainImagesPerPerson = numOfImagesPerPerson - numOfTestImagesPerPerson;
        private static int numOfDifferentTestPeople = Directory.GetDirectories(testImagesRootFolder).Length;

        static void Main()
        {
            // create the recogniser objects
            FaceRecognizer eigenfaceRecognizer = new EigenFaceRecognizer();

            // Declare training and testing images/labels
            var trainingImages = new List<Mat>();
            var trainingLabels = new List<int>();
            var testImages = new List<Mat>();
            var testLabels = new List<int>();

            // TEMP!
            TrimImagesToSize();

            // here we get the images and labels for the training and testing, already spit randomly
            GetAllTrainingAndTestData(testImagesRootFolder, ref trainingImages, ref trainingLabels, ref testImages, ref testLabels);

            // need to convert both of these into a datatype the Train method accepts
            var vectorOfTrainingImages = new VectorOfMat(trainingImages.ToArray());
            var vectorOfTrainingLabels = new VectorOfInt(trainingLabels.ToArray());

            // call general function that deals with the testing of each of the FaceRecognizer sub classes
            TestRecognizer(eigenfaceRecognizer, vectorOfTrainingImages, vectorOfTrainingLabels, testImages, testLabels);
        }

        private static void GetAllTrainingAndTestData(string rootFolderLocation, ref List<Mat> trainImages, ref List<int> trainLabels, ref List<Mat> testImages, ref List<int> testLabels)
        {
            // create our randomizer
            Random randomizer = new Random();
            List<int> testIndexValues = new List<int>();

            // create an array of index's to be removed
            while(testIndexValues.Count < numOfTestImagesPerPerson)
            {
                int numToAdd = randomizer.Next(0, numOfImagesPerPerson);
                if (!testIndexValues.Contains(numToAdd))
                {
                    testIndexValues.Add(numToAdd);
                }
            }

            // For future work - Include sub directories here for each of the different test cases stated in test plan
            // E.G: Illumination, Pose, Scale, etc...
            // Use these subclasses to define each set of tests with the different test candidates being set in each of
            // these subclasses, should be passed as an argument for the method

            // go through top level directory for each test person
            foreach (var directory in Directory.GetDirectories(rootFolderLocation))
            {
                int iter = 0;
                foreach (var imageFile in Directory.GetFiles(directory))
                {
                    if (testIndexValues.Contains(iter))
                    {
                        testImages.Add(new Image<Gray, byte>(imageFile).Mat);
                        testLabels.Add(Convert.ToInt32(Path.GetFileName(Path.GetDirectoryName(imageFile))));
                    }
                    else
                    {
                        trainImages.Add(new Image<Gray, byte>(imageFile).Mat);
                        trainLabels.Add(Convert.ToInt32(Path.GetFileName(Path.GetDirectoryName(imageFile))));
                    }

                    iter++;
                }
            }
        }
        
        private static void TestRecognizer(FaceRecognizer recognizer, VectorOfMat trainImages, VectorOfInt trainLabels, List<Mat> testImages, List<int> testLabels)
        {
            // Train the recogniser on the images, and time it using the stopwatch class
            Stopwatch trainingTimer = new Stopwatch();
            trainingTimer.Start();
            recognizer.Train(trainImages, trainLabels);
            trainingTimer.Stop();

            // need to convert test images into a format the predict method accepts
            var vectorOfTestImages = new VectorOfMat(testImages.ToArray());

            GetTestResults(recognizer, vectorOfTestImages, testLabels);
        }

        private static void GetTestResults(FaceRecognizer recognizer, VectorOfMat vectorOfTestImages, List<int> testLabels)
        {
            // setting up our counters for correct and incorrect predictions
            int correctAmount = 0, incorrectAmount = 0, totalTestImages = testLabels.Count;

            // Test the Recognizer
            for (int i = 0; i < vectorOfTestImages.Size; i++)
            {
                // get our predicted result
                var preictionResult = recognizer.Predict(vectorOfTestImages[i]);

                // if prediction is correct
                if (preictionResult.Label == testLabels[i])
                {
                    correctAmount++;
                }
                else
                {
                    incorrectAmount++;
                }
            }

            // calculate percentages of correct/incorrect predictions
            double correctPercentage = (correctAmount / totalTestImages) * 100;
            double incorrectPercentage = (incorrectAmount / totalTestImages) * 100;

            // output results to a file on disk
            string fileText = "Test results for recognizer: " + recognizer.ToString() + Environment.NewLine;
            fileText += "Test results on the following directory of test images: " + testImagesRootFolder + Environment.NewLine;
            fileText += "Total number of images in directory: " + (numOfDifferentTestPeople * numOfImagesPerPerson) +
                " Spread across " + numOfDifferentTestPeople + " different test subjects" + Environment.NewLine;
            fileText += "Total number of training images used for each person: " + numOfTrainImagesPerPerson + Environment.NewLine;
            fileText += "Total number of test images used for each person: " + numOfTestImagesPerPerson + Environment.NewLine;
            fileText += "---------- TEST RESULTS ----------" + Environment.NewLine;
            fileText += "Total Correct: " + correctAmount + " (" + correctPercentage + "%)" + Environment.NewLine;
            fileText += "Total Incorrect: " + incorrectAmount + " (" + incorrectPercentage + "%)" + Environment.NewLine;

            File.WriteAllText(Path.Combine(testImagesRootFolder, "testResults.txt"), fileText);
        }

        private static void TrimImagesToSize()
        {
            // This function is only used when trimming images to the same dimension
            // we will assume that each face in the images are centered

            string haarFaceFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "haarcascade_frontalface_default.xml");
            CascadeClassifier faceClassifier = new CascadeClassifier(haarFaceFile);
            int xPoint = 0, yPoint = 0;
            Mat imageFileRoi;

            foreach (var file in Directory.GetFiles(testImagesRootFolder, "*.jpg", SearchOption.AllDirectories))
            {
                // Create our image file
                var imageFile = new Mat(file);

                // get folder name and file name to preserve the structure of the images
                var directoryName = Path.GetFileName(Path.GetDirectoryName(file));
                var fileName = Path.GetFileName(file);

                // See if we can detect any faces to get a more accurate face for testing
                var greyImage = imageFile.ToImage<Gray, byte>();
                var allFaces = faceClassifier.DetectMultiScale(greyImage, 1.1, 10);

                // if face detected
                if(allFaces.Length == 1)
                {
                    xPoint = allFaces[0].X - 50;
                    yPoint = allFaces[0].Y - 75;
                }
                else
                {
                    // Get centre point and offset the new area by midway point plus half of width and half of height
                    yPoint = (imageFile.Rows / 2) - (RecognizerUtility.imageHeight / 2);
                    xPoint = (imageFile.Cols / 2) - (RecognizerUtility.imageWidth / 2);
                }

                // create ROI of new area
                var trimmedArea = new Rectangle(xPoint, yPoint, RecognizerUtility.imageWidth, RecognizerUtility.imageHeight);

                // Introduced so if the ROI is not good for image will default to centre of image
                try
                {
                    imageFileRoi = new Mat(imageFile, trimmedArea);
                }
                catch
                {
                    yPoint = (imageFile.Rows / 2) - (RecognizerUtility.imageHeight / 2);
                    xPoint = (imageFile.Cols / 2) - (RecognizerUtility.imageWidth / 2);
                    trimmedArea.X = xPoint;
                    trimmedArea.Y = yPoint;
                    imageFileRoi = new Mat(imageFile, trimmedArea);
                }
                

                // before saving need to check directory exists and if not create it
                if(!Directory.Exists(Path.Combine(trimmedTestImagesRootFolder, directoryName)))
                {
                    Directory.CreateDirectory(Path.Combine(trimmedTestImagesRootFolder, directoryName));
                }

                // save the ROI
                imageFileRoi.Save(Path.Combine(trimmedTestImagesRootFolder, directoryName, fileName));
            }
        }
    }
}
