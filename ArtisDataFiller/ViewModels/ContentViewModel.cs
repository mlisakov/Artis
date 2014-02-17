using System;
using System.Windows;

namespace Artis.ArtisDataFiller.ViewModels
{
    /// <summary>
    /// ViewModel с возможностью отображения динамического контента
    /// </summary>
    public class ContentViewModel: ViewModel
    {
        private FrameworkElement _content;

        /// <summary>
        /// Контент основной области
        /// </summary>
        public FrameworkElement ViewContent
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
        protected void DisposeViewContent()
        {
            if (ViewContent != null)
            {
                var viewModel = ViewContent.DataContext as IDisposable;
                if (viewModel != null)
                    viewModel.Dispose();
            }
            ViewContent = null;
        }
    }
}
