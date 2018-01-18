﻿using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.IO;

namespace FaceAuthenticators
{
    public class EigenfaceAuthenticator
    {
        private byte[] averageFace;
        private List<string> imageLabels = new List<string>();
        private List<int[]> faceMatrix = new List<int[]>();
        private List<float[]> eigenfaceMatrix;
        private List<float[]> weightVector;

        public double percentEigenfaceCreated { get; set; }

        public override string ToString()
        {
            return "Eigenface Recognizer - Own Implementation";
        }

        public EigenfaceAuthenticator(double percentEigenfaceCreated = 0.95)
        {
            this.percentEigenfaceCreated = percentEigenfaceCreated;
        }
        
        public void TrainEigenfaceAuthenticator()
        {
            // generate the matrix of image vectors
            RecognizerUtility.GetAllImageVectorsAndLabels(RecognizerUtility.rootFolder, ref faceMatrix, ref imageLabels);

            // generate the mean or average face
            this.averageFace = RecognizerUtility.GetAverageFace(faceMatrix);

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

            // need to multiply the eigenVectors with the original image dataset to create our eigenfaces
            this.eigenfaceMatrix = CalculateEigenFaces(faceMatrix, eigenVectors, eigenValues);

            this.weightVector = CalculateWeightMatrix(faceMatrix);
        }

        public string PredictImage(Image<Gray, byte> inputImage)
        {
            // vectorize the image
            int[] imageVector = RecognizerUtility.ImageToVector(inputImage);
            
            // subtract the mean image
            for (int i = 0; i < averageFace.Length; i++)
            {
                imageVector[i] -= averageFace[i];
            }

            // calculate the weights of the image
            var imageWeights = CalculateImageWeights(imageVector);

            // calculate the euclidean distance from the other weights
            var indexOfName = CalculateClosestVector(imageWeights);

            // Return the name of the closest label
            return imageLabels[indexOfName];
        }

        private List<float[]> CalculateEigenFaces(List<int[]> faceMatrix, Matrix<float> eigenVectors, Matrix<float> eigenValues)
        {
            // eigenvectors are alaready in order, so first eigenvector will be first eigenface
            int numberOfColumns = faceMatrix[0].Length;
            var eigenVectorMatrix = eigenVectors.Data;
            var faceVectorArray = faceMatrix.ToArray();
            var eigenValueArray = eigenValues.Data;
            var eigenValueSum = eigenValues.Sum;
            float cumulativeSum = 0;
            int counter = 0;
            List<float[]> eigenFaceMatrix = new List<float[]>();

            // used to calcualte the i'th eigenvector
            while((cumulativeSum / eigenValueSum) < percentEigenfaceCreated)
            {
                cumulativeSum += eigenValueArray[counter, 0];
                var eigenVectorRow = RecognizerUtility.GetMatrixRow(eigenVectorMatrix, counter);
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

                eigenFaceJpeg.Save(Path.Combine(RecognizerUtility.rootFolder, "EigenFace" + counter + ".jpg"));
                counter++;
            }

            return eigenFaceMatrix;
        }

        private List<float[]> CalculateWeightMatrix(List<int[]> normalizedFaceMatrix)
        {
            List<float[]> weightMatrix = new List<float[]>();
            int[][] normalizedMatrix = normalizedFaceMatrix.ToArray();

            // loop though each (normalized) training image and multiply it with each eigenface to generate our weights
            for(int i = 0; i < normalizedMatrix.Length; i++)
            {
                float[] weightArray = new float[eigenfaceMatrix.Count];
                var normalizedImageVector = RecognizerUtility.GetMatrixRow(normalizedMatrix, i);

                weightMatrix.Add(CalculateImageWeights(normalizedImageVector));
            }
            
            return weightMatrix;
        }

        private float[] CalculateImageWeights(int[] imageVector)
        {
            float[][] eigenFaceMatrix = eigenfaceMatrix.ToArray();
            float[] weightArray = new float[eigenFaceMatrix.Length];

            for (int eigCount = 0; eigCount < eigenFaceMatrix.Length; eigCount++)
            {
                weightArray[eigCount] = RecognizerUtility.CalculateVectorProduct(imageVector, RecognizerUtility.GetMatrixRow(eigenFaceMatrix, eigCount));
            }

            return weightArray;
        }

        private int CalculateClosestVector(float[] imageWeights)
        {
            double lowestVal = double.MaxValue;
            int indexOfLowest = 0;

            for(int i = 0; i < weightVector.Count; i++)
            {
                var distance = CalculateEuclideanDistance(imageWeights, weightVector[i]);

                if(distance < lowestVal)
                {
                    lowestVal = distance;
                    indexOfLowest = i;
                }
            }

            return indexOfLowest;
        }

        private double CalculateEuclideanDistance(float[] firstImage, float[] secondImage)
        {
            // Assert firstImage is equal length to secondImage!

            double totalDistance = 0;

            for(int i = 0; i < firstImage.Length; i++)
            {
                totalDistance += Math.Pow((firstImage[i] - secondImage[i]), 2);
            }

            return Math.Sqrt(totalDistance);
        }
    }
}
