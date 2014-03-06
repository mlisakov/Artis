using Artis.ArtisDataFiller.ViewModels;

namespace Artis.ArtisDataFiller
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class GenresSettingsPage
    {
        public GenresSettingsPage()
        {
            InitializeComponent();
            DataContext = new GenresSettingsViewModel();
        }
    }
}
