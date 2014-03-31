using Artis.ArtisDataFiller.Properties;
using Artis.Consts;

namespace Artis.ArtisDataFiller.ViewModels
{
    public class InformationViewModel: ContentViewModel
    {
        private string _connectionString;

        /// <summary>
        /// Строка подключения
        /// </summary>
        public string ConnectionString
        {
            get { return _connectionString; }
            set
            {
                _connectionString = value; 
                OnPropertyChanged();
                ServiceAddress.SetConnectionString(ConnectionString);
            }
        }

        public InformationViewModel()
        {
            ConnectionString = ServiceAddress.ArtisConnectionString;
        }

        /// <summary>
        /// Сохранение настроек
        /// </summary>
        private void SaveSettings()
        {
            Settings.Default.ConnectionString = ConnectionString;
            Settings.Default.Save();
        }

        public override void OnDispose()
        {
            base.OnDispose();
            SaveSettings();
        }
    }
}
