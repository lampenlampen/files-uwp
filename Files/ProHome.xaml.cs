﻿using Files.Filesystem;
using Files.Interacts;
using Files.Navigation;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Files.Enums;
using System.Drawing;
using Files.View_Models;

namespace Files
{
    public sealed partial class ProHome : Page
    {
        public Grid deleteProgressBox;
        public ProHome currentInstance;
        public ProgressBar deleteProgressBoxIndicator;
        public TextBlock deleteProgressBoxTitle;
        public TextBlock deleteProgressBoxTextInfo;
        public DisplayedPathText PathText { get; set; } = new DisplayedPathText();
        public Interacts.Home.HomeItemsState HomeItems { get; set; } = new Interacts.Home.HomeItemsState();
        public Interacts.Share.ShareItemsState ShareItems { get; set; } = new Interacts.Share.ShareItemsState();
        public Interacts.Layout.LayoutItemsState LayoutItems { get; set; } = new Interacts.Layout.LayoutItemsState();
        public AlwaysPresentCommandsState AlwaysPresentCommands { get; set; } = new AlwaysPresentCommandsState();
        public ObservableCollection<PathBoxItem> pathBoxItems = new ObservableCollection<PathBoxItem>();
        public Interaction instanceInteraction;
        private ItemViewModel _instanceViewModel;
        public ItemViewModel instanceViewModel
        {
            get
            {
                return _instanceViewModel;
            }
            set
            {
                _instanceViewModel = value;
                Bindings.Update();
            }
        }

        public ProHome()
        {
            this.InitializeComponent();
            PathText.Text = "New tab";
            deleteProgressBox = DeleteProgressFakeDialog;
            deleteProgressBoxIndicator = deleteInfoCurrentIndicator;
            deleteProgressBoxTitle = title;
            deleteProgressBoxTextInfo = deleteInfoCurrentText;
            if (ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.ThemeShadow"))
            {
                themeShadow.Receivers.Add(ShadowReceiver);
            }

            // Acrylic sidebar
            var localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Values["acrylicSidebar"] != null && localSettings.Values["acrylicSidebar"].Equals(true))
            {
                splitView.PaneBackground = (Brush)Application.Current.Resources["BackgroundAcrylicBrush"];
                Application.Current.Resources["NavigationViewExpandedPaneBackground"] = Application.Current.Resources["BackgroundAcrylicBrush"];
            }

            if (App.AppSettings.DrivesManager.ShowUserConsentOnInit)
            {
                App.AppSettings.DrivesManager.ShowUserConsentOnInit = false;
                DisplayFilesystemConsentDialog();
            }
        }

        private async void DisplayFilesystemConsentDialog()
        {
            await App.consentDialog.ShowAsync(ContentDialogPlacement.Popup);
        }

        string NavParams = null;

        protected override void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            base.OnNavigatedTo(eventArgs);
            NavParams = eventArgs.Parameter.ToString();
        }

