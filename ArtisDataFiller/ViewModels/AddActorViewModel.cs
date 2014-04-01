using Artis.Data;

namespace Artis.ArtisDataFiller.ViewModels
{
    public class AddActorViewModel : DialogViewModel
    {
        private Actor _actor;

        /// <summary>
        /// ФИО актера
        /// </summary>
        public string Name
        {
            get { return _actor.FIO; }
            set
            {
                _actor.FIO = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Наименование на инглише
        /// </summary>
        public string EnglishName
        {
            get { return _actor.EnglishFIO; }
            set
            {
                _actor.EnglishFIO = value; 
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description
        {
            get { return _actor.Description; }
            set
            {
                _actor.Description = value; 
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Описание на инглише
        /// </summary>
        public string EnglishDescription
        {
            get { return _actor.EnglishDescription; }
            set
            {
                _actor.EnglishDescription = value;
                OnPropertyChanged();
            }
        }
        

        public Actor Actor
        {
            get { return _actor; }
            set
            {
                _actor = value;
                OnPropertyChanged();
            }
        }

        public AddActorViewModel()
        {
            _actor = new Actor();
        }

        public override bool CanExecuteOkCommand(object parameter)
        {
            return !string.IsNullOrEmpty(Name);            
        }

        public override void ExecuteOkCommand(object parameter)
        {
//            _actor = new Actor() {FIO = Name};
            //await ActorRepository.AddActor(_actor, _action);
        }
    }
}
