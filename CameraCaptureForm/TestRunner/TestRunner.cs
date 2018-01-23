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
        private static int numOfTestImagesPerPerson = 5;
        private static int numOfImagesPerPerson = 15;
        //private static int numOfTrainImagesPerPerson = numOfImagesPerPerson - numOfTestImagesPerPerson;
        //private static int numOfDifferentTestPeople = Directory.GetDirectories(testImagesRootFolder).Length;

        // Stopwatch times for training/testing
        private static TimeSpan trainingTime;
        private static TimeSpan testingTime;

        static void Main()
        {
            // create the recogniser objects
            FaceRecognizer eigenfaceRecognizer = new EigenFaceRecognizer();

            TestCaseRunner(eigenfaceRecognizer, "Pose");

            // TEMP!
            //KFoldParamterTesting(trimmedTestImagesRootFolder, 10, 0.95);
        }

        // Takes the root directory of the test case, defining what test case it will run
        public static void TestCaseRunner(FaceRecognizer recognizer, string testCase)
        {
            // Declare the image location by using the testCase name
            var testCaseRootFolder = Path.Combine(testImagesRootFolder, testCase);

            // Declare training and testing images/labels
            var trainingImages = new List<Mat>();
            var trainingLabels = new List<int>();
            var testImages = new List<Mat>();
            var testLabels = new List<int>();

            // here we get the images and labels for the training and testing, already spit randomly
            GetAllTrainingAndTestData(testCaseRootFolder, ref trainingImages, ref trainingLabels, ref testImages, ref testLabels, "Training", "Testing");

            // need to convert both of these into a datatype the Train method accepts
            var vectorOfTrainingImages = new VectorOfMat(trainingImages.ToArray());
            var vectorOfTrainingLabels = new VectorOfInt(trainingLabels.ToArray());
            var vectorOfTestImages = new VectorOfMat(testImages.ToArray());

            // call general function that deals with the testing of each of the FaceRecognizer sub classes
            TrainAndTimeRecognizer(recognizer, vectorOfTrainingImages, vectorOfTrainingLabels);

            // Now we test the trained recognizer on the test images
            PredictTestData(recognizer, vectorOfTestImages, testLabels, testCaseRootFolder);
        }

        // have to use Mat rather than Image, Emgu doesn't do image dimensions properly with image but does with Mat
        private static void GetAllTrainingAndTestData(string rootFolderLocation, ref List<Mat> trainImages, ref List<int> trainLabels, ref List<Mat> testImages, ref List<int> testLabels, 
            string specificTrainDir = null, string specificTestDir = null)
        {
            // If the method had specified test/train directory
            if(specificTrainDir != null && specificTestDir != null)
            {
                // go through top level directory for each test person in the train directory
                foreach (var directory in Directory.GetDirectories(Path.Combine(rootFolderLocation, specificTrainDir)))
                {
                    foreach (var imageFile in Directory.GetFiles(directory))
                    {
                        trainImages.Add(new Image<Gray, byte>(imageFile).Mat);
                        trainLabels.Add(Convert.ToInt32(Path.GetFileName(Path.GetDirectoryName(imageFile))));
                    }
                }

                // go through top level directory for each test person in the test directory
                foreach (var directory in Directory.GetDirectories(Path.Combine(rootFolderLocation, specificTestDir)))
                {
                    foreach (var imageFile in Directory.GetFiles(directory))
                    {
                        testImages.Add(new Image<Gray, byte>(imageFile).Mat);
                        testLabels.Add(Convert.ToInt32(Path.GetFileName(Path.GetDirectoryName(imageFile))));
                    }
                }
            }
            // if we want a randomized set of test/train images by not specifying a train/test directory
            else
            {
                // create our randomizer
                Random randomizer = new Random();
                List<int> testIndexValues = new List<int>();

                // create an array of index's to be removed
                while (testIndexValues.Count < numOfTestImagesPerPerson)
                {
                    int numToAdd = randomizer.Next(0, numOfImagesPerPerson);
                    if (!testIndexValues.Contains(numToAdd))
                    {
                        testIndexValues.Add(numToAdd);
                    }
                }

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
        }
        
        private static void TrainAndTimeRecognizer(FaceRecognizer recognizer, VectorOfMat trainImages, VectorOfInt trainLabels)
        {
            // Train the recogniser on the images, and time it using the stopwatch class
            Stopwatch trainingTimer = new Stopwatch();

            trainingTimer.Start();
            recognizer.Train(trainImages, trainLabels);
            trainingTimer.Stop();

            trainingTime = trainingTimer.Elapsed;
        }

        private static void PredictTestData(FaceRecognizer recognizer, VectorOfMat vectorOfTestImages, List<int> testLabels, string testDirectory)
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

            OutputResultsToDisk(recognizer.ToString(), testDirectory, correctAmount, incorrectAmount, totalTestImages);
        }

        // Illumination requires a more sophisticated prediction test function, to capture different variables in the test
        private static void PredictIlluminationTestData(FaceRecognizer recognizer, VectorOfMat vectorOfTestImages, List<int> testLabels)
        {

        }

        private static void OutputResultsToDisk(string recognizerName, string testDirectory, int correctAmount, int incorrectAmount, int totalTestImages)
        {
            // calculate percentages of correct/incorrect predictions
            double correctPercentage = (correctAmount / totalTestImages) * 100;
            double incorrectPercentage = (incorrectAmount / totalTestImages) * 100;

            // output results to a file on disk
            string fileText = "Test results for recognizer: " + recognizerName + Environment.NewLine;
            fileText += "Test results on the following directory of test images: " + testDirectory + Environment.NewLine;
            //fileText += "Total number of images in directory: " + (numOfDifferentTestPeople * numOfImagesPerPerson) +
            //    " Spread across " + numOfDifferentTestPeople + " different test subjects" + Environment.NewLine;
            //fileText += "Total number of training images used for each person: " + numOfTrainImagesPerPerson + Environment.NewLine;
            //fileText += "Total number of test images used for each person: " + numOfTestImagesPerPerson + Environment.NewLine;
            fileText += "---------- TEST RESULTS ----------" + Environment.NewLine;
            fileText += "Total elapsed time for training the recogniser: " + trainingTime + Environment.NewLine;
            fileText += "Total elapsed time for predicting all test images: " + testingTime + Environment.NewLine;
            fileText += "Total Correct: " + correctAmount + " (" + correctPercentage + "%)" + Environment.NewLine;
            fileText += "Total Incorrect: " + incorrectAmount + " (" + incorrectPercentage + "%)" + Environment.NewLine;

            File.WriteAllText(Path.Combine(testDirectory, "testResults.txt"), fileText);
        }

        private static void OutputResultsToDisk(string recognizerName, string testLocation, int totalTestImages,
            List<Tuple<string, int>> testResults)
        {
            // output results to a file on disk
            string fileText = "Test results for recognizer: " + recognizerName + Environment.NewLine;
            fileText += "Test results on the following directory of test images: " + testLocation + Environment.NewLine;
            fileText += "---------- TEST RESULTS ----------" + Environment.NewLine;
            fileText += "Total elapsed time for training the recogniser: " + trainingTime + Environment.NewLine;
            fileText += "Total elapsed time for predicting all test images: " + testingTime + Environment.NewLine;

            // adding each test result to the output file and calculating the percentage
            foreach(var testResultPair in testResults)
            {
                fileText += "Total " + testResultPair.Item1 + ": " + testResultPair.Item2 + " (" + 
                    (double)((testResultPair.Item2 / totalTestImages) * 100) + "%)" + Environment.NewLine;
            }

            File.WriteAllText(Path.Combine(testLocation, "testResults.txt"), fileText);
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

        public static void FaceDetectionTesting(string rootFaceDetectionTestLocation, int numberOfNeighbours = 3, double scaleFactor = 1.1)
        {
            // Create the classifier
            CascadeClassifier faceClassifier = new CascadeClassifier(BackendGuiUtility.haarFaceFile);
            
            var imageList = new List<Image<Gray, byte>>();
            var correctFaceNumber = new List<int>();
            // folder names will be the number of people in image, so will use that as comparison
            foreach (var directory in Directory.GetDirectories(rootFaceDetectionTestLocation))
            {
                foreach (var imageFile in Directory.GetFiles(directory))
                {
                    imageList.Add(new Image<Gray, byte>(imageFile));
                    correctFaceNumber.Add(Convert.ToInt32(Path.GetFileName(Path.GetDirectoryName(imageFile))));
                }
            }
            
            var correctAmount = 0;
            var overPredicted = 0;
            var underPredicted = 0;

            Stopwatch totalDetectionTime = new Stopwatch();

            for(int i = 0; i < imageList.Count; i++)
            {
                // get the detected amount
                totalDetectionTime.Start();
                var numberOfDetectedFaces = faceClassifier.DetectMultiScale(imageList[i], scaleFactor, numberOfNeighbours).Length;
                totalDetectionTime.Stop();

                // compare with the actual amount, 0 is correct, > 0 is overestimages and < 0 is underestimated amount
                var differenceOfFaces = numberOfDetectedFaces - correctFaceNumber[i];
                
                // correctly guessed the amount of faces
                if(differenceOfFaces == 0)
                {
                    correctAmount += numberOfDetectedFaces;
                }
                // over predicted
                else if (differenceOfFaces > 0)
                {
                    // correctly predicts the amount of faces, but does add the extra to over predicting
                    overPredicted += differenceOfFaces;
                    correctAmount += correctFaceNumber[i];
                }
                // under predicted
                else if (differenceOfFaces < 0)
                {
                    // underpredicts but, adds the ones found as correct
                    underPredicted += Math.Abs(differenceOfFaces);
                    correctAmount += (correctFaceNumber[i] - Math.Abs(differenceOfFaces));
                }
            }

            var totalTests = correctFaceNumber.Sum();
            double correctPercentage = (correctAmount / totalTests) * 100;
            double overPredPerctange = (overPredicted / totalTests) * 100;
            double underPredPercentage = (underPredicted / totalTests) * 100;

            string fileText = "Test results for face detector" + Environment.NewLine;
            fileText += "Total amount of test images: " + totalTests;
            fileText += "---------- TEST RESULTS ----------" + Environment.NewLine;
            fileText += "Total elapsed time for detecting all faces: " + totalDetectionTime.Elapsed + Environment.NewLine;
            fileText += "Total Correct: " + correctAmount + " (" + correctPercentage + "%)" + Environment.NewLine;
            fileText += "Total Over Predicted: " + overPredicted + " (" + overPredPerctange + "%)" + Environment.NewLine;
            fileText += "Total Under Predicted: " + underPredicted + " (" + underPredPercentage + "%)" + Environment.NewLine;

            File.WriteAllText(Path.Combine(rootFaceDetectionTestLocation, "faceDetectionResults.txt"), fileText);
        }
    }
}
