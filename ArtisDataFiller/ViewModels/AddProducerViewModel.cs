using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Artis.Consts;
using Artis.Data;
using Artis.Utils;
using Microsoft.Win32;

namespace Artis.ArtisDataFiller.ViewModels
{
    public class AddProducerViewModel : DialogViewModel
    {
        private Producer _producer;
        private ObservableCollection<DataImage> _images;
        private List<long> _deletedImages;
        private List<DataImage> _addedImages;
        private DataImage _selectedImage;


        /// <summary>
        /// Команда добавления картинки для мероприятия
        /// </summary>
        public ArtisCommand AddImageCommand { get; private set; }

        /// <summary>
        /// Команда удаления картинки для мероприятия
        /// </summary>
        public ArtisCommand RemoveImageCommand { get; private set; }

        /// <summary>
        /// ФИО режисера
        /// </summary>
        public string Name
        {
            get { return _producer.FIO; }
            set
            {
                _producer.FIO = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Наименование на инглише
        /// </summary>
        public string EnglishName
        {
            get { return _producer.EnglishFIO; }
            set
            {
                _producer.EnglishFIO = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description
        {
            get { return _producer.Description; }
            set
            {
                _producer.Description = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Описание на инглише
        /// </summary>
        public string EnglishDescription
        {
            get { return _producer.EnglishDescription; }
            set
            {
                _producer.EnglishDescription = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Список картинок для редактируемого/создаваемого актера
        /// </summary>
        public ObservableCollection<DataImage> Images
        {
            get { return _images; }
            set
            {
                _images = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Выделенная картинка
        /// </summary>
        public DataImage SelectedImage
        {
            get { return _selectedImage; }
            set
            {
                _selectedImage = value;
                OnPropertyChanged();
            }
        }

        public Producer Producer
        {
            get { return _producer; }
            set
            {
                _producer = value;
                InitImagesDataSource();
                OnPropertyChanged();
            }
        }

        public AddProducerViewModel()
        {
            _producer = new Producer();
            AddImageCommand = new ArtisCommand(CanExecute, ExecuteAddImagesCommand);
            RemoveImageCommand = new ArtisCommand(CanExecuteRemoveImageCommand, ExecuteRemoveImagesCommand);
        }

        private async Task InitImagesDataSource()
        {
            if (Producer.Data != null)
                Images = await ImageHelper.ConvertImages(Producer.Data);
        }

        public override bool CanExecuteOkCommand(object parameter)
        {
            return !string.IsNullOrEmpty(Name);
        }
        private bool CanExecute(object parameters)
        {
            return true;
        }

        public async override void ExecuteOkCommand(object parameter)
        {
//            _producer = new Producer() {FIO = Name};
            //await ProducerRepository.AddProducer(_producer, _action);
        }

        private bool CanExecuteRemoveImageCommand(object parameter)
        {
            return SelectedImage != null;
        }

        private void ExecuteAddImagesCommand(object parameter)
        {
            OpenFileDialog openDialog = new OpenFileDialog
            {
                Filter = "jpg(*.jpg)|*.jpg|jpeg(*.jpeg)|*.jpeg|png(*.png)|*.png",
                Title = "Пожалуйста, выберите необхожимые изображения.",
                Multiselect = true
            };

            if (openDialog.ShowDialog().Value)
            {
                Stream[] selectedFiles = openDialog.OpenFiles();
                foreach (Stream file in selectedFiles)
                {
                    //MemoryStream stream = new MemoryStream();
                    //file.CopyTo(stream);
                    //file.Close();
                    //byte[] imageArray = stream.ToArray();
                    //string base64String = Convert.ToBase64String(imageArray, 0, imageArray.Length);

                    //stream.Seek(0, SeekOrigin.Begin);
                    //BitmapImage bitMapImage = new BitmapImage();
                    //bitMapImage.BeginInit();
                    //bitMapImage.StreamSource = stream;
                    //bitMapImage.EndInit();
                    BitmapImage bitMapImage = ImageHelper.ResizeImage(file, ImageConsts.WidthConst,openDialog.FileName);
                    string base64String = ImageHelper.ConvertImageToBase64String(bitMapImage);
                    file.Close();

                    if (!string.IsNullOrEmpty(base64String))
                    {
                        Data.Data image = new Data.Data() { Base64StringData = base64String };
                        if (Producer.Data == null)
                            Producer.Data = new Collection<Data.Data>();
                        Producer.Data.Add(image);
                        if (Images == null)
                            Images = new ObservableCollection<DataImage>();
                        Images.Add(new DataImage() { Base64String = base64String, Image = bitMapImage });
                        OnPropertyChanged("Images");
                    }
                }
            }
        }


        private void ExecuteRemoveImagesCommand(object parameter)
        {
            if (Images == null)
                Images = new ObservableCollection<DataImage>();

            if (Producer.Data.Any(i => i.Base64StringData.Equals(SelectedImage.Base64String)))
            {
                Data.Data image = Producer.Data.First(i => i.Base64StringData.Equals(SelectedImage.Base64String));
                Producer.Data.Remove(image);
            }

            Images.Remove(Images.First(i => i.Base64String.Equals(SelectedImage.Base64String)));

            OnPropertyChanged("Images");
        }
    }
}
