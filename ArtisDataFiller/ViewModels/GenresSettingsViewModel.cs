using System.Collections.ObjectModel;
using Artis.Consts;
using Artis.Data;

namespace Artis.ArtisDataFiller.ViewModels
{
    public class GenresSettingsViewModel:ViewModel
    {
        private ObservableCollection<string> _categories;
        private string _currentCategory;
        private ObservableCollection<Genre> _usedGenres;
        private ObservableCollection<Genre> _othersGenres;
        private Genre _selectedUsedGenre;
        private Genre _selectedOtherGenre;


        public ArtisCommand LeftCommand { get; private set; }
        public ArtisCommand AllLeftCommand { get; private set; }
        public ArtisCommand RightCommand { get; private set; }
        public ArtisCommand AllRightCommand { get; private set; }

        /// <summary>
        /// Коллекция категорий с гуи сайта
        /// </summary>
        public ObservableCollection<string> Categories
        {
            get { return _categories; }
            set
            {
                _categories = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Текущая категория с гуи сайта
        /// </summary>
        public string CurrentCategory
        {
            get { return _currentCategory; }
            set
            {
                _currentCategory = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Коллекция используемых жанров для выбранной категории
        /// </summary>
        public ObservableCollection<Genre> UsedGenres
        {
            get { return _usedGenres; }
            set
            {
                _usedGenres = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Выделенный используемый жанр
        /// </summary>
        public Genre SelectedUsedGenre
        {
            get { return _selectedUsedGenre; }
            set
            {
                _selectedUsedGenre = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Коллекция оставшихся жанров, еще не используемых для выбранной категории
        /// </summary>
        public ObservableCollection<Genre> OthersGenres
        {
            get { return _othersGenres; }
            set
            {
                _othersGenres = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Выделенный оставшийся жанр
        /// </summary>
        public Genre SelectedOtherGenre
        {
            get { return _selectedOtherGenre; }
            set
            {
                _selectedOtherGenre = value; 
                OnPropertyChanged();
            }
        }

        public GenresSettingsViewModel()
        {
            InitCommands();

            UsedGenres = new ObservableCollection<Genre>
            {
                new Genre {Name = "some name"},
                new Genre {Name = "some name2"}
            };

            OthersGenres = new ObservableCollection<Genre>
            {
                new Genre {Name = "some name3"},
                new Genre {Name = "some name4"}
            };            
        }

        private void InitCommands()
        {
            LeftCommand = new ArtisCommand(CanExecute, ExecuteLeftCommand);
            RightCommand = new ArtisCommand(CanExecute, ExecuteRightCommand);
            AllLeftCommand = new ArtisCommand(CanExecute, ExecuteAllLeftCommand);
            AllRightCommand = new ArtisCommand(CanExecute, ExecuteAllRightCommand);
        }

        private bool CanExecute(object parameter)
        {
            return true;
        }

        private void ExecuteLeftCommand(object parameter)
        {
            
        }

        private void ExecuteRightCommand(object parameter)
        {
            
        }

        private void ExecuteAllLeftCommand(object parameter)
        {
            
        }

        private void ExecuteAllRightCommand(object parameter)
        {
            
        }
    }
}
