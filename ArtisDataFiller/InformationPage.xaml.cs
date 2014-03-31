using Artis.ArtisDataFiller.ViewModels;

namespace Artis.ArtisDataFiller
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class InformationPage
    {
        public InformationPage()
        {
            InitializeComponent();
            DataContext = new InformationViewModel();
        }
    }
}
