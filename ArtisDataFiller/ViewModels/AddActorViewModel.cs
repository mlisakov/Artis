namespace Artis.ArtisDataFiller.ViewModels
{
    public class AddActorViewModel : DialogViewModel
    {
        private string _name;
        /// <summary>
        /// ФИО актера
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public override bool CanExecuteOkCommand(object parameter)
        {
            return !string.IsNullOrEmpty(Name);            
        }
    }
}
