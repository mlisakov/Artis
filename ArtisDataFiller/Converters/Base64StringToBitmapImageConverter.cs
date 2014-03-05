using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
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
                return new ObservableCollection<DataImage>();
            ICollection<Data.Data> images = (ICollection<Data.Data>) value;
            Task<ObservableCollection<DataImage>> convertedImagesTask = Task.Run(
                () =>
                {
                    Task<ObservableCollection<DataImage>> task = ConvertImages(images);
                    task.Wait();
                    return task.Result;
                });
            convertedImagesTask.Wait();
            return convertedImagesTask.Result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private async Task<ObservableCollection<DataImage>> ConvertImages(IEnumerable<Data.Data> images)
        {
            ObservableCollection<DataImage> convertedImages = new ObservableCollection<DataImage>();
            foreach (Data.Data data in images)
            {
                Data.Data data1 = data;

                BitmapImage convertedImage = await GetImage(data1.Base64StringData);

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
        private async Task<BitmapImage> GetImage(string data)
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
