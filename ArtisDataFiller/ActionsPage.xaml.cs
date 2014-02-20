using Artis.ArtisDataFiller.ViewModels;

namespace Artis.ArtisDataFiller
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class ActionsPage
    {
        public ActionsPage()
        {
            InitializeComponent();
            DataContext = new ActionsViewModel();
        }
    }
}
