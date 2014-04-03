using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artis.Consts;
using Artis.Data;

namespace Artis.ArtisDataFiller.ViewModels
{
    public class AddActorFromListViewModel: ViewModel
    {
        private WcfServiceCaller _wcfServiceCaller;
        /// <summary>
        /// true - если нужно грузить инфу о продюсерах, false - об актерах
        /// </summary>
        private readonly bool _isProducers;

        private string _title;
        private string _name;
        private ObservableCollection<IPeople> _producersItemsSource;

        /// <summary>
        /// Команда поиска людей
        /// </summary>
        public ArtisCommand SearchCommand { get; private set; }

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

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }



        /// <summary>
        /// Список актеров/продюсеров
        /// </summary>
        public ObservableCollection<IPeople> PeopleItemsSource
        {
            get { return _producersItemsSource; }
            set
            {
                _producersItemsSource = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Выделенный актер
        /// </summary>
        public IPeople SelectedPeople { get; set; }

        public AddActorFromListViewModel(bool isProducers = false)
        {
            _wcfServiceCaller=new WcfServiceCaller();
            _isProducers = isProducers;

            LoadPeople();

            SearchCommand = new ArtisCommand(CanExecute, ExecuteSearchPeople);
        }

        private bool CanExecute(object param)
        {
            return true;
        }

        private async void ExecuteSearchPeople(object obj)
        {
            //todo поиск
            await LoadPeople(Name);
        }

        private async Task LoadPeople(string filter="")
        {
            PeopleItemsSource = _isProducers
                ? new ObservableCollection<IPeople>(
                    (await _wcfServiceCaller.GetProducersAsync(filter)).Select(i => (IPeople)i))
                : new ObservableCollection<IPeople>(
                    (await _wcfServiceCaller.GetActorsAsync(filter)).Select(i => (IPeople)i));

            //todo асинхронный метод загрузки либо актеров, либо продюсеров (см. _isProducers)
            //todo PeopleItemsSource - список людей. Надо продюсеров и Актеров привести к одному интерфейсу IPeople и положить их в это поле после загрузки
        }
    }
}
