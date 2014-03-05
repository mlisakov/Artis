using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Artis.Consts;
using Artis.Data;
using Microsoft.Win32;
using NLog;
using Action = Artis.Data.Action;

namespace Artis.ArtisDataFiller.ViewModels
{
    public sealed class ActionsViewModel : ViewModel
    {
        private string _filterName;
        private DataImage _selectedImage;
        
        private Area _filterArea;
        private DateTime? _fromDate;
        private DateTime? _toDate;
        private ActionDate _currentActionDate;
        //private Action _currentAction;
        private Actor _selectedActor;
        private Producer _selectedProducer;

        private ObservableCollection<DataImage> _images;
        private List<long> _deletedImages;
        private List<DataImage> _addedImages;

        private ObservableCollection<Area> _filterAreasItemsSource;        
        private ObservableCollection<Genre> _genresItemsSource;
        private ObservableCollection<State> _statesItemsSource;
        private ObservableCollection<ActionDate> _actionsItemsSource;
        private bool _isNewOne;

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
        public ArtisCommand AddActorCommand { get; private set; }

        /// <summary>
        /// Команда удаления актера для мероприятия
        /// </summary>
        public ArtisCommand RemoveActorCommand { get; private set; }

        /// <summary>
        /// Команда добавления продюсера для мероприятия
        /// </summary>
        public ArtisCommand AddProducerCommand { get; private set; }

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
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Текущий создаваемое/редактируемое мероприятие
        /// </summary>
        //public Action CurrentAction
        //{
        //    get { return _currentAction; }
        //    set
        //    {
        //        _currentAction = value; 
        //        OnPropertyChanged();
        //    }
        //}

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
        /// Источник данных для актеров создаваемого/редактируемого мероприятия
        /// </summary>
        //public ObservableCollection<Actor> CurrentActionActors
        //{
        //    get
        //    {
        //        if (CurrentActionDate != null && CurrentActionDate.Action.Actor != null)
        //            return new ObservableCollection<Actor>(CurrentActionDate.Action.Actor);
        //        return new ObservableCollection<Actor>();
        //    }
        //    set
        //    {
        //        if (CurrentActionDate != null)
        //            CurrentActionDate.Action.Actor = value;
        //    }
        //}

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
        /// Источник данных для продюсеров создаваемого/редактируемого мероприятия
        /// </summary>
        //public ObservableCollection<Producer> CurrentActionProducers
        //{
        //    get
        //    {
        //        if (CurrentActionDate != null && CurrentActionDate.Action.Producer != null)
        //            return new ObservableCollection<Producer>(CurrentActionDate.Action.Producer);
        //        return new ObservableCollection<Producer>();
        //    }
        //    set
        //    {
        //        if (CurrentActionDate != null)
        //            CurrentActionDate.Action.Producer = value;
        //    }
        //}

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

        public ActionsViewModel()
        {
            InitCommands();
            FromDate = ToDate = DateTime.Today;
            InitDataSource();
            InitVariables();
        }

        /// <summary>
        /// Инициализация переменных
        /// </summary>
        private void InitVariables()
        {
            _deletedImages = new List<long>();
            _addedImages = new List<DataImage>();
        }

        /// <summary>
        /// Инициализация источников данных
        /// </summary>
        private async void InitDataSource()
        {
            //FilterName = "Наименование мероприятия";
            FilterAreasItemsSource = new ObservableCollection<Area>(await DataRequestFactory.GetAreas());
            ActionsItemsSource = new ObservableCollection<ActionDate>();
            GenresItemsSource = new ObservableCollection<Genre>(await DataRequestFactory.GetGenres());
            StatesItemsSource = new ObservableCollection<State>();
        }

