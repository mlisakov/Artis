using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Artis.Consts;
using Artis.Data;
using Artis.Utils;
using Microsoft.Win32;
using NLog;

namespace Artis.ArtisDataFiller.ViewModels
{
    public sealed class ActionsViewModel : ViewModel
    {
        private WcfServiceCaller _wcfAdminService; 

        private string _filterName;
        private DataImage _selectedImage;
        
        private Area _filterArea;
        private DateTime? _fromDate;
        private DateTime? _toDate;
        private ActionDate _currentActionDate;
        private Actor _selectedActor;
        private Producer _selectedProducer;

        private ObservableCollection<DataImage> _images;
        private ObservableCollection<DataImage> _smallImages;
        private List<long> _deletedImages;
        private List<long> _deletedSmallImages;
        private List<DataImage> _addedImages;
        private List<DataImage> _addedSmallImages;

        private ObservableCollection<Area> _filterAreasItemsSource;        
        private ObservableCollection<Genre> _genresItemsSource;
        private ObservableCollection<State> _statesItemsSource;
        private ObservableCollection<ActionDate> _actionsItemsSource;
        private ObservableCollection<Actor> _actorsItemsSource;
        private ObservableCollection<Producer> _producersItemsSource;
        private bool _isNewOne;
        private ImageSource _newImage;
        private bool _isHorizontalImage;

        //Соотношение ширины к высоте для последней добавленной картинки
        private double _lastPercentOfImage;
        //ширина оригинала - картинки на гуи
        //const int WidthConst = 834;

        private bool _isOnLeft;
        private bool _isCenter;
        private bool _isTop;
        private bool _isBotton;


        
        /// <summary>
        /// Логгер
        /// </summary>
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Команда поиска
        /// </summary>
        public ArtisCommand SearchCommand { get; private set; }

        /// <summary>
        /// Команда создания нового мероприятия
        /// </summary>
        public ArtisCommand CreateActionCommand { get; private set; }

        /// <summary>
        /// Команда копирования мероприятия
        /// </summary>
        public ArtisCommand CopyActionCommand { get; private set; }

        /// <summary>
        /// Команда редактирования мероприятия
        /// </summary>
        public ArtisCommand EditActionCommand { get; private set; }

        /// <summary>
        /// Команда удаления мероприятия
        /// </summary>
        public ArtisCommand RemoveActionCommand { get; private set; }

        /// <summary>
        /// Команда добавления актера для мероприятия
        /// </summary>
        public ArtisCommand AddNewActorCommand { get; private set; }

        /// <summary>
        /// Команда добавления актера для мероприятия из списка всех актеров 
        /// </summary>
        public ArtisCommand AddActorFromListCommand { get; private set; }

        /// <summary>
        /// Команда удаления актера для мероприятия
        /// </summary>
        public ArtisCommand RemoveActorCommand { get; private set; }

        /// <summary>
        /// Команда редактирования актера
        /// </summary>
        public ArtisCommand EditActorCommand { get; private set; }

        /// <summary>
        /// Команда добавления продюсера для мероприятия
        /// </summary>
        public ArtisCommand AddProducerCommand { get; private set; }

        /// <summary>
        /// Команда редактирования продюсера
        /// </summary>
        public ArtisCommand EditProducerCommand { get; private set; }

        /// <summary>
        /// Команда удаления продюсера для мероприятия
        /// </summary>
        public ArtisCommand RemoveProducerCommand { get; private set; }

        /// <summary>
        /// Команда добавления картинки для мероприятия
        /// </summary>
        public ArtisCommand AddImageCommand { get; private set; }

        /// <summary>
        /// Команда удаления картинки для мероприятия
        /// </summary>
        public ArtisCommand RemoveImageCommand { get; private set; }

        /// <summary>
        /// Команда сохранения изменений
        /// </summary>
        public ArtisCommand SaveCommand { get; private set; }

        /// <summary>
        /// Команда отмены изменений
        /// </summary>
        public ArtisCommand CancelCommand { get; private set; }

        /// <summary>
        /// Команда загрузки новой картинки
        /// </summary>
        public ArtisCommand OpenImageCommand { get; private set; }

