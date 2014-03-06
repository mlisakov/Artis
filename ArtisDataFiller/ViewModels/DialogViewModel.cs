using Artis.Consts;

namespace Artis.ArtisDataFiller.ViewModels
{
    public class DialogViewModel : ViewModel
    {
        private string _title;
        
        /// <summary>
        /// Заголовок диалогового окна
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value; 
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// OK command
        /// </summary>
        public ArtisCommand OkCommand { get; protected set; }

        public DialogViewModel()
        {
            OkCommand = new ArtisCommand(CanExecuteOkCommand, ExecuteOkCommand);
        }

        public virtual bool CanExecuteOkCommand(object parameter)
        {
            return true;
        }

        public virtual void ExecuteOkCommand(object parameter)
        {
            
        }
    }
}
