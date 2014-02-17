using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Artis.ArtisDataFiller.Annotations;
using Artis.Consts;

namespace Artis.ArtisDataFiller.ViewModels
{
    public class DonwloadViewModel: ViewModel
    {
        private readonly object _actionLoadedLockObject;
        private readonly object _actionNotLoadedLockObject;
        private readonly object _logLockObject;

        private DateTime _fromDate;
        private DateTime _toDate;
        private string _errorMessage;
        
        private readonly ActionParser.DataFiller _dataFiller;
        private string _currentLoadingAction;
        private bool _loadingFinished;

        /// <summary>
        /// Команда начала загрузки данных
        /// </summary>
        public ArtisCommand DownloadCommand { private set; get; }
        /// <summary>
        /// Команда остановки
        /// <remarks>Не реализовано</remarks>
        /// </summary>
        public ArtisCommand CancelCommand { private set; get; }

        /// <summary>
        /// Команда завершения загрузки, отображения результатов
        /// </summary>
        public ArtisCommand CompleteCommand { private set; get; }

        /// <summary>
        /// Дата, с которой грузим данные
        /// </summary>
        public DateTime FromDate
        {
            get { return _fromDate; }
            set
            {
                _fromDate = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();                
            }
        }

        /// <summary>
        /// Дата, до которой грузим данные
        /// </summary>
        public DateTime ToDate
        {
            get { return _toDate; }
            set
            {
                _toDate = value; 
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value; 
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        /// <summary>
        /// Название текущего загружаемого мероприятия
        /// </summary>
        public string CurrentLoadingAction
        {
            get { return _currentLoadingAction; }
            set
            {
                _currentLoadingAction = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Загруженные мероприятия
        /// </summary>
        public ObservableCollection<ActionWeb> LoadedItems { get; set; }
        /// <summary>
        /// Незагруженные мероприятия
        /// </summary>
        public ObservableCollection<ActionWeb> NotLoadedItems { get; set; }

        /// <summary>
        /// Лог
        /// </summary>
        public ObservableCollection<string> LogItems { get; set; }

        /// <summary>
        /// true - успешное завершение загрузки
        /// </summary>
        public bool IsLoadingFinished
        {
            get { return _loadingFinished; }
            set
            {
                _loadingFinished = value;
                OnPropertyChanged();
            }
        }

        public DonwloadViewModel()
        {
            _actionLoadedLockObject = new object();
            _actionNotLoadedLockObject=new object();
            _logLockObject=new object();

            DownloadCommand = new ArtisCommand(CanExecuteDownloadCommand, ExecuteDownloadCommand);
            CancelCommand = new ArtisCommand(CanExecuteDownloadCommand, ExecuteCancelCommand);
            CompleteCommand = new ArtisCommand(CanExecuteDownloadCommand, ExecuteCompleteCommand);

            FromDate = ToDate = DateTime.Today;

            _dataFiller = new ActionParser.DataFiller();

            _dataFiller.WorkDoneEvent += _dataFiller_WorkDoneEvent;
            _dataFiller.ActionLoadedEvent += _dataFiller_ActionLoadedEvent;
            _dataFiller.ActionNotLoadedEvent += _dataFiller_ActionNotLoadedEvent;
            _dataFiller.ActionWebLoadedEvent += _dataFiller_ActionWebLoadedEvent;
            _dataFiller.FatalErrorEvent += _dataFiller_FatalErrorEvent;
        }

        #region Обработчики событий
        private void _dataFiller_ActionNotLoadedEvent(ActionWeb action)
        {
            AddNotLoadedAction(action);
        }

        private void _dataFiller_ActionLoadedEvent(ActionWeb action)
        {
            AddLoadedAction(action);
        }
        private void _dataFiller_ActionWebLoadedEvent(UrlActionLoadingSource source, ActionWeb action)
        {
            CurrentLoadingAction = source + ":" + action.Name;
            AddLog(action);
        }
        void _dataFiller_FatalErrorEvent(string message)
        {
            IsLoadingFinished = true;
            ErrorMessage = message;
        }

        private void _dataFiller_WorkDoneEvent(UrlActionLoadingSource source)
        {            
            IsLoadingFinished = true;
        }
        #endregion
        private void AddLog(ActionWeb action)
        {
            lock (_logLockObject)
            {
                LogItems.Add(action.Name);
                OnPropertyChanged("LogItems");
            }
        }

        private void AddNotLoadedAction(ActionWeb action)
        {
            lock (_actionNotLoadedLockObject)
            {
                NotLoadedItems.Add(action);
                OnPropertyChanged("NotLoadedItems");
            }
        }
        private void AddLoadedAction(ActionWeb action)
        {
            lock (_actionLoadedLockObject)
            {
                LoadedItems.Add(action);
                OnPropertyChanged("LoadedItems");
            }
        }

        private bool CanExecuteDownloadCommand(object parameter)
        {
            return (FromDate <= ToDate);
        }

        private void ExecuteDownloadCommand(object parameter)
        {
            IsLoadingFinished = false; //скрываем кнопку "Отчеты"

            LoadedItems = new ObservableCollection<ActionWeb>();
            NotLoadedItems = new ObservableCollection<ActionWeb>();
            LogItems = new ObservableCollection<string>();

            try
            {
                _dataFiller.ParseActionsAsync(FromDate, ToDate);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        private void ExecuteCancelCommand(object parameter)
        {
            IsLoadingFinished = true;

            _dataFiller.CancelParseData();
        }

        private void ExecuteCompleteCommand(object parameter)
        {
            //скрываем кнопку "Отчеты"
            IsLoadingFinished = false;
            CurrentLoadingAction = ErrorMessage = null;
        }

        public new void Dispose()
        {
            //todo Макс, освободи ресурсы
            _dataFiller.WorkDoneEvent -= _dataFiller_WorkDoneEvent;
            _dataFiller.ActionLoadedEvent -= _dataFiller_ActionLoadedEvent;
            _dataFiller.ActionNotLoadedEvent -= _dataFiller_ActionNotLoadedEvent;
            _dataFiller.ActionWebLoadedEvent -= _dataFiller_ActionWebLoadedEvent;
            _dataFiller.FatalErrorEvent -= _dataFiller_FatalErrorEvent;
        }
    }
}
