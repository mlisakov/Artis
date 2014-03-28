using Artis.ArtisDataFiller.ViewModels;

namespace Artis.ArtisDataFiller
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class AddActorFromListDialogWindow
    {
        private ViewModel _viewModel;

        public AddActorFromListDialogWindow()
        {
            InitializeComponent();
        }

        public ViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                _viewModel = value;
                DataContext = _viewModel;
            }
        }
    }
}
