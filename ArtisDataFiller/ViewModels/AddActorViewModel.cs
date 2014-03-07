﻿using Artis.Data;
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

        public AddActorViewModel(Action action)
        {
            _action = action;
        }

        public override bool CanExecuteOkCommand(object parameter)
        {
            return !string.IsNullOrEmpty(Name);            
        }

        public async override void ExecuteOkCommand(object parameter)
        {
           await ActorRepository.AddActor(new Actor() {FIO = Name}, _action);
        }
    }
}
