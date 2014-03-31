using Artis.ArtisDataFiller.ViewModels;

namespace Artis.ArtisDataFiller
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class GenresPage
    {
        public GenresPage()
        {
            InitializeComponent();
            DataContext = new GenresViewModel();
        }
    }
}
