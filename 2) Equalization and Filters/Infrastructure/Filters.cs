using System;
using System.Drawing;

namespace Equalization_and_Filters.Infrastructure
{
    public class Filters
    {
        public static Bitmap Equalize(Bitmap bmp)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int bytes = bmpData.Stride * bmp.Height;
            byte[] grayValues = new byte[bytes];
            int[] R = new int[256];
            byte[] N = new byte[256];
            byte[] left = new byte[256];
            byte[] right = new byte[256];
            System.Runtime.InteropServices.Marshal.Copy(ptr, grayValues, 0, bytes);
            for (int i = 0; i < grayValues.Length; ++i) ++R[grayValues[i]];
            int z = 0;
            int Hint = 0;
            int Havg = grayValues.Length / R.Length;
            for (int i = 0; i < N.Length - 1; ++i)
            {
                N[i] = 0;
            }
            for (int j = 0; j < R.Length; ++j)
            {
                if (z > 255) left[j] = 255;
                else left[j] = (byte)z;
                Hint += R[j];
                while (Hint > Havg)
                {
                    Hint -= Havg;
                    z++;
                }
                if (z > 255) right[j] = 255;
                else right[j] = (byte)z;

                N[j] = (byte)((left[j] + right[j]) / 2);
            }
            for (int i = 0; i < grayValues.Length; ++i)
            {
                if (left[grayValues[i]] == right[grayValues[i]]) grayValues[i] = left[grayValues[i]];
                else grayValues[i] = N[grayValues[i]];
            }

            System.Runtime.InteropServices.Marshal.Copy(grayValues, 0, ptr, bytes);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        public static Bitmap Roberts(Bitmap original)
        {
            Bitmap result = new Bitmap(original.Width, original.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int width = original.Width;
            int height = original.Height;

            int[,] GX = new int[,]
            {
                { 0, -1 },
                { 1,  0 }
            };
            int[,] GY = new int[,]
            {
                { -1, 0 },
                {  0, 1 }
            };

            int[,] R = new int[width, height];
            int[,] G = new int[width, height];
            int[,] B = new int[width, height];
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    R[i, j] = original.GetPixel(i, j).R;
                    G[i, j] = original.GetPixel(i, j).G;
                    B[i, j] = original.GetPixel(i, j).B;
                }
            }

            int Rx = 0, Ry = 0;
            int Gx = 0, Gy = 0;
            int Bx = 0, By = 0;
            int RChannel, GChannel, BChannel;
            for (int i = 1; i < original.Width - 1; ++i)
            {
                for (int j = 1; j < original.Height - 1; ++j)
                {

                    Rx = 0;
                    Ry = 0;
                    Gx = 0;
                    Gy = 0;
                    Bx = 0;
                    By = 0;
                    RChannel = 0;
                    GChannel = 0;
                    BChannel = 0;
                    for (int x = -1; x < 1; ++x)
                    {
                        for (int y = -1; y < 1; ++y)
                        {
                            RChannel = R[i + y, j + x];
                            Rx += GX[x + 1, y + 1] * RChannel;
                            Ry += GY[x + 1, y + 1] * RChannel;

                            GChannel = G[i + y, j + x];
                            Gx += GX[x + 1, y + 1] * GChannel;
                            Gy += GY[x + 1, y + 1] * GChannel;

                            BChannel = B[i + y, j + x];
                            Bx += GX[x + 1, y + 1] * BChannel;
                            By += GY[x + 1, y + 1] * BChannel;
                        }
                    }
                    result.SetPixel(i, j, Color.FromArgb(SatureCast(Rx + Ry), SatureCast(Gx + Gy), SatureCast(Bx + By)));
                }
            }
            return result;

        }

        public static Bitmap Previt(Bitmap original)
        {
            Bitmap result = new Bitmap(original.Width, original.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int width = original.Width;
            int height = original.Height;

            int[,] GX = new int[,] 
            { 
                { 1, 0, -1 },
                { 1, 0, -1 },
                { 1, 0, -1 }
            };
            int[,] GY = new int[,] 
            {
                { -1, -1, -1 },
                { 0, 0, 0 },
                { 1, 1, 1 }
            };

            int[,] R = new int[width, height];
            int[,] G = new int[width, height];
            int[,] B = new int[width, height];
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    R[i, j] = original.GetPixel(i, j).R;
                    G[i, j] = original.GetPixel(i, j).G;
                    B[i, j] = original.GetPixel(i, j).B;
                }
            }

