using Equalization_and_Filters.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows;

namespace Equalization_and_Filters.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Bitmap input;
        private Bitmap equalized;
        private Bitmap roberts;
        private Bitmap previt;
        private Bitmap sobel;

        private BitmapImage inputImage;
        private BitmapSource equalizedImage;
        private BitmapSource robertsImage;
        private BitmapSource previtImage;
        private BitmapSource sobelImage;
        private string imageLocation;

        public BitmapImage InputImage
        {
            get
            {
                return inputImage;
            }
            set
            {
                inputImage = value;
                OnPropertyChanged(nameof(InputImage));
            }
        }
        public BitmapSource EqualizedImage
        {
            get
            {
                return equalizedImage;
            }
            set
            {
                equalizedImage = value;
                OnPropertyChanged(nameof(EqualizedImage));
            }
        }
        public BitmapSource RobertsImage
        {
            get
            {
                return robertsImage;
            }
            set
            {
                robertsImage = value;
                OnPropertyChanged(nameof(RobertsImage));
            }
        }
        public BitmapSource PrevitImage
        {
            get
            {
                return previtImage;
            }
            set
            {
                previtImage = value;
                OnPropertyChanged(nameof(PrevitImage));
            }
        }
        public BitmapSource SobelImage
        {
            get
            {
                return sobelImage;
            }
            set
            {
                sobelImage = value;
                OnPropertyChanged(nameof(SobelImage));
            }
        }
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

        public ICommand ChooseImageCommand { get; private set; }
        public ICommand EqualizationCommand { get; private set; }
        public ICommand SaveEqualizedCommand { get; private set; }
        public ICommand RobertsCommand { get; private set; }
        public ICommand PrevitCommand { get; private set; }
        public ICommand SobelCommand { get; private set; }
        public ICommand SaveRobertsCommand { get; private set; }
        public ICommand SavePrevitCommand { get; private set; }
        public ICommand SaveSobelCommand { get; private set; }

        public MainViewModel()
        {
            ChooseImageCommand = new Command(ChooseImage);
            EqualizationCommand = new Command(Equalization);
            SaveEqualizedCommand = new Command(SaveEqualized);
            RobertsCommand = new Command(Roberts);
            PrevitCommand = new Command(Previt);
            SobelCommand = new Command(Sobel);
            SaveRobertsCommand = new Command(SaveRoberts);
            SavePrevitCommand = new Command(SavePrevit);
            SaveSobelCommand = new Command(SaveSobel);
        }

        private void ChooseImage(object parametr)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "Картинки|*.jpg;*.jpeg;*.png;*.bmp;*.tiff";
            if (ofd.ShowDialog() ?? true)
            {
                input = (Bitmap)Image.FromFile(ofd.FileName);
                InputImage = new BitmapImage(new Uri(ofd.FileName));
                ImageLocation = ofd.FileName;
            }
        }

        private void Equalization(object parametr)
        {
            Bitmap copy = input.Clone(new Rectangle(0, 0, input.Width, input.Height), input.PixelFormat);
            equalized = Filters.Equalize(copy);
            EqualizedImage = equalized.ConvertToBitmapSource();
        }

        private void Roberts(object parametr)
        {
            Bitmap copy = input.Clone(new Rectangle(0, 0, input.Width, input.Height), input.PixelFormat);
            roberts = Filters.Roberts(copy);
            RobertsImage = roberts.ConvertToBitmapSource();
        }


        private void Previt(object parametr)
        {
            Bitmap copy = input.Clone(new Rectangle(0, 0, input.Width, input.Height), input.PixelFormat);
            previt = Filters.Previt(copy);
            PrevitImage = previt.ConvertToBitmapSource();
        }

        private void Sobel(object parametr)
        {
            Bitmap copy = input.Clone(new Rectangle(0, 0, input.Width, input.Height), input.PixelFormat);
            sobel = Filters.Sobel(copy);
            SobelImage = sobel.ConvertToBitmapSource();
        }

        private void SaveEqualized(object parametr)
        {
            SaveImage(equalized);
        }

        private void SaveRoberts(object parametr)
        {
            SaveImage(roberts);
        }

        private void SavePrevit(object parametr)
        {
            SaveImage(previt);
        }

        private void SaveSobel(object parametr)
        {
            SaveImage(sobel);
        }

        private void SaveImage(Bitmap image)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "Картинки|*.jpg;*.jpeg;*.png;*.bmp;*.tiff";
            if (sfd.ShowDialog() ?? true)
            {
                image.Save(sfd.FileName);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
