using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Artis.Consts;
using Artis.Data;

namespace Artis.ArtisDataFiller.ViewModels
{
    public class GenresSettingsViewModel:ViewModel
    {

        private WcfServiceCaller _wcfAdminService; 

        private ObservableCollection<GuiSection> _categories;
        private GuiSection _currentCategory;
        private ObservableCollection<Genre> _usedGenres;
        private ObservableCollection<Genre> _othersGenres;
        private Genre _selectedUsedGenre;
        private Genre _selectedOtherGenre;

        //private GuiSectionRepository _guiSectionRepository;

        public ArtisCommand SaveCommand { get; private set; }
        public ArtisCommand LeftCommand { get; private set; }
        public ArtisCommand AllLeftCommand { get; private set; }
        public ArtisCommand RightCommand { get; private set; }
        public ArtisCommand AllRightCommand { get; private set; }

        /// <summary>
        /// Коллекция категорий с гуи сайта
        /// </summary>
        public ObservableCollection<GuiSection> Categories
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
        public GuiSection CurrentCategory
        {
            get { return _currentCategory; }
            set
            {
                _currentCategory = value;
                OnPropertyChanged();
                InitGenres();
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
            //_guiSectionRepository=new GuiSectionRepository();
            _wcfAdminService=new WcfServiceCaller();
            InitCommands();
            InitDataSource();
        }

        /// <summary>
        /// Инициализация источников данных
        /// </summary>
        private async void InitDataSource()
        {
            //Categories = await DataRequestFactory.GetGuiSections();
            Categories = await _wcfAdminService.GetGuiSections();
        }

        private async void InitGenres()
        {
            UsedGenres = await _wcfAdminService.GetGuiSectionGenres(CurrentCategory.ID);
            OthersGenres = await _wcfAdminService.GetGuiSectionRestGenres(CurrentCategory.ID);
            //UsedGenres = await DataRequestFactory.GetGuiSectionGenres(CurrentCategory.ID);
            //OthersGenres = await DataRequestFactory.GetGuiSectionRestGenres(CurrentCategory.ID);
        }


        private void InitCommands()
        {
            LeftCommand = new ArtisCommand(CanExecute, ExecuteLeftCommand);
            RightCommand = new ArtisCommand(CanExecute, ExecuteRightCommand);
            AllLeftCommand = new ArtisCommand(CanExecute, ExecuteAllLeftCommand);
            AllRightCommand = new ArtisCommand(CanExecute, ExecuteAllRightCommand);
            SaveCommand = new ArtisCommand(CanExecute, ExecuteSaveCommand);
        }

        private bool CanExecute(object parameter)
        {
            return true;
        }

        private void ExecuteLeftCommand(object parameter)
        {
            UsedGenres.Add(SelectedOtherGenre);
            OthersGenres.Remove(SelectedOtherGenre);
        }

        private void ExecuteRightCommand(object parameter)
        {            
            OthersGenres.Add(SelectedUsedGenre);
            UsedGenres.Remove(SelectedUsedGenre);
        }

        private void ExecuteAllLeftCommand(object parameter)
        {
            foreach (Genre genre in OthersGenres)
            {
                UsedGenres.Add(genre);
            }
            OthersGenres.Clear();
        }

        private void ExecuteAllRightCommand(object parameter)
        {
            foreach (Genre genre in UsedGenres)
            {
                OthersGenres.Add(genre);
            }
            UsedGenres.Clear();
        }

        private void ExecuteSaveCommand(object parameter)
        {
            SaveGuiSectionGenres(UsedGenres);
        }

        private async Task<bool> SaveGuiSectionGenres(ObservableCollection<Genre> usedGenres)
        {
            bool result = await _wcfAdminService.UpdateGuiSectionGenres(CurrentCategory.ID,usedGenres);
            //bool result = await
            //       _guiSectionRepository.Save(CurrentCategory.ID, usedGenres);
            return result;
        }
    }
}
