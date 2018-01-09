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
        // Emgu CV requires these to be divisable by 4
        public static int imageHeight = 300;
        public static int imageWidth = 252;

        // Creates the eigenfaces
        public static void TrainEigenfaceAuthenticator()
        {
            // get the faces to train on
            //string rootFolder = @"C:\Users\RockInTheBox\Documents\University\Project\DatabaseImages\s01";
            string rootFolder = @"C:\Users\RockInTheBox\Documents\University\Project\TestEnrolLocation";

            List<int[]> faceMatrix = new List<int[]>();

            // loop through the directory getting each file
            foreach(var file in Directory.GetFiles(rootFolder))
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
            
            // create a matrix of all face vectors

            // PCA on the matrix
            var averageFace = getAverageFace(faceMatrix);

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
            var covMatrix = createCovarianceMatrix(faceMatrix);
            

            // Refold to create the Eigenfaces

            Console.WriteLine("");
        }

        private static byte[] getAverageFace(List<int[]> faceMatrix)
        {
            var arrays = faceMatrix.ToArray();
            byte[] averageFace = new byte[imageWidth * imageHeight];

            for (int i = 0; i < averageFace.Length; i++)
            {
                averageFace[i] = getAverageArrayValue(arrays, i);
            }
            return averageFace;
        }

        private static byte getAverageArrayValue(int[][] arrays, int index)
        {
            int totalValue = 0;

            for(int i = 0; i < arrays.Length; i++)
            {
                totalValue += arrays[i][index];
            }
            int averagePixelValue = totalValue / arrays.Length;
            return (byte) averagePixelValue;
        }

        private static int[,] createCovarianceMatrix(List<int[]> matrix)
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
            int[,] finalCovMatrix = new int[transposeHeight, transposeHeight];

            // take first row from orig and times each element with corres element in first column of transpose
            // then first row from orig with next colmn... etc

            for (int row = 0; row < transposeHeight; row++)
            {
                for (int column = 0; column < transposeHeight; column++)
                {
                    var currentRow = getMatrixRow(originalMatrix, row);
                    finalCovMatrix[row, column] = calculateVectorProduct(getMatrixColumn(transposedMatrix, column), currentRow);
                }
            }

            return finalCovMatrix;
        }

        private static int[] getMatrixRow(int [][] matrix, int index)
        {
            int[] matrixRow = new int[matrix[index].Length];
            
            for(int i = 0; i < matrix[index].Length; i++)
            {
                matrixRow[i] = matrix[index][i];
            }

            return matrixRow;
        }

        private static int[] getMatrixColumn(int [][] matrix, int index)
        {
            int[] matrixColumn = new int[matrix.Length];

            for (int i = 0; i < matrix.Length; i++)
            {
                matrixColumn[i] = matrix[i][index];
            }

            return matrixColumn;
        }

        private static int calculateVectorProduct(int[] column, int[] row)
        {
            int totalValue = 0;

            for(int i = 0; i < column.Length; i++)
            {
                totalValue += (column[i] * row[i]);
            }

            return totalValue;
        }
    }
}
