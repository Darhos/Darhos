using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.IO;    
using System.Drawing;
using System.Windows.Media;

namespace Image.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        //Proměnné
        private string _Image;
        private BitmapImage src;
        int width, height;
        int[,] array;


        //Commandy
        public Command LoadImage { get; set; }
        public Command RedChannel { get; set; }
        public Command GreenChannel { get; set; }
        public Command GreyFilter { get; set; }
        public Command Pixelization { get; set; }
        public Command YellowSubmarine { get; set; }


        public BitmapImage Src
        {
            get { return src; }
            set
            {
                src = value;
                ChangeProperty("Src");
            }
        }
        public int[,] Array
        {
            get { return BitmapImageToArray(Src); }
        }
        public string Image
        {
            get { return _Image; }
            set
            {
                _Image = value;
                ChangeProperty("Image");
            }
        }

        public MainViewModel()
        {
            LoadImage = new Command(LoadImageExecute);
            RedChannel = new Command(RedChannelExecute);
            GreenChannel = new Command(GreenChannelExecute);
            //GreyFilter = new Command(GreyFilterExecute);
            //Pixelization = new Command(PixelizationExecute);
            YellowSubmarine = new Command(YellowSubmarineExecute);
        }

        //Načtení obrázku
        public void LoadImageExecute()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = ".jpg";
            ofd.Filter = "Obrázek (.jpg)|*.jpg|Všechny soubory|*.*";
            if (ofd.ShowDialog() == true)
            {
                Image = ofd.FileName;
                src = new BitmapImage();
                src.BeginInit();
                src.UriSource = new Uri(Image, UriKind.Relative);
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();
                ChangeProperty("Src");
            }
        }

        //Odstíny červené
        public void RedChannelExecute()
        {
            if (src != null)
            {
                width = src.PixelWidth;
                height = src.PixelHeight;
                int o_kolik = 50;
                int cervene_zesileni = o_kolik << 16;
                array = BitmapImageToArray(Src);

                for (int x = 0; x < height; x++)
                    for (int y = 0; y < width; y++)
                    {
                        int soucet = (array[x, y] & 0xFF0000) + cervene_zesileni;
                        soucet >>= 16;
                        if (soucet >= 255)
                        {
                            soucet = 255;
                        }
                        soucet <<= 16;
                        array[x, y] = array[x, y] + soucet;
                    }
                src = Array2DToBitmapImage(array);
                ChangeProperty("Src");
            }
        }

        //Odstíny zelené
        public void GreenChannelExecute()
        {
            if (src != null)
            {
                width = src.PixelWidth;
                height = src.PixelHeight;
                int o_kolik = 50;
                int zelene_zesileni = o_kolik << 16;
                array = BitmapImageToArray(Src);

                for (int x = 0; x < height; x++)
                    for (int y = 0; y < width; y++)
                    {
                        int soucet = (array[x, y] & 0x00FF00) + zelene_zesileni;
                        soucet >>= 16;
                        if (soucet >= 255)
                        {
                            soucet = 255;
                        }
                        soucet <<= 16;
                        array[x, y] = array[x, y] + soucet;
                    }
                src = Array2DToBitmapImage(array);
                ChangeProperty("Src");
            }
        }

        //Šedý filtr
        /*public void GreyFilterExecute()
        {
            if (src != null)
            {
                width = src.PixelWidth;
                height = src.PixelHeight;
                array = BitmapImageToArray(Src);

                for (int x = 0; x < src.PixelWidth; x++)
                    for (int y = 0; y < src.Width; y++)
                    {
                        Color c = src.GetPixel(x, y);

                        //Apply conversion equation
                        byte gray = (byte)(.21 * c.R + .71 * c.G + .071 * c.B);

                        array[x, y] = array[x, y];
                    }
                src = Array2DToBitmapImage(array);
                ChangeProperty("Src");
            }
        }

        //Pixelizace obrázku
        public void PixelizationExecute()
        {
            if (src != null)
            {
                var sum = Pixel[0, 0];
                for (y = 0; y < Height; y++)
                {
                    for (x = 0; x < Width + 1; x++)
                    {
                        if (x % k == 0)
                        {
                            sum /= k;
                            for (xl = Max(0, x - k); xl < x; xl++)
                                Pixel[y, xl] = sum;
                            sum = 0;
                        }
                        if (x == Width)
                            break;
                        sum += Pixel[y, x];
                    }
                }
            }
        }*/

        public void YellowSubmarineExecute()
        {
            if (src != null)
            {
                width = src.PixelWidth;
                height = src.PixelHeight;
                array = BitmapImageToArray(src);

                for (int x = 0; x < height; x++)
                {
                    for (int y = 0; y < width; y++)
                    {
                        array[x, y] = 69721 - array[x, y];
                    }
                        
                }      
                src = Array2DToBitmapImage(array);
                ChangeProperty("Src");
            }
        }

        public int[,] BitmapImageToArray(BitmapImage image)
        {
            int[,] array = new int[image.PixelHeight, image.PixelWidth];

            WriteableBitmap wb = new WriteableBitmap(image);
            int width = wb.PixelWidth;
            int height = wb.PixelHeight;
            int bytesPerPixel = (wb.Format.BitsPerPixel + 7) / 8;
            int stride = wb.BackBufferStride;
            wb.Lock();
            unsafe
            {
                byte* pImgData = (byte*)wb.BackBuffer;
                int cRowStart = 0;
                int cColStart = 0;
                for (int row = 0; row < height; row++)
                {
                    cColStart = cRowStart;
                    for (int col = 0; col < width; col++)
                    {
                        byte* bPixel = pImgData + cColStart;
                        //bPixel[0] // Blue
                        //bPixel[1] // Green
                        //bPixel[2] // Red
                        int pixel = bPixel[2]; //Red
                        pixel = (pixel << 8) + bPixel[1]; //Green
                        pixel = (pixel << 8) + bPixel[0]; //Blue
                        array[row, col] = pixel;

                        cColStart += bytesPerPixel;
                    }
                    cRowStart += stride;
                }
            }
            wb.Unlock();
            wb.Freeze();
            return array;
        }
        public BitmapImage Array2DToBitmapImage(int[,] array)
        {
            WriteableBitmap wb = new WriteableBitmap(src);
            int width = wb.PixelWidth;
            int height = wb.PixelHeight;
            int bytesPerPixel = (wb.Format.BitsPerPixel + 7) / 8;
            int stride = wb.BackBufferStride;
            wb.Lock();
            unsafe
            {
                byte* pImgData = (byte*)wb.BackBuffer;
                int cRowStart = 0;
                int cColStart = 0;
                for (int row = 0; row < height; row++)
                {
                    cColStart = cRowStart;
                    for (int col = 0; col < width; col++)
                    {
                        byte* bPixel = pImgData + cColStart;

                        bPixel[0] = (byte)((array[row, col] & 0xFF));// Blue
                        bPixel[1] = (byte)((array[row, col] & 0xFF00) >> 8);// Green
                        bPixel[2] = (byte)((array[row, col] & 0xFF0000) >> 16);// Red

                        cColStart += bytesPerPixel;
                    }
                    cRowStart += stride;
                }
            }
            wb.Unlock();
            wb.Freeze();
            return ConvertWriteableBitmapToBitmapImage(wb);
        }
        public BitmapImage ConvertWriteableBitmapToBitmapImage(WriteableBitmap wbm)
        {
            BitmapImage bmImage = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(wbm));
                encoder.Save(stream);
                bmImage.BeginInit();
                bmImage.CacheOption = BitmapCacheOption.OnLoad;
                bmImage.StreamSource = stream;
                bmImage.EndInit();
                bmImage.Freeze();
            }
            return bmImage;
        }
        private void ChangeProperty(string nazevVlastnosti)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(nazevVlastnosti));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
