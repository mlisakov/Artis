﻿using Artis.ArtisDataFiller.ViewModels;

namespace Artis.ArtisDataFiller
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class EditPage
    {
        public EditPage()
        {
            InitializeComponent();
            DataContext = new EditViewModel();
        }
    }
}
