using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Artis.ArtisDataFiller.Annotations;

namespace Artis.ArtisDataFiller.ViewModels
{
    /// <summary>
    /// Базовый класс для всех ViewModel
    /// </summary>
    public class ViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Срабатывает при Dispose
        /// </summary>
        public virtual void OnDispose()
        {
            
        }

        public void Dispose()
        {
            OnDispose();
        }
    }
}
