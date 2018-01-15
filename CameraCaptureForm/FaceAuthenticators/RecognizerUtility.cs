﻿using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.IO;

namespace FaceAuthenticators
{
    public class RecognizerUtility
    {
        // Emgu CV requires these to be divisible by 4
        public static int imageHeight = 300;
        public static int imageWidth = 252;
        public static string rootFolder = @"C:\Users\RockInTheBox\Documents\University\Project\TestEnrolLocation";

        public static void GetAllImageVectorsAndLabels(string folderLocation, ref List<int[]> faceMatrix, ref List<string> faceLabels)
        {
            // Decided to keep the vector and label grabbing in 1 method, so no chance of mismatch of order

            // We loop through the root directory and get images from each folder, using the folder as the name
            foreach (var directory in Directory.GetDirectories(folderLocation))
            {
                // loop through the directory getting each file
                foreach (var file in Directory.GetFiles(directory))
                {
                    Image<Gray, byte> imageFile = new Image<Gray, byte>(file);

                    var imageVector = ImageToVector(imageFile);

                    // adding the unfolded image to the 'matrix'
                    faceMatrix.Add(imageVector);
                    // adding the folder name to the label 'matrix'
                    faceLabels.Add(Path.GetFileName(Path.GetDirectoryName(file)));
                }
            }
        }

        public static int[] ImageToVector(Image<Gray, byte> imageToConvert)
        {
            var pixelData = imageToConvert.Data;

            // unfold them
            var imageVector = new int[imageWidth * imageHeight];
            int count = 0;

            for (int row = imageToConvert.Rows - 1; row >= 0; row--)
            {
                for (int column = imageToConvert.Cols - 1; column >= 0; column--)
                {
                    imageVector[count] = pixelData[row, column, 0];
                    count++;
                }
            }

            // need to reverse the image vector before adding
            Array.Reverse(imageVector);

            return imageVector;
        }

        public static byte[] GetAverageFace(List<int[]> faceMatrix)
        {
            var arrays = faceMatrix.ToArray();
            byte[] averageFace = new byte[imageWidth * imageHeight];

            for (int i = 0; i < averageFace.Length; i++)
            {
                averageFace[i] = GetAverageArrayValue(arrays, i);
            }
            return averageFace;
        }

        public static byte GetAverageArrayValue(int[][] arrays, int index)
        {
            int totalValue = 0;

            for (int i = 0; i < arrays.Length; i++)
            {
                totalValue += arrays[i][index];
            }
            int averagePixelValue = totalValue / arrays.Length;
            return (byte)averagePixelValue;
        }

        public static float[,] CreateCovarianceMatrix(List<int[]> matrix)
        {
            // convert list into 2D array
            var originalMatrix = matrix.ToArray();

            var transposeWidth = originalMatrix[0].Length;
            var transposeHeight = originalMatrix.Length;

            // transpose the array
            var transposedMatrix = new int[transposeWidth][];
            for (int i = 0; i < transposeWidth; i++)
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

        public static int[] GetMatrixRow(int[][] matrix, int index)
        {
            int[] matrixRow = new int[matrix[index].Length];

            for (int i = 0; i < matrix[index].Length; i++)
            {
                matrixRow[i] = matrix[index][i];
            }

            return matrixRow;
        }

        public static float[] GetMatrixRow(float[,] eigenVectorMatrix, int index)
        {
            var arraySize = eigenVectorMatrix.GetLength(0);
            float[] matrixRow = new float[arraySize];

            for (int i = 0; i < arraySize; i++)
            {
                matrixRow[i] = eigenVectorMatrix[index, i];
            }

            return matrixRow;
        }

        internal static float[] GetMatrixRow(float[][] matrix, int index)
        {
            float[] matrixRow = new float[matrix[index].Length];

            for (int i = 0; i < matrix[index].Length; i++)
            {
                matrixRow[i] = matrix[index][i];
            }

            return matrixRow;
        }

        public static int[] GetMatrixColumn(int[][] matrix, int index)
        {
            int[] matrixColumn = new int[matrix.Length];

            for (int i = 0; i < matrix.Length; i++)
            {
                matrixColumn[i] = matrix[i][index];
            }

            return matrixColumn;
        }

        public static int CalculateVectorProduct(int[] column, int[] row)
        {
            int totalValue = 0;

            for (int i = 0; i < column.Length; i++)
            {
                totalValue += (column[i] * row[i]);
            }

            return totalValue;
        }

        public static float CalculateVectorProduct(int[] column, float[] row)
        {
            float totalValue = 0;

            for (int i = 0; i < column.Length; i++)
            {
                totalValue += (column[i] * row[i]);
            }

            /*
            // TODO: Check this is correct
            if (totalValue < 0)
            {
                totalValue = Math.Abs(totalValue);
            }
            */
            return totalValue;
        }
    }
}
