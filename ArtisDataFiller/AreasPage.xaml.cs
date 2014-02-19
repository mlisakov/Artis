using Artis.ArtisDataFiller.ViewModels;

namespace Artis.ArtisDataFiller
{
    /// <summary>
    /// Interaction logic for AreasPage.xaml
    /// </summary>
    public partial class AreasPage
    {
        public AreasPage()
        {
            InitializeComponent();
            DataContext = new AreasViewModel();
        }
    }
}