            int Rx = 0, Ry = 0;
            int Gx = 0, Gy = 0;
            int Bx = 0, By = 0;
            int RChannel, GChannel, BChannel;
            for (int i = 1; i < original.Width - 1; ++i)
            {
                for (int j = 1; j < original.Height - 1; ++j)
                {

                    Rx = 0;
                    Ry = 0;
                    Gx = 0;
                    Gy = 0;
                    Bx = 0;
                    By = 0;
                    RChannel = 0;
                    GChannel = 0;
                    BChannel = 0;
                    for (int x = -1; x < 2; ++x)
                    {
                        for (int y = -1; y < 2; ++y)
                        {
                            RChannel = R[i + y, j + x];
                            Rx += GX[x + 1, y + 1] * RChannel;
                            Ry += GY[x + 1, y + 1] * RChannel;

                            GChannel = G[i + y, j + x];
                            Gx += GX[x + 1, y + 1] * GChannel;
                            Gy += GY[x + 1, y + 1] * GChannel;

                            BChannel = B[i + y, j + x];
                            Bx += GX[x + 1, y + 1] * BChannel;
                            By += GY[x + 1, y + 1] * BChannel;
                        }
                    }
                    result.SetPixel(i, j, Color.FromArgb(SatureCast(Rx + Ry), SatureCast(Gx + Gy), SatureCast(Bx + By)));
                }
            }
            return result;
        }

        public static Bitmap Sobel(Bitmap original)
        {
            Bitmap result = new Bitmap(original.Width, original.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            int width = original.Width;
            int height = original.Height;

            int[,] GX = new int[,]
            {
                { 1, 0, -1 },
                { 2, 0, -2 },
                { 1, 0, -1 }
            };
            int[,] GY = new int[,]
            {
                { -1, -2, -1 },
                {  0,  0,  0 },
                {  1,  2,  1 }
            };

            int[,] R = new int[width, height];
            int[,] G = new int[width, height];
            int[,] B = new int[width, height];
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    R[i, j] = original.GetPixel(i, j).R;
                    G[i, j] = original.GetPixel(i, j).G;
                    B[i, j] = original.GetPixel(i, j).B;
                }
            }

            int Rx = 0, Ry = 0;
            int Gx = 0, Gy = 0;
            int Bx = 0, By = 0;
            int RChannel, GChannel, BChannel;
            for (int i = 1; i < original.Width - 1; ++i)
            {
                for (int j = 1; j < original.Height - 1; ++j)
                {

                    Rx = 0;
                    Ry = 0;
                    Gx = 0;
                    Gy = 0;
                    Bx = 0;
                    By = 0;
                    RChannel = 0;
                    GChannel = 0;
                    BChannel = 0;
                    for (int x = -1; x < 2; ++x)
                    {
                        for (int y = -1; y < 2; ++y)
                        {
                            RChannel = R[i + y, j + x];
                            Rx += GX[x + 1, y + 1] * RChannel;
                            Ry += GY[x + 1, y + 1] * RChannel;

                            GChannel = G[i + y, j + x];
                            Gx += GX[x + 1, y + 1] * GChannel;
                            Gy += GY[x + 1, y + 1] * GChannel;

                            BChannel = B[i + y, j + x];
                            Bx += GX[x + 1, y + 1] * BChannel;
                            By += GY[x + 1, y + 1] * BChannel;
                        }
                    }
                    result.SetPixel(i, j, Color.FromArgb(SatureCast(Rx+Ry), SatureCast(Gx + Gy), SatureCast(Bx + By)));
                }
            }
            return result;
        }

        private static int SatureCast(double number)
        {
            if(number < 0)
            {
                return 0;
            }
            if(number > 255)
            {
                return 255;
            }
            return (int)number;
        }
    }
}
