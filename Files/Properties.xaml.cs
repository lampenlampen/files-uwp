﻿using Files.Filesystem;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Files
{

    public sealed partial class Properties : Page
    {
        ListedItem Item { get; set; }
        ContentDialog PropertiesDialog { get; set; }
        public Properties()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Item = e.Parameter as ListedItem;
            PropertiesDialog = Frame.Tag as ContentDialog;
            base.OnNavigatedTo(e);
        }
    }
}
