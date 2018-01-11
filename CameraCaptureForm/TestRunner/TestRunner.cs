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
        private static int numOfTestImagesPerPerson = 2;
        private static int numOfImagesPerPerson = 15;
        private static int numOfTrainImagesPerPerson = numOfImagesPerPerson - numOfTestImagesPerPerson;
        private static int numOfDifferentTestPeople = Directory.GetDirectories(testImagesRootFolder).Length;

        static void Main()
        {
            // create our randomizer
            Random randomizer = new Random();
            int[] elementsToRemove = new int[numOfTestImagesPerPerson];

            // create an array of index's to be removed
            for (int i = 0; i < numOfTestImagesPerPerson; i++)
            {
                elementsToRemove[i] = randomizer.Next(0, numOfImagesPerPerson);
            }

            // create the recogniser objects
            FaceRecognizer eigenfaceRecognizer = new EigenFaceRecognizer();

            // get the training and test data
            var trainingImages = new List<Mat>();
            var trainingLabels = new List<int>();
            var testImages = new List<Mat>();
            var testLabels = new List<int>();

            GetAllTrainingAndTestData(testImagesRootFolder, elementsToRemove, ref trainingImages, ref trainingLabels, ref testImages, ref testLabels);

            // need to convert both of these into a datatype the Train method accepts
            var vectorOfTrainingImages = new VectorOfMat(trainingImages.ToArray());
            var vectorOfTrainingLabels = new VectorOfInt(trainingLabels.ToArray());

            // Train the recogniser on the images, and time it using the stopwatch class
            Stopwatch trainingTimer = new Stopwatch();
            trainingTimer.Start();
            eigenfaceRecognizer.Train(vectorOfTrainingImages, vectorOfTrainingLabels);
            trainingTimer.Stop();

            // need to convert test images into a format the predict method accepts
            var vectorOfTestImages = new VectorOfMat(testImages.ToArray());

            GetTestResults(eigenfaceRecognizer, vectorOfTestImages, testLabels);
        }

        private static void GetAllTrainingAndTestData(string rootFolderLocation, int[] testIndexValues, ref List<Mat> trainImages, ref List<int> trainLabels, ref List<Mat> testImages, ref List<int> testLabels)
        {
            // go through top level directory for each test person
            foreach(var directory in Directory.GetDirectories(rootFolderLocation))
            {
                int iter = 0;
                foreach (var imageFile in Directory.GetFiles(directory))
                {
                    if (iter == testIndexValues[0])
                    {
                        testImages.Add(new Image<Gray, byte>(imageFile).Mat);
                        testLabels.Add(Convert.ToInt32(Path.GetFileName(Path.GetDirectoryName(imageFile))));
                        //testIndexValues[0] += numOfTrainImagesPerPerson;
                    }
                    else if (iter == testIndexValues[1])
                    {
                        testImages.Add(new Image<Gray, byte>(imageFile).Mat);
                        testLabels.Add(Convert.ToInt32(Path.GetFileName(Path.GetDirectoryName(imageFile))));
                        //testIndexValues[1] += numOfTrainImagesPerPerson;
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
