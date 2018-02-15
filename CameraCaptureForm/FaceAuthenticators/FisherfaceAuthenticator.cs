using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceAuthenticators
{
    class FisherfaceAuthenticator
    {

        private int numberOfPeople;
        private int totalNumberOfImages;

        private List<int[]> faceMatrix;
        private List<string> classLabels;
        private byte[] averageFace;

        public FisherfaceAuthenticator()
        {
            
        }

        public void TrainFisherfaceAuthenticator()
        {
            // get all the vectorised images and class labels
            RecognizerUtility.GetAllImageVectorsAndLabels(RecognizerUtility.rootEnrolImagesFolder, ref faceMatrix, ref classLabels);

            // generate global mean image
            averageFace = RecognizerUtility.GetAverageFace(faceMatrix);

            // use PCA to reduce image dimentions to (N-c)
            // where N = number of samples (images)
            // c = unique classes

            // Calculate between class scatter matrix

            // Calculate within class scattrer matrix

            // use LDA, maximise ratio between between and withing class matrix
            // determined by eigenvectors

            // faces = Wpca * Wfld
        }

        private void CalculateBetweenClassScatterMatrix()
        {
            // N is number of samples for that class
            // uses the mean image of class - global mean image
        }

        private void CalculateWithinClassScatterMatrix()
        {
            // sample image from set - class mean image
        }
    }
}
