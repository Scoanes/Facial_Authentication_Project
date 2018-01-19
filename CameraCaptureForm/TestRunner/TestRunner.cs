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
using System.Linq;

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

        // Stopwatch metrics
        private static TimeSpan trainingTime;
        private static TimeSpan testingTime;

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
            //TestUtility.PgmToJpgConversion(@"C:\Users\RockInTheBox\Documents\University\Project\DatabaseImages\Yale_pgm", @"C:\Users\RockInTheBox\Documents\University\Project\DatabaseImages\Yale_jpg");
            //TestUtility.TrimImagesToSize(testImagesRootFolder, trimmedTestImagesRootFolder);
            KFoldParamterTesting(trimmedTestImagesRootFolder, 10, 0.95);

            // here we get the images and labels for the training and testing, already spit randomly
            GetAllTrainingAndTestData(trimmedTestImagesRootFolder, ref trainingImages, ref trainingLabels, ref testImages, ref testLabels);

            // need to convert both of these into a datatype the Train method accepts
            var vectorOfTrainingImages = new VectorOfMat(trainingImages.ToArray());
            var vectorOfTrainingLabels = new VectorOfInt(trainingLabels.ToArray());

            // call general function that deals with the testing of each of the FaceRecognizer sub classes
            TrainAndTestRecognizer(eigenfaceRecognizer, vectorOfTrainingImages, vectorOfTrainingLabels, testImages, testLabels);
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
        
        private static void TrainAndTestRecognizer(FaceRecognizer recognizer, VectorOfMat trainImages, VectorOfInt trainLabels, List<Mat> testImages, List<int> testLabels)
        {
            // Train the recogniser on the images, and time it using the stopwatch class
            Stopwatch trainingTimer = new Stopwatch();

            trainingTimer.Start();
            recognizer.Train(trainImages, trainLabels);
            trainingTimer.Stop();

            trainingTime = trainingTimer.Elapsed;

            // need to convert test images into a format the predict method accepts
            var vectorOfTestImages = new VectorOfMat(testImages.ToArray());

            GetTestResults(recognizer, vectorOfTestImages, testLabels);
        }

        private static void GetTestResults(FaceRecognizer recognizer, VectorOfMat vectorOfTestImages, List<int> testLabels)
        {
            // setting up our counters for correct and incorrect predictions
            int correctAmount = 0, incorrectAmount = 0, totalTestImages = testLabels.Count;
            Stopwatch testingTimer = new Stopwatch();

            // Test the Recognizer
            for (int i = 0; i < vectorOfTestImages.Size; i++)
            {
                // get our predicted result
                testingTimer.Start();
                var preictionResult = recognizer.Predict(vectorOfTestImages[i]);
                testingTimer.Stop();

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
            testingTime = testingTimer.Elapsed;

            OutputResultsToDisk(recognizer.ToString(), correctAmount, incorrectAmount, totalTestImages);
        }

        private static void OutputResultsToDisk(string recognizerName, int correctAmount, int incorrectAmount, int totalTestImages)
        {
            // calculate percentages of correct/incorrect predictions
            double correctPercentage = (correctAmount / totalTestImages) * 100;
            double incorrectPercentage = (incorrectAmount / totalTestImages) * 100;

            // output results to a file on disk
            string fileText = "Test results for recognizer: " + recognizerName + Environment.NewLine;
            fileText += "Test results on the following directory of test images: " + testImagesRootFolder + Environment.NewLine;
            fileText += "Total number of images in directory: " + (numOfDifferentTestPeople * numOfImagesPerPerson) +
                " Spread across " + numOfDifferentTestPeople + " different test subjects" + Environment.NewLine;
            fileText += "Total number of training images used for each person: " + numOfTrainImagesPerPerson + Environment.NewLine;
            fileText += "Total number of test images used for each person: " + numOfTestImagesPerPerson + Environment.NewLine;
            fileText += "---------- TEST RESULTS ----------" + Environment.NewLine;
            fileText += "Total elapsed time for training the recogniser: " + trainingTime + Environment.NewLine;
            fileText += "Total elapsed time for predicting all test images: " + testingTime + Environment.NewLine;
            fileText += "Total Correct: " + correctAmount + " (" + correctPercentage + "%)" + Environment.NewLine;
            fileText += "Total Incorrect: " + incorrectAmount + " (" + incorrectPercentage + "%)" + Environment.NewLine;

            File.WriteAllText(Path.Combine(testImagesRootFolder, "testResults.txt"), fileText);
        }

        public static void KFoldParamterTesting(string dataRootLocation, int numberOfKFolds, double percentParameterValue)
        {
            // create our authenticator, with the parameter value
            EigenfaceAuthenticator eigenfaceAuth = new EigenfaceAuthenticator(percentParameterValue);

            var masterImageList = new List<Image<Gray, byte>>();
            var masterTestLabelsList = new List<string>();

            // Get all of the test data
            foreach(var imageFile in Directory.GetFiles(dataRootLocation, "*.jpg", SearchOption.AllDirectories))
            {
                masterImageList.Add(new Image<Gray, byte>(imageFile));
                masterTestLabelsList.Add(Path.GetFileName(Path.GetDirectoryName(imageFile)));
            }

            // randomise the order of the lists
            var indexes = Enumerable.Range(0, masterImageList.Count).ToList();
            TestUtility.ShuffleList(ref indexes);
            var randomizedImageList = indexes.Select(index => masterImageList[index]).ToList();
            var randomizedLabelList = indexes.Select(index => masterTestLabelsList[index]).ToList();

            var kFoldedLabels = new List<string>[numberOfKFolds];

            var kFoldedLists = TestUtility.GetKFold(randomizedImageList, randomizedLabelList,  ref kFoldedLabels, numberOfKFolds);

            int totalCorrect = 0, totalIncorrect = 0;

            for (int kFoldIter = 0; kFoldIter < numberOfKFolds; kFoldIter++)
            {
                var testingData = kFoldedLists[kFoldIter];
                var testingLabels = kFoldedLabels[kFoldIter];
                var trainingData = new List<Image<Gray, byte>>();
                var trainingLabels = new List<string>();

                // populate the training data list
                for(int totalFolds = 0; totalFolds < numberOfKFolds; totalFolds++)
                {
                    // ensure that the testing data is not added to the list
                    if(kFoldIter != totalFolds)
                    {
                        trainingData.AddRange(kFoldedLists[totalFolds]);
                        trainingLabels.AddRange(kFoldedLabels[totalFolds]);
                    }
                }

                // train the authenticator with the training data
                eigenfaceAuth.TrainEigenfaceAuthenticator(trainingData, trainingLabels);

                // test the authenticator with the test data
                for(int testIter = 0; testIter < testingData.Count; testIter++)
                {
                    if (testingLabels[testIter].Equals(eigenfaceAuth.PredictImage(testingData[testIter]))){
                        totalCorrect++;
                    }
                    else
                    {
                        totalIncorrect++;
                    }
                }
            }
        }
    }
}
