using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.CV.Structure;
using System.IO;

namespace FaceAuthenticators
{
    public class EigenfaceAuthenticator
    {

        // Creates the eigenfaces
        public static void TrainEigenfaceAutrhenticator()
        {
            // get the faces to train on
            String rootFolder = @"C:\Users\RockInTheBox\Documents\University\Project\DatabaseImages\s01";
            List<byte[]> faceMatrix = new List<byte[]>();

            // loop through the directory getting each file
            foreach(var file in Directory.GetFiles(rootFolder))
            {
                Image<Gray, byte> imageFile = new Image<Gray, byte>(file);
                var pixelData = imageFile.Data;

                // unfold them
                var imageVector = new byte[640 * 480];
                int count = 0;

                for (int row = imageFile.Rows - 1; row >= 0; row--)
                {
                    for (int column = imageFile.Cols - 1; column >= 0; column--)
                    {
                        imageVector[count] = pixelData[row, column, 0];
                        count++;
                    }
                }

                // adding the unfolded image to the 'matrix'
                faceMatrix.Add(imageVector);
            }
            
            // create a matrix of each face vector

            // PCA on the matrix
            var averageFace = getAverageFace(faceMatrix);
            Image<Gray, byte> averageFaceJpeg = new Image<Gray, byte>(640, 480);
            averageFaceJpeg.Bytes = averageFace;

            averageFaceJpeg.Save(Path.Combine(rootFolder, "AverageFace.jpg"));
            // Refold to create the Eigenfaces

            
        }

        private static byte[] getAverageFace(List<byte[]> faceMatrix)
        {
            var arrays = faceMatrix.ToArray();
            byte[] averageFace = new byte[640*480];

            for (int i = 0; i < averageFace.Length; i++)
            {
                averageFace[i] = getAverageArrayValue(arrays, i);
                //sums[i] = (array1[i] + array2[i] + array3[i] + array4[i]) / 4;
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
