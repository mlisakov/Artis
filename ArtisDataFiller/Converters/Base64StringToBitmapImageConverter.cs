using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using NLog;

namespace Artis.ArtisDataFiller
{
    public class Base64StringToBitmapImageConverter : IValueConverter
    {
        /// <summary>
        /// Логгер
        /// </summary>
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return new ObservableCollection<BitmapImage>();

            ObservableCollection<DataImage> convertedImages = new ObservableCollection<DataImage>();
            ICollection<Data.Data> images=(ICollection<Data.Data>) value;
                foreach (Data.Data data in images)
                {
                    Data.Data data1 = data;
                    BitmapImage convertedImage = GetImage(data1.Base64StringData);

                    if (convertedImage != null)
                        convertedImages.Add(new DataImage(data.ID,convertedImage,data.Base64StringData));
                }
                return convertedImages;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Конвертирование Base64String в BitmapImage
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private BitmapImage GetImage(string data)
        {
            try
            {
                byte[] binaryData = System.Convert.FromBase64String(data);

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
    }
}
