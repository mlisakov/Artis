using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Artis.Consts;
using Artis.Data;
using Microsoft.Win32;
using NLog;

namespace Artis.ArtisDataFiller.ViewModels
{
    public sealed class AreasViewModel: ViewModel
    {
        /// <summary>
        /// Логгер
        /// </summary>
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        private string _filterName;
       
        private Area _currentArea;
        //private DataImage _currentAreaImage;        
        private DataImage _selectedImage;
        private bool _isEdit;
        //private string _editName; 

        private ObservableCollection<Area> _areas;
        private ObservableCollection<DataImage> _images;
        private List<long> _deletedImages;
        private List<DataImage> _addedImages;

        /// <summary>
        /// Команда поиска
        /// </summary>
        public ArtisCommand SearchCommand { get; private set; }

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
        /// Команда удаления площадки
        /// </summary>
        public ArtisCommand RemoveCommand { private set; get; }

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
                    CurrentArea = null;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Коллекция загруженных площадок
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
        /// Текущая площадка(Выделенная в списке из существующих или новая)
        /// </summary>
        public Area CurrentArea  
        {
            get { return _currentArea; }
            set
            {
                _currentArea = value; 
                OnPropertyChanged();
            }
        }

        //public DataImage CurrentAreaImage  
        //{
        //    get { return _currentAreaImage; }
        //    set
        //    {
        //        _currentAreaImage = value; 
        //        OnPropertyChanged();
        //    }
        //}
        

        /// <summary>
        /// Поле - наименование редактируемой или создаваемой площадки
        /// </summary>
        //public string EditName
        //{
        //    get { return _editName; }
        //    set
        //    {
        //        _editName = value;
        //        OnPropertyChanged();
        //    }
        //}

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

        public AreasViewModel()
        {
            InitCommands();
            InitDataSource();
            InitVariables();

            #region

            //FilterName = "some string";
            //Areas = new ObservableCollection<Area>();
            //Areas.Add(new Area{Name = "1"});
            //Areas.Add(new Area
            //{
            //    Name = "2",
            //    Data = new Collection<Data.Data>
            //    {
            //        new Data.Data {Base64StringData = "askldjflkajsdklfjaklsjdf"},
            //        new Data.Data {Base64StringData = "askldjflkajsdklfjaklsjdf"},
            //        new Data.Data {Base64StringData = "askldjflkajsdklfjaklsjdf"},
            //        new Data.Data {Base64StringData = "askldjflkajsdklfjaklsjdf"},
            //        new Data.Data {Base64StringData = "askldjflkajsdklfjaklsjdf"}
            //    }
            //});
            //Areas.Add(new Area{Name = "3"});
            //Areas.Add(new Area{Name = "4"});
            //OnPropertyChanged("Areas");

            //SelectedItem = Areas[1];

            //EditName = "edit text";
            //IsEdit = true;

            #endregion
        }

        private void InitCommands()
        {
            SearchCommand = new ArtisCommand(CanExecuteSearchCommand, ExecuteSearchCommand);
            EditCommand = new ArtisCommand(CanExecuteEditCommand, ExecuteEditCommand);
            AddImagesCommand = new ArtisCommand(CanExecuteEditCommand, ExecuteAddImagesCommand);
            RemoveImagesCommand = new ArtisCommand(CanExecuteEditCommand, ExecuteRemoveImagesCommand);
            SaveCommand = new ArtisCommand(CanExecuteEditCommand, ExecuteSaveCommand);
            RemoveCommand = new ArtisCommand(CanExecuteRemoveCommand, ExecuteRemoveCommand);
            BackCommand = new ArtisCommand(CanExecuteEditCommand, ExecuteBackCommand);
            NewAreaCommand = new ArtisCommand(CanExecuteEditCommand, ExecuteNewAreaCommand);
        }

        /// <summary>
        /// Заполнение источников данных
        /// </summary>
        //private async void InitDataSource()
        //{
        //    IList<Area> areas = await Task.Run(
        //    () =>
        //    {
        //        Task<IList<Area>> t = DataRequestFactory.GetAreas();
        //        t.Wait();
        //        return t.Result;
        //    });
        //    Areas = new ObservableCollection<Area>(areas);
        //}

