using Emgu.CV;
using Emgu.CV.Structure;
using System.IO;

namespace FaceAuthenticators
{
    public class EigenfaceAuthenticator
    {
        public EigenfaceAuthenticator()
        {

        }

        // Creates the eigenfaces
        public void TrainEigenfaceAuthenticator()
        {
            // generate the matrix of image vectors
            var faceMatrix = RecognizerUtility.GetAllImagesVectors(RecognizerUtility.rootFolder);

            // generate the mean or average face
            var averageFace = RecognizerUtility.GetAverageFace(faceMatrix);

            Image<Gray, byte> averageFaceJpeg = new Image<Gray, byte>(RecognizerUtility.imageWidth, RecognizerUtility.imageHeight)
            {
                Bytes = averageFace
            };

            averageFaceJpeg.Save(Path.Combine(RecognizerUtility.rootFolder, "AverageFace.jpg"));

            // minus the average face from each image
            foreach(var face in faceMatrix)
            {
                for(int i = 0; i < face.Length; i++)
                {
                    face[i] -= averageFace[i];
                }
            }

            // Create the face matrix for covariance matrix
            var covMatrix = new Matrix<float>(RecognizerUtility.CreateCovarianceMatrix(faceMatrix));

            // I have to create a vector to put into the matrix, Emgu will output 0's otherwise!
            var eigValVector = new float[covMatrix.Width];
            var eigenValues = new Matrix<float>(eigValVector);
            var eigenVectors = new Matrix<float>(covMatrix.Size);

            CvInvoke.Eigen(covMatrix, eigenValues, eigenVectors);

            //var other = new Matrix<float>(test.Size);
            //CvInvoke.SVDecomp(test, eigenValues, eigenVectors, other, SvdFlag.Default);

            // need to multiply the eigenVectors with the original image dataset to create our eigenfaces
            RecognizerUtility.CalculateEigenFace(faceMatrix, eigenVectors, 4);
        }

        public void predictImage()
        {

        }
    }
}
