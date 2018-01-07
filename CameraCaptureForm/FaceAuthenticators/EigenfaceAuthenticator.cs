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

        static byte[] test = new byte[72000];

        // Creates the eigenfaces
        public static void TrainEigenfaceAuthenticator()
        {
            // get the faces to train on
            //String rootFolder = @"C:\Users\RockInTheBox\Documents\University\Project\DatabaseImages\s01";
            String rootFolder = @"C:\Users\RockInTheBox\Documents\University\Project\TestEnrolLocation";

            List<byte[]> faceMatrix = new List<byte[]>();

            // loop through the directory getting each file
            foreach(var file in Directory.GetFiles(rootFolder))
            {
                Image<Gray, byte> imageFile = new Image<Gray, byte>(file);
                var pixelData = imageFile.Data;

                // unfold them
                var imageVector = new byte[imageWidth * imageHeight];
                int count = 0;

                for (int row = imageFile.Rows - 1; row >= 0; row--)
                {
                    for (int column = imageFile.Cols - 1; column >= 0; column--)
                    {
                        imageVector[count] = pixelData[row, column, 0];
                        count++;
                    }
                }
                test = imageVector;
                // adding the unfolded image to the 'matrix'
                faceMatrix.Add(imageVector);
            }
            
            // create a matrix of all face vectors

            // PCA on the matrix
            var averageFace = getAverageFace(faceMatrix);

            // Image appears reversed otherwise, need to ensure this doesn't affect other calculations
            Array.Reverse(averageFace);

            Image<Gray, byte> averageFaceJpeg = new Image<Gray, byte>(imageWidth, imageHeight)
            {
                Bytes = averageFace
            };

            averageFaceJpeg.Save(Path.Combine(rootFolder, "AverageFace.jpg"));


            // Refold to create the Eigenfaces

            
        }

        private static byte[] getAverageFace(List<byte[]> faceMatrix)
        {
            var arrays = faceMatrix.ToArray();
            byte[] averageFace = new byte[imageWidth * imageHeight];

            for (int i = 0; i < averageFace.Length; i++)
            {
                averageFace[i] = getAverageArrayValue(arrays, i);
            }
            return averageFace;
        }

        private static byte getAverageArrayValue(byte[][] arrays, int index)
        {
            int totalValue = 0;

            for(int i = 0; i < arrays.Length; i++)
            {
                totalValue += arrays[i][index];
            }
            int averagePixelValue = totalValue / arrays.Length;
            return (byte) averagePixelValue;
        }
    }
}
