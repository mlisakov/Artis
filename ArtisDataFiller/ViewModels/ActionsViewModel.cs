using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Artis.Consts;
using Artis.Data;
using Action = Artis.Data.Action;

namespace Artis.ArtisDataFiller.ViewModels
{
    public sealed class ActionsViewModel : ViewModel
    {
        private string _filterName;
        private ObservableCollection<Area> _filterAreasItemsSource;
        private Area _filterArea;
        private DateTime? _fromDate;
        private DateTime? _toDate;
        private ObservableCollection<ActionDate> _actionsItemsSource;
        private ActionDate _selectedAction;
        private Action _currentAction;
        private ObservableCollection<Genre> _genresItemsSource;
        private ObservableCollection<State> _statesItemsSource;
        private Actor _selectedActor;
        private Producer _selectedProducer;

        /// <summary>
        /// Команда поиска
        /// </summary>
        public ArtisCommand SearchCommand { get; private set; }

        /// <summary>
        /// Команда создания нового мероприятия
        /// </summary>
        public ArtisCommand CreateActionCommand { get; private set; }

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
        /// Выделенная площадка
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
        /// Выделенное мероприятие в результатах поиска
        /// </summary>
        public ActionDate SelectedAction
        {
            get { return _selectedAction; }
            set
            {
                _selectedAction = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Текущий создаваемое/редактируемое мероприятие
        /// </summary>
        public Action CurrentAction
        {
            get { return _currentAction; }
            set
            {
                _currentAction = value; 
                OnPropertyChanged();
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
        public ObservableCollection<Actor> CurrentActionActors
        {
            get
            {
                if (CurrentAction != null && CurrentAction.Actor != null)
                    return new ObservableCollection<Actor>(CurrentAction.Actor);
                return new ObservableCollection<Actor>();
            }
            set
            {
                if (CurrentAction != null)
                    CurrentAction.Actor = value;
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
        /// Источник данных для продюсеров создаваемого/редактируемого мероприятия
        /// </summary>
        public ObservableCollection<Producer> CurrentActionProducers
        {
            get
            {
                if (CurrentAction != null && CurrentAction.Producer != null)
                    return new ObservableCollection<Producer>(CurrentAction.Producer);
                return new ObservableCollection<Producer>();
            }
            set
            {
                if (CurrentAction != null)
                    CurrentAction.Producer = value;
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

        public ActionsViewModel()
        {
            InitCommands();

            FilterName = "Test filter name";
            FilterAreasItemsSource = new ObservableCollection<Area>();

            FromDate = ToDate = DateTime.Now;
            ActionsItemsSource = new ObservableCollection<ActionDate>();

            CurrentAction = new Action();
            CurrentAction.Name = "Naimenovanie";         

            GenresItemsSource = new ObservableCollection<Genre>();
            StatesItemsSource = new ObservableCollection<State>();
        }

        private void InitCommands()
        {
            SearchCommand = new ArtisCommand(CanExecuteSearchCommand, ExecuteSearchCommand);

            CreateActionCommand = new ArtisCommand(CanExecute, ExecuteCreateActionCommand);
            EditActionCommand = new ArtisCommand(CanExecute, ExecuteEditActionCommand);
            RemoveActionCommand = new ArtisCommand(CanExecute, ExecuteRemoveActionCommand);

            AddActorCommand = new ArtisCommand(CanExecute, ExecuteAddActorCommand);
            RemoveActorCommand = new ArtisCommand(CanExecute, ExecuteRemoveActorCommand);

            AddProducerCommand = new ArtisCommand(CanExecute, ExecuteAddProducerCommand);
            RemoveProducerCommand = new ArtisCommand(CanExecute, ExecuteRemoveProducerCommand);

            AddImageCommand = new ArtisCommand(CanExecute, ExecuteAddImageCommand);
            RemoveImageCommand = new ArtisCommand(CanExecute, ExecuteRemoveImageCommand);

            SaveCommand = new ArtisCommand(CanExecute, ExecuteSaveCommand);
            CancelCommand = new ArtisCommand(CanExecute, ExecuteCancelCommand);
        }


        private bool CanExecute(object parameters)
        {
            return true;
        }

        private void ExecuteCancelCommand(object obj)
        {
        }

        private void ExecuteSaveCommand(object obj)
        {
        }

        private void ExecuteRemoveImageCommand(object obj)
        {
        }

        private void ExecuteAddImageCommand(object obj)
        {
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

        private void ExecuteCreateActionCommand(object obj)
        {
            IsEdit = false; // не удалять
        }

        private void ExecuteEditActionCommand(object obj)
        {
            IsEdit = true; // не удалять
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
            ActionsItemsSource=new ObservableCollection<ActionDate>(await DataRequestFactory.GetActions(FromDate.Value.Date, ToDate.Value.Date));
        }

    }
}
