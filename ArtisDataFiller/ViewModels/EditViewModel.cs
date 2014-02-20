using System.Runtime.CompilerServices;
using System.Windows;

namespace Artis.ArtisDataFiller.ViewModels
{
    /// <summary>
    /// ViewModel страницы просмотра данных
    /// </summary>
    public class EditViewModel : ContentViewModel
    {
        private bool _isAreasPageOpened;
        private bool _isActionsPageOpened;

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
                DisposeViewContent();

                if (IsActionPageOpened)
                {
                    IsActionPageOpened = false;
                    OnPropertyChanged("IsActionPageOpened");
                }

                _isAreasPageOpened = value;

                if (_isAreasPageOpened)
                {
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
                DisposeViewContent();

                if (IsAreasPageOpened)
                {
                    IsAreasPageOpened = false;
                    OnPropertyChanged("IsAreasPageOpened");
                }

                _isActionsPageOpened = value;

                if (_isActionsPageOpened)
                {
                    ViewContent = new ActionsPage();
                }
            }
        }
    }
}
