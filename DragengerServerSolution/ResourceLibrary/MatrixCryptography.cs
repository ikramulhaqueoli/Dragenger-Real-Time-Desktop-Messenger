using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceLibrary
{
    public static class MatrixCryptography
    {
        private static readonly double [,]cryptoMatrix;

        static MatrixCryptography()
        {
            cryptoMatrix = new double[3, 3] {
                                                {-3, 8, -5},
                                                {2, -6, 3},
                                                {-1, 2, 0}  };
            MatrixCryptography.CryptoMatrix = MatrixCryptography.cryptoMatrix;
            MatrixCryptography.InverseMatrix = MatrixCryptography.CalculateInverseMatrix();
        }

        private static double[,] CryptoMatrix
        {
            set;
            get;
        }

        private static double[,] InverseMatrix
        {
            set;
            get;
        }

        public static List<double> Encrypt(string input)        //returns a double(datatype) type list encrypted data
        {
            int[] assignedNumbers = new int[input.Length];
            for ( int i = 0; i < input.Length; i++ )
            {
                assignedNumbers[i] = (int)input[i];
            }

            int row = input.Length / 3;
            if(input.Length % 3 != 0) row++;
            int[,] inputMatrix = new int[row, 3];
            for (int i = 0, k = 0; i < row; i++)
            {
                for (int j = 0; j < 3; j++, k++)
                {
                    if (k < input.Length) inputMatrix[i, j] = assignedNumbers[k];
                    else inputMatrix[i, j] = 0;
                }
            }

            //Matrix multiplication
            double[,] outputMatrix = new double[row, 3];
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    outputMatrix[i, j] = 0;
                    for (int k = 0; k < 3; k++)
                    {
                        outputMatrix[i, j] += (inputMatrix[i, k] * MatrixCryptography.InverseMatrix[k, j]);
                    }
                }
            }

            List<double> output = new List<double>();
            for (int i = 0, k = 0; i < row; i++)
            {
                for (int j = 0; j < 3; j++, k++)
                {
                    output.Add(outputMatrix[i, j]);
                }
            }

            return output;
        }

        public static string Decrypt(List<double> inputList)            //returns a string(datatype) type decrypted data
        {
            int row = inputList.Count / 3;
            if (inputList.Count % 3 != 0) row++;
            double[,] inputMatrix = new double[row, 3];
            for (int i = 0, k = 0; i < row; i++)
            {
                for (int j = 0; j < 3; j++, k++)
                {
                    inputMatrix[i, j] = inputList[k];
                }
            }

            //Matrix multiplication
            double[,] outputMatrix = new double[row, 3];
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    outputMatrix[i, j] = 0;
                    for (int k = 0; k < 3; k++)
                    {
                        outputMatrix[i, j] += (inputMatrix[i, k] * MatrixCryptography.CryptoMatrix[k, j]);
                    }
                }
            }

            string output = "";
            for (int i = 0, k = 0; i < row; i++)
            {
                for (int j = 0; j < 3; j++, k++)
                {
                    if ((int)outputMatrix[i, j] == 0) return output;
                    output += (char)((int)outputMatrix[i, j]);
                }
            }

            return output;
        }

        //supplementary methods
        private static double[,] CalculateInverseMatrix()
        {
            double[,] inverseMatrix = new double[3, 3];
            inverseMatrix[0, 0] = (cryptoMatrix[1, 1] * cryptoMatrix[2, 2]) - (cryptoMatrix[1, 2] * cryptoMatrix[2, 1]);
            inverseMatrix[0, 1] = (cryptoMatrix[1, 2] * cryptoMatrix[2, 0]) - (cryptoMatrix[1, 0] * cryptoMatrix[2, 2]);
            inverseMatrix[0, 2] = (cryptoMatrix[1, 0] * cryptoMatrix[2, 1]) - (cryptoMatrix[1, 1] * cryptoMatrix[2, 0]);

            inverseMatrix[1, 0] = (cryptoMatrix[0, 2] * cryptoMatrix[2, 1]) - (cryptoMatrix[0, 1] * cryptoMatrix[2, 2]);
            inverseMatrix[1, 1] = (cryptoMatrix[0, 0] * cryptoMatrix[2, 2]) - (cryptoMatrix[0, 2] * cryptoMatrix[2, 0]);
            inverseMatrix[1, 2] = (cryptoMatrix[0, 1] * cryptoMatrix[2, 0]) - (cryptoMatrix[0, 0] * cryptoMatrix[2, 1]);

            inverseMatrix[2, 0] = (cryptoMatrix[0, 1] * cryptoMatrix[1, 2]) - (cryptoMatrix[0, 2] * cryptoMatrix[1, 1]);
            inverseMatrix[2, 1] = (cryptoMatrix[0, 2] * cryptoMatrix[1, 0]) - (cryptoMatrix[0, 0] * cryptoMatrix[1, 2]);
            inverseMatrix[2, 2] = (cryptoMatrix[0, 0] * cryptoMatrix[1, 1]) - (cryptoMatrix[0, 1] * cryptoMatrix[1, 0]);

            for (int i = 0; i < 3; i++)
            {
                for (int j = i + 1; j < 3; j++)
                {
                    Universal.Swap(ref inverseMatrix[i, j], ref inverseMatrix[j, i]);
                }
            }

            double m1 = cryptoMatrix[0, 0] * ((cryptoMatrix[1, 1] * cryptoMatrix[2, 2]) - (cryptoMatrix[1, 2] * cryptoMatrix[2, 1]));
            double m2 = cryptoMatrix[0, 1] * ((cryptoMatrix[1, 0] * cryptoMatrix[2, 2]) - (cryptoMatrix[1, 2] * cryptoMatrix[2, 0]));
            double m3 = cryptoMatrix[0, 2] * ((cryptoMatrix[1, 0] * cryptoMatrix[2, 1]) - (cryptoMatrix[1, 1] * cryptoMatrix[2, 0]));
            double determinant = m1 - m2 + m3;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    inverseMatrix[i, j] /= determinant;
                }
            }

            return inverseMatrix;
        }

        private static void ShowInverseMatrix()
        {
            string text = "";
            for (int i = 0; i < InverseMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < InverseMatrix.GetLength(1); j++)
                {
                    text += InverseMatrix[i, j] + " ";
                }
                text += "\r\n";
            }
        }
    }
}
