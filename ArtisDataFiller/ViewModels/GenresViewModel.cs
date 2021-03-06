﻿using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Artis.Consts;
using Artis.Data;

namespace Artis.ArtisDataFiller.ViewModels
{
    public class GenresViewModel : ViewModel
    {
        private readonly WcfServiceCaller _wcfAdminService;
        private string _filterName;
        private ObservableCollection<Genre> _genres;
        private Genre _currentGenre;
        private bool _isEdit;

        /// <summary>
        /// Команда поиска
        /// </summary>
        public ArtisCommand SearchCommand { get; private set; }

        /// <summary>
        /// Команда добавления нового жанра
        /// </summary>
        public ArtisCommand NewGenreCommand { private set; get; }

        /// <summary>
        /// Команда редактирования жанра
        /// </summary>
        public ArtisCommand EditCommand { private set; get; }

        /// <summary>
        /// Команда удаления жанра
        /// </summary>
        public ArtisCommand RemoveCommand { private set; get; }

        /// <summary>
        /// Команда сохранения жанра после редактирования/создания
        /// </summary>
        public ArtisCommand SaveCommand { private set; get; }

        /// <summary>
        /// Команда возврата с карточки жанра на просмотр всех жанра
        /// </summary>
        public ArtisCommand BackCommand { private set; get; }

        /// <summary>
        /// Наименование жанра, учавствующее в поиске
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
        /// Список всех жанров
        /// </summary>
        public ObservableCollection<Genre> Genres
        {
            get { return _genres; }
            set
            {
                _genres = value; 
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Текущий выделенный жанр
        /// </summary>
        public Genre CurrentGenre
        {
            get { return _currentGenre; }
            set
            {
                _currentGenre = value; 
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// True - если редактируется жанр, False - создается новый
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

        public GenresViewModel()
        {
            InitCommands();

            _wcfAdminService = new WcfServiceCaller();

            InitDataSource();
        }

        /// <summary>
        /// Инициализация команд
        /// </summary>
        private void InitCommands()
        {
            SearchCommand = new ArtisCommand(CanExecuteSearchCommand, ExecuteSearchCommand);
            NewGenreCommand = new ArtisCommand(CanExecuteSearchCommand, ExecuteNewGenreCommand);
            EditCommand = new ArtisCommand(CanExecuteEditCommand, ExecuteEditCommand);
            RemoveCommand = new ArtisCommand(CanExecuteRemoveCommand, ExecuteRemoveCommand);
            SaveCommand = new ArtisCommand(CanExecuteSaveCommand, ExecuteSaveCommand);
            BackCommand = new ArtisCommand(CanExecuteSearchCommand, ExecuteBackCommand);
        }

        /// <summary>
        /// Загрузка списка жанров
        /// </summary>
        private async void InitDataSource()
        {
            Genres = await _wcfAdminService.GetGenres();
        }

        private void ExecuteBackCommand(object obj)
        {
            //обнуляем выделение после отмены редактирования/создания
            CurrentGenre = null;
        }

        private async void ExecuteSaveCommand(object obj)
        {
            if (IsEdit)
                await SaveGenre();
            else
                await AddGenre();

            ClearVariables();
        }

        private async void ExecuteRemoveCommand(object obj)
        {
            await RemoveGenre();
        }

        private void ExecuteEditCommand(object obj)
        {
            IsEdit = true; // устанавливаем флаг
        }

        private void ExecuteNewGenreCommand(object obj)
        {
            IsEdit = false; // устанавливаем флаг
            //Создаем новый жанр
            CurrentGenre = new Genre();
        }

        private async void ExecuteSearchCommand(object obj)
        {
            Genres = await _wcfAdminService.GetGenres(FilterName);
        }

        private bool CanExecuteRemoveCommand(object obj)
        {
            return CurrentGenre != null;
        }

        private bool CanExecuteEditCommand(object obj)
        {
            return CurrentGenre != null;
        }

        private bool CanExecuteSearchCommand(object obj)
        {
            return true;
        }

        private bool CanExecuteSaveCommand(object obj)
        {
            if (CurrentGenre != null && !string.IsNullOrEmpty(CurrentGenre.Name))
                return true;
            return false;
        }

        private async Task<bool> RemoveGenre()
        {
            bool result = await _wcfAdminService.RemoveGenre(CurrentGenre.ID);
            if (result)
            {
                Genres.Remove(CurrentGenre);
                OnPropertyChanged("Genres");
            }
            return result;

        }

        private async Task<bool> AddGenre()
        {
            long result =
                await
                    _wcfAdminService.AddGenre(CurrentGenre);
            if (result != -1)
            {
                CurrentGenre.ID = result;
                Genres.Add(CurrentGenre);
                OnPropertyChanged("Genres");
            }
            return result == -1;
        }

        private async Task<bool> SaveGenre()
        {
            return await _wcfAdminService.SaveGenre(CurrentGenre);
        }

        private void ClearVariables()
        {
            CurrentGenre = null;
        }
    }
}
