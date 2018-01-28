using Emgu.CV;
using Emgu.CV.Structure;
using FaceAuthenticators;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TestRunner
{
    public static class TestUtility
    {
        // this class is used for calling one-off methods that are needed to populate testing images or
        // ensure they are appropriate for testing (i.e correct size and type)
        // also includes other small methods for things like file info gathering

        public static void TrimImagesToSize(string inputImageRootFolder, string outputImageRootFolder)
        {
            // This function is only used when trimming images to the same dimension
            // we will assume that each face in the images are centered

            string haarFaceFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "haarcascade_frontalface_default.xml");
            CascadeClassifier faceClassifier = new CascadeClassifier(haarFaceFile);
            int xPoint = 0, yPoint = 0;
            Mat imageFileRoi;

            foreach (var file in Directory.GetFiles(inputImageRootFolder, "*.jpg", SearchOption.AllDirectories))
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
                if (allFaces.Length == 1)
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
                if (!Directory.Exists(Path.Combine(outputImageRootFolder, directoryName)))
                {
                    Directory.CreateDirectory(Path.Combine(outputImageRootFolder, directoryName));
                }

                // save the ROI
                imageFileRoi.Save(Path.Combine(outputImageRootFolder, directoryName, fileName));
            }
        }

        public static void PgmToJpgConversion(string inputPgmRootFolder, string outputPgmRootFolder)
        {
            // This function should only be used when database images are of type .pgm and need
            // to be converted so they are view-able

            foreach(var pgmFile in Directory.GetFiles(inputPgmRootFolder, "*.pgm", SearchOption.AllDirectories))
            {
                // get folder name and file name to preserve the structure of the images
                var directoryName = Path.GetFileName(Path.GetDirectoryName(pgmFile));
                var fileName = Path.GetFileName(pgmFile);

                // replace the file type, so saves at .jpg
                fileName = fileName.Replace("pgm", "jpg");

                // create the image file
                var imageFile = new Image<Gray, byte>(pgmFile);

                // before saving need to check directory exists and if not create it
                if (!Directory.Exists(Path.Combine(outputPgmRootFolder, directoryName)))
                {
                    Directory.CreateDirectory(Path.Combine(outputPgmRootFolder, directoryName));
                }

                // save the image as jpg
                imageFile.Save(Path.Combine(outputPgmRootFolder, directoryName, fileName));
            }
        }

        public static List<Image<Gray, byte>>[] GetKFold(List<Image<Gray, byte>> masterImageList, List<string> testingLabels, ref List<string>[] kFoldedLabels, int kFolds)
        {
            // we assume the list is already randomised
            var kFoldedImageData = new List<Image<Gray, byte>>[kFolds];
            int counter = 0, innerCount = 0;
            
            // initializes each list
            for (int i = 0; i < kFolds; i++)
            {
                kFoldedImageData[i] = new List<Image<Gray, byte>>();
                kFoldedLabels[i] = new List<string>();
            }

            // divide data as evenly as possible, each loop will use the small group as the test data
            // with the rest of the data being used to train the authenticator
            while (counter < masterImageList.Count)
            {
                kFoldedImageData[innerCount].Add(masterImageList[counter]);
                kFoldedLabels[innerCount].Add(testingLabels[counter]);

                counter++;

                innerCount++;
                if (innerCount > kFolds - 1)
                {
                    innerCount = 0;
                }
            }
            
            return kFoldedImageData;
        }

        public static void ShuffleList(ref List<int> indexArray)
        {
            Random randomizer = new Random();
            int indexSize = indexArray.Count;

            // create random location and swap
            for (int i = 0; i < indexSize; i++)
            {
                int randomIndex = i + randomizer.Next(indexSize - i);
                int temp = indexArray[randomIndex];
                indexArray[randomIndex] = indexArray[i];
                indexArray[i] = temp;
            }
        }

        public static List<int> GenerateRandomList(int listSize, int maxRange)
        {
            // create our randomizer
            Random randomizer = new Random();
            List<int> testIndexValues = new List<int>();

            // create an array of index's to be removed
            while (testIndexValues.Count < listSize)
            {
                int numToAdd = randomizer.Next(0, maxRange);
                if (!testIndexValues.Contains(numToAdd))
                {
                    testIndexValues.Add(numToAdd);
                }
            }

            return testIndexValues;
        }

        public static List<string> GetAllDirectoryNames(string folderLocation)
        {
            var directoryNames = new List<string>();

            foreach(var directory in Directory.GetDirectories(folderLocation))
            {
                directoryNames.Add(Path.GetFileName(directory));
            }

            return directoryNames;
        }

        public static string GetAzimuthFromYaleFile(string fileName)
        {
            return fileName.Split('A', 'E')[1];
        }

        public static string GetElevationFromYaleFile(string fileName)
        {
            var matches = Regex.Split(fileName, "(E)");
            return (matches[1] += matches[2]);
        }

        public static void TryIncAtKeyValue(Dictionary<string, int[]> testResultDictionary, string keyVal, int locationValToInc)
        {
            // first off we need to create at key value if it doesn't already exist
            if (!testResultDictionary.TryGetValue(keyVal, out int[] valueTest))
            {
                valueTest = new int[3];
                testResultDictionary.Add(keyVal, valueTest);
            }

            // now it is created, we can incremement the index value
            testResultDictionary[keyVal][locationValToInc]++;
        }

        public static List<Image<Gray, byte>> ConvertMatToImage(List<Mat> matList)
        {
            var newList = new List<Image<Gray, byte>>();

            foreach(var matImage in matList)
            {
                newList.Add(matImage.ToImage<Gray, byte>());
            }

            return newList;
        }

        public static List<string> ConvertIntToString(List<int> intList)
        {
            var newList = new List<string>();

            foreach (var matImage in intList)
            {
                newList.Add(matImage.ToString());
            }

            return newList;
        }
    }
}
