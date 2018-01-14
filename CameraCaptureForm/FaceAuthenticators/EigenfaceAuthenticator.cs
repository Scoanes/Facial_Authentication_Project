using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
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
            for(int i =0; i < averageFace.Length; i++)
            {
                foreach(var face in faceMatrix)
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

            //var other = new Matrix<float>(covMatrix.Size);
            //CvInvoke.SVDecomp(covMatrix, eigenValues, eigenVectors, other, SvdFlag.Default);

            // TODO - possible change this to a % value
            int eigenVectorsCreated = 3;

            // need to multiply the eigenVectors with the original image dataset to create our eigenfaces
            var eigenfaceMatrix = CalculateEigenFaces(faceMatrix, eigenVectors, eigenVectorsCreated);

            var weightVector = CalculateWeights(faceMatrix, eigenfaceMatrix);
        }

        public void PredictImage()
        {

        }

        private List<float[]> CalculateEigenFaces(List<int[]> faceMatrix, Matrix<float> eigenVectors, int numberToCreate)
        {
            // eigenvectors are alaready in order, so first eigenvector will be first eigenface
            int numberOfColumns = faceMatrix[0].Length;
            var eigenVectorMatrix = eigenVectors.Data;
            var faceVectorArray = faceMatrix.ToArray();
            List<float[]> eigenFaceMatrix = new List<float[]>();

            // used to calcualte the i'th eigenvector
            for (int i = 0; i < numberToCreate; i++)
            {
                var eigenVectorRow = RecognizerUtility.GetMatrixRow(eigenVectorMatrix, i);
                var eigenFaceVector = new float[numberOfColumns];

                for (int column = 0; column < numberOfColumns; column++)
                {
                    // we want the column of the faceVectorData and the Row of the eigenVectors
                    var faceVectorColumn = RecognizerUtility.GetMatrixColumn(faceVectorArray, column);
                    var eigenPixelValue = RecognizerUtility.CalculateVectorProduct(faceVectorColumn, eigenVectorRow);

                    // scalar value is 255/2 (half of max value for a byte)
                    float scalarValue = (float)127.5;

                    eigenPixelValue += scalarValue;
                    if (eigenPixelValue > 255)
                    {
                        eigenPixelValue = 255;
                    }
                    else if (eigenPixelValue < 0)
                    {
                        eigenPixelValue = 0;
                    }

                    eigenFaceVector[column] = eigenPixelValue;
                }

                eigenFaceMatrix.Add(eigenFaceVector);

                Image<Gray, byte> eigenFaceJpeg = new Image<Gray, byte>(RecognizerUtility.imageWidth, RecognizerUtility.imageHeight)
                {
                    Bytes = Array.ConvertAll(eigenFaceVector, item => (byte)item)
                };

                eigenFaceJpeg.Save(Path.Combine(RecognizerUtility.rootFolder, "EigenFace" + i + ".jpg"));
            }

            return eigenFaceMatrix;
        }

        private List<float[]> CalculateWeights(List<int[]> normalizedFaceMatrix, List<float[]> eigenFaces)
        {
            List<float[]> weightMatrix = new List<float[]>();
            float[][] eigenFaceMatrix = eigenFaces.ToArray();
            int[][] normalizedMatrix = normalizedFaceMatrix.ToArray();

            // loop though each (normalized) training image and multiply it with each eigenface to generate our weights
            for(int i = 0; i < normalizedMatrix.Length; i++)
            {
                float[] weightArray = new float[eigenFaceMatrix.Length];
                var normalizedImageVector = RecognizerUtility.GetMatrixRow(normalizedMatrix, i);

                for(int eigCount = 0; eigCount < eigenFaceMatrix.Length; eigCount++)
                {
                    weightArray[eigCount] = RecognizerUtility.CalculateVectorProduct(normalizedImageVector, RecognizerUtility.GetMatrixRow(eigenFaceMatrix, eigCount));
                }

                weightMatrix.Add(weightArray);
            }
            
            return weightMatrix;
        }
    }
}