        private void InitCommands()
        {
            SearchCommand = new ArtisCommand(CanExecuteSearchCommand, ExecuteSearchCommand);

            CreateActionCommand = new ArtisCommand(CanExecute, ExecuteCreateActionCommand);
            CopyActionCommand = new ArtisCommand(CanExecute, ExecuteCopyActionCommand);
            EditActionCommand = new ArtisCommand(CanExecute, ExecuteEditActionCommand);
            RemoveActionCommand = new ArtisCommand(CanExecute, ExecuteRemoveActionCommand);

            AddActorCommand = new ArtisCommand(CanExecuteAddCommands, ExecuteAddActorCommand);
            RemoveActorCommand = new ArtisCommand(CanExecute, ExecuteRemoveActorCommand);

            AddProducerCommand = new ArtisCommand(CanExecuteAddCommands, ExecuteAddProducerCommand);
            RemoveProducerCommand = new ArtisCommand(CanExecute, ExecuteRemoveProducerCommand);

            AddImageCommand = new ArtisCommand(CanExecuteAddCommands, ExecuteAddImageCommand);
            RemoveImageCommand = new ArtisCommand(CanExecute, ExecuteRemoveImageCommand);

            SaveCommand = new ArtisCommand(CanExecute, ExecuteSaveCommand);
            CancelCommand = new ArtisCommand(CanExecute, ExecuteCancelCommand);
        }


        private bool CanExecute(object parameters)
        {
            return true;
        }

        private bool CanExecuteAddCommands(object parameters)
        {
            return IsNewOne;
        }

        private void ExecuteCancelCommand(object obj)
        {
        }

        private void ExecuteSaveCommand(object obj)
        {
            if (IsEdit)
                SaveActionDate();
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

        private void ExecuteAddImageCommand(object obj)
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
                    MemoryStream stream = new MemoryStream();
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

        private void ExecuteRemoveProducerCommand(object obj)
        {

        }

        private void ExecuteAddProducerCommand(object obj)
        {
        }

        private void ExecuteRemoveActorCommand(object obj)
        {

        }

        private void ExecuteAddActorCommand(object obj)
        {

        }

        private void ExecuteRemoveActionCommand(object obj)
        {

        }

        private async void ExecuteCopyActionCommand(object obj)
        {
            IsNewOne = false;
            Images = new ObservableCollection<DataImage>(await ImageHelper.ConvertImages(CurrentActionDate.Action.Data));
            _addedImages = new List<DataImage>();
            _deletedImages = new List<long>();
        }

        private void ExecuteCreateActionCommand(object obj)
        {
            IsEdit = false; // не удалять
            IsNewOne = true; // не удалять

            CurrentActionDate=new ActionDate(){Date = DateTime.Today,Action=new Action()};
            Images=new ObservableCollection<DataImage>();
        }

        private async void ExecuteEditActionCommand(object obj)
        {
            IsEdit = true; // не удалять
            IsNewOne = true;
            Images = new ObservableCollection<DataImage>(await ImageHelper.ConvertImages(CurrentActionDate.Action.Data));
            _addedImages = new List<DataImage>();
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
            ActionsItemsSource = new ObservableCollection<ActionDate>(await DataRequestFactory.GetActions(FilterName, FilterArea, FromDate, ToDate));
        }

        private void ClearVariables()
        {
            CurrentActionDate = null;
            SelectedImage = null;
            Images = null;
            _deletedImages = null;
            _addedImages = null;
        }

        private async Task<bool> SaveActionDate()
        {
            return
                await
                    ActionDateRepository.Save(CurrentActionDate,
                        _addedImages.Select(i => new Data.Data() {Base64StringData = i.Base64String}).ToList(),
                        _deletedImages);
        }

        private async Task<bool> AddActionDate()
        {
            bool result =
                await
                    ActionDateRepository.Add(CurrentActionDate,
                        _addedImages.Select(i => new Data.Data() {Base64StringData = i.Base64String}).ToList());
            if (result)
            {
                ActionsItemsSource.Add(CurrentActionDate);
                OnPropertyChanged("ActionsItemsSource");
            }
            return result;
        }

        private async Task<bool> RemoveActionDate()
        {
            //bool result = await ActionDateRepository.Remove(CurrentArea);
            //if (result)
            //{
            //    Areas.Remove(CurrentArea);
            //    OnPropertyChanged("Areas");
            //}
            //return result;
            return true;
        }

    }
}
