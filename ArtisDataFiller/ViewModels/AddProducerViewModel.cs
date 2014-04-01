using Artis.Data;

namespace Artis.ArtisDataFiller.ViewModels
{
    public class AddProducerViewModel : DialogViewModel
    {
        private Producer _producer;
        /// <summary>
        /// ФИО режисера
        /// </summary>
        public string Name
        {
            get { return _producer.FIO; }
            set
            {
                _producer.FIO = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Наименование на инглише
        /// </summary>
        public string EnglishName
        {
            get { return _producer.EnglishFIO; }
            set
            {
                _producer.EnglishFIO = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description
        {
            get { return _producer.Description; }
            set
            {
                _producer.Description = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Описание на инглише
        /// </summary>
        public string EnglishDescription
        {
            get { return _producer.EnglishDescription; }
            set
            {
                _producer.EnglishDescription = value;
                OnPropertyChanged();
            }
        }

        public Producer Producer
        {
            get { return _producer; }
            set
            {
                _producer = value;
                OnPropertyChanged();
            }
        }

        public AddProducerViewModel()
        {
            _producer = new Producer();
        }

        public override bool CanExecuteOkCommand(object parameter)
        {
            return !string.IsNullOrEmpty(Name);
        }

        public async override void ExecuteOkCommand(object parameter)
        {
//            _producer = new Producer() {FIO = Name};
            //await ProducerRepository.AddProducer(_producer, _action);
        }
    }
}
