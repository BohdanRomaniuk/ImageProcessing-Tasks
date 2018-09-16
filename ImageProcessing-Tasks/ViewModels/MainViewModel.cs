using ImageProcessing_Tasks.Infrastructure;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageProcessing_Tasks.ViewModels
{
    public class MainViewModel: INotifyPropertyChanged
    {
        private string imageLocation;
        private long sizeBeforeCompressing;
        private long sizeAlfterCompressing;
        private Bitmap Image;
        private BitmapImage bmpImage;

        public string ImageLocation
        {
            get
            {
                return imageLocation;
            }
            set
            {
                imageLocation = value;
                OnPropertyChanged(nameof(ImageLocation));
            }
        }
        public long SizeBeforeCompressing
        {
            get
            {
                return sizeBeforeCompressing;
            }
            set
            {
                sizeBeforeCompressing = value;
                OnPropertyChanged(nameof(SizeBeforeCompressing));
            }
        }
        public long SizeAfterCompressing
        {
            get
            {
                return sizeAlfterCompressing;
            }
            set
            {
                sizeAlfterCompressing = value;
                OnPropertyChanged(nameof(SizeAfterCompressing));
            }
        }
        public BitmapImage BmpImage
        {
            get
            {
                return bmpImage;
            }
            set
            {
                bmpImage = value;
                OnPropertyChanged(nameof(BmpImage));
            }
        }

        public ICommand ChooseImageCommand { get; private set; }
        public ICommand SaveAsBMPRLECommand { get; private set; }
        public ICommand SaveAsTIFFCommand { get; private set; }
        public ICommand SaveAsJPEGCommand { get; private set; }

        public ICommand DifferenceJPEGCommand { get; private set; }

        public MainViewModel()
        {
            ChooseImageCommand = new Command(ChooseImage);
            SaveAsBMPRLECommand = new Command(SaveAsBMPRLE);
            SaveAsTIFFCommand = new Command(SaveAsTIFF);
            SaveAsJPEGCommand = new Command(SaveAsJPEG);

            DifferenceJPEGCommand = new Command(DifferenceJPEG);
        }

        private void ChooseImage(object parametr)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "bmp(*.bmp)|*.bmp";
            if (ofd.ShowDialog() ?? true)
            {
                BmpImage = new BitmapImage(new Uri(ofd.FileName));
                Image = new Bitmap(ofd.FileName);
                ImageLocation = ofd.FileName;
                SizeBeforeCompressing = new FileInfo(ofd.FileName).Length;
            }
        }

        private void SaveAsBMPRLE(object parametr)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "bmp(*.bmp)|*.bmp";
            if (sfd.ShowDialog() ?? true)
            {
                ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/bmp"); ;
                Encoder myEncoder = Encoder.Compression;
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, (long)EncoderValue.CompressionRle);
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                myEncoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
                Image.Save(sfd.FileName, myImageCodecInfo, myEncoderParameters);
                SizeAfterCompressing = new FileInfo(sfd.FileName).Length;
            }
        }

        private void SaveAsTIFF(object parametr)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "tif(*.tif)|*.tif";
            if (sfd.ShowDialog() ?? true)
            {
                ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/tiff"); ;
                Encoder myEncoder = Encoder.Compression;
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, (long)EncoderValue.CompressionLZW);
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                myEncoderParameters.Param[0] = myEncoderParameter;
                Image.Save(sfd.FileName, myImageCodecInfo, myEncoderParameters);
                SizeAfterCompressing = new FileInfo(sfd.FileName).Length;
            }
        }

        private void SaveAsJPEG(object parametr)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "jpeg(*.jpeg)|*.jpeg";
            if (sfd.ShowDialog() ?? true)
            {
                ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg"); ;
                Encoder myEncoder = Encoder.Compression;
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, (long)EncoderValue.CompressionNone);
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                myEncoderParameters.Param[0] = myEncoderParameter;
                Image.Save(sfd.FileName, myImageCodecInfo, myEncoderParameters);
                SizeAfterCompressing = new FileInfo(sfd.FileName).Length;
            }
        }

        private void DifferenceJPEG(object parametr)
        {
            Bitmap first = Image;
            Bitmap second = new Bitmap("D:/c.jpeg");
            Bitmap dif = GetDiffBitmap(first, second);
            int witdh = first.Width;
            int height = first.Height;
            for(int i=0; i<witdh; ++i)
            {
                for(int j=0; j<height; ++j)
                {
                    //int color = first.GetPixel(i, j).ToArgb() - second.GetPixel(i, j).ToArgb();
                    //dif.SetPixel(i, j, Color.FromArgb(color));
                    int color = first.GetPixel(i, j).G - second.GetPixel(i, j).G;
                    dif.SetPixel(i, j, Color.FromArgb(color));
                }
            }
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "bmp(*.bmp)|*.bmp";
            if (sfd.ShowDialog() ?? true)
            {
                dif.Save(sfd.FileName);
            }
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        private unsafe Bitmap GetDiffBitmap(Bitmap bmp, Bitmap bmp2)
        {
            if (bmp.Width != bmp2.Width || bmp.Height != bmp2.Height)
                throw new Exception("Sizes must be equal.");

            Bitmap bmpRes = null;

            System.Drawing.Imaging.BitmapData bmData = null;
            System.Drawing.Imaging.BitmapData bmData2 = null;
            System.Drawing.Imaging.BitmapData bmDataRes = null;

            try
            {
                bmpRes = new Bitmap(bmp.Width, bmp.Height);

                bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                bmData2 = bmp2.LockBits(new Rectangle(0, 0, bmp2.Width, bmp2.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                bmDataRes = bmpRes.LockBits(new Rectangle(0, 0, bmpRes.Width, bmpRes.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                IntPtr scan0 = bmData.Scan0;
                IntPtr scan02 = bmData2.Scan0;
                IntPtr scan0Res = bmDataRes.Scan0;

                int stride = bmData.Stride;
                int stride2 = bmData2.Stride;
                int strideRes = bmDataRes.Stride;

                int nWidth = bmp.Width;
                int nHeight = bmp.Height;

                //for(int y = 0; y < nHeight; y++)
                System.Threading.Tasks.Parallel.For(0, nHeight, y =>
                {
                    //define the pointers inside the first loop for parallelizing
                    byte* p = (byte*)scan0.ToPointer();
                    p += y * stride;
                    byte* p2 = (byte*)scan02.ToPointer();
                    p2 += y * stride2;
                    byte* pRes = (byte*)scan0Res.ToPointer();
                    pRes += y * strideRes;

                    for (int x = 0; x < nWidth; x++)
                    {
                        //always get the complete pixel when differences are found
                        if (p[0] != p2[0] || p[1] != p2[1] || p[2] != p2[2])
                        {
                            pRes[0] = p2[0];
                            pRes[1] = p2[1];
                            pRes[2] = p2[2];

                            //alpha (opacity)
                            pRes[3] = p2[3];
                        }

                        p += 4;
                        p2 += 4;
                        pRes += 4;
                    }
                });

                bmp.UnlockBits(bmData);
                bmp2.UnlockBits(bmData2);
                bmpRes.UnlockBits(bmDataRes);
            }
            catch
            {
                if (bmData != null)
                {
                    try
                    {
                        bmp.UnlockBits(bmData);
                    }
                    catch
                    {

                    }
                }

                if (bmData2 != null)
                {
                    try
                    {
                        bmp2.UnlockBits(bmData2);
                    }
                    catch
                    {

                    }
                }

                if (bmDataRes != null)
                {
                    try
                    {
                        bmpRes.UnlockBits(bmDataRes);
                    }
                    catch
                    {

                    }
                }

                if (bmpRes != null)
                {
                    bmpRes.Dispose();
                    bmpRes = null;
                }
            }

            return bmpRes;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
