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
        /// <summary>
        /// true - если нужно грузить инфу о продюсерах, false - об актерах
        /// </summary>
        private readonly bool _isProducers;

        private string _title;
        private ObservableCollection<Producer> _producersItemsSource;

        /// <summary>
        /// Команда поиска людей
        /// </summary>
        public ArtisCommand SearchPeople { get; private set; }

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
        /// Список актеров/продюсеров
        /// </summary>
        public ObservableCollection<Producer> PeopleItemsSource
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
        /// todo раскоментишь и поправишь, когда создашь интерфейс
//        public IPeople SelectedPeople { get; set; }

        public AddActorFromListViewModel(bool isProducers = false)
        {
            _isProducers = isProducers;

            LoadPeople();

            SearchPeople = new ArtisCommand(CanExecute, ExecuteSearchPeople);
        }

        private bool CanExecute(object param)
        {
            return true;
        }

        private void ExecuteSearchPeople(object obj)
        {
            //todo поиск
        }

        private void LoadPeople()
        {
            //todo асинхронный метод загрузки либо актеров, либо продюсеров (см. _isProducers)
            //todo PeopleItemsSource - список людей. Надо продюсеров и Актеров привести к одному интерфейсу IPeople и положить их в это поле после загрузки
        }
    }
}
