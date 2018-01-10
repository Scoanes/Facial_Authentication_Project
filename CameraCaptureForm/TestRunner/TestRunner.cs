using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.CV.Face;
using System.IO;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace TestRunner
{
    public class TestRunner
    {
        private static string imageLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestImages");
        private static int numOfTestElements = 2;
        private static int numOfElementsPerPerson = 15;
        private static int numOfTrainImagesPerPerson = numOfElementsPerPerson - numOfTestElements;
        private static int numOfDifferentTestPeople = Directory.GetDirectories(imageLocation).Length;

        static void Main()
        {
            // create our randomizer
            Random randomizer = new Random();
            int[] elementsToRemove = new int[numOfTestElements];

            // create an array of index's to be removed
            for (int i = 0; i < numOfTestElements; i++)
            {
                elementsToRemove[i] = randomizer.Next(0, numOfElementsPerPerson);
            }

            // create the recogniser objects
            FaceRecognizer eigenfaceRecognizer = new EigenFaceRecognizer();

            // get the training and test data
            var trainingImages = new List<Mat>();
            var trainingLabels = new List<int>();
            var testImages = new List<Mat>();
            var testLabels = new List<int>();

            GetAllTrainingAndTestData(imageLocation, elementsToRemove, ref trainingImages, ref trainingLabels, ref testImages, ref testLabels);

            // need to convert both of these into a datatype the Train method accepts
            var vectorOfTrainingImages = new VectorOfMat(trainingImages.ToArray());
            var vectorOfTrainingLabels = new VectorOfInt(trainingLabels.ToArray());

            // Train the recogniser on the images
            eigenfaceRecognizer.Train(vectorOfTrainingImages, vectorOfTrainingLabels);

            // need to convert both of these into a datatype the Train method accepts
            var vectorOfTestImages = new VectorOfMat(testImages.ToArray());
            var vectorOfTestLabels = new VectorOfInt(testLabels.ToArray());

            // Test the Recognizer
            for(int i = 0; i < vectorOfTestImages.Size; i++)
            {
                eigenfaceRecognizer.Predict(vectorOfTestImages[i]);
            }
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
    }
}
