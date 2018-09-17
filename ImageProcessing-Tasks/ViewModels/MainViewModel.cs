using ImageProcessing_Tasks.Infrastructure;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace ImageProcessing_Tasks.ViewModels
{
    public class MainViewModel: INotifyPropertyChanged
    {
        private string imageLocation;
        private long sizeBeforeCompressing;
        private long sizeAlfterCompressing;
        private Image BmpImage;
        private Image TiffImage;
        private Image JpegImage;
        private BitmapImage dispayImage;

        private RGB color;
        private int writingTime;
        private int readingTime;
        private double encodingTime;
        private double decodingTime;

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
        public BitmapImage DisplayImage
        {
            get
            {
                return dispayImage;
            }
            set
            {
                dispayImage = value;
                OnPropertyChanged(nameof(DisplayImage));
            }
        }

        public RGB Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
                OnPropertyChanged(nameof(Color));
            }
        }
        public int ReadingTime
        {
            get
            {
                return readingTime;
            }
            set
            {
                readingTime = value;
                OnPropertyChanged(nameof(ReadingTime));
            }
        }
        public double DecodingTime
        {
            get
            {
                return decodingTime;
            }
            set
            {
                decodingTime = value;
                OnPropertyChanged(nameof(DecodingTime));
            }
        }
        public double EncodingTime
        {
            get
            {
                return encodingTime;
            }
            set
            {
                encodingTime = value;
                OnPropertyChanged(nameof(EncodingTime));
            }
        }
        public int WritingTime
        {
            get
            {
                return writingTime;
            }
            set
            {
                writingTime = value;
                OnPropertyChanged(nameof(WritingTime));
            }
        }

        public ICommand ChooseImageCommand { get; private set; }
        public ICommand SaveAsBMPRLECommand { get; private set; }
        public ICommand SaveAsTIFFCommand { get; private set; }
        public ICommand SaveAsJPEGCommand { get; private set; }

        public ICommand BmpDiffTiffCommand { get; private set; }
        public ICommand BmpDiffJpegCommand { get; private set; }
        public ICommand TiffDiffJpegCommand { get; private set; }

        public MainViewModel()
        {
            ChooseImageCommand = new Command(ChooseImage);

            SaveAsBMPRLECommand = new Command(SaveAsBMPRLE);
            SaveAsTIFFCommand = new Command(SaveAsTIFF);
            SaveAsJPEGCommand = new Command(SaveAsJPEG);

            BmpDiffTiffCommand = new Command(BmpDiffTiff);
            BmpDiffJpegCommand = new Command(BmpDiffJpeg);
            TiffDiffJpegCommand = new Command(TiffDiffJpeg);
        }

        private void ChooseImage(object parametr)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "bmp(*.bmp)|*.bmp";
            if (ofd.ShowDialog() ?? true)
            {
                DisplayImage = new BitmapImage(new Uri(ofd.FileName));
                BmpImage = Image.FromFile(ofd.FileName);
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
                myEncoderParameters.Param[0] = myEncoderParameter;

                Stopwatch timer = new Stopwatch();
                timer.Start();
                BmpImage.Save(sfd.FileName, ImageFormat.Bmp);
                timer.Stop();
                WritingTime = timer.Elapsed.Milliseconds;

                timer.Restart();
                BmpImage = Image.FromFile(sfd.FileName);
                timer.Stop();
                ReadingTime = timer.Elapsed.Milliseconds;

                timer.Restart();
                DisplayImage = new BitmapImage(new Uri(sfd.FileName));
                timer.Stop();
                DecodingTime = timer.Elapsed.TotalMilliseconds;

                timer.Restart();
                BitmapImage encoded = DisplayImage;
                timer.Stop();
                EncodingTime = timer.Elapsed.TotalMilliseconds;

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

                Stopwatch timer = new Stopwatch();
                timer.Start();
                BmpImage.Save(sfd.FileName, ImageFormat.Tiff);
                timer.Stop();
                WritingTime = timer.Elapsed.Milliseconds;

                timer.Restart();
                TiffImage = Image.FromFile(sfd.FileName);
                timer.Stop();
                ReadingTime = timer.Elapsed.Milliseconds;

                timer.Restart();
                DisplayImage = new BitmapImage(new Uri(sfd.FileName));
                timer.Stop();
                DecodingTime = timer.Elapsed.TotalMilliseconds;

                timer.Restart();
                BitmapImage encoded = DisplayImage;
                timer.Stop();
                EncodingTime = timer.Elapsed.TotalMilliseconds;

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

                Stopwatch timer = new Stopwatch();
                timer.Start();
                BmpImage.Save(sfd.FileName, ImageFormat.Jpeg);
                timer.Stop();
                WritingTime = timer.Elapsed.Milliseconds;

                timer.Restart();
                JpegImage = Image.FromFile(sfd.FileName);
                timer.Stop();
                ReadingTime = timer.Elapsed.Milliseconds;

                timer.Restart();
                DisplayImage = new BitmapImage(new Uri(sfd.FileName));
                timer.Stop();
                DecodingTime = timer.Elapsed.TotalMilliseconds;

                timer.Restart();
                BitmapImage encoded = DisplayImage;
                timer.Stop();
                EncodingTime = timer.Elapsed.TotalMilliseconds;

                SizeAfterCompressing = new FileInfo(sfd.FileName).Length;
            }
        }

        private void BmpDiffTiff(object parametr)
        {
            RGB bmpRGB = GetImageRGB(BmpImage);
            RGB tiffRGB = GetImageRGB(TiffImage);
            Color = bmpRGB - tiffRGB;
        }

        private void BmpDiffJpeg(object parametr)
        {
            RGB bmpRGB = GetImageRGB(BmpImage);
            RGB jpegRGB = GetImageRGB(JpegImage);
            Color = bmpRGB - jpegRGB;
        }

        private void TiffDiffJpeg(object parametr)
        {
            RGB tiffRGB = GetImageRGB(TiffImage);
            RGB jpegRGB = GetImageRGB(JpegImage);
            Color = tiffRGB - jpegRGB;
        }

        private static RGB GetImageRGB(Image img)
        {
            Bitmap image = new Bitmap(img);
            int red = 0, blue = 0, green = 0;
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color pixel = image.GetPixel(x, y);
                    red += pixel.R;
                    green += pixel.G;
                    blue += pixel.B;
                }
            }
            return new RGB(red, green, blue);
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (int j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                {
                    return encoders[j];
                }
            }
            return null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
