﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Artis.Consts;
using Artis.Data;
using NLog;
using System.Drawing;

namespace Artis.Utils
{
    public class ImageHelper
    {
        /// <summary>
        /// Логгер
        /// </summary>
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        public static async Task<ObservableCollection<DataImage>> ConvertImages(IEnumerable<Data.Data> images)
        {
            ObservableCollection<DataImage> convertedImages = new ObservableCollection<DataImage>();
            foreach (Data.Data data in images)
            {
                Data.Data data1 = data;

                BitmapImage convertedImage = GetImage(data1.Base64StringData);

                if (convertedImage != null)
                    convertedImages.Add(new DataImage(data.ID, convertedImage, data.Base64StringData));
            }
            return convertedImages;
        }

        /// <summary>
        /// Конвертирование Base64String в BitmapImage
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static BitmapImage GetImage(string data)
        {
            try
            {
                byte[] binaryData = Convert.FromBase64String(data);

                var bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = new MemoryStream(binaryData);
                bi.EndInit();
                return bi;
            }
            catch (Exception ex)
            {
                _logger.ErrorException("Ошибка конвертирования изображения из строки в BitmapImage", ex);
                return null;
            }
        }

        public static BitmapImage ResizeImage(Stream stream, int widthImage,string fileName)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                BitmapImage bitMapImage = new BitmapImage();
                bitMapImage.BeginInit();
                bitMapImage.StreamSource = new MemoryStream(memoryStream.ToArray());
                //http://msdn.microsoft.com/ru-ru/library/aa970271(v=vs.110).aspx
                // Note: In order to preserve aspect ratio, set DecodePixelWidth
                // or DecodePixelHeight but not both.
                if (bitMapImage.DecodePixelWidth==0 || bitMapImage.DecodePixelWidth > widthImage)
                    bitMapImage.DecodePixelWidth = widthImage;
                bitMapImage.CacheOption = BitmapCacheOption.OnDemand;
                //bitMapImage.CreateOptions = BitmapCreateOptions.DelayCreation;
                bitMapImage.UriSource = new Uri(fileName);
                bitMapImage.EndInit();
                return bitMapImage;
            }
        }

        public static string ConvertImageToBase64String(BitmapImage image)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BitmapEncoder encoder;
                if (Path.GetExtension(image.UriSource.AbsoluteUri).ToLower() == ".png")
                    encoder = new PngBitmapEncoder();
                else
                    encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(memoryStream);
                byte[] bitmapdata = memoryStream.ToArray();

                return Convert.ToBase64String(bitmapdata);
            }
        }

        //private static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        //{
        //    // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

        //    using (MemoryStream outStream = new MemoryStream())
        //    {
        //        BitmapEncoder enc = new BmpBitmapEncoder();
        //        enc.Frames.Add(BitmapFrame.Create(bitmapImage));
        //        enc.Save(outStream);
        //        Bitmap bitmap = new Bitmap(outStream);
        //        return new Bitmap(bitmap);
        //    }
        //}
        //public static string ResizeAndConvertImageToBase64String(Stream stream)
        //{
        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        //stream.Seek(0, SeekOrigin.Begin);
        //        BitmapImage bitmapImage=ResizeImage(stream, ImageConsts.WidthConst);
        //        bitmapImage.StreamSource.CopyTo(memoryStream);
        //        byte[] convertedImageArray = memoryStream.ToArray();
        //        string base64String = Convert.ToBase64String(convertedImageArray, 0, convertedImageArray.Length);
        //        return base64String;
        //    }
        //}

        //public static void ResizeImage(Stream input, Stream output, int width)
        //{
        //    BitmapImage image = ResizeImage(input, width);
        //    //using (var image = Image.FromStream(input))
        //    //{
        //    //    if (image.Width > width)
        //    //    {
        //    //        //определяем пропорции
        //    //        int scale;
        //    //        if (image.Width > image.Height)
        //    //            scale = image.Width/image.Height;
        //    //        else
        //    //            scale = image.Height/image.Width;

        //    //        int height = width/scale;


        //    //        using (var bmp = new Bitmap(width, height))
        //    //        {
        //    //            using (var gr = Graphics.FromImage(bmp))
        //    //            {
        //    //                gr.CompositingQuality = CompositingQuality.HighSpeed;
        //    //                gr.SmoothingMode = SmoothingMode.HighSpeed;
        //    //                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //    //                gr.DrawImage(image, new Point(0, 0));
        //    //                bmp.Save(output, ImageFormat.Png);
        //    //            }
        //    //        }
        //    //    }
        //    //    else
        //    //    {
        //    //        input.Seek(0, SeekOrigin.Begin);
        //    //        input.CopyTo(output);
        //    //        output.Seek(0, SeekOrigin.Begin);
        //    //    }
        //    //}
        //}

        public static BitmapImage GetImages(Stream stream)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                BitmapImage bitMapImage = new BitmapImage();
                bitMapImage.BeginInit();
                bitMapImage.StreamSource = new MemoryStream(memoryStream.ToArray());
                bitMapImage.EndInit();
                return bitMapImage;
            }
        }
    }
}
