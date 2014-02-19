using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Artis.Consts;
using Artis.Data;

namespace Artis.ArtisDataFiller.ViewModels
{
    public sealed class AreasViewModel: ViewModel
    {
        private string _filterName;
        private ObservableCollection<Area> _areas;
        private Area _selectedItem;
        private string _editName;
        private ObservableCollection<BitmapImage> _images;
        private BitmapImage _selectedImage;
        private bool _isEdit;

        /// <summary>
        /// Команда редактирования площадки
        /// </summary>
        public ArtisCommand EditCommand { private set; get; }

        /// <summary>
        /// Команда добавления новых картинок
        /// </summary>
        public ArtisCommand AddImagesCommand { private set; get; }

        /// <summary>
        /// Команда удаления новых картинок
        /// </summary>
        public ArtisCommand RemoveImagesCommand { private set; get; }

        /// <summary>
        /// Команда сохранения площадки после редактирования/создания
        /// </summary>
        public ArtisCommand SaveCommand { private set; get; }

        /// <summary>
        /// Команда возврата с карточки площадки на просмотр всех площадок
        /// </summary>
        public ArtisCommand BackCommand { private set; get; }

        /// <summary>
        /// Команда добавления новой площадки
        /// </summary>
        public ArtisCommand NewAreaCommand { private set; get; }

        /// <summary>
        /// Поле-фильтр по наименованию
        /// </summary>
        public string FilterName
        {
            get { return _filterName; }
            set
            {
                _filterName = value;
                if (value == "1!")
                    SelectedItem = null;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Коллекция загруженных площадок
        /// todo Макс, вставь нужный тип ObservableCollection здесь 
        /// </summary>
        public ObservableCollection<Area> Areas
        {
            get { return _areas; }
            set
            {
                _areas = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Выделенный элемент в списке
        /// </summary>
        public Area SelectedItem  
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value; 
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Поле - наименование редактируемой или создаваемой площадки
        /// </summary>
        public string EditName
        {
            get { return _editName; }
            set
            {
                _editName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// true - если сейчас редактируется площадка.
        /// false - создается новая
        /// <remarks>Данный флаг необходим для ГУИ, чтобы различать - когда давать редактировать наименование площадки,
        /// а когда нет</remarks>
        /// </summary>
        public bool IsEdit
        {
            get { return _isEdit; }
            set
            {
                _isEdit = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Список картинок для редактируемой/создаваемой площадки
        /// </summary>
        public ObservableCollection<BitmapImage> Images
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
        public BitmapImage SelectedImage    
        {
            get { return _selectedImage; }
            set
            {
                _selectedImage = value; 
                OnPropertyChanged();
            }
        }

        public AreasViewModel()
        {
            EditCommand = new ArtisCommand(CanExecuteEditCommand, ExecuteEditCommand);
            AddImagesCommand = new ArtisCommand(CanExecuteEditCommand, ExecuteAddImagesCommand);
            RemoveImagesCommand = new ArtisCommand(CanExecuteEditCommand, ExecuteRemoveImagesCommand);
            SaveCommand = new ArtisCommand(CanExecuteEditCommand, ExecuteSaveCommand);
            BackCommand = new ArtisCommand(CanExecuteEditCommand, ExecuteBackCommand);                        
            NewAreaCommand = new ArtisCommand(CanExecuteEditCommand, ExecuteNewAreaCommand);                        

            FilterName = "some string";
            Areas = new ObservableCollection<Area>();
            Areas.Add(new Area{Name = "1"});
            Areas.Add(new Area
            {
                Name = "2",
                Data = new Collection<Data.Data>
                {
                    new Data.Data {Base64StringData = "askldjflkajsdklfjaklsjdf"},
                    new Data.Data {Base64StringData = "askldjflkajsdklfjaklsjdf"},
                    new Data.Data {Base64StringData = "askldjflkajsdklfjaklsjdf"},
                    new Data.Data {Base64StringData = "askldjflkajsdklfjaklsjdf"},
                    new Data.Data {Base64StringData = "askldjflkajsdklfjaklsjdf"}
                }
            });
            Areas.Add(new Area{Name = "3"});
            Areas.Add(new Area{Name = "4"});
            OnPropertyChanged("Areas");

            SelectedItem = Areas[1];

            EditName = "edit text";
            IsEdit = true;

            Images = new ObservableCollection<BitmapImage>();
        }

        private bool CanExecuteEditCommand(object parameter)
        {
            return true;
        }

        private void ExecuteNewAreaCommand(object parameter)
        {
            IsEdit = false; // устанавливаем флаг
        }

        private void ExecuteBackCommand(object parameter)
        {
            //todo возврат на просмотр всех площадок
        }

        private void ExecuteSaveCommand(object parameter)
        {
            //todo сохранение площадки после редактирования/создания
        }

        private void ExecuteAddImagesCommand(object parameter)
        {
            //todo открыть диалог, выбрать картинки, добавить их в Images
        }

        private void ExecuteRemoveImagesCommand(object parameter)
        {
            //todo удалить SelectedImage
        }

        private void ExecuteEditCommand(object parameter)
        {
            IsEdit = true; // устанавливаем флаг
            EditName = string.Empty;

            //todo WPF не может отображать base64 пикчи.
            //todo тебе нужно асинхронно конвертить их и отправлять в массив.
            //todo пример реализации ниже

            ConvertImages();
        }

        /// <summary>
        /// Асинхронная конвертация картинок из base64 в BitmapImage
        /// </summary>
        private async void ConvertImages()
        {
            if (SelectedItem != null && SelectedItem.Data != null)
            {
                foreach (Data.Data data in SelectedItem.Data)
                {
                    Data.Data data1 = data;
                    var f = await Task<BitmapImage>.Factory.StartNew(() => GetImage(data1.Base64StringData));

                    if (f != null)
                    {
                        Images.Add(f);
                        OnPropertyChanged("Images");
                    }
                }
            }
        }

        private BitmapImage GetImage(string data)
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
            catch (Exception)
            {

                return null;
            }
        }
    }
}
