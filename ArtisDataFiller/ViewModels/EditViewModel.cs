namespace Artis.ArtisDataFiller.ViewModels
{
    /// <summary>
    /// ViewModel страницы просмотра данных
    /// </summary>
    public class EditViewModel : ContentViewModel
    {
        private bool _isAreasPageOpened;
        private bool _isActionsPageOpened;
        private bool _isGenresSettingsOpened;

        public EditViewModel()
        {
        }

        /// <summary>
        /// Открыта ли сейчас страница площадок
        /// </summary>
        public bool IsAreasPageOpened
        {
            get { return _isAreasPageOpened; }
            set
            {
                _isAreasPageOpened = value;

                if (_isAreasPageOpened)
                {
                    DisposeViewContent();
                    ViewContent = new AreasPage();
                }
            }
        }

        /// <summary>
        /// Открыта ли сейчас страница мероприятий
        /// </summary>
        public bool IsActionPageOpened
        {
            get { return _isActionsPageOpened; }
            set
            {
                _isActionsPageOpened = value;

                if (_isActionsPageOpened)
                {
                    DisposeViewContent();
                    ViewContent = new ActionsPage();
                }
            }
        }


        /// <summary>
        /// Открыта ли сейчас страница настройки жанров
        /// </summary>
        public bool IsGenresSettingsOpened
        {
            get { return _isGenresSettingsOpened; }
            set
            {
                _isGenresSettingsOpened = value;
                if (_isGenresSettingsOpened)
                {
                    DisposeViewContent();
                    ViewContent = new GenresSettingsPage();
                }
            }
        }
    }
}
