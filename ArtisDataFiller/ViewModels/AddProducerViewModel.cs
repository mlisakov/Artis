using Artis.Data;
using NLog;

namespace Artis.ArtisDataFiller.ViewModels
{
    public class AddProducerViewModel : DialogViewModel
    {
        /// <summary>
        /// Логгер
        /// </summary>
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        private string _name;
        private Action _action;
        private Producer _producer;
        /// <summary>
        /// ФИО режисера
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

        public Producer Producer
        {
            get { return _producer; }
            set
            {
                _producer = value;
                OnPropertyChanged();
            }
        }

        public AddProducerViewModel(Action action)
        {
            _action = action;
        }

        public override bool CanExecuteOkCommand(object parameter)
        {
            return !string.IsNullOrEmpty(Name);
        }

        public async override void ExecuteOkCommand(object parameter)
        {
            _producer = new Producer() {FIO = Name};
            //await ProducerRepository.AddProducer(_producer, _action);
        }
    }
}
