using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace TestRunner
{
    public class TestRunner
    {
        private static string testImagesRootFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestImages");
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
            int[] testIndexValues = new int[numOfTestImagesPerPerson];

            // create an array of index's to be removed
            for (int i = 0; i < numOfTestImagesPerPerson; i++)
            {
                testIndexValues[i] = randomizer.Next(0, numOfImagesPerPerson);
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
                    if (iter == testIndexValues[0])
                    {
                        testImages.Add(new Image<Gray, byte>(imageFile).Mat);
                        testLabels.Add(Convert.ToInt32(Path.GetFileName(Path.GetDirectoryName(imageFile))));
                    }
                    else if (iter == testIndexValues[1])
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
    }
}
