using Artis.Data;
using NLog;

namespace Artis.ArtisDataFiller.ViewModels
{
    public class AddActorViewModel : DialogViewModel
    {
        /// <summary>
        /// Логгер
        /// </summary>
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        private string _name;
        private Action _action;
        private Actor _actor;
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

        public Actor Actor
        {
            get { return _actor; }
        }

        public AddActorViewModel(Action action)
        {
            _action = action;
        }

        public override bool CanExecuteOkCommand(object parameter)
        {
            return !string.IsNullOrEmpty(Name);            
        }

        public override void ExecuteOkCommand(object parameter)
        {
            _actor = new Actor() {FIO = Name};
            //await ActorRepository.AddActor(_actor, _action);
        }
    }
}
