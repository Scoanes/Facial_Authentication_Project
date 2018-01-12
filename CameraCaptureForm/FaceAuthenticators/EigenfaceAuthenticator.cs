using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace FaceAuthenticators
{
    public class EigenfaceAuthenticator
    {
        // Emgu CV requires these to be divisible by 4
        public static int imageHeight = 300;
        public static int imageWidth = 252;
        private static string rootFolder = @"C:\Users\RockInTheBox\Documents\University\Project\TestEnrolLocation";

        // Creates the eigenfaces
        public static void TrainEigenfaceAuthenticator()
        {
            // generate the matrix of image vectors
            var faceMatrix = GetAllImagesVectors(rootFolder);

            // generate the mean or average face
            var averageFace = GetAverageFace(faceMatrix);

            Image<Gray, byte> averageFaceJpeg = new Image<Gray, byte>(imageWidth, imageHeight)
            {
                Bytes = averageFace
            };

            averageFaceJpeg.Save(Path.Combine(rootFolder, "AverageFace.jpg"));

            // minus the average face from each image
            foreach(var face in faceMatrix)
            {
                for(int i = 0; i < face.Length; i++)
                {
                    face[i] -= averageFace[i];
                }
            }

            // Create the face matrix for covariance matrix
            var covMatrix = new Matrix<float>(CreateCovarianceMatrix(faceMatrix));

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

        private static List<int[]> GetAllImagesVectors(string folderLocaiton)
        {
            List<int[]> faceMatrix = new List<int[]>();

            // loop through the directory getting each file
            foreach (var file in Directory.GetFiles(rootFolder))
            {
                Image<Gray, byte> imageFile = new Image<Gray, byte>(file);
                var pixelData = imageFile.Data;

                // unfold them
                var imageVector = new int[imageWidth * imageHeight];
                int count = 0;

                for (int row = imageFile.Rows - 1; row >= 0; row--)
                {
                    for (int column = imageFile.Cols - 1; column >= 0; column--)
                    {
                        imageVector[count] = pixelData[row, column, 0];
                        count++;
                    }
                }

                // need to reverse the image vector before adding
                Array.Reverse(imageVector);

                // adding the unfolded image to the 'matrix'
                faceMatrix.Add(imageVector);
            }

            return faceMatrix;
        }

        private static byte[] GetAverageFace(List<int[]> faceMatrix)
        {
            var arrays = faceMatrix.ToArray();
            byte[] averageFace = new byte[imageWidth * imageHeight];

            for (int i = 0; i < averageFace.Length; i++)
            {
                averageFace[i] = GetAverageArrayValue(arrays, i);
            }
            return averageFace;
        }

        private static byte GetAverageArrayValue(int[][] arrays, int index)
        {
            int totalValue = 0;

            for(int i = 0; i < arrays.Length; i++)
            {
                totalValue += arrays[i][index];
            }
            int averagePixelValue = totalValue / arrays.Length;
            return (byte) averagePixelValue;
        }

        private static float[,] CreateCovarianceMatrix(List<int[]> matrix)
        {
            // convert list into 2D array
            var originalMatrix = matrix.ToArray();

            var transposeWidth = originalMatrix[0].Length;
            var transposeHeight = originalMatrix.Length;

            // transpose the array
            var transposedMatrix = new int[transposeWidth][];
            for(int i = 0; i < transposeWidth; i++)
            {
                transposedMatrix[i] = new int[transposeHeight];
            }
            
            for (int row = 0; row < transposeWidth; row++)
            {
                for (int column = 0; column < transposeHeight; column++)
                {
                    transposedMatrix[row][column] = originalMatrix[column][row];
                }
            }

            // multiply the 2
            float[,] finalCovMatrix = new float[transposeHeight, transposeHeight];

            // take first row from orig and times each element with corres element in first column of transpose
            // then first row from orig with next colmn... etc

            for (int row = 0; row < transposeHeight; row++)
            {
                for (int column = 0; column < transposeHeight; column++)
                {
                    var currentRow = GetMatrixRow(originalMatrix, row);
                    finalCovMatrix[row, column] = CalculateVectorProduct(GetMatrixColumn(transposedMatrix, column), currentRow);
                }
            }

            return finalCovMatrix;
        }

        private static int[] GetMatrixRow(int [][] matrix, int index)
        {
            int[] matrixRow = new int[matrix[index].Length];
            
            for(int i = 0; i < matrix[index].Length; i++)
            {
                matrixRow[i] = matrix[index][i];
            }

            return matrixRow;
        }

        private static float[] GetMatrixRow(float[,] eigenVectorMatrix, int index)
        {
            var arraySize = eigenVectorMatrix.GetLength(0);
            float[] matrixRow = new float[arraySize];

            for (int i = 0; i < arraySize; i++)
            {
                matrixRow[i] = eigenVectorMatrix[index, i];
            }

            return matrixRow;
        }

        private static int[] GetMatrixColumn(int [][] matrix, int index)
        {
            int[] matrixColumn = new int[matrix.Length];

            for (int i = 0; i < matrix.Length; i++)
            {
                matrixColumn[i] = matrix[i][index];
            }

            return matrixColumn;
        }

        private static int CalculateVectorProduct(int[] column, int[] row)
        {
            int totalValue = 0;

            for(int i = 0; i < column.Length; i++)
            {
                totalValue += (column[i] * row[i]);
            }

            return totalValue;
        }

        private static float CalculateVectorProduct(int[] column, float[] row)
        {
            float totalValue = 0;

            for (int i = 0; i < column.Length; i++)
            {
                totalValue += (column[i] * row[i]);
            }

            // VERY TEMP
            if(totalValue < 0)
            {
                totalValue = 0;
            }
            return totalValue;
        }

        private static void CalculateEigenFace(List<int[]> faceMatrix, Matrix<float> eigenVectors, int numberToCreate)
        {
            // eigenvectors are alaready in order, so first eigenvector will be first eigenface
            int numberOfColumns = faceMatrix[0].Length;
            var eigenVectorMatrix = eigenVectors.Data;
            var faceVectorArray = faceMatrix.ToArray();
            var eigenFaceVector = new byte[numberOfColumns];

            // used to calcualte the i'th eigenvector
            for(int i = 0; i < numberToCreate; i++)
            {
                var eigenVectorRow = GetMatrixRow(eigenVectorMatrix, i);

                for (int column = 0; column < numberOfColumns; column++)
                {
                    // we want the column of the faceVectorData and the Row of the eigenVectors
                    var faceVectorColumn = GetMatrixColumn(faceVectorArray, column);

                    eigenFaceVector[column] = (byte)CalculateVectorProduct(faceVectorColumn, eigenVectorRow);
                }

                Image<Gray, byte> eigenFaceJpeg = new Image<Gray, byte>(imageWidth, imageHeight)
                {
                    Bytes = eigenFaceVector
                };

                eigenFaceJpeg.Save(Path.Combine(rootFolder, "EigenFace" + i + ".jpg"));
            }
        }
    }
}
