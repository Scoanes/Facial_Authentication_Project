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
        static void Main()
        {
            // call the relevant testing from here

            // create the recogniser objects
            FaceRecognizer eigenfaceRecognizer = new EigenFaceRecognizer();

            // Get Images from test location
            var imageLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestImages");
            var testImages = GetAllTrainingImages(imageLocation);

            // Get the labels for each image
            var testLabels = GetLabelsForTrainingImages(imageLocation);

            // need to convert both of these into a datatype the Train method accepts
            var vectorOfImages = new VectorOfMat(testImages.ToArray());
            var vectorOfLabels = new VectorOfInt(testLabels.ToArray());

            // Train the recogniser on the images
            eigenfaceRecognizer.Train(new VectorOfMat(testImages.ToArray()), vectorOfLabels);

        }

        private static List<Mat> GetAllTrainingImages(string rootFolderLocation)
        {
            // create our list of image files
            List<Mat> allTestImages = new List<Mat>();

            foreach(var imageFile in Directory.GetFiles(rootFolderLocation, "*", SearchOption.AllDirectories))
            {
                allTestImages.Add(new Mat(imageFile));
            }

            return allTestImages;
        }

        private static List<int> GetLabelsForTrainingImages(string rootFolderLocation)
        {
            List<int> trainingImageLabels = new List<int>();

            foreach (var imageFile in Directory.GetFiles(rootFolderLocation, "*", SearchOption.AllDirectories))
            {
                trainingImageLabels.Add(Convert.ToInt32(Path.GetFileName(Path.GetDirectoryName(imageFile))));
            }

            return trainingImageLabels;
        }
    }
}