        /// <summary>
        /// Заполнение источников данных
        /// </summary>
        private async void InitDataSource()
        {
            Areas = new ObservableCollection<Area>(await DataRequestFactory.GetAreas());
        }

        private void InitVariables()
        {
            _deletedImages=new List<long>();
            _addedImages=new List<DataImage>();
        }

        private bool CanExecuteSearchCommand(object parameters)
        {
            return true;
        }

        private bool CanExecuteEditCommand(object parameter)
        {
            return true;
        }

        private bool CanExecuteRemoveCommand(object parameter)
        {
            if (CurrentArea != null)
                return true;
            return false;
        }

        private async void ExecuteSearchCommand(object parameters)
        {
            Areas = new ObservableCollection<Area>(await DataRequestFactory.GetAreas(FilterName));
        }

        private void ExecuteNewAreaCommand(object parameter)
        {
            IsEdit = false; // устанавливаем флаг
            //Создаем новую площадку
            CurrentArea=new Area();
            Images = new ObservableCollection<DataImage>();
        }

        private void ExecuteBackCommand(object parameter)
        {
            ClearVariables();
        }

        private void ExecuteSaveCommand(object parameter)
        {
            if (IsEdit)
                SaveArea();
            else
                AddArea();

            ClearVariables();
        }

        private void ExecuteRemoveCommand(object parameter)
        {
            RemoveArea();
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
                Stream[] selectedFiles=openDialog.OpenFiles();
                foreach (Stream file in selectedFiles)
                {
                    MemoryStream stream=new MemoryStream();
                    file.CopyTo(stream);
                    file.Close();
                    byte[] imageArray = stream.ToArray();
                    string base64String = Convert.ToBase64String(imageArray, 0, imageArray.Length);
                    if (!string.IsNullOrEmpty(base64String))
                    {
                        DataImage image = new DataImage() { Base64String = base64String };

                        _addedImages.Add(image);

                        Images.Add(image);
                        OnPropertyChanged("Images");
                    }
                }
            }
        } 
        

        private void ExecuteRemoveImagesCommand(object parameter)
        {
            if (SelectedImage.ID != 0)
            {
                _deletedImages.Add(SelectedImage.ID);
                Images.Remove(Images.First(i => i.ID == SelectedImage.ID));
            }
            else
                Images.Remove(Images.First(i => i.Base64String.Equals(SelectedImage.Base64String)));

            if (_addedImages.Any(i => i.Base64String.Equals(SelectedImage.Base64String)))
            {
                DataImage image = _addedImages.First(i => i.ID == SelectedImage.ID);
                _addedImages.Remove(image);
            }

            OnPropertyChanged("Images");
        }

        private async void ExecuteEditCommand(object parameter)
        {
            IsEdit = true; // устанавливаем флаг
            Images = new ObservableCollection<DataImage>(await ImageHelper.ConvertImages(CurrentArea.Data));
            _addedImages = new List<DataImage>();
            _deletedImages = new List<long>();
        }

        private void ClearVariables()
        {
            CurrentArea = null;
            SelectedImage = null;
            Images = null;
            _deletedImages = null;
            _addedImages = null;
        }

        private async Task<bool> SaveArea()
        {
            return
                await
                    AreaRepository.Save(CurrentArea,
                        _addedImages.Select(i => new Data.Data() {Base64StringData = i.Base64String}).ToList(),
                        _deletedImages);
        }

        private async Task<bool> AddArea()
        {
            bool result =
                await
                    AreaRepository.Add(CurrentArea,
                        _addedImages.Select(i => new Data.Data() {Base64StringData = i.Base64String}).ToList());
            if (result)
            {
                Areas.Add(CurrentArea);
                OnPropertyChanged("Areas");
            }
            return result;

        }

        private async Task<bool> RemoveArea()
        {
            bool result = await AreaRepository.Remove(CurrentArea);
            if (result)
            {
                Areas.Remove(CurrentArea);
                OnPropertyChanged("Areas");
            }
            return result;

        }
    }
}
