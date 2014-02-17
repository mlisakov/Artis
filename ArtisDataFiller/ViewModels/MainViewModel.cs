using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace Artis.ArtisDataFiller.ViewModels
{
    public class MainViewModel: INotifyPropertyChanged
    {
        private bool _isDownloadingPageOpened;
        private bool _isEditPageOpened;
        private object _content;
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
                }
            }
        }

        /// <summary>
        /// Контент основной области
        /// </summary>
        public object ViewContent
        {
            get { return _content; }
            set
            {
                _content = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Освобождение ресурсов у основной области
        /// </summary>
        private void DisposeViewContent()
        {
            var grid = ViewContent as Grid;
            if (grid != null)
            {
                var viewModel = grid.DataContext as IDisposable;
                if (viewModel != null)
                    viewModel.Dispose();
            }
            ViewContent = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
