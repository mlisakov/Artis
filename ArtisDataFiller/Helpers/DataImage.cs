using System.Windows.Media.Imaging;

namespace Artis.ArtisDataFiller
{
    public class DataImage
    {
        /// <summary>
        /// Идентификатор изображения
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// Изображение в виде Base64String
        /// </summary>
        public string Base64String { get; set; }
        /// <summary>
        /// Изображение
        /// </summary>
        public BitmapImage Image { get; set; }

        public DataImage(long id, BitmapImage image,string base64String)
        {
            ID = id;
            Image = image;
            Base64String = base64String;
        }

        public DataImage()
        {
        }
    }
}
