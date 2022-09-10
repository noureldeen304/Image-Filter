using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace ImageFilters
{
    public class ImageOperations
    {
        /// <summary>
        /// Open an image, convert it to gray scale and load it into 2D array of size (Height x Width)
        /// </summary>
        /// <param name="ImagePath">Image file path</param>
        /// <returns>2D array of gray values</returns>
        public static byte[,] OpenImage(string ImagePath)
        {
            Bitmap original_bm = new Bitmap(ImagePath);
            int Height = original_bm.Height;
            int Width = original_bm.Width;

            byte[,] Buffer = new byte[Height, Width];

            unsafe
            {
                BitmapData bmd = original_bm.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, original_bm.PixelFormat);
                int x, y;
                int nWidth = 0;
                bool Format32 = false;
                bool Format24 = false;
                bool Format8 = false;

                if (original_bm.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    Format24 = true;
                    nWidth = Width * 3;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format32bppArgb || original_bm.PixelFormat == PixelFormat.Format32bppRgb || original_bm.PixelFormat == PixelFormat.Format32bppPArgb)
                {
                    Format32 = true;
                    nWidth = Width * 4;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    Format8 = true;
                    nWidth = Width;
                }
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (y = 0; y < Height; y++)
                {
                    for (x = 0; x < Width; x++)
                    {
                        if (Format8)
                        {
                            Buffer[y, x] = p[0];
                            p++;
                        }
                        else
                        {
                            Buffer[y, x] = (byte)((int)(p[0] + p[1] + p[2]) / 3);
                            if (Format24) p += 3;
                            else if (Format32) p += 4;
                        }
                    }
                    p += nOffset;
                }
                original_bm.UnlockBits(bmd);
            }

            return Buffer;
        }

        /// <summary>
        /// Get the height of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Height</returns>
        public static int GetHeight(byte[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(0);
        }

        /// <summary>
        /// Get the width of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Width</returns>
        public static int GetWidth(byte[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(1);
        }

        /// <summary>
        /// Display the given image on the given PictureBox object
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <param name="PicBox">PictureBox object to display the image on it</param>
        public static void DisplayImage(byte[,] ImageMatrix, PictureBox PicBox)
        {
            // Create Image:
            //==============
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            Bitmap ImageBMP = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            unsafe
            {
                BitmapData bmd = ImageBMP.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, ImageBMP.PixelFormat);
                int nWidth = 0;
                nWidth = Width * 3;
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        p[0] = p[1] = p[2] = ImageMatrix[i, j];
                        p += 3;
                    }

                    p += nOffset;
                }
                ImageBMP.UnlockBits(bmd);
            }
            PicBox.Image = ImageBMP;
        }
        public static void Swap(byte [] arr,int i,int j)
        {
            byte temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
        }
        public static int partion(byte[] arr, int low, int high)
        {
            int p = low, i = low, j = high;
            while (i < j)
            {
                while (i < high && arr[i] <= arr[p])
                    i++;
                while (j > 0 && arr[j] > arr[p])
                    j--;
                if (i < j)
                    Swap(arr, i, j);
            }
            Swap(arr, j, p);
            return j;
        }
        public static void quickSort(byte[] arr, int low, int high)
        {
            if (low >= high)
                return;
            int p = partion(arr, low, high);
            quickSort(arr, low, p - 1);
            quickSort(arr, p + 1, high);
        }
        public static int RANDOMIZED_SELECT_Small(byte []arr,int low,int high,int i)
        {
            if (low >= high)
                return low;
            int pivot = partion(arr, low, high);
            int k = pivot - low + 1;
            if (k == i)
                return pivot;
            else if (i < k)
                return RANDOMIZED_SELECT_Small(arr, low, pivot - 1, i);
            else
                return RANDOMIZED_SELECT_Small(arr, pivot + 1, high, i);
        }
        public static int RANDOMIZED_SELECT_Large(byte[] arr, int p, int r, int i)
        {
            if (p >= r)
                return p;
            int q = partion(arr, p, r);
            int k = r - q + 1;
            if (k == i)
                return q;
            else if (i < k)
                return RANDOMIZED_SELECT_Large(arr, q + 1, r, i);
            else
                return RANDOMIZED_SELECT_Large(arr, p, q - 1, i);
        }
        public static void CountingSort(byte []arr)
        {
            int n = arr.Length;
            int[] freq = new int[256];
            byte[] extra = new byte[n];
            for (int i = 0; i < 256; i++)
                freq[i] = 0;
            for (int i = 0; i < n; i++)
                freq[arr[i]]++;
            for (int i = 1; i <= 255; i++)
                freq[i] += freq[i - 1];
            for (int i = 0; i < n; i++)
            {
                extra[freq[arr[i]] - 1] = arr[i];
                freq[arr[i]]--;
            }
            for (int i = 0; i < n; i++)
                arr[i] = extra[i];
        }
        public static void Neighboring(byte [,] ImageMatrix,int i,int j,int sz, byte[] Pixels)
        {
            int width = GetWidth(ImageMatrix);
            int height = GetHeight(ImageMatrix);
            int a1 = i - (sz / 2);
            int b1 = j - (sz / 2);
            int a2 = i + (sz / 2);
            int b2 = j + (sz / 2);
            int z = 0;
            for(int r = a1;r <= a2; r++)
            {
                for(int q = b1; q <= b2; q++)
                {
                    if (r >= 0 && r < height && q >= 0 && q < width)
                    {
                        Pixels[z] = ImageMatrix[r, q];
                        z++;
                    }
                }
            }
        }
        public static int CalaulateAvg(byte[] arr,int s,int e)
        {
            int res = 0, num = 1;
            for (int i = s; i < e; i++,num++)
                res += arr[i];
            res /= num;
            return res;
        }
        public static byte [,] AlphaTrimFileter(byte[,] ImageMatrix,int WindowSize,int T,int Type)
        {
            int width = GetWidth(ImageMatrix);
            int height = GetHeight(ImageMatrix);
            byte[] Pixels = new byte[WindowSize * WindowSize];
            byte [,] NewImageMatrix = new byte[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Neighboring(ImageMatrix, i, j, WindowSize, Pixels);
                    if (Type == 1)
                    {
                        CountingSort(Pixels);
                        int res = CalaulateAvg(Pixels, T, Pixels.Length - T);
                        NewImageMatrix[i, j] = (byte)res;
                    }
                    else
                    {
                        int s = RANDOMIZED_SELECT_Small(Pixels, 0, Pixels.Length - 1, T);
                        int e = RANDOMIZED_SELECT_Large(Pixels, 0, Pixels.Length - 1, T);
                        int res = CalaulateAvg(Pixels, s, e);
                        NewImageMatrix[i, j] = (byte)res;
                    }
                    
                }
            }
            return NewImageMatrix;
        }
        public static byte[,] MedianFilter(byte[,] ImageMatrix,int WS,int Type)
        {
            int width = GetWidth(ImageMatrix);
            int height = GetHeight(ImageMatrix);
            byte [,] NewImageMatrix = new byte[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int sz = 3;
                    while (true)
                    {
                        byte[] Pixels = new byte[sz * sz];
                        Neighboring(ImageMatrix, i, j, sz, Pixels);
                        if (Type == 1)
                            CountingSort(Pixels);
                        else
                            quickSort(Pixels, 0, Pixels.Length - 1);
                        byte zxy = ImageMatrix[i, j];
                        byte zmax = Pixels[Pixels.Length - 1];
                        byte zmin = Pixels[0];
                        byte zmed = Pixels[Pixels.Length / 2];
                        byte A1 = (byte)(zmed - zmin);
                        byte A2 = (byte)(zmax - zmed);
                        if (A1 > 0 && A2 > 0)
                        {
                            byte B1 = (byte)(zxy - zmin);
                            byte B2 = (byte)(zmax - zxy);
                            if (B1 > 0 && B2 > 0)
                                NewImageMatrix[i, j] = zxy;
                            else
                                NewImageMatrix[i, j] = zmed;
                            break;
                        }
                        else
                        {
                            sz += 2;
                            if (sz > WS)
                            {
                                NewImageMatrix[i, j] = zmed;
                                break;
                            }
                        }
                    }
                }
            }
            return NewImageMatrix;
        }
    }
}