        private void DrivesList_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                LocationsList.SelectedItem = null;
                if (LinuxList != null)
                {
                    LinuxList.SelectedItem = null;
                }
            }
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.OccupiedInstance == null && ItemViewModel.GetCurrentSelectedTabInstance<ProHome>().Equals(this))
            {
                App.OccupiedInstance = this;
            }

            switch (NavParams)
            {
                case "Start":
                    ItemDisplayFrame.Navigate(typeof(YourHome), NavParams, new SuppressNavigationTransitionInfo());
                    LocationsList.SelectedItem = App.sideBarItems[0];
                    break;
                case "New tab":
                    ItemDisplayFrame.Navigate(typeof(YourHome), NavParams, new SuppressNavigationTransitionInfo());
                    LocationsList.SelectedItem = App.sideBarItems[0];
                    break;
                case "Desktop":
                    ItemDisplayFrame.Navigate(typeof(GenericFileBrowser), App.AppSettings.DesktopPath, new SuppressNavigationTransitionInfo());
                    LocationsList.SelectedItem = App.sideBarItems.First(x => x.Path.Equals(App.AppSettings.DesktopPath, StringComparison.OrdinalIgnoreCase));
                    break;
                case "Downloads":
                    ItemDisplayFrame.Navigate(typeof(GenericFileBrowser), App.AppSettings.DownloadsPath, new SuppressNavigationTransitionInfo());
                    LocationsList.SelectedItem = App.sideBarItems.First(x => x.Path.Equals(App.AppSettings.DownloadsPath, StringComparison.OrdinalIgnoreCase));
                    break;
                case "Documents":
                    ItemDisplayFrame.Navigate(typeof(GenericFileBrowser), App.AppSettings.DocumentsPath, new SuppressNavigationTransitionInfo());
                    LocationsList.SelectedItem = App.sideBarItems.First(x => x.Path.Equals(App.AppSettings.DocumentsPath, StringComparison.OrdinalIgnoreCase));
                    break;
                case "Pictures":
                    ItemDisplayFrame.Navigate(typeof(PhotoAlbum), App.AppSettings.PicturesPath, new SuppressNavigationTransitionInfo());
                    LocationsList.SelectedItem = App.sideBarItems.First(x => x.Path.Equals(App.AppSettings.PicturesPath, StringComparison.OrdinalIgnoreCase));
                    break;
                case "Music":
                    ItemDisplayFrame.Navigate(typeof(GenericFileBrowser), App.AppSettings.MusicPath, new SuppressNavigationTransitionInfo());
                    LocationsList.SelectedItem = App.sideBarItems.First(x => x.Path.Equals(App.AppSettings.MusicPath, StringComparison.OrdinalIgnoreCase));
                    break;
                case "Videos":
                    ItemDisplayFrame.Navigate(typeof(GenericFileBrowser), App.AppSettings.VideosPath, new SuppressNavigationTransitionInfo());
                    LocationsList.SelectedItem = App.sideBarItems.First(x => x.Path.Equals(App.AppSettings.VideosPath, StringComparison.OrdinalIgnoreCase));
                    break;

                default:
                    if (NavParams.Contains("C:", StringComparison.OrdinalIgnoreCase))
                    {
                        DrivesList.SelectedItem = SettingsViewModel.foundDrives.First(x => x.tag.ToString().Equals("C:\\", StringComparison.OrdinalIgnoreCase));
                    }
                    else
                    {
                        DrivesList.SelectedItem = null;
                    }
                    break;

            }

            this.Loaded -= Page_Loaded;
        }


        private void ItemDisplayFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (ItemDisplayFrame.CurrentSourcePageType == typeof(GenericFileBrowser))
            {
                // Reset DataGrid Rows that may be in "cut" command mode
                App.OccupiedInstance.instanceInteraction.dataGridRows.Clear();
                Interaction.FindChildren<DataGridRow>(App.OccupiedInstance.instanceInteraction.dataGridRows, (ItemDisplayFrame.Content as GenericFileBrowser).AllView);
                foreach (DataGridRow dataGridRow in App.OccupiedInstance.instanceInteraction.dataGridRows)
                {
                    if ((ItemDisplayFrame.Content as GenericFileBrowser).AllView.Columns[0].GetCellContent(dataGridRow).Opacity < 1)
                    {
                        (ItemDisplayFrame.Content as GenericFileBrowser).AllView.Columns[0].GetCellContent(dataGridRow).Opacity = 1;
                    }
                }
            }
            RibbonArea.Focus(FocusState.Programmatic);
        }

        private void HideFakeDialogButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteProgressFakeDialog.Visibility = Visibility.Collapsed;
        }

        private void LocationsList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {

        }

        private void LocationsList_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem != null)
            {
                DrivesList.SelectedItem = null;
                if (LinuxList != null)
                {
                    LinuxList.SelectedItem = null;
                }
            }
        }

        private void LocationsList_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            HomeItems.isEnabled = false;
            ShareItems.isEnabled = false;
            if (LinuxList != null)
            {
                if (LinuxList.SelectedItem != null)
                {
                    LinuxList.SelectedItem = null;
                    LayoutItems.isEnabled = false;
                }
            }

            if (DrivesList.SelectedItem != null)
            {
                DrivesList.SelectedItem = null;
                LayoutItems.isEnabled = false;
            }

            switch (args.InvokedItem.ToString())
            {
                case "Home":
                    ItemDisplayFrame.Navigate(typeof(YourHome), "New tab", new SuppressNavigationTransitionInfo());
                    PathText.Text = "New tab";
                    HomeItems.isEnabled = false;
                    ShareItems.isEnabled = false;

                    LayoutItems.isEnabled = false;
                    break;
                case "Desktop":
                    ItemDisplayFrame.Navigate(typeof(GenericFileBrowser), App.AppSettings.DesktopPath, new SuppressNavigationTransitionInfo());
                    PathText.Text = "Desktop";
                    HomeItems.isEnabled = false;
                    ShareItems.isEnabled = false;

                    LayoutItems.isEnabled = true;
                    break;
                case "Downloads":
                    ItemDisplayFrame.Navigate(typeof(GenericFileBrowser), App.AppSettings.DownloadsPath, new SuppressNavigationTransitionInfo());
                    PathText.Text = "Downloads";
                    HomeItems.isEnabled = false;
                    ShareItems.isEnabled = false;

                    LayoutItems.isEnabled = true;
                    break;
                case "Documents":
                    ItemDisplayFrame.Navigate(typeof(GenericFileBrowser), App.AppSettings.DocumentsPath, new SuppressNavigationTransitionInfo());
                    PathText.Text = "Documents";
                    HomeItems.isEnabled = false;
                    ShareItems.isEnabled = false;

                    LayoutItems.isEnabled = true;
                    break;
                case "Pictures":
                    ItemDisplayFrame.Navigate(typeof(PhotoAlbum), App.AppSettings.PicturesPath, new SuppressNavigationTransitionInfo());
                    PathText.Text = "Pictures";
                    HomeItems.isEnabled = false;
                    ShareItems.isEnabled = false;

                    LayoutItems.isEnabled = true;
                    break;
                case "Music":
                    ItemDisplayFrame.Navigate(typeof(GenericFileBrowser), App.AppSettings.MusicPath, new SuppressNavigationTransitionInfo());
                    PathText.Text = "Music";
                    HomeItems.isEnabled = false;
                    ShareItems.isEnabled = false;

                    LayoutItems.isEnabled = true;
                    break;
                case "Videos":
                    ItemDisplayFrame.Navigate(typeof(GenericFileBrowser), App.AppSettings.VideosPath, new SuppressNavigationTransitionInfo());
                    PathText.Text = "Videos";
                    HomeItems.isEnabled = false;
                    ShareItems.isEnabled = false;

                    LayoutItems.isEnabled = true;
                    break;
                default:
                    ItemDisplayFrame.Navigate(typeof(GenericFileBrowser), args.InvokedItemContainer.Tag.ToString(), new SuppressNavigationTransitionInfo());
                    PathText.Text = args.InvokedItem.ToString();
                    HomeItems.isEnabled = false;
                    ShareItems.isEnabled = false;

                    LayoutItems.isEnabled = true;
                    break;
            }
        }

        private void DrivesList_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            HomeItems.isEnabled = false;
            ShareItems.isEnabled = false;
            if (LocationsList.SelectedItem != null)
            {
                LocationsList.SelectedItem = null;
                LayoutItems.isEnabled = false;
            }

            if (LinuxList != null)
            {
                if (LinuxList.SelectedItem != null)
                {
                    LinuxList.SelectedItem = null;
                    LayoutItems.isEnabled = false;
                }
            }


            var clickedItem = args.InvokedItemContainer;

            if (clickedItem.Tag.ToString() == "LocalDisk")
            {
                ItemDisplayFrame.Navigate(typeof(GenericFileBrowser), @"C:\", new SuppressNavigationTransitionInfo());
                PathText.Text = @"Local Disk (C:\)";
                LayoutItems.isEnabled = true;
            }
            else if (clickedItem.Tag.ToString() == "OneDrive")
            {
                ItemDisplayFrame.Navigate(typeof(GenericFileBrowser), App.AppSettings.OneDrivePath, new SuppressNavigationTransitionInfo());
                PathText.Text = "OneDrive";
                LayoutItems.isEnabled = true;
            }
            else
            {
                ItemDisplayFrame.Navigate(typeof(GenericFileBrowser), clickedItem.Tag.ToString(), new SuppressNavigationTransitionInfo());
                PathText.Text = clickedItem.Tag.ToString();
                LayoutItems.isEnabled = true;
            }
        }

        private void LocationsList_Loaded(object sender, RoutedEventArgs e)
        {
            LocationsList.SelectedItem = App.sideBarItems[0];
        }

        private void NavigationViewItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            Microsoft.UI.Xaml.Controls.NavigationViewItem sidebar = (Microsoft.UI.Xaml.Controls.NavigationViewItem)sender;
            var item = ((FrameworkElement)e.OriginalSource).DataContext as SidebarItem;
            if (!item.isDefaultLocation)
            {
                SideBarItemContextFlyout.ShowAt(sidebar, e.GetPosition(sidebar));
                App.rightClickedItem = item;
            }
        }

        public void UpdateProgressFlyout(InteractionOperationType operationType, int amountComplete, int amountTotal)
        {
            this.FindName("ProgressFlyout");

            string operationText = null;
            switch (operationType)
            {
                case InteractionOperationType.PasteItems:
                    operationText = "Completing Paste";
                    break;
                case InteractionOperationType.DeleteItems:
                    operationText = "Deleting Items";
                    break;
            }
            ProgressFlyoutTextBlock.Text = operationText + " (" + amountComplete + "/" + amountTotal + ")" + "...";
            ProgressFlyoutProgressBar.Value = amountComplete;
            ProgressFlyoutProgressBar.Maximum = amountTotal;

            if (amountComplete == amountTotal)
            {
                UnloadObject(ProgressFlyout);
            }
        }

        private void LinuxList_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem != null)
            {
                DrivesList.SelectedItem = null;
                LocationsList.SelectedItem = null;
            }
        }

        private void LinuxList_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            HomeItems.isEnabled = false;
            ShareItems.isEnabled = false;
            if (LocationsList.SelectedItem != null)
            {
                LocationsList.SelectedItem = null;
                LayoutItems.isEnabled = false;
            }

            if (DrivesList.SelectedItem != null)
            {
                DrivesList.SelectedItem = null;
                LayoutItems.isEnabled = false;
            }

            var clickedItem = args.InvokedItemContainer;

            ItemDisplayFrame.Navigate(typeof(GenericFileBrowser), clickedItem.Tag.ToString(), new SuppressNavigationTransitionInfo());
            PathText.Text = clickedItem.Tag.ToString();
            LayoutItems.isEnabled = true;
        }
    }

    public enum InteractionOperationType
    {
        PasteItems = 0,
        DeleteItems = 1,
    }

    public class PathBoxItem
    {
        public string Title { get; set; }
        public string Path { get; set; }
    }
}
