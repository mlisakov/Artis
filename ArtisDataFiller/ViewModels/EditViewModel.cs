using System.Windows;

namespace Artis.ArtisDataFiller.ViewModels
{
    /// <summary>
    /// ViewModel страницы просмотра данных
    /// </summary>
    public class EditViewModel: ContentViewModel
    {
        private bool _isAreasPageOpened;

        public EditViewModel()
        {            
        }

        /// <summary>
        /// Открыта ли сейчас стартовая страница
        /// </summary>
        public bool IsAreasPageOpened
        {
            get { return _isAreasPageOpened; }
            set
            {
                _isAreasPageOpened = value;
                DisposeViewContent();
                if (_isAreasPageOpened)
                {
                    ViewContent = new AreasPage();
                }
            }
        }
    }
}
