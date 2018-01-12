using Emgu.CV;
using Emgu.CV.Structure;
using System.Collections.Generic;
using System.IO;

namespace FaceAuthenticators
{
    public class EigenfaceAuthenticator
    {
        public EigenfaceAuthenticator()
        {

        }
        
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
            CalculateEigenFace(faceMatrix, eigenVectors, 4);
        }

        public void PredictImage()
        {

        }

        public static void CalculateEigenFace(List<int[]> faceMatrix, Matrix<float> eigenVectors, int numberToCreate)
        {
            // eigenvectors are alaready in order, so first eigenvector will be first eigenface
            int numberOfColumns = faceMatrix[0].Length;
            var eigenVectorMatrix = eigenVectors.Data;
            var faceVectorArray = faceMatrix.ToArray();
            var eigenFaceVector = new byte[numberOfColumns];

            // used to calcualte the i'th eigenvector
            for (int i = 0; i < numberToCreate; i++)
            {
                var eigenVectorRow = RecognizerUtility.GetMatrixRow(eigenVectorMatrix, i);

                for (int column = 0; column < numberOfColumns; column++)
                {
                    // we want the column of the faceVectorData and the Row of the eigenVectors
                    var faceVectorColumn = RecognizerUtility.GetMatrixColumn(faceVectorArray, column);

                    eigenFaceVector[column] = (byte)RecognizerUtility.CalculateVectorProduct(faceVectorColumn, eigenVectorRow);
                }

                Image<Gray, byte> eigenFaceJpeg = new Image<Gray, byte>(RecognizerUtility.imageWidth, RecognizerUtility.imageHeight)
                {
                    Bytes = eigenFaceVector
                };

                eigenFaceJpeg.Save(Path.Combine(RecognizerUtility.rootFolder, "EigenFace" + i + ".jpg"));
            }
        }
    }
}
