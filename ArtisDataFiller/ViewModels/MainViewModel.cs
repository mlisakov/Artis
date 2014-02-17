namespace Artis.ArtisDataFiller.ViewModels
{
    public class MainViewModel: ContentViewModel
    {
        private bool _isDownloadingPageOpened;
        private bool _isEditPageOpened;
        private bool _isHomePageOpened;        

        public MainViewModel()
        {
            IsHomePageOpened = true;
        }

        /// <summary>
        /// Открыта ли сейчас стартовая страница
        /// </summary>
        public bool IsHomePageOpened
        {
            get { return _isHomePageOpened; }
            set
            {
                _isHomePageOpened = value;
                if (_isHomePageOpened)
                {
                    IsEditPageOpened = false;
                    IsDownloadingPageOpened = false;
// ReSharper disable ExplicitCallerInfoArgument
                    OnPropertyChanged("IsEditPageOpened");
                    OnPropertyChanged("IsDownloadingPageOpened");
// ReSharper restore ExplicitCallerInfoArgument

                    DisposeViewContent();

                    ViewContent = new HomePage();
                }
            }
        }

        /// <summary>
        /// Открыта ли сейчас страница "Загрузка данных"
        /// </summary>
        public bool IsDownloadingPageOpened
        {
            get { return _isDownloadingPageOpened; }
            set
            {
                _isDownloadingPageOpened = value;
                if (_isDownloadingPageOpened)
                {
                    IsEditPageOpened = false;
                    IsHomePageOpened = false;

// ReSharper disable ExplicitCallerInfoArgument
                    OnPropertyChanged("IsEditPageOpened");
                    OnPropertyChanged("IsHomePageOpened");
// ReSharper restore ExplicitCallerInfoArgument

                    DisposeViewContent();

                    var page = new DownloadPage();
                    var pageViewModel = new DonwloadViewModel();
                    page.DataContext = pageViewModel;

                    ViewContent = page;
                }
            }
        }

        /// <summary>
        /// Открыта ли сейчас страница "Редактирование данных"
        /// </summary>
        public bool IsEditPageOpened
        {
            get { return _isEditPageOpened; }
            set
            {
                _isEditPageOpened = value;
                if (_isEditPageOpened)
                {
                    IsDownloadingPageOpened = false;
                    IsHomePageOpened = false;

// ReSharper disable ExplicitCallerInfoArgument
                    OnPropertyChanged("IsDownloadingPageOpened");
                    OnPropertyChanged("IsHomePageOpened");
// ReSharper restore ExplicitCallerInfoArgument
                    
                    DisposeViewContent();


                    var page = new EditPage();

                    ViewContent = page;
                }
            }
        }
    }
}