        /// <summary>
        /// Команда обрезания и сохранения новой картинки
        /// </summary>
        public ArtisCommand SaveNewImageCommand { get; private set; }

        /// <summary>
        /// Наименование мероприятия
        /// <remarks>Учавствует в поиске мероприятий</remarks>
        /// </summary>
        public string FilterName
        {
            get { return _filterName; }
            set
            {
                _filterName = value;
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
        /// Список маленьких картинок для редактируемой/создаваемой площадки
        /// </summary>
        public ObservableCollection<DataImage> SmallImages
        {
            get { return _smallImages; }
            set
            {
                _smallImages = value;
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


        /// <summary>
        /// Источник данных для комбо-бокса с площадками, учавствует в поиске
        /// <remarks>Учавствует в поиске мероприятий</remarks>
        /// </summary>
        public ObservableCollection<Area> FilterAreasItemsSource    
        {
            get { return _filterAreasItemsSource; }
            set
            {
                _filterAreasItemsSource = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Фильтрующая площадка
        /// <remarks>Учавствует в поиске мероприятий</remarks>
        /// </summary>
        public Area FilterArea  
        {
            get { return _filterArea; }
            set
            {
                _filterArea = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Дата проведения мероприятий, С которой начинается поиск
        /// <remarks>Учавствует в поиске мероприятий</remarks>
        /// </summary>
        public DateTime? FromDate
        {
            get { return _fromDate; }
            set
            {
                _fromDate = value; 
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Дата проведения мероприятий, ДО которой начинается поиск
        /// <remarks>Учавствует в поиске мероприятий</remarks>
        /// </summary>
        public DateTime? ToDate
        {
            get { return _toDate; }
            set
            {
                _toDate = value; 
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Результаты поиска
        /// </summary>
        public ObservableCollection<ActionDate> ActionsItemsSource
        {
            get { return _actionsItemsSource; }
            set
            {
                _actionsItemsSource = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Флаг - редактируется ли сейчас мероприятие или создается.
        /// <remarks>Необходимо для задания readonly полей</remarks>
        /// </summary>
        public bool IsEdit { get; set; }

        /// <summary>
        /// Флаг - создается ли новое мероприятие, или к существующему мапятся дата/место проведения
        /// <remarks>Необходимо для задания readonly полей</remarks>
        /// </summary>
        public bool IsNewOne
        {
            get { return _isNewOne; }
            set
            {
                _isNewOne = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Выделенное мероприятие в результатах поиска
        /// </summary>
        public ActionDate CurrentActionDate
        {
            get { return _currentActionDate; }
            set
            {
                _currentActionDate = value;
                //if (CurrentActionDate != null && CurrentActionDate.Action.Actor != null)
                //{
                //    ActorsItemsSource = new ObservableCollection<Actor>(CurrentActionDate.Action.Actor);
                //    ProducersItemsSource=new ObservableCollection<Producer>(CurrentActionDate.Action.Producer);
                //}
                //else
                //{
                //    ActorsItemsSource=new ObservableCollection<Actor>();
                //    ProducersItemsSource=new ObservableCollection<Producer>();
                //}
                
                OnPropertyChanged();
                //OnPropertyChanged("ActorsItemsSource");
                //OnPropertyChanged("ProducersItemsSource");
            }
        }

        /// <summary>
        /// Источник данных для списка с жанрами
        /// </summary>
        public ObservableCollection<Genre> GenresItemsSource
        {
            get { return _genresItemsSource; }
            set
            {
                _genresItemsSource = value; 
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Список актеров
        /// </summary>
        public ObservableCollection<Actor> ActorsItemsSource
        {
            get { return _actorsItemsSource; }
            set
            {
                _actorsItemsSource = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Список актеров
        /// </summary>
        public ObservableCollection<Producer> ProducersItemsSource
        {
            get { return _producersItemsSource; }
            set
            {
                _producersItemsSource = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Источник данных для списка состояний
        /// </summary>
        public ObservableCollection<State> StatesItemsSource
        {
            get { return _statesItemsSource; }
            set
            {
                _statesItemsSource = value; 
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Выделенный актер в списке актеров создаваемого/редактируемого мероприятия
        /// </summary>
        public Actor SelectedActor
        {
            get { return _selectedActor; }
            set
            {
                _selectedActor = value; 
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Выделенный продюсер в списке продюсеров создаваемого/редактируемого мероприятия
        /// </summary>
        public Producer SelectedProducer
        {
            get { return _selectedProducer; }
            set
            {
                _selectedProducer = value; 
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Новая загруженная картинка
        /// </summary>
        public ImageSource NewImage
        {
            get { return _newImage; }
            set
            {
                _newImage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Горизонтальный режим добавления новой картинки
        /// </summary>
        public bool IsHorizontalImage
        {
            get { return _isHorizontalImage; }
            set
            {
                _isHorizontalImage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Выравнена ли добавляемая картинка по левому краю
        /// </summary>
        public bool IsLeft
        {
            get { return _isOnLeft; }
            set
            {
                _isOnLeft = value; 
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Выравнена ли добавляемая картинка по центру по горизонтали
        /// </summary>
        public bool IsCenter
        {
            get { return _isCenter; }
            set
            {
                _isCenter = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Выравнена ли добавляемая картинка по верхнему краю
        /// </summary>
        public bool IsTop
        {
            get { return _isTop; }
            set
            {
                _isTop = value; 
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Выравнена ли добавляемая картинка по нижнему краю
        /// </summary>
        public bool IsBotton
        {
            get { return _isBotton; }
            set
            {
                _isBotton = value;
                OnPropertyChanged();
            }
        }

        public ActionsViewModel()
        {
            InitCommands();
            FromDate = ToDate = DateTime.Today;            
            InitVariables();
            InitDataSource();
        }

        /// <summary>
        /// Инициализация переменных
        /// </summary>
        private void InitVariables()
        {
            _wcfAdminService=new WcfServiceCaller();
            _deletedImages = new List<long>();
            _addedImages = new List<DataImage>();
            _addedSmallImages=new List<DataImage>();
        }

        /// <summary>
        /// Инициализация источников данных
        /// </summary>
        private async void InitDataSource()
        {
            //FilterName = "Наименование мероприятия";
            FilterAreasItemsSource = await _wcfAdminService.GetAreas(-1);
            ActionsItemsSource = new ObservableCollection<ActionDate>();
            GenresItemsSource = await _wcfAdminService.GetGenres();
            StatesItemsSource = new ObservableCollection<State>();
            ActorsItemsSource=new ObservableCollection<Actor>();
            ProducersItemsSource=new ObservableCollection<Producer>();
        }

        private void InitCommands()
        {
            SearchCommand = new ArtisCommand(CanExecuteSearchCommand, ExecuteSearchCommand);

            CreateActionCommand = new ArtisCommand(CanExecute, ExecuteCreateActionCommand);
            CopyActionCommand = new ArtisCommand(CanExecute, ExecuteCopyActionCommand);
            EditActionCommand = new ArtisCommand(CanExecute, ExecuteEditActionCommand);
            RemoveActionCommand = new ArtisCommand(CanExecute, ExecuteRemoveActionCommand);

            AddNewActorCommand = new ArtisCommand(CanExecuteAddCommands, ExecuteAddNewActorCommand);
            AddActorFromListCommand = new ArtisCommand(CanExecuteAddCommands, ExecuteAddActorFromListCommand);
            EditActorCommand = new ArtisCommand(CanExecute, ExecuteEditActorCommand);
            RemoveActorCommand = new ArtisCommand(CanExecute, ExecuteRemoveActorCommand);

            AddProducerCommand = new ArtisCommand(CanExecuteAddCommands, ExecuteAddProducerCommand);
            EditProducerCommand = new ArtisCommand(CanExecute, ExecuteEditProducerCommand);
            RemoveProducerCommand = new ArtisCommand(CanExecute, ExecuteRemoveProducerCommand);

            AddImageCommand = new ArtisCommand(CanExecuteAddCommands, ExecuteAddImageCommand);
            RemoveImageCommand = new ArtisCommand(CanExecute, ExecuteRemoveImageCommand);

            SaveCommand = new ArtisCommand(CanExecute, ExecuteSaveCommand);
            CancelCommand = new ArtisCommand(CanExecute, ExecuteCancelCommand);

            OpenImageCommand = new ArtisCommand(CanExecuteAddCommands, ExecuteOpenImageCommand);
            SaveNewImageCommand = new ArtisCommand(CanExecuteSaveNewImageCommand, ExecuteSaveNewImageCommand);
        }




        private bool CanExecute(object parameters)
        {
            return true;
        }

        private bool CanExecuteAddCommands(object parameters)
        {
            return IsNewOne;
        }

        private bool CanExecuteSaveNewImageCommand(object parameter)
        {
            return NewImage != null;
        }

        private void ExecuteCancelCommand(object obj)
        {
            ClearVariables();
        }

        private void ExecuteSaveCommand(object obj)
        {
            if (IsEdit)
            {
                if (IsNewOne)
                    SaveActionDate();
                else
                    AddNewActionDate();
            }
            else
                AddActionDate();

            ClearVariables();
        }

        private void ExecuteRemoveImageCommand(object obj)
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

        private void ExecuteSaveNewImageCommand(object obj)
        {
            BitmapImage originalImage = NewImage as BitmapImage;

            if (originalImage == null)
                return;
            //сохраняем оригинал сначала
            SaveImage(originalImage);

            if (IsHorizontalImage)
            {
                //обрезаем для горизонтального варианта
                int dx;
                int dy;

                if (IsLeft)
                    dx = 0;
                else dx = IsCenter ? (ImageConsts.WidthConst - 580)/2 : ImageConsts.WidthConst - 580;

                if (IsTop)
                    dy = 0;
                else
                {
                    double height = (ImageConsts.WidthConst/_lastPercentOfImage);
                    dy = IsBotton ? Convert.ToInt32(height - 150) : Convert.ToInt32((height - 150)/2);
                }

                BitmapSource result = new CroppedBitmap(originalImage, new Int32Rect(dx, dy, 580, 150));

                MemoryStream mStream = new MemoryStream();
                BitmapEncoder encoder;
                if (Path.GetExtension(originalImage.UriSource.AbsoluteUri).ToLower().Equals(".png"))
                    encoder = new PngBitmapEncoder();
                else
                    encoder = new JpegBitmapEncoder();

                encoder.Frames.Add(BitmapFrame.Create(result));
                encoder.Save(mStream);

                mStream.Seek(0, SeekOrigin.Begin);
                string base64String = Convert.ToBase64String(mStream.ToArray());

                //сохраняем обрезанную картинку
                SaveSmallImage(base64String);
                CurrentActionDate.Action.IsVerticalSmallImage = false;
            }
            else
            {
                //обрезаем для вертикального варианта
                BitmapImage bi = ImageHelper.ResizeImage(originalImage.StreamSource, 100, originalImage.UriSource.AbsoluteUri);
                //BitmapEncoder encoder = new JpegBitmapEncoder();
                //encoder.Frames.Add(BitmapFrame.Create(bi));
                //SaveFileDialog openDialog = new SaveFileDialog
                //{
                //    Filter = "jpg(*.jpg)|*.jpg|jpeg(*.jpeg)|*.jpeg|png(*.png)|*.png",
                //    Title = "Пожалуйста, выберите файл для сохранения.",
                //};
                //if (openDialog.ShowDialog().Value)
                //    using (var fs = openDialog.OpenFile())
                //    {
                //        encoder.Save(fs);
                //    }
               
                //сохраняем обрезанную вертикально картинку
                SaveSmallImage(bi);
                CurrentActionDate.Action.IsVerticalSmallImage = true;
            }

            //SaveFileDialog openDialog = new SaveFileDialog
            //{
            //    Filter = "jpg(*.jpg)|*.jpg|jpeg(*.jpeg)|*.jpeg|png(*.png)|*.png",
            //    Title = "Пожалуйста, выберите файл для сохранения.",
            //};
            //if (openDialog.ShowDialog().Value)
            //    using (var fs = openDialog.OpenFile())
            //    {
            //        encoder.Save(fs);
            //    }

            NewImage = null;
        }

        private void SaveImage(BitmapImage originalImage)
        {
            BitmapImage bi = ImageHelper.ResizeImage(originalImage.StreamSource, ImageConsts.WidthConst, originalImage.UriSource.AbsoluteUri);
            //BitmapEncoder encoder = new JpegBitmapEncoder();

            //encoder.Frames.Add(BitmapFrame.Create(bi));


            //SaveFileDialog openDialog = new SaveFileDialog
            //{
            //    Filter = "jpg(*.jpg)|*.jpg|jpeg(*.jpeg)|*.jpeg|png(*.png)|*.png",
            //    Title = "Пожалуйста, выберите файл для сохранения.",
            //};
            //if (openDialog.ShowDialog().Value)
            //    using (var fs = openDialog.OpenFile())
            //    {
            //        encoder.Save(fs);
            //    }

            string base64String = ImageHelper.ConvertImageToBase64String(bi);
            //MemoryStream str = new MemoryStream(Convert.FromBase64String(base64String));
            //SaveFileDialog openDialog = new SaveFileDialog
            //{
            //    Filter = "jpg(*.jpg)|*.jpg|jpeg(*.jpeg)|*.jpeg|png(*.png)|*.png",
            //    Title = "Пожалуйста, выберите файл для сохранения.",
            //};
            //if (openDialog.ShowDialog().Value)
            //    using (var fs = openDialog.OpenFile())
            //    {
            //        str.CopyTo(fs);
            //    }

            if (!string.IsNullOrEmpty(base64String))
            {
                DataImage image = new DataImage() { Base64String = base64String, Image = bi };
                if (_addedImages == null)
                    _addedImages = new List<DataImage>();
                _addedImages.Add(image);

                if (Images == null)
                    Images = new ObservableCollection<DataImage>();
                else
                    Images.Clear();

                Images.Add(image);
                OnPropertyChanged("Images");
            }
        }

        private void SaveSmallImage(BitmapImage originalImage)
        {
            string base64String = ImageHelper.ConvertImageToBase64String(originalImage);

            MemoryStream str = new MemoryStream(Convert.FromBase64String(base64String));
            SaveFileDialog openDialog = new SaveFileDialog
            {
                Filter = "jpg(*.jpg)|*.jpg|jpeg(*.jpeg)|*.jpeg|png(*.png)|*.png",
                Title = "Пожалуйста, выберите файл для сохранения.",
            };
            if (openDialog.ShowDialog().Value)
                using (var fs = openDialog.OpenFile())
                {
                    str.CopyTo(fs);
                }

            if (!string.IsNullOrEmpty(base64String))
            {
                DataImage image = new DataImage() { Base64String = base64String, Image = originalImage };
                if (_addedSmallImages == null)
                    _addedSmallImages = new List<DataImage>();
                _addedSmallImages.Add(image);


                    if (SmallImages == null)
                        SmallImages = new ObservableCollection<DataImage>();
                    SmallImages.Add(image);
                    OnPropertyChanged("SmallImages");
                
            }
        }

        private void SaveSmallImage(string base64String)
        {
            //MemoryStream str = new MemoryStream(Convert.FromBase64String(base64String));
            //SaveFileDialog openDialog = new SaveFileDialog
            //{
            //    Filter = "jpg(*.jpg)|*.jpg|jpeg(*.jpeg)|*.jpeg|png(*.png)|*.png",
            //    Title = "Пожалуйста, выберите файл для сохранения.",
            //};
            //if (openDialog.ShowDialog().Value)
            //    using (var fs = openDialog.OpenFile())
            //    {
            //        str.CopyTo(fs);
            //    }

            if (!string.IsNullOrEmpty(base64String))
            {
                DataImage image = new DataImage() { Base64String = base64String };
                if (_addedSmallImages == null)
                    _addedSmallImages = new List<DataImage>();
                _addedSmallImages.Add(image);


                if (SmallImages == null)
                    SmallImages = new ObservableCollection<DataImage>();
                SmallImages.Add(image);
                OnPropertyChanged("SmallImages");

            }
        }

        private void ExecuteOpenImageCommand(object obj)
        {
            OpenFileDialog openDialog = new OpenFileDialog
            {
                Filter = "jpg(*.jpg)|*.jpg|jpeg(*.jpeg)|*.jpeg|png(*.png)|*.png",
                Title = "Пожалуйста, выберите необхожимые изображения.",
                Multiselect = true
            };


            if (openDialog.ShowDialog().Value)
            {
                using (Stream stream = openDialog.OpenFile())
                {
                    BitmapImage bitMapImage = new BitmapImage();

                    bitMapImage.BeginInit();
                    bitMapImage.StreamSource = stream;
                    bitMapImage.EndInit();

                    ////определяем пропорции
                    _lastPercentOfImage = (double)bitMapImage.PixelWidth / bitMapImage.PixelHeight;

                    //double height = WidthConst / _lastPercentOfImage;

                    ////устанавливаем пропорции, исходя из ширины 834 (ширина картинки на гуи)
                    //BitmapImage bi = new BitmapImage();
                    //bi.BeginInit();
                    //bi.CacheOption = BitmapCacheOption.OnDemand;
                    //bi.CreateOptions = BitmapCreateOptions.DelayCreation;
                    //bi.DecodePixelHeight = Convert.ToInt32(height);
                    //bi.DecodePixelWidth = 834;                    

                    //bi.UriSource = new Uri(openDialog.FileName);
                    //bi.EndInit();

                    NewImage = ImageHelper.ResizeImage(stream, ImageConsts.WidthConst, openDialog.FileName);
                }
            }
        }

        private void ExecuteAddImageCommand(object obj)
        {
            #region Сбрасываем настройки перед показом окна
            NewImage = null;
            IsHorizontalImage = true;
            IsLeft = true;
            IsTop = true;
            #endregion

            //            OpenFileDialog openDialog = new OpenFileDialog
//            {
//                Filter = "jpg(*.jpg)|*.jpg|jpeg(*.jpeg)|*.jpeg|png(*.png)|*.png",
//                Title = "Пожалуйста, выберите необхожимые изображения.",
//                Multiselect = true
//            };

//            if (openDialog.ShowDialog().Value)
//            {
//                Stream[] selectedFiles = openDialog.OpenFiles();
//                foreach (Stream file in selectedFiles)
//                {
//                    MemoryStream stream = new MemoryStream();
//                    file.CopyTo(stream);
//                    file.Close();
//                    byte[] imageArray = stream.ToArray();
//                    string base64String = Convert.ToBase64String(imageArray, 0, imageArray.Length);
//
//                    stream.Seek(0, SeekOrigin.Begin);
//                    BitmapImage bitMapImage = new BitmapImage();
//                    bitMapImage.BeginInit();
//                    bitMapImage.StreamSource = stream;
//                    bitMapImage.EndInit();
//
//                    if (!string.IsNullOrEmpty(base64String))
//                    {
//                        DataImage image = new DataImage() {Base64String = base64String, Image = bitMapImage};
//
//                        _addedImages.Add(image);
//
//                        Images.Add(image);
//                        OnPropertyChanged("Images");
//                    }
//                }
//            }
        }

        private async void ExecuteRemoveProducerCommand(object obj)
        {
            //await RemoveProducer();
            ProducersItemsSource.Remove(SelectedProducer);
            OnPropertyChanged("ProducersItemsSource");
        }

        private async void ExecuteRemoveActorCommand(object obj)
        {
            //await RemoveActor();
            ActorsItemsSource.Remove(SelectedActor);
            OnPropertyChanged("ActorsItemsSource");
        }

        private void ExecuteEditProducerCommand(object obj)
        {
            if (ProducersItemsSource == null)
                ProducersItemsSource = new ObservableCollection<Producer>();
            var viewModel = new AddProducerViewModel()
            {
                Title = "Добавление нового продюсера",
                Producer = SelectedProducer
            };

            var window = new AddActorDialogWindow { ViewModel = viewModel };

            var dialogResult = window.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                //todo сохранить изменения в описании продюсера
//                ProducersItemsSource.Add(viewModel.Producer);
//                OnPropertyChanged("ProducersItemsSource");
            }
        }

        private void ExecuteEditActorCommand(object obj)
        {
            if (ActorsItemsSource == null)
                ActorsItemsSource = new ObservableCollection<Actor>();

            var viewModel = new AddActorViewModel()
            {
                Title = "Добавление нового актера",
                Actor = SelectedActor
            };

            var window = new AddActorDialogWindow { ViewModel = viewModel };

            var dialogResult = window.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                //todo сохранить измененного актера
                //if(ChangedActors.Any(i=>i.ID==viewModel.Actor.ID))
                //ActorsItemsSource.Add(viewModel.Actor);
                //OnPropertyChanged("ActorsItemsSource");
            }
        }

        private void ExecuteAddProducerCommand(object obj)
        {
            if (ProducersItemsSource==null)
                ProducersItemsSource=new ObservableCollection<Producer>();
            var viewModel = new AddProducerViewModel() {Title = "Добавление нового продюсера"};

            var window = new AddActorDialogWindow {ViewModel = viewModel};

            var dialogResult = window.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                ProducersItemsSource.Add(viewModel.Producer);
                OnPropertyChanged("ProducersItemsSource");
            }
        }

        private void ExecuteAddNewActorCommand(object obj)
        {
            if (ActorsItemsSource == null)
                ActorsItemsSource = new ObservableCollection<Actor>();
            var viewModel = new AddActorViewModel() { Title = "Добавление нового актера" };

            var window = new AddActorDialogWindow { ViewModel = viewModel };

            var dialogResult = window.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                ActorsItemsSource.Add(viewModel.Actor);
                OnPropertyChanged("ActorsItemsSource");
            }
        }

        private void ExecuteAddActorFromListCommand(object obj)
        {
            if (obj != null)
            {
                bool producers = Convert.ToBoolean(obj);

                AddActorFromListViewModel viewModel;
                if (producers)
                {
                    if (ProducersItemsSource == null)
                        ProducersItemsSource = new ObservableCollection<Producer>();
                }
                else
                {
                    if (ActorsItemsSource == null)
                        ActorsItemsSource = new ObservableCollection<Actor>();
                }

                viewModel = new AddActorFromListViewModel(producers);

                var window = new AddActorFromListDialogWindow {ViewModel = viewModel};

                var dialogResult = window.ShowDialog();
                if (dialogResult.HasValue && dialogResult.Value)
                {

                    if (producers)
                    {
                        //todo добавляем продюсера 
                        ProducersItemsSource.Add((Producer)viewModel.SelectedPeople);
                        OnPropertyChanged("ProducersItemsSource");
                    }
                    else
                    {
                        //todo добавляем актера
                        ActorsItemsSource.Add((Actor)viewModel.SelectedPeople);
                        OnPropertyChanged("ActorsItemsSource");
                    }

                    
                }
            }
        }


        private void ExecuteRemoveActionCommand(object obj)
        {

        }

        private async void ExecuteCopyActionCommand(object obj)
        {
            IsEdit = true;
            IsNewOne = false;

            ObservableCollection<Data.Data> images = await _wcfAdminService.GetActionImages(CurrentActionDate.Action.ID);
            Images = await ImageHelper.ConvertImages(images);


            ObservableCollection<Data.Data> smallImages = await _wcfAdminService.GetActionSmallImages(CurrentActionDate.Action.ID);
            SmallImages = await ImageHelper.ConvertImages(smallImages);
            _addedImages = new List<DataImage>();
            _addedSmallImages = new List<DataImage>();
            _deletedImages = new List<long>();
        }

        private void ExecuteCreateActionCommand(object obj)
        {
            IsEdit = false; // не удалять
            IsNewOne = true; // не удалять

            CurrentActionDate=new ActionDate(){Date = DateTime.Today,Action=new Data.Action()};
            Images=new ObservableCollection<DataImage>();
            SmallImages=new ObservableCollection<DataImage>();
            ActorsItemsSource = new ObservableCollection<Actor>();
            ProducersItemsSource = new ObservableCollection<Producer>();
        }

        private async void ExecuteEditActionCommand(object obj)
        {
            IsEdit = true; // не удалять
            IsNewOne = true;

            ObservableCollection<Data.Data> images = await _wcfAdminService.GetActionImages(CurrentActionDate.Action.ID);
            ObservableCollection<Data.Data> smallImages = await _wcfAdminService.GetActionSmallImages(CurrentActionDate.Action.ID);
            ActorsItemsSource = await _wcfAdminService.GetActionActors(CurrentActionDate.Action.ID);
            ProducersItemsSource = await _wcfAdminService.GetActionProducers(CurrentActionDate.Action.ID);
            Images = await ImageHelper.ConvertImages(images);
            SmallImages = await ImageHelper.ConvertImages(smallImages);
            _addedImages = new List<DataImage>();
            _addedSmallImages = new List<DataImage>();
            _deletedImages = new List<long>();
        }

        private bool CanExecuteSearchCommand(object parameters)
        {
            if (FromDate.HasValue && ToDate.HasValue)
            {
                if (FromDate.Value > ToDate.Value)
                    return false;
            }
            return true;
        }

        private async void ExecuteSearchCommand(object parameters)
        {
            ActionsItemsSource = await _wcfAdminService.GetActions(FilterName, FilterArea, FromDate, ToDate);
        }

        private void ClearVariables()
        {
            CurrentActionDate = null;
            ActorsItemsSource = null;
            ProducersItemsSource = null;
            SelectedImage = null;
            Images = null;
            _deletedImages = null;
            _addedImages = null;
            _addedSmallImages = null;
        }

        /// <summary>
        /// Сохранение изменений 
        /// </summary>
        /// <returns></returns>
        private async Task<bool> SaveActionDate()
        {
            //bool result = await
            //        ActionDateRepository.Save(CurrentActionDate,
            //            _addedImages.Select(i => new Data.Data() { Base64StringData = i.Base64String }).ToList(),
            //            _deletedImages);
            bool result =
                await
                    _wcfAdminService.SaveActionDate(CurrentActionDate,
                        _addedImages.Select(i => new Data.Data() {Base64StringData = i.Base64String}).ToList(),
                        _deletedImages, _addedSmallImages.Select(i => new Data.Data() {Base64StringData = i.Base64String}).ToList(),ActorsItemsSource.ToList(),ProducersItemsSource.ToList());
            return result;

        }

        /// <summary>
        /// Добавление нового мероприятия
        /// </summary>
        /// <returns></returns>
        private async Task<bool> AddActionDate()
        {
            if (_addedImages==null)
                _addedImages=new List<DataImage>();
            bool result =
                await
                    _wcfAdminService.AddActionDate(CurrentActionDate,
                        _addedImages.Select(i => i.Base64String).ToList(),ActorsItemsSource.ToList(),ProducersItemsSource.ToList());
            //bool result =
            //    await
            //        ActionDateRepository.Add(CurrentActionDate,
            //            _addedImages.Select(i => new Data.Data() {Base64StringData = i.Base64String}).ToList());
            if (result)
            {
                //ActionsItemsSource.Add(CurrentActionDate);
                //OnPropertyChanged("ActionsItemsSource");
            }
            return result;
        }

        /// <summary>
        /// Добавление новой даты проведения мероприятия
        /// </summary>
        /// <returns></returns>
        private async Task<bool> AddNewActionDate()
        {
            //bool result =
            //    await
            //        ActionDateRepository.AddActionDate(CurrentActionDate);
            bool result =
                await
                    _wcfAdminService.AddActionDate(CurrentActionDate);
            if (result)
            {
                //ActionsItemsSource.Add(CurrentActionDate);
                //OnPropertyChanged("ActionsItemsSource");
            }
            return result;
        }

        //private async Task<bool> RemoveActionDate()
        //{
        //    //bool result = await ActionDateRepository.Remove(CurrentArea);
        //    //if (result)
        //    //{
        //    //    Areas.Remove(CurrentArea);
        //    //    OnPropertyChanged("Areas");
        //    //}
        //    //return result;
        //    return true;
        //}

        //private async Task<bool> RemoveActor()
        //{
        //    bool result =
        //       await
        //           ActorRepository.Remove(SelectedActor,CurrentActionDate.Action);
        //    if (result)
        //    {
        //        //ActionsItemsSource.Add(CurrentActionDate);
        //        //OnPropertyChanged("ActionsItemsSource");
        //    }
        //    return result;
        //}

        //private async Task<bool> RemoveProducer()
        //{
        //    bool result =
        //       await
        //           ProducerRepository.Remove(SelectedProducer, CurrentActionDate.Action);
        //    if (result)
        //    {
        //        //ActionsItemsSource.Add(CurrentActionDate);
        //        //OnPropertyChanged("ActionsItemsSource");
        //    }
        //    return result;
        //}

    }
}
