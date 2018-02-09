using CameraCaptureForm;
using Emgu.CV;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using FaceAuthenticators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace TestRunner
{
    public class TestRunner
    {
        private static string testImagesRootFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestImages");

        // Stopwatch times for training/testing
        private static TimeSpan trainingTime;
        private static TimeSpan testingTime;

        static void Main()
        {
            // create the recogniser objects
            //EigenfaceAuthenticator eigenfaceRecognizer = new EigenfaceAuthenticator();
            FaceRecognizer emguEigenfaceRecognizer = new EigenFaceRecognizer();

            //EmguTestCaseRunner(emguEigenfaceRecognizer, "Scale");
            //EigenfaceTestCaseRunner(eigenfaceRecognizer, "Pose");

            // TEMP!
            /*
            var testCaseRootFolder = Path.Combine(testImagesRootFolder, "Background_Noise");
            RecognizerUtility.rootFolder = testCaseRootFolder;
            KFoldParamterTesting(testCaseRootFolder, 10, 0.45);
            */
        }

        // Takes the root directory of the test case, defining what test case it will run
        public static void EmguTestCaseRunner(FaceRecognizer recognizer, string testCase)
        {
            // Declare the image location by using the testCase name
            var testCaseRootFolder = Path.Combine(testImagesRootFolder, testCase);

            // scale test images are of a different dimension so change them here
            if (testCase == "Scale")
            {
                RecognizerUtility.imageHeight = 196;
                RecognizerUtility.imageWidth = 196;
            }

            // Declaring the training and testing locations
            // these are lists incase we want more than 1 folder to be a training location
            string[] trainingLocations = new string[]
            {
                "Group2"
            };
            string[] testingLocations = new string[]
            {
                "Group1"
            };

            // Declare training and testing images/labels
            var trainingImages = new List<Mat>();
            var trainingLabels = new List<int>();
            var testImages = new List<Mat>();
            var testLabels = new List<int>();
            var testImageNames = new List<string>();

            if (testCase == "Base")
            {
                testCaseRootFolder = testImagesRootFolder;
                RecognizerUtility.rootFolder = testCaseRootFolder;

                trainingLocations = new string[]
                {
                    Path.Combine(testCaseRootFolder, "Illumination", "Training"),
                    Path.Combine(testCaseRootFolder, "Pose", "Training")
                };

                GetAllTrainingAndTestDataRandomly(trainingLocations, ref trainingImages, ref trainingLabels, ref testImages, ref testLabels, 3);
            }
            else
            {
                // here we get the images and labels for the training and testing, already spit randomly
                GetAllTrainingAndTestData(testCaseRootFolder, ref trainingImages, ref trainingLabels, ref testImages, ref testLabels, ref testImageNames, trainingLocations, testingLocations);
            }

            // need to convert both of these into a datatype the Train method accepts
            var vectorOfTrainingImages = new VectorOfMat(trainingImages.ToArray());
            var vectorOfTrainingLabels = new VectorOfInt(trainingLabels.ToArray());
            var vectorOfTestImages = new VectorOfMat(testImages.ToArray());

            // call general function that deals with the testing of each of the FaceRecognizer sub classes
            TrainAndTimeEmguRecognizer(recognizer, vectorOfTrainingImages, vectorOfTrainingLabels);

            if(testCase == "Illumination")
            {
                PredictEmguTestDataIllumination(recognizer, vectorOfTestImages, testLabels, testImageNames, testCaseRootFolder);
            }
            else
            {
                // Now we test the trained recognizer on the test images
                PredictEmguTestData(recognizer, vectorOfTestImages, testLabels, testCaseRootFolder);
            }
        }

        public static void EigenfaceTestCaseRunner(EigenfaceAuthenticator recognizer, string testCase)
        {
            // Declare the image location by using the testCase name
            var testCaseRootFolder = Path.Combine(testImagesRootFolder, testCase);
            RecognizerUtility.rootFolder = testCaseRootFolder;

            // scale test images are of a different dimension so change them here
            if (testCase == "Scale")
            {
                RecognizerUtility.imageHeight = 196;
                RecognizerUtility.imageWidth = 196;
            }

            // Declaring the training and testing locations
            // these are lists incase we want more than 1 folder to be a training location
            string[] trainingLocations = new string[]
            {
                "Group1"
            };
            string[] testingLocations = new string[]
            {
                "Group2"
            };

            // Declare training and testing images/labels
            var trainingImages = new List<Mat>();
            var trainingLabels = new List<int>();
            var testImages = new List<Mat>();
            var testLabels = new List<int>();
            var testImageNames = new List<string>();

            if (testCase == "Base")
            {
                testCaseRootFolder = testImagesRootFolder;
                RecognizerUtility.rootFolder = testCaseRootFolder;

                trainingLocations = new string[]
                {
                    Path.Combine(testCaseRootFolder, "Illumination", "Training"),
                    Path.Combine(testCaseRootFolder, "Pose", "Training")
                };
                
                GetAllTrainingAndTestDataRandomly(trainingLocations, ref trainingImages, ref trainingLabels, ref testImages, ref testLabels, 3);
            }
            else
            {
                // here we get the images and labels for the training and testing, already spit randomly
                GetAllTrainingAndTestData(testCaseRootFolder, ref trainingImages, ref trainingLabels, ref testImages, ref testLabels, ref testImageNames, trainingLocations, testingLocations);
            }
            

            // need to convert both of these into a datatype the Train method accepts
            var vectorOfTrainingImages = TestUtility.ConvertMatToImage(trainingImages);
            var vectorOfTrainingLabels = TestUtility.ConvertIntToString(trainingLabels);
            var vectorOfTestImages = TestUtility.ConvertMatToImage(testImages);
            var newTestLabels = TestUtility.ConvertIntToString(testLabels);

            // call general function that deals with the testing of each of the FaceRecognizer sub classes
            TrainAndTimeRecognizer(recognizer, vectorOfTrainingImages, vectorOfTrainingLabels);

            if (testCase == "Illumination")
            {
                PredictEigenfaceTestDataIllumination(recognizer, vectorOfTestImages, newTestLabels, testImageNames, testCaseRootFolder);
            }
            else
            {
                // Now we test the trained recognizer on the test images
                PredictEigenfaceTestData(recognizer, vectorOfTestImages, newTestLabels, testCaseRootFolder);
            }
        }

        // have to use Mat rather than Image, Emgu doesn't do image dimensions properly with image but does with Mat
        private static void GetAllTrainingAndTestData(string rootFolderLocation, ref List<Mat> trainImages, ref List<int> trainLabels, ref List<Mat> testImages, ref List<int> testLabels,
            ref List<string> testImageNames, string[] specificTrainDirs, string[] specificTestDirs)
        {
            // go through top level directory for each test person in the train directory
            foreach (var trainDir in specificTrainDirs)
            {
                foreach (var directory in Directory.GetDirectories(Path.Combine(rootFolderLocation, trainDir)))
                {
                    foreach (var imageFile in Directory.GetFiles(directory))
                    {
                        trainImages.Add(new Image<Gray, byte>(imageFile).Mat);
                        trainLabels.Add(Convert.ToInt32(Path.GetFileName(Path.GetDirectoryName(imageFile))));
                    }
                }
            }

            // go through top level directory for each test person in the test directory
            foreach (var testDir in specificTestDirs)
            {
                foreach (var directory in Directory.GetDirectories(Path.Combine(rootFolderLocation, testDir)))
                {
                    foreach (var imageFile in Directory.GetFiles(directory))
                    {
                        testImages.Add(new Image<Gray, byte>(imageFile).Mat);
                        testLabels.Add(Convert.ToInt32(Path.GetFileName(Path.GetDirectoryName(imageFile))));
                        testImageNames.Add(Path.GetFileName(imageFile));
                    }
                }
            }
        }

        // this is called when you want a random split between training and test data
        private static void GetAllTrainingAndTestDataRandomly(string[] rootFolderLocations, ref List<Mat> trainImages, ref List<int> trainLabels, ref List<Mat> testImages, ref List<int> testLabels,
            int numOfTestImagesPerPerson = 5)
        {
            // loop through the root directories in each of the defined locations
            foreach(var rootDirectory in rootFolderLocations)
            {
                // go through top level directory for each test person
                foreach (var directory in Directory.GetDirectories(rootDirectory))
                {
                    int iter = 0;

                    // we will randomly generate our training/testing values each time, based on how many images per directory
                    var totalImages = Directory.GetFiles(directory).Length;

                    var testIndexValues = TestUtility.GenerateRandomList(numOfTestImagesPerPerson, totalImages);

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

        // ----- Training Functions ----- //
        private static void TrainAndTimeEmguRecognizer(FaceRecognizer recognizer, VectorOfMat trainImages, VectorOfInt trainLabels)
        {
            // Train the recogniser on the images, and time it using the stopwatch class
            Stopwatch trainingTimer = new Stopwatch();

            trainingTimer.Start();
            recognizer.Train(trainImages, trainLabels);
            trainingTimer.Stop();

            trainingTime = trainingTimer.Elapsed;
        }

        private static void TrainAndTimeRecognizer(EigenfaceAuthenticator recognizer, List<Image<Gray, byte>> trainImages, List<string> trainLabels)
        {
            // Train the recogniser on the images, and time it using the stopwatch class
            Stopwatch trainingTimer = new Stopwatch();

            trainingTimer.Start();
            recognizer.TrainEigenfaceAuthenticator(trainImages, trainLabels);
            trainingTimer.Stop();

            trainingTime = trainingTimer.Elapsed;
        }

        // ----- Prediction Functions ----- //
        private static void PredictEmguTestData(FaceRecognizer recognizer, VectorOfMat vectorOfTestImages, List<int> testLabels, string testDirectory)
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

        private static void PredictEmguTestDataIllumination(FaceRecognizer recognizer, VectorOfMat vectorOfTestImages, List<int> testLabels, List<string> testImageNames, string testDirectory)
        {
            var testResults = new Dictionary<string, int[]>();
            Stopwatch testingTimer = new Stopwatch();

            // Test the Recognizer
            for (int i = 0; i < vectorOfTestImages.Size; i++)
            {
                var fileAzimuth = TestUtility.GetAzimuthFromYaleFile(testImageNames[i]);
                var fileElevation = TestUtility.GetElevationFromYaleFile(testImageNames[i]);
                // get our predicted result
                testingTimer.Start();
                var preictionResult = recognizer.Predict(vectorOfTestImages[i]);
                testingTimer.Stop();

                // if prediction is correct
                if (preictionResult.Label == testLabels[i])
                {
                    TestUtility.TryIncAtKeyValue(testResults, fileAzimuth, 0);
                    TestUtility.TryIncAtKeyValue(testResults, fileElevation, 0);
                }
                else
                {
                    TestUtility.TryIncAtKeyValue(testResults, fileAzimuth, 1);
                    TestUtility.TryIncAtKeyValue(testResults, fileElevation, 1);
                }

                // 'Total' column should be already created by this point
                testResults[fileAzimuth][2]++;
                testResults[fileElevation][2]++;
            }

            testingTime = testingTimer.Elapsed;
            OutputResultsToDisk(recognizer.ToString(), testDirectory, testLabels.Count, testResults);
        }

        private static void PredictEigenfaceTestData(EigenfaceAuthenticator recognizer, List<Image<Gray, byte>> vectorOfTestImages, List<string> testLabels, string testDirectory)
        {
            // setting up our counters for correct and incorrect predictions
            int correctAmount = 0, incorrectAmount = 0, totalTestImages = testLabels.Count;
            Stopwatch testingTimer = new Stopwatch();

            // Test the Recognizer
            for (int i = 0; i < vectorOfTestImages.Count; i++)
            {
                // get our predicted result
                testingTimer.Start();
                var preictionResult = recognizer.PredictImage(vectorOfTestImages[i]);
                testingTimer.Stop();

                // if prediction is correct
                if (preictionResult == testLabels[i])
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

        private static void PredictEigenfaceTestDataIllumination(EigenfaceAuthenticator recognizer, List<Image<Gray, byte>> vectorOfTestImages, List<string> testLabels, 
            List<string> testImageNames, string testDirectory)
        {
            var testResults = new Dictionary<string, int[]>();
            Stopwatch testingTimer = new Stopwatch();

            // Test the Recognizer
            for (int i = 0; i < vectorOfTestImages.Count; i++)
            {
                var fileAzimuth = TestUtility.GetAzimuthFromYaleFile(testImageNames[i]);
                var fileElevation = TestUtility.GetElevationFromYaleFile(testImageNames[i]);
                // get our predicted result
                testingTimer.Start();
                var preictionResult = recognizer.PredictImage(vectorOfTestImages[i]);
                testingTimer.Stop();

                // if prediction is correct
                if (preictionResult == testLabels[i])
                {
                    TestUtility.TryIncAtKeyValue(testResults, fileAzimuth, 0);
                    TestUtility.TryIncAtKeyValue(testResults, fileElevation, 0);
                }
                else
                {
                    TestUtility.TryIncAtKeyValue(testResults, fileAzimuth, 1);
                    TestUtility.TryIncAtKeyValue(testResults, fileElevation, 1);
                }

                // 'Total' column should be already created by this point
                testResults[fileAzimuth][2]++;
                testResults[fileElevation][2]++;
            }

            testingTime = testingTimer.Elapsed;
            OutputResultsToDisk(recognizer.ToString(), testDirectory, testLabels.Count, testResults);
        }

        // ----- Output results Functions ----- //
        private static void OutputResultsToDisk(string recognizerName, string testDirectory, int correctAmount, int incorrectAmount, int totalTestImages)
        {
            // calculate percentages of correct/incorrect predictions
            double correctPercentage = ((double)correctAmount / totalTestImages) * 100;
            double incorrectPercentage = ((double)incorrectAmount / totalTestImages) * 100;

            // output results to a file on disk
            string fileText = "Test results for recognizer: " + recognizerName + Environment.NewLine;
            fileText += "Test results on the following directory of test images: " + testDirectory + Environment.NewLine;
            fileText += "---------- TEST RESULTS ----------" + Environment.NewLine;
            fileText += "Total elapsed time for training the recogniser: " + trainingTime + Environment.NewLine;
            fileText += "Total elapsed time for predicting all test images: " + testingTime + Environment.NewLine;
            fileText += "Total Correct: " + correctAmount + " (" + correctPercentage + "%)" + Environment.NewLine;
            fileText += "Total Incorrect: " + incorrectAmount + " (" + incorrectPercentage + "%)" + Environment.NewLine;

            File.WriteAllText(Path.Combine(testDirectory, "testResults.txt"), fileText);
        }

        private static void OutputResultsToDisk(string recognizerName, string testLocation, int totalTestImages,
            Dictionary<string, int[]> testResults)
        {
            // output results to a file on disk
            string fileText = "Test results for recognizer: " + recognizerName + Environment.NewLine;
            fileText += "Test results on the following directory of test images: " + testLocation + Environment.NewLine;
            fileText += "---------- TEST RESULTS ----------" + Environment.NewLine;
            fileText += "Total elapsed time for training the recogniser: " + trainingTime + Environment.NewLine;
            fileText += "Total elapsed time for predicting all test images: " + testingTime + Environment.NewLine;

            // adding each test result to the output file and calculating the percentage
            // key name of parameter
            // value[0] correct predictions
            // value[1] incorrect predictions
            // value[2] total predictions for this parameter
            foreach (var testResultPair in testResults)
            {
                fileText += "Results for parameter: " + testResultPair.Key + Environment.NewLine;
                fileText += "Total Correct: " + testResultPair.Value[0] + " (" + 
                    (((double)testResultPair.Value[0] / testResultPair.Value[2]) * 100) + "%)" + Environment.NewLine;
                fileText += "Total Incorrect: " + testResultPair.Value[1] + " (" +
                    (((double)testResultPair.Value[1] / testResultPair.Value[2]) * 100) + "%)" + Environment.NewLine;
            }

            File.WriteAllText(Path.Combine(testLocation, "testResults.txt"), fileText);
        }

        // ----- Other - Specialized - Test Cases ----- //
        private static void KFoldParamterTesting(string[] testDataLocations, int numberOfKFolds, double percentParameterValue)
        {
            // create our authenticator, with the parameter value
            EigenfaceAuthenticator eigenfaceAuth = new EigenfaceAuthenticator(percentParameterValue);

            // Stopwatches
            Stopwatch trainingStopwatch = new Stopwatch();
            Stopwatch testingStopwatch = new Stopwatch();

            var masterImageList = new List<Image<Gray, byte>>();
            var masterTestLabelsList = new List<string>();

            // Get all of the test data
            foreach(var testDir in testDataLocations)
            {
                foreach(var imageFile in Directory.GetFiles(testDir, "*.jpg", SearchOption.AllDirectories))
                {
                    masterImageList.Add(new Image<Gray, byte>(imageFile));
                    masterTestLabelsList.Add(Path.GetFileName(Path.GetDirectoryName(imageFile)));
                }
            }
            

            // randomise the order of the lists
            var indexes = Enumerable.Range(0, masterImageList.Count).ToList();
            TestUtility.ShuffleList(ref indexes);
            var randomizedImageList = indexes.Select(index => masterImageList[index]).ToList();
            var randomizedLabelList = indexes.Select(index => masterTestLabelsList[index]).ToList();

            var kFoldedLabels = new List<string>[numberOfKFolds];
            var kFoldedLists = TestUtility.GetKFold(randomizedImageList, randomizedLabelList,  ref kFoldedLabels, numberOfKFolds);

            int totalCorrect = 0, totalIncorrect = 0;
            string predictedLabel;

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

                trainingStopwatch.Start();
                // train the authenticator with the training data
                eigenfaceAuth.TrainEigenfaceAuthenticator(trainingData, trainingLabels);
                trainingStopwatch.Stop();

                // test the authenticator with the test data
                for(int testIter = 0; testIter < testingData.Count; testIter++)
                {
                    testingStopwatch.Start();
                    predictedLabel = eigenfaceAuth.PredictImage(testingData[testIter]);
                    testingStopwatch.Stop();
                    if (testingLabels[testIter].Equals(predictedLabel)){
                        totalCorrect++;
                    }
                    else
                    {
                        totalIncorrect++;
                    }
                }
            }
            trainingTime = trainingStopwatch.Elapsed;
            testingTime = testingStopwatch.Elapsed;
            OutputResultsToDisk("KFoldTest", testDataLocations[0], totalCorrect, totalIncorrect, indexes.Count);
        }

        private static void FaceDetectionTesting(string rootFaceDetectionTestLocation, int numberOfNeighbours = 3, double scaleFactor = 1.1)
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
            double correctPercentage = ((double)correctAmount / totalTests) * 100;
            double overPredPerctange = ((double)overPredicted / totalTests) * 100;
            double underPredPercentage = ((double)underPredicted / totalTests) * 100;

            string fileText = "Test results for face detector" + Environment.NewLine;
            fileText += "Total amount of test images: " + totalTests;
            fileText += "---------- TEST RESULTS ----------" + Environment.NewLine;
            fileText += "Total elapsed time for detecting all faces: " + totalDetectionTime.Elapsed + Environment.NewLine;
            fileText += "Total Correct: " + correctAmount + " (" + correctPercentage + "%)" + Environment.NewLine;
            fileText += "Total Over Predicted: " + overPredicted + " (" + overPredPerctange + "%)" + Environment.NewLine;
            fileText += "Total Under Predicted: " + underPredicted + " (" + underPredPercentage + "%)" + Environment.NewLine;

            File.WriteAllText(Path.Combine(rootFaceDetectionTestLocation, "faceDetectionResults.txt"), fileText);
        }

        private static void EnrolmentTesting(string rootEnrolmentTesting, int totalUnknownUsers, float thresholdValue)
        {
            // total set of images from a directory, Randomly select one of them to not be trained on
            // use that as the 'uknown' test user

            EigenfaceAuthenticator eigenfaceRecognizer = new EigenfaceAuthenticator(threshold: thresholdValue);
            Random randomizer = new Random();

            RecognizerUtility.rootFolder = rootEnrolmentTesting;

            // get the images

            // Declare training and testing images/labels
            var trainingImages = new List<Image<Gray, byte>>();
            var trainingLabels = new List<string>();
            var testImages = new List<Image<Gray, byte>>();
            var testLabels = new List<string>();

            int correct = 0, unknownCorrect = 0, incorrect = 0, falseNeg = 0, falsePos = 0, totalUnknownLabels = 0;

            var listOfUniqueLabels = TestUtility.GetAllDirectoryNames(Path.Combine(rootEnrolmentTesting, "Training"));
            var listOfTrainingLabels = listOfUniqueLabels;
            var listOfUnknownLabels = new List<string>();

            for(int i = 0; i < totalUnknownUsers; i++)
            {
                var indexToRemove = randomizer.Next(listOfTrainingLabels.Count);

                listOfUnknownLabels.Add(listOfTrainingLabels[indexToRemove]);
                listOfTrainingLabels.RemoveAt(indexToRemove);
            }

            // here we get all of the training images from the training directory, that aren't in the random list
            foreach(var dir in Directory.GetDirectories(Path.Combine(rootEnrolmentTesting, "Training")))
            {
                // if directory is in the list of training directories
                if (listOfTrainingLabels.Contains(Path.GetFileName(dir)))
                {
                    foreach(var file in Directory.GetFiles(dir))
                    {
                        trainingImages.Add(new Image<Gray, byte>(file));
                        trainingLabels.Add(Path.GetFileName(Path.GetDirectoryName(file)));
                    }
                }
            }

            // populate the test directory
            foreach (var dir in Directory.GetDirectories(Path.Combine(rootEnrolmentTesting, "Testing")))
            {
                foreach (var file in Directory.GetFiles(dir))
                {
                    var labelName = Path.GetFileName(Path.GetDirectoryName(file));

                    if (listOfUnknownLabels.Contains(labelName))
                    {
                        totalUnknownLabels++;
                    }

                    testImages.Add(new Image<Gray, byte>(file));
                    testLabels.Add(labelName);
                }
            }

            // train on train images
            eigenfaceRecognizer.TrainEigenfaceAuthenticator(trainingImages, trainingLabels);

            // test on set of images, with some of them not being trained on
            for(int i = 0; i < testImages.Count; i++)
            {
                var predictionResult = eigenfaceRecognizer.PredictImage(testImages[i]);

                // comapre which ones have been predicted as 'unknown'
                if (predictionResult == "Uknown")
                {
                    // if correctly predicted unknown
                    if (listOfUnknownLabels.Contains(testLabels[i]))
                    {
                        unknownCorrect++;
                    }
                    else
                    {
                        falseNeg++;
                    }
                }
                else
                {
                    if (listOfUnknownLabels.Contains(testLabels[i]))
                    {
                        falsePos++;
                    }
                    else if(predictionResult == testLabels[i])
                    {
                        correct++;
                    }
                    else
                    {
                        incorrect++;
                    }
                }
            }

            // output these results to disk
            string fileText = "Test results for First Test Wave" + Environment.NewLine;
            fileText += "---------- TEST RESULTS ----------" + Environment.NewLine;
            fileText += "Total Correct: " + correct + " (" + "%)" + Environment.NewLine;
            fileText += "Total Incorrect: " + incorrect + " ("  + "%)" + Environment.NewLine;
            fileText += "Total Unknown Correct: " + unknownCorrect + " (" + "%)" + Environment.NewLine;
            fileText += "Total False Positives: " + falsePos + " (" + "%)" + Environment.NewLine;
            fileText += "Total False Negatives: " + falseNeg + " (" + "%)" + Environment.NewLine;
            fileText += "Total Unknown tests: " + totalUnknownLabels + Environment.NewLine;
            fileText += "Total tests: " + testLabels.Count + " (" + (testLabels.Count - totalUnknownLabels) 
                + " known tests)" + Environment.NewLine;

            File.WriteAllText(Path.Combine(rootEnrolmentTesting, "testResultsTest1.txt"), fileText);

            // retrain with the user included
            // here we get all of the training images from the training directory, that aren't in the random list
            foreach (var dir in Directory.GetDirectories(Path.Combine(rootEnrolmentTesting, "Training")))
            {
                // now we only add the images previously left out
                if (!listOfTrainingLabels.Contains(Path.GetFileName(dir)))
                {
                    foreach (var file in Directory.GetFiles(dir))
                    {
                        trainingImages.Add(new Image<Gray, byte>(file));
                        trainingLabels.Add(Path.GetFileName(Path.GetDirectoryName(file)));
                    }
                }
            }

            // retrain and test results
            eigenfaceRecognizer.TrainEigenfaceAuthenticator(trainingImages, trainingLabels);

            correct = 0; unknownCorrect = 0; incorrect = 0; falseNeg = 0; falsePos = 0;

            for (int i = 0; i < testImages.Count; i++)
            {
                var predictionResult = eigenfaceRecognizer.PredictImage(testImages[i]);

                // We should have no unknowns now
                if (predictionResult == "Uknown")
                {
                    falseNeg++;
                }
                else
                {
                    if (predictionResult == testLabels[i])
                    {
                        correct++;
                    }
                    else
                    {
                        incorrect++;
                    }
                }
            }
            
            // output these results to disk
            string fileText2 = "Test results for First Test Wave" + Environment.NewLine;
            fileText2 += "---------- TEST RESULTS ----------" + Environment.NewLine;
            fileText2 += "Total Correct: " + correct + " (" + "%)" + Environment.NewLine;
            fileText2 += "Total Incorrect: " + incorrect + " (" + "%)" + Environment.NewLine;
            fileText2 += "Total False Positives: " + falsePos + " (" + "%)" + Environment.NewLine;
            fileText2 += "Total False Negatives: " + falseNeg + " (" + "%)" + Environment.NewLine;

            File.WriteAllText(Path.Combine(rootEnrolmentTesting, "testResultsTest2.txt"), fileText2);
        }
    }
}
